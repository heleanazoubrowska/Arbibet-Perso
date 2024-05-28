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
    public partial class Aa2888 : CasinoAPI
    {

        protected override BetResult findBet(HtmlDocument htmlDoc, string matchToFind, string hdpToFind, decimal winOddToFind, bool winOddFav, bool hedgeBet)
        {
            //NLogger.Log(EventLevel.Debug,htmlDoc.DocumentNode.InnerText);

            string currentRawMatch = "";
            int iCurrentMatch = -1;
            string matchName = "";
            string teamFav = "";
            string teamNoFav = "";
            string matchDate = "";
            string matchTime = "";
            string classFirstTeam = "";
            string classSecondTeam = "";

            int colOddPos = 0;
            int lineOddPos = 0;

            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            bool canUseCurrentLeague = true;
            string league = "";
            string typeLeague = "";
            string currentLeague = "";
            string currentLeagueType = "";

            int line1I = 0;
            BetResult betResult = new BetResult();

            var prv_Rows = htmlDoc.DocumentNode.SelectNodes("//tr");
            int prv_NumRows = prv_Rows.Count;

            //foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes("//tr"))
            for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
            {
	            var rowMatch = prv_Rows[Loopy];
                line1I++;
                if (rowMatch.Descendants("th").Any()) // Is line League
                {
                    canUseCurrentLeague = true;
                    league = rowMatch.SelectSingleNode(".//th[2]").InnerText;
                    league = league.Replace("&nbsp;", "");

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
                        colOddPos = 3;

                        if (currentRawMatch == "")
                        {
                            currentLeague = league;
                            currentLeagueType = typeLeague;
                        }

                        // Get the position of the favorite

                        teamFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/span[1]").InnerText;
                        teamNoFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/span[2]").InnerText;

                        teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                        teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;

                        matchName = $"{teamFav} - {teamNoFav}";
                        currentRawMatch = matchName;
                        currentLeague = league;

                    }
                    else
                    {
                        colOddPos = 1;
                    }


                    if (matchName == matchToFind)
                    {

                        // ++++++++++++++++++++
                        // OVER UNDER ODD FT & FH
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

                            int colTarget = colOddPos + iCol;

                            if (rowMatch.SelectSingleNode($".//td[{colTarget}]/span[1]") != null 
                                && rowMatch.SelectSingleNode($".//td[{colTarget}]/span[2]") != null)
                            {
                                string overUnder = rowMatch.SelectSingleNode($".//td[{colTarget}]/span[1]").InnerText;
                                string strOddsOver = rowMatch.SelectSingleNode($".//td[{colTarget}]/a[1]").InnerText;
                                string strOddsUnder = rowMatch.SelectSingleNode($".//td[{colTarget}]/a[2]").InnerText;
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
                                            if (decimal.Parse(strOddsOver, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                            {
                                                string oddPath = $"/td[{colTarget}]/a[1]";
                                                NLogger.Log(EventLevel.Trace, $"Click on odds : 004 {typeTime}");
                                                betResult = checkBet(oddPath, rowMatch.Attributes["id"].Value, hdpToFind);
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
                                            if (decimal.Parse(strOddsUnder, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                            {
                                                string oddPath = $"/td[{colTarget}]/a[2]";
                                                NLogger.Log(EventLevel.Trace, $"Click on odds : 003 {typeTime}");
                                                betResult = checkBet(oddPath, rowMatch.Attributes["id"].Value, hdpToFind);
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
                        // HANDICAP ODD FT & FH
                        // ++++++++++++++++++++



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

                            int colTarget = colOddPos + iCol;

                            string sensHdp = "";
                            int posHdp = 0;
                            if (rowMatch.SelectSingleNode($".//td[{colTarget}]/span[1]") == null && rowMatch.SelectSingleNode(".//td[" + (colOddPos + iCol) + "]/span[2]") == null)
                            {
                                continue;
                            }

                            // Get the position of the favorite
                            if (rowMatch.SelectSingleNode($".//td[{colTarget}]/span[1]").InnerText == "" 
                                || 
                                rowMatch.SelectSingleNode($".//td[{colTarget}]/span[1]").InnerText == "&nbsp;")
                            {
                                sensHdp = "- ";
                                posHdp = 2;
                            }
                            else
                            {
                                sensHdp = "+ ";
                                posHdp = 1;
                            }

                            string handicapType = rowMatch.SelectSingleNode($".//td[{colTarget}]/span[{posHdp}]").InnerText;
                            if (handicapType == "")
                            {
                                continue;
                            }
                            handicapType = handicapType.Replace("/", "-");

                            if (handicapType != "0")
                            {
                                handicapType = sensHdp + handicapType;
                            }

                            handicapType = $"{typeTime} {handicapType}";

                            if (handicapType == hdpToFind)
                            {
                                string strOddsFav = rowMatch.SelectSingleNode($".//td[{colTarget}]/a[1]").InnerText;
                                string strOddsNoFav = rowMatch.SelectSingleNode($".//td[{colTarget}]/a[2]").InnerText;

                                strOddsFav = strOddsFav.Replace("&nbsp;", "");
                                strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");

                                if (winOddFav)
                                {
                                    if (decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                    {
                                        string oddPath = $"/td[{colTarget}]/a[1]";
                                        NLogger.Log(EventLevel.Trace, $"Click on odds : 002 {typeTime}");
                                        betResult = checkBet(oddPath, rowMatch.Attributes["id"].Value, hdpToFind);
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
                                    if (decimal.Parse(strOddsNoFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                    {
                                        string oddPath = $"/td[{colTarget}]/a[2]";
                                        NLogger.Log(EventLevel.Trace, $"Click on odds : 001 {typeTime}");
                                        betResult = checkBet(oddPath, rowMatch.Attributes["id"].Value, hdpToFind);
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
