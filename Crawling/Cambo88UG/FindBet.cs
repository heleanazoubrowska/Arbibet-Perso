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
	public partial class Cambo88UG : CasinoAPI
	{
		protected override BetResult findBet(HtmlDocument htmlDoc, string matchToFind, string hdpToFind, decimal winOddToFind, bool winOddFav, bool hedgeBet)
        {

            string currentRawMatch = "";
            string currentTeamFav = "";
            string currentTeamNoFav = "";

            int iCurrentMatch = -1;
            string matchName = "";
            string teamFav = "";
            string teamNoFav = "";
            string matchDate = "";
            string matchTime = "";
            bool matchIsLive = false;

            string league = "";
            bool canUseCurrentLeague = true;
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
                    league = rowMatch.SelectSingleNode(".//th[2]/div").InnerText;
                    league = league.ToUpper();

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
                else // is Match line
                {

                    if (rowMatch.SelectSingleNode(".//td[1]").InnerText.Contains("Time"))
                    {
                        continue;
                    }

                    if (!canUseCurrentLeague)
                    {
                        continue;
                    }

                    teamFav = rowMatch.SelectSingleNode(".//td[2]/p[1]/a").InnerText;
                    teamNoFav = rowMatch.SelectSingleNode(".//td[2]/p[2]/a").InnerText;
                    teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                    teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;
                    matchName = $"{teamFav} - {teamNoFav}";


                    if (currentRawMatch == "")
                    {
                        currentRawMatch = matchName;
                        currentTeamFav = teamFav;
                        currentTeamNoFav = teamNoFav;
                        currentLeague = league;
                    }

                    if (currentRawMatch != matchName)
                    {

                        currentRawMatch = matchName;
                        currentTeamFav = teamFav;
                        currentTeamNoFav = teamNoFav;
                        currentLeague = league;
                    }

                   // NLogger.Log(EventLevel.Notice, $"UG matchName : {matchName} - matchToFind : {matchToFind}");
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
                                iCol = 4;
                                typeTime = "ft";
                            }
                            else
                            {
                                iCol = 6;
                                typeTime = "fh";
                            }

                            if (rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[3]/span/a[1]") != null 
                                && 
                                rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[3]/span/a[2]") != null)
                            {
                                string overUnder = rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[3]/span/a[1]").InnerText;
                                string strOddsOver = rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[4]/span/b[1]/a/span").InnerText;
                                string strOddsUnder = rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[4]/span/b[2]/a/span").InnerText;
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
                                            string oddPath = $"/td[{iCol}]/div[1]/div[4]/span/b[1]/a";
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
                                        NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsUnder}");

                                        if (decimal.Parse(strOddsUnder, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                        {
                                            string oddPath = $"/td[{iCol}]/div[1]/div[4]/span/b[2]/a";
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
                                iCol = 6;
                                typeTime = "fh";
                            }

                            string sensHdp = "";
                            int posHdp = 0;

                            if (rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[1]/span/a[1]") == null 
                                && rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[1]/span/a[2]") == null)
                            {
                                continue;
                            }

                            // Get the position of the favorite
                            if (rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[1]/span/a[1]").InnerText == "" 
                                || 
                                rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[1]/span/a[1]").InnerText == "&nbsp;")
                            {
                                sensHdp = "- ";
                                posHdp = 2;
                            }
                            else
                            {
                                sensHdp = "+ ";
                                posHdp = 1;
                            }

                            string handicapType = rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[1]/span/a[{posHdp}]").InnerText;
                            handicapType = handicapType.Replace("&nbsp;", "");
                            handicapType = handicapType.Replace("/", "-");

                            if (handicapType != "0")
                            {
                                handicapType = sensHdp + handicapType;
                            }

                            handicapType = $"{typeTime} {handicapType}";

                            if (handicapType == hdpToFind)
                            {
                                string strOddsFav = rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[2]/span/b[1]/a/span").InnerText;
                                string strOddsNoFav = rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[2]/span/b[2]/a/span").InnerText;
                                strOddsFav = strOddsFav.Replace("&nbsp;", "");
                                strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");

                                if (winOddFav)
                                {
                                    NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsFav}");

                                    if (decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                    {
                                        string oddPath = $"/td[{iCol}]/div[1]/div[2]/span/b[1]/a";
                                        NLogger.Log(EventLevel.Trace, $"Click on odds : 002 {handicapType}");
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
                                    NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsNoFav}");

                                    if (decimal.Parse(strOddsNoFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                    {
                                        string oddPath = $"/td[{iCol}]/div[1]/div[2]/span/b[2]/a";
                                        NLogger.Log(EventLevel.Trace, $"Click on odds : 001 {handicapType}");
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
