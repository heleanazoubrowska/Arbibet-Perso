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
    public partial class B855bet : CasinoAPI
    {

        protected override BetResult findBet(HtmlDocument htmlDoc, string matchToFind, string hdpToFind, decimal winOddToFind, bool winOddFav, bool hedgeBet)
        {
            string currentRawMatch = "";
            string currentTeamFav = "";
            string currentTeamNoFav = "";
            string league = "";

            int iCurrentMatch = -1;
            string matchName = "";
            string teamFav = "";
            string teamNoFav = "";
            string matchDate = "";
            string matchTime = "";


            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            string typeLeague = "";
            string currentLeague = "";
            string currentLeagueType = "";
            bool canUseCurrentLeague = true;

            BetResult betResult = new BetResult();

            var prv_Rows = htmlDoc.DocumentNode.SelectNodes(".//tr");
            int prv_NumRows = prv_Rows.Count;

            //foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes(".//tr"))
            for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
            {
                var rowMatch = prv_Rows[Loopy];
                if (rowMatch.SelectSingleNode(".//td[1]/div[1]") != null)
                {
                    if (rowMatch.SelectSingleNode(".//td[1]/div[1]").Attributes["class"].Value == "leagueName") // Is line League
                    {
                        canUseCurrentLeague = true;
                        league = rowMatch.SelectSingleNode(".//td/div").InnerText;
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
                }
                else if (rowMatch.SelectSingleNode(".//td[1]") != null)
                {

                    if (!canUseCurrentLeague)
                    {
                        continue;
                    }

                    if (rowMatch.SelectNodes(".//td").Count > 1)
                    {

                        teamFav = rowMatch.SelectSingleNode(".//td[3]/div[1]").InnerText;
                        teamNoFav = rowMatch.SelectSingleNode(".//td[3]/div[2]").InnerText;

                        teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                        teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;

                        matchName = $"{teamFav} - {teamNoFav}";
                        if (currentRawMatch == "")
                        {
                            currentRawMatch = matchName;
                            currentTeamFav = teamFav;
                            currentTeamNoFav = teamNoFav;
                            currentLeague = league;
                            currentLeagueType = typeLeague;
                        }

                        if (currentRawMatch != matchName)
                        {

                            currentRawMatch = matchName;
                            currentTeamFav = teamFav;
                            currentTeamNoFav = teamNoFav;
                            currentLeague = league;
                            hasOdd = false;
                            listOdds = new List<Odd>();
                            listHdp = new List<string>();
                        }

                        if (currentRawMatch == matchToFind)
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
                                    iCol = 7;
                                    typeTime = "ft";
                                }
                                else
                                {
                                    iCol = 10;
                                    typeTime = "fh";
                                }


                                if (rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]") != null)
                                {
                                    string overUnder = rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]").InnerText;
                                    overUnder = overUnder.Replace("/", "-");
                                    overUnder = overUnder.Replace("<br>", "");
                                    overUnder = overUnder.Replace("&nbsp;", "");
                                    overUnder = overUnder.Replace("\n", "");
                                    if (overUnder != "")
                                    {
                                        string strOddsOver = rowMatch.SelectSingleNode($".//td[{iCol}]/div[2]/a[1]").InnerText;
                                        string strOddsUnder = rowMatch.SelectSingleNode($".//td[{iCol}]/div[2]/a[2]").InnerText;
                                        if (overUnder != "")
                                        {
                                            overUnder = overUnder.Replace("/", "-");
                                            overUnder = $"{typeTime} o/u {overUnder}";
                                            strOddsOver = strOddsOver.Replace("&nbsp;", "");
                                            strOddsOver = strOddsOver.Replace("\n", "");
                                            strOddsUnder = strOddsUnder.Replace("&nbsp;", "");
                                            strOddsUnder = strOddsUnder.Replace("\n", "");
                                            if (overUnder == hdpToFind)
                                            {
                                                if (winOddFav)
                                                {

                                                    NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsOver}");

                                                    if (decimal.Parse(strOddsOver, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                                    {
                                                        string oddPath = $"/td[{iCol}]/div[2]/a[1]";
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
                                                        string oddPath = $"/td[{iCol}]/div[2]/a[2]";
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
                            }




                            // ++++++++++++++++++++
                            // HANDICAP ODD FT & FH
                            // ++++++++++++++++++++

                            for (int i = 0; i < 2; i++)
                            {
                                if (i == 0)
                                {
                                    iCol = 6;
                                    typeTime = "ft";
                                }
                                else
                                {
                                    iCol = 9;
                                    typeTime = "fh";
                                }

                                if (rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]").InnerText != "")
                                {
                                    string sensHdp = "";

                                    // Get the position of the favorite

                                    string handicapType = rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]").InnerText;
                                    if (handicapType == "")
                                    {
                                        continue;
                                    }

                                    string oddTypeHtml = rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]").InnerHtml;
                                    int index1 = oddTypeHtml.IndexOf("<br>");
                                    if (index1 == 1)
                                    {
                                        sensHdp = "- ";
                                    }
                                    else
                                    {
                                        sensHdp = "+ ";
                                    }

                                    handicapType = handicapType.Replace("/", "-");
                                    handicapType = handicapType.Replace("<br>", "");
                                    handicapType = handicapType.Replace("&nbsp;", "");
                                    handicapType = handicapType.Replace("\n", "");

                                    if (handicapType != "0")
                                    {
                                        handicapType = sensHdp + handicapType;
                                    }
                                    handicapType = $"{typeTime} {handicapType}";
                                    if (handicapType == hdpToFind)
                                    {
                                        string strOddsFav = rowMatch.SelectSingleNode($".//td[{iCol}]/div[2]/a[1]").InnerText;
                                        string strOddsNoFav = rowMatch.SelectSingleNode($".//td[{iCol}]/div[2]/a[2]").InnerText;

                                        strOddsFav = strOddsFav.Replace("&nbsp;", "");
                                        strOddsFav = strOddsFav.Replace("\n", "");
                                        strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");
                                        strOddsNoFav = strOddsNoFav.Replace("\n", "");

                                        if (winOddFav)
                                        {
                                            NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsFav}");

                                            if (decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                            {
                                                string oddPath = $"/td[{iCol}]/div[2]/a[1]";
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
                                            NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsNoFav}");

                                            if (decimal.Parse(strOddsNoFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                            {
                                                string oddPath = $"/td[{iCol}]/div[2]/a[2]";
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
