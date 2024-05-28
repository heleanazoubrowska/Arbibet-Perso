using System;
using System.Globalization;
using System.Linq;
using System.Threading;

using HtmlAgilityPack;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Ibc05 : CasinoAPI
    {

        protected override BetResult findBet(HtmlDocument htmlDoc, string matchToFind, string hdpToFind, decimal winOddToFind, bool winOddFav, bool hedgeBet)
        {

            Console.WriteLine("we are in find bet");

            string currentRawMatch = "";
            int currentTeamFavPos = 0;
            int currentTeamNoFavPos = 0;
            int iCurrentMatch = -1;
            string matchName = "";
            string teamFav = "";
            string teamNoFav = "";
            string matchDate = "";
            string matchTime = "";
            string classFirstTeam = "";
            string classSecondTeam = "";

            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            bool canUseCurrentLeague = true;
            string league = "";
            string typeLeague = "";
            string currentLeague = "";
            string currentLeagueType = "";
            string currentSensHdp = "";

            BetResult betResult = new BetResult();

            
            var prv_RowLeagues = htmlDoc.DocumentNode.SelectNodes("//tbody");
            int prv_NumRowLeagues = prv_RowLeagues.Count;

            CheckBet_ExtraParams prv_ExtraParams = new CheckBet_ExtraParams();

            //foreach (HtmlNode rowLeague in htmlDoc.DocumentNode.SelectNodes("//tbody"))
            for (int LeagueCounter = 0; LeagueCounter < prv_NumRowLeagues; LeagueCounter++)
            {
	            var rowLeague = prv_RowLeagues[LeagueCounter];
                int line2I = 0;

                var prv_RowMatches = rowLeague.SelectNodes("./tr");
                int prv_NumRowMatches = prv_RowMatches.Count;

                //foreach (HtmlNode rowMatch in rowLeague.SelectNodes("./tr"))
                for (int RowMatchCounter = 0; RowMatchCounter < prv_NumRowMatches; RowMatchCounter++)
                {
	                var rowMatch = prv_RowMatches[RowMatchCounter];
                    line2I++;

                    prv_ExtraParams.line2I = line2I.ToString();

                    try
                    {
                        if (rowMatch.Attributes["class"].Value == "GridHeader" || rowMatch.Attributes["class"].Value == "y_titlebg")
                        {
                            continue;
                        }

                    if (rowMatch.SelectNodes(".//td").Count == 1) // is line league
                    {
                        canUseCurrentLeague = true;
                        league = rowMatch.SelectSingleNode(".//td/span").InnerText;
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

                        if (!rowMatch.SelectSingleNode(".//td[3]/span").Descendants("span").Any())
                        {
                            continue;
                        }

                        // Check if match name is on this line
                        if (rowMatch.SelectSingleNode(".//td[1]").Descendants("span").Any())
                        {
                            if (currentRawMatch == "")
                            {

                            }
                            else
                            {   // We check if we need to add the new match
                                hasOdd = false;
                                listOdds = new List<Odd>();
                                listHdp = new List<string>();
                            }


                            if (rowMatch.SelectSingleNode(".//td[2]/div[1]/a").Attributes["class"].Value == "Give")
                            {
                                currentSensHdp = "+ ";
                            }
                            else
                            {
                                currentSensHdp = "- ";
                            }

                            teamFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/a").InnerText;
                            teamNoFav = rowMatch.SelectSingleNode(".//td[2]/div[2]/a").InnerText;

                            teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                            teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;

                            matchName = $"{teamFav} - {teamNoFav}";
                            currentRawMatch = matchName;

                        }

                        if (matchName == matchToFind)
                        {


                            // ++++++++++++++++++++
                            // OVER UNDER ODD
                            // ++++++++++++++++++++


                            int iCol = 0;
                            string typeTime = "";

                            for (int i = 0; i < 2; i++)
                            {
                                if (i == 0)
                                {
                                    iCol = 6;
                                    typeTime = "ft";
                                }
                                else
                                {
                                    iCol = 12;
                                    typeTime = "fh";
                                }

                                if (rowMatch.SelectSingleNode($".//td[{iCol}]/span/span") != null)
                                {
	                                int prv_ColIndex_Over = iCol + 1;
	                                int prv_ColIndex_Under = iCol + 2;

                                    string overUnder = rowMatch.SelectSingleNode($".//td[{iCol}]/span/span").InnerText;
                                    string strOddsOver = rowMatch.SelectSingleNode($".//td[{prv_ColIndex_Over}]/span/span/label").InnerText;
                                    string strOddsUnder = rowMatch.SelectSingleNode($".//td[{prv_ColIndex_Under}]/span/span/label").InnerText;
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
                                                string oddPath = $"/td[{prv_ColIndex_Over}]/span/span";
                                                NLogger.Log(EventLevel.Trace, $"Click on odds : 004 {typeTime}");

                                                //betResult = checkBet(oddPath, line2I, rowLeague.Attributes["id"].Value, hdpToFind);
                                                prv_ExtraParams.idRowLeague = rowLeague.Attributes["id"].Value;
                                                betResult = checkBet(oddPath, "", hdpToFind, prv_ExtraParams);
                                                
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
                                                string oddPath = $"/td[{prv_ColIndex_Under}]/span/span";
                                                NLogger.Log(EventLevel.Trace, $"Click on odds : 003 {typeTime}");

                                                //betResult = checkBet(oddPath, line2I, rowLeague.Attributes["id"].Value, hdpToFind);
                                                prv_ExtraParams.idRowLeague = rowLeague.Attributes["id"].Value;
                                                betResult = checkBet(oddPath, "", hdpToFind, prv_ExtraParams);
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
                            }


                            // ++++++++++++++++++++
                            // HANDICAP ODD
                            // ++++++++++++++++++++

                            for (int i = 0; i < 2; i++)
                            {
                                if (i == 0)
                                {
                                    iCol = 3;
                                    typeTime = "ft";
                                }
                                else
                                {
                                    iCol = 9;
                                    typeTime = "fh";
                                }


                                if (rowMatch.SelectSingleNode($".//td[{iCol}]/span/span") == null)
                                {
                                    continue;
                                }

                                string originalHandicapType = rowMatch.SelectSingleNode($".//td[{iCol}]/span/span").InnerText;
                                originalHandicapType = originalHandicapType.Replace("/", "-");

                                string handicapType = currentSensHdp + originalHandicapType;

                                if (originalHandicapType == "")
                                {
                                    continue;
                                }

                                if (originalHandicapType != "0")
                                {
                                    handicapType = currentSensHdp + originalHandicapType;
                                }
                                else
                                {
                                    handicapType = originalHandicapType;
                                }

                                handicapType = $"{typeTime} {handicapType}";

                                if (handicapType == hdpToFind)
                                {
	                                int prv_ColIndex_Over = iCol + 1;
	                                int prv_ColIndex_Under = iCol + 2;

                                    string strOddsFav = rowMatch.SelectSingleNode($".//td[{prv_ColIndex_Over}]/span").InnerText;
                                    string strOddsNoFav = rowMatch.SelectSingleNode($".//td[{prv_ColIndex_Under}]/span").InnerText;

                                    strOddsFav = strOddsFav.Replace("&nbsp;", "");
                                    strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");

                                    if (winOddFav)
                                    {
                                        NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsFav}");

                                        if (decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                        {
                                            string oddPath = $"/td[{prv_ColIndex_Over}]/span";
                                            NLogger.Log(EventLevel.Trace, $"Click on odds : 002 {typeTime}");

                                            //betResult = checkBet(oddPath, line2I, rowLeague.Attributes["id"].Value, hdpToFind);
                                            prv_ExtraParams.idRowLeague = rowLeague.Attributes["id"].Value;
                                            betResult = checkBet(oddPath, "", hdpToFind, prv_ExtraParams);
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
                                            string oddPath = $"/td[{prv_ColIndex_Under}]/span";
                                            NLogger.Log(EventLevel.Trace, $"Click on odds : 001 {typeTime}");

                                            //betResult = checkBet(oddPath, line2I, rowLeague.Attributes["id"].Value, hdpToFind);
                                            prv_ExtraParams.idRowLeague = rowLeague.Attributes["id"].Value;
                                            betResult = checkBet(oddPath, "", hdpToFind, prv_ExtraParams);
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
                        }
                    }
                }
                    catch(Exception e)
                {
                    NLogger.Log(EventLevel.Error, "Catch Error click for place bet on IBC");
                    //NLogger.LogError(e);
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
