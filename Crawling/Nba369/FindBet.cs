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
    public partial class Nba369 : CasinoAPI
    {
        protected override BetResult findBet(HtmlDocument htmlDoc, string matchToFind, string hdpToFind, decimal winOddToFind, bool winOddFav, bool hedgeBet)
        {
            string currentRawMatch = "";
            int iCurrentMatch = -1;
            string matchName = "";
            string teamFav = "";
            string teamNoFav = "";
            string matchDate = "";
            string matchTime = "";
            string currentTeamFav = "";
            string currentTeamNoFav = "";
            string currentmatchName = "";

            int colOddPos = 0;
            int lineOddPos = 0;

            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            bool canUseCurrentLeague = true;
            string league = "";
            string typeLeague = "";
            string currentLeague = "";
            string currentSensHdp = "";

            int line1I = 0;
            BetResult betResult = new BetResult();

            
            var prv_RowMatches = htmlDoc.DocumentNode.SelectNodes("//tr");
            int prv_NumRowMatches = prv_RowMatches.Count;

            //foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes("//tr"))
            for (int RowMatchCounter = 0; RowMatchCounter < prv_NumRowMatches; RowMatchCounter++)
            {
	            var rowMatch = prv_RowMatches[RowMatchCounter];
                line1I++;
                if (rowMatch.Descendants("th").Any()) // Is line League
                {
                    canUseCurrentLeague = true;
                    league = rowMatch.SelectSingleNode(".//th[2]").InnerText;
                    league = currentLeague.ToUpper();

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

                    // Check if match name is on this line
                    if (rowMatch.SelectNodes(".//td").Count == 7)
                    {

                        teamFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/span[1]").InnerText;
                        teamNoFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/span[3]").InnerText;
                        teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                        teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;
                        matchName = $"{teamFav} - {teamNoFav}";

                        colOddPos = 3;

                        if (currentRawMatch == "")
                        {
                            currentRawMatch = matchName;
                            currentTeamFav = teamFav;
                            currentTeamNoFav = teamNoFav;
                            currentLeague = league;
                            currentmatchName = matchName;
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

                        // Get the position of the favorit

                    }
                    else
                    {
                        colOddPos = 1;
                    }

                    //NLogger.Log(EventLevel.Notice, $"NBA matchName : {matchName} - matchToFind : {matchToFind}");
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
                                iCol = 1;
                                typeTime = "ft";
                            }
                            else
                            {
                                iCol = 3;
                                typeTime = "fh";
                            }

                            int prv_ColIndex = colOddPos + iCol;
                            
                            if (rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/span") != null)
                            {
                                string overUnder = rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/span").InnerText;
                                string strOddsOver = rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/a[1]").InnerText;
                                string strOddsUnder = rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/a[2]").InnerText;
                                if (overUnder != "")
                                {
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
                                                string oddPath = $"/td[{prv_ColIndex}]/a[1]";
                                                NLogger.Log(EventLevel.Trace, $"Click on odds : 004 {typeTime}");
                                                //protected override BetResult checkBet(string idClick, string hdpToFind)
                                                //betResult = checkBet(rowMatch.SelectSingleNode(".//td[" + prv_ColIndex + "]/a[1]").Attributes["id"].Value, hdpToFind);
                                                betResult = checkBet(rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/a[1]").Attributes["id"].Value, "", hdpToFind);

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
                                                string oddPath = $"/td[{prv_ColIndex}]/a[2]";
                                                NLogger.Log(EventLevel.Trace, $"Click on odds : 003 {typeTime}");
                                                //protected override BetResult checkBet(string idClick, string hdpToFind)
                                                //betResult = checkBet(rowMatch.SelectSingleNode(".//td[" + prv_ColIndex + "]/a[2]").Attributes["id"].Value, hdpToFind);
                                                betResult = checkBet(rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/a[2]").Attributes["id"].Value, "", hdpToFind);
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




                        // ++++++++++++++++++++
                        // HANDICAP ODD
                        // ++++++++++++++++++++

                        // Get the position of the favorite



                        for (int i = 0; i < 2; i++)
                        {
                            if (i == 0)
                            {
                                iCol = 0;
                                typeTime = "ft";
                            }
                            else
                            {
                                iCol = 2;
                                typeTime = "fh";
                            }

                            int prv_ColIndex = colOddPos + iCol;

                            string sensHdp = "";
                            int posHdp = 0;

                            if (rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/span") == null)
                            {
                                continue;
                            }

                            string oddTypeHtml = rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]").InnerHtml;
                            int index1 = oddTypeHtml.IndexOf("<span");
                            if (index1 == 0)
                            {
                                sensHdp = "+ ";
                            }
                            else
                            {
                                sensHdp = "- ";
                            }

                            string originalHandicapType = rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/span").InnerText;

                            if (originalHandicapType == "")
                            {
                                continue;
                            }

                            originalHandicapType = originalHandicapType.Replace("/", "-");

                            string handicapType = originalHandicapType;


                            if (handicapType != "0")
                            {
                                handicapType = sensHdp + handicapType;
                            }

                            handicapType = $"{typeTime} {handicapType}";

                            if (handicapType == hdpToFind)
                            {
                                string strOddsFav = rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/a[1]").InnerText;
                                string strOddsNoFav = rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/a[2]").InnerText;

                                strOddsFav = strOddsFav.Replace("&nbsp;", "");
                                strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");

                                if (winOddFav)
                                {

                                    NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsFav}");

                                    if (decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                    {
                                        string oddPath = $"/td[{prv_ColIndex}]/a[1]";
                                        NLogger.Log(EventLevel.Trace, $"Click on odds : 002 {typeTime}");
                                        //protected override BetResult checkBet(string idClick, string hdpToFind)
                                        //betResult = checkBet(rowMatch.SelectSingleNode(".//td[" + prv_ColIndex + "]/a[1]").Attributes["id"].Value, hdpToFind);
                                        betResult = checkBet(rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/a[1]").Attributes["id"].Value, "", hdpToFind);
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
                                        string oddPath = $"/td[{colOddPos}]/a[2]";
                                        NLogger.Log(EventLevel.Trace, $"Click on odds : 001 {typeTime}");
                                        //protected override BetResult checkBet(string idClick, string hdpToFind)
                                        //betResult = checkBet(rowMatch.SelectSingleNode(".//td[" + (colOddPos + iCol) + "]/a[2]").Attributes["id"].Value, hdpToFind);
                                        betResult = checkBet(rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/a[2]").Attributes["id"].Value, "", hdpToFind);
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
            betResult.BetStatus = 1;
            betResult.BetErrorStatus = 2;
            betResult.BetIsConfirmed = false;
            betResult.BetIsPlaced = false;
            betResult.BetMessage = "Match and odd not founded";
            return betResult;
        }
    }
}
