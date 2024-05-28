using System;
using HtmlAgilityPack;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using System.Threading;
using Newtonsoft.Json;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
	public partial class Va2888 : CasinoAPI
	{
		protected override BetResult findBet(HtmlDocument htmlDoc, string matchToFind, string hdpToFind, decimal winOddToFind, bool winOddFav, bool hedgeBet)
        {
            string currentRawMatch = "";
            int currentTeamFavPos = 0;
            int currentTeamNoFavPos = 0;
            int iCurrentMatch = -1;
            string matchName = "";
            string teamFav = "";
            string teamNoFav = "";
            string matchDate = "";
            string matchTime = "";
            string currentTeamFav = "";
            string currentTeamNoFav = "";

            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            string league = "";
            string typeLeague = "";
            string currentLeague = "";
            string currentLeagueType = "";
            string currentSensHdp = "";
            bool canUseCurrentLeague = true;
            int line1I = 0;

            BetResult betResult = new BetResult();

            
            var prv_RowBodies = htmlDoc.DocumentNode.SelectNodes(".//tbody");
            int prv_NumRowBodies = prv_RowBodies.Count;

            //foreach (HtmlNode rowBody in htmlDoc.DocumentNode.SelectNodes(".//tbody"))
            for (int RowBodyCounter = 0; RowBodyCounter < prv_NumRowBodies; RowBodyCounter++)
            {
	            var rowBody = prv_RowBodies[RowBodyCounter];

                line1I++;
                int line2I = 0;

                
                var prv_RowMatches = rowBody.SelectNodes(".//tr");
                int prv_NumRowMatches = prv_RowMatches.Count;

                //foreach (HtmlNode rowMatch in rowBody.SelectNodes(".//tr"))
                for (int RowMatchCounter = 0; RowMatchCounter < prv_NumRowMatches; RowMatchCounter++)
                {
	                var rowMatch = prv_RowMatches[RowMatchCounter];
                    line2I++;

                    //NLogger.Log(EventLevel.Debug,rowMatch.InnerHtml);


                    //NLogger.Log(EventLevel.Debug,"Case 0");
                    if (rowMatch.Attributes["class"].Value == "GridHeader" || rowMatch.Attributes["class"].Value == "y_titlebg")
                    {
                        continue;
                    }

                    if (rowMatch.SelectNodes(".//td").Count == 1) // is line league
                    {
                        //NLogger.Log(EventLevel.Debug,"Case 1");
                        canUseCurrentLeague = true;
                        league = rowMatch.SelectSingleNode(".//td/span[1]").InnerText;
                        league.ToUpper();

                        //foreach (string str in Config.Config.uselessLeague)
                        for (int UselessLeagueCounter = 0; UselessLeagueCounter < prv_NumUselessLeagues; UselessLeagueCounter++)
                        {
	                        if (league.Contains(Config.Config.uselessLeague[UselessLeagueCounter]))
	                        {
		                        //NLogger.Log(EventLevel.Debug,currentLeague);
		                        canUseCurrentLeague = false;
		                        break;
	                        }
                        }

                        typeLeague = Common.getLeagueType(league);
                    }
                    else
                    {

                        if (!canUseCurrentLeague)
                        {
                            continue;
                        }

                        //NLogger.Log(EventLevel.Debug,"Case 2");
                        if (!rowMatch.SelectSingleNode(".//td[4]/span").Descendants("span").Any())
                        {
                            continue;
                        }

                        // Check if match name is on this line
                        if (rowMatch.SelectSingleNode(".//td[1]").Descendants("a").Any())
                        {
                            //NLogger.Log(EventLevel.Debug,"Case 2.1");

                            if (rowMatch.SelectSingleNode(".//td[3]/div[1]/*").Attributes["class"].Value == "Give")
                            {
                                currentSensHdp = "+ ";

                            }
                            else
                            {
                                currentSensHdp = "- ";
                            }

                            teamFav = rowMatch.SelectSingleNode(".//td[3]/div[1]/*").InnerText;
                            teamNoFav = rowMatch.SelectSingleNode(".//td[3]/div[2]/*").InnerText;

                            teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                            teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;

                            matchName = $"{teamFav} - {teamNoFav}";
                            //NLogger.Log(EventLevel.Debug,"Case 2.2");
                            if (currentRawMatch == "")
                            {
                                currentRawMatch = matchName;
                                currentTeamFav = teamFav;
                                currentTeamNoFav = teamNoFav;
                                currentLeague = league;
                                currentLeagueType = typeLeague;
                            }
                            if (currentRawMatch != matchName)
                            {   // We check if we need to add the new match
                                currentRawMatch = matchName;
                                currentTeamFav = teamFav;
                                currentTeamNoFav = teamNoFav;
                                currentLeague = league;
                                hasOdd = false;
                                listOdds = new List<Odd>();
                                listHdp = new List<string>();
                            }

                            //currentRawMatch = matchName;

                        }


                        if (currentRawMatch == matchToFind)
                        {
                            // ++++++++++++++++++++
                            // OVER UNDER ODD
                            // ++++++++++++++++++++

                            int iCol;
                            string typeTime = "";

                            for (int i = 0; i < 2; i++)
                            {
                                if (i == 0)
                                {
                                    iCol = 7;
                                    typeTime = "ft";
                                }
                                else
                                {
                                    iCol = 13;
                                    typeTime = "fh";
                                }

                                if (rowMatch.SelectSingleNode(".//td[" + iCol + "]/span/span") != null)
                                {
                                    try
                                    {
                                        string overUnder = rowMatch.SelectSingleNode(".//td[" + iCol + "]/span/span").InnerText;
                                        string strOddsOver = rowMatch.SelectSingleNode(".//td[" + (iCol + 1) + "]/span/span").InnerText;
                                        string strOddsUnder = rowMatch.SelectSingleNode(".//td[" + (iCol + 2) + "]/span/span").InnerText;
                                        overUnder = overUnder.Replace("/", "-");
                                        overUnder = $"{typeTime} o/u {overUnder}";
                                        strOddsOver = strOddsOver.Replace("&nbsp;", "");
                                        strOddsUnder = strOddsUnder.Replace("&nbsp;", "");


                                        if (overUnder == hdpToFind)
                                        {
                                            if (winOddFav)
                                            {
                                                NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsOver}");

                                                if (decimal.Parse(strOddsOver, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                                {
                                                    string oddPath = "/td[" + (iCol + 1) + "]/span/span";
                                                    NLogger.Log(EventLevel.Trace, "Click on odds : 004");
                                                    //betResult = placebet(oddPath, rowMatch.Attributes["oddsid"].Value);
                                                    betResult = checkBet(oddPath, rowMatch.Attributes["oddsid"].Value, hdpToFind);
                                                    return betResult;
                                                }
                                                else
                                                {
                                                    betResult.BetStatus = 1;
                                                    betResult.BetErrorStatus = 4;
                                                    betResult.BetIsConfirmed = false;
                                                    betResult.BetIsPlaced = false;
                                                    betResult.BetMessage = "Odd from panel > 1";
                                                    return betResult;
                                                }
                                            }
                                            else
                                            {
                                                NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsUnder}");

                                                if (decimal.Parse(strOddsUnder, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                                {
                                                    string oddPath = "/td[" + (iCol + 2) + "]/span/span";
                                                    NLogger.Log(EventLevel.Trace, "Click on odds : 003");
                                                    //betResult = placebet(oddPath, rowMatch.Attributes["oddsid"].Value);
                                                    betResult = checkBet(oddPath, rowMatch.Attributes["oddsid"].Value, hdpToFind);
                                                    return betResult;
                                                }
                                                else
                                                {
                                                    betResult.BetStatus = 1;
                                                    betResult.BetErrorStatus = 4;
                                                    betResult.BetIsConfirmed = false;
                                                    betResult.BetIsPlaced = false;
                                                    betResult.BetMessage = "Odd from panel > 1";
                                                    return betResult;
                                                }
                                            }
                                        }

                                    }
                                    catch
                                    {
                                        //NLogger.Log(EventLevel.Debug,"error va 003");
                                    }
                                }
                            }

                            // ++++++++++++++++++++
                            // HANDICAP ODD
                            // ++++++++++++++++++++


                            for (int i = 0; i < 2; i++)
                            {
                                if (i == 0)
                                {
                                    iCol = 4;
                                    typeTime = "ft";
                                }
                                else
                                {
                                    iCol = 10;
                                    typeTime = "fh";
                                }

                                if (rowMatch.SelectSingleNode(".//td[" + iCol + "]/span/span") == null)
                                {
                                    continue;
                                }
                                //NLogger.Log(EventLevel.Debug,"Case 3");
                                string originalHandicapType = rowMatch.SelectSingleNode(".//td[" + iCol + "]/span/span").InnerText;
                                originalHandicapType = originalHandicapType.Replace("/", "-");

                                string handicapType = currentSensHdp + originalHandicapType;

                                if (originalHandicapType != "0")
                                {
                                    //if (listHdp.Contains(handicapType))
                                    //{
                                    //    if (currentSensHdp == "+ ")
                                    //    {
                                    //        handicapType = $"- {originalHandicapType}";
                                    //    }
                                    //    else
                                    //    {
                                    //        handicapType = $"+ {originalHandicapType}";
                                    //    }

                                    //}

                                    listHdp.Add(handicapType);
                                }
                                else
                                {
                                    handicapType = originalHandicapType;
                                }

                                handicapType = $"{typeTime} {handicapType}";

                                if (handicapType == hdpToFind)
                                {
                                    try
                                    {
                                        //NLogger.Log(EventLevel.Debug,"Case 4");
                                        string strOddsFav = rowMatch.SelectSingleNode(".//td[" + (iCol + 1) + "]/span/label").InnerText;
                                        string strOddsNoFav = rowMatch.SelectSingleNode(".//td[" + (iCol + 2) + "]/span/label").InnerText;

                                        strOddsFav = strOddsFav.Replace("&nbsp;", "");
                                        strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");

                                        if (winOddFav)
                                        {

                                            NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsFav}");

                                            if (decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                            {
                                                string oddPath = "/td[" + (iCol + 1) + "]/span";
                                                NLogger.Log(EventLevel.Trace, "Click on odds : 002");
                                                //betResult = placebet(oddPath, rowMatch.Attributes["oddsid"].Value);
                                                betResult = checkBet(oddPath, rowMatch.Attributes["oddsid"].Value, hdpToFind);
                                                return betResult;
                                            }
                                            else
                                            {
                                                betResult.BetStatus = 1;
                                                betResult.BetErrorStatus = 4;
                                                betResult.BetIsConfirmed = false;
                                                betResult.BetIsPlaced = false;
                                                betResult.BetMessage = "Odd from panel > 1";
                                                return betResult;
                                            }
                                        }
                                        else
                                        {

                                            NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsNoFav}");

                                            if (decimal.Parse(strOddsNoFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                            {
                                                string oddPath = "/td[" + (iCol + 2) + "]/span";
                                                NLogger.Log(EventLevel.Trace, "Click on odds : 001");
                                                //betResult = placebet(oddPath, rowMatch.Attributes["oddsid"].Value);
                                                betResult = checkBet(oddPath, rowMatch.Attributes["oddsid"].Value, hdpToFind);
                                                return betResult;
                                            }
                                            else
                                            {
                                                betResult.BetStatus = 1;
                                                betResult.BetErrorStatus = 4;
                                                betResult.BetIsConfirmed = false;
                                                betResult.BetIsPlaced = false;
                                                betResult.BetMessage = "Odd from panel > 1";
                                                return betResult;
                                            }
                                        }
                                    }

                                    catch
                                    {
                                        //NLogger.Log(EventLevel.Debug,"error va 002");
                                    }
                                }

                            }
                        }
                    }
                }
            }

            betResult.BetStatus = 1;
            betResult.BetErrorStatus = 2;
            betResult.BetIsConfirmed = false;
            betResult.BetIsPlaced = false;
            betResult.BetMessage = "Match and odd not founded";
            return betResult;
        }
    }
}
