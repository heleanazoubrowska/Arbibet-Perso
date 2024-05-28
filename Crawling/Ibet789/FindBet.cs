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
    public partial class Ibet789 : CasinoAPI
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
            int nbLine = 0;

            BetResult betResult = new BetResult();

            
            var prv_RowLeagues = htmlDoc.DocumentNode.SelectNodes("//tbody");
            int prv_NumRowLeagues = prv_RowLeagues.Count;

            //foreach (HtmlNode rowLeage in htmlDoc.DocumentNode.SelectNodes("./tbody"))
            for (int LeagueCounter = 0; LeagueCounter < prv_NumRowLeagues; LeagueCounter++)
            {
	            var rowLeague = prv_RowLeagues[LeagueCounter];
                //NLogger.Log(EventLevel.Debug,rowLeage.InnerHtml);
                //if(rowLeage.Attributes["soclid"] == null)
                //{
                //    continue;
                //}

                
                var prv_RowMatches = rowLeague.SelectNodes("./tr");
                int prv_NumRowMatches = prv_RowMatches.Count;

                //foreach (HtmlNode rowMatch in rowLeague.SelectNodes("./tr"))
                for (int RowMatchCounter = 0; RowMatchCounter < prv_NumRowMatches; RowMatchCounter++)
                {
	                var rowMatch = prv_RowMatches[RowMatchCounter];
                    //NLogger.Log(EventLevel.Debug,rowMatch.InnerHtml);
                    //NLogger.Log(EventLevel.Debug,"+++++++++++++++++++++++++");

                    if (rowMatch.Attributes["class"] == null)
                    {
                        continue;
                    }

                    if (rowMatch.Attributes["class"].Value == "event" || rowMatch.Attributes["class"].Value == "EventRun") // Row League
                    {
                        canUseCurrentLeague = true;
                        league = rowMatch.SelectSingleNode("./td/table/tbody/tr/td[2]/div[1]").InnerText;
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

                    if (rowMatch.Attributes["oddsid"] != null)
                    {
                        teamFav = rowMatch.SelectSingleNode("./td[2]/table/tbody/tr[1]/td[2]/span").InnerText;
                        teamNoFav = rowMatch.SelectSingleNode("./td[2]/table/tbody/tr[2]/td[2]/span").InnerText;

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

                        // ++++++++++++++++++++
                        // OVER UNDER ODD FT & FH
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

                            string rowOverUnder = rowMatch.SelectSingleNode("./td[4]/table/tbody/tr[1]/td[1]").InnerText;
                            rowOverUnder = rowOverUnder.Replace("o", "");
                            rowOverUnder = rowOverUnder.Replace("<br>", "");
                            rowOverUnder = rowOverUnder.Replace("&nbsp;", "");
                            rowOverUnder = rowOverUnder.Replace(" ", "");

                            string overUnder = rowOverUnder;

                            if (rowOverUnder != "")
                            {
                                string strOddsOver = rowMatch.SelectSingleNode($"./td[{iCol}]/table/tbody/tr[1]/td[2]/span/label").InnerText;
                                string strOddsUnder = rowMatch.SelectSingleNode($"./td[{iCol}]/table/tbody/tr[2]/td[2]/span/label").InnerText;

                                overUnder = overUnder.Replace("/", "-");
                                overUnder = $"{typeTime} o/u {overUnder}";
                                strOddsOver = strOddsOver.Replace("&nbsp;", "");
                                strOddsOver = strOddsOver.Replace("\n", "");
                                strOddsUnder = strOddsUnder.Replace("&nbsp;", "");
                                strOddsUnder = strOddsUnder.Replace("\n", "");
                                hasOdd = true;

                                if (overUnder == hdpToFind)
                                {
                                    if (winOddFav)
                                    {
                                        if (decimal.Parse(strOddsOver, CultureInfo.InvariantCulture.NumberFormat) >= 0)
                                        {
                                            string oddPath = $"/td[{iCol}]/table/tbody/tr[1]/td[2]/span";
                                            NLogger.Log(EventLevel.Trace, $"Click on odds : 004 {typeTime}");
                                            betResult = checkBet(oddPath, rowMatch.Attributes["oddsid"].Value, hdpToFind);
                                            return betResult;
                                        }
                                        else
                                        {
                                            betResult.BetStatus = 1;
                                            betResult.BetIsConfirmed = false;
                                            betResult.BetIsPlaced = false;
                                            betResult.BetMessage = "Odd from panel > 1";
                                            return betResult;
                                        }
                                    }
                                    else
                                    {
                                        if (decimal.Parse(strOddsUnder, CultureInfo.InvariantCulture.NumberFormat) >= 0)
                                        {
                                            string oddPath = $"/td[{iCol}]/table/tbody/tr[2]/td[2]/span";
                                            NLogger.Log(EventLevel.Trace, $"Click on odds : 003 {typeTime}");
                                            betResult = checkBet(oddPath, rowMatch.Attributes["oddsid"].Value, hdpToFind);
                                            return betResult;
                                        }
                                        else
                                        {
                                            betResult.BetStatus = 1;
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
                        // HANDICAP ODD FT & FH
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
                                iCol = 5;
                                typeTime = "fh";
                            }

                            string handicap1 = rowMatch.SelectSingleNode("./td[3]/table/tbody/tr/td[2]/table/tbody/tr[1]/td[1]").InnerText;
                            string handicap2 = rowMatch.SelectSingleNode("./td[3]/table/tbody/tr/td[2]/table/tbody/tr[2]/td[1]").InnerText;

                            handicap1 = handicap1.Replace("&nbsp;", "");
                            handicap1 = handicap1.Replace(" ", "");

                            handicap2 = handicap2.Replace("&nbsp;", "");
                            handicap2 = handicap2.Replace(" ", "");

                            string handicapType = "";

                            if (handicap1 != "" || handicap1 != "")
                            {
                                string sensHdp = "";

                                // Get the position of the favorite

                                if (handicap1 == "")
                                {
                                    sensHdp = "- ";
                                    handicapType = handicap2;
                                }
                                else
                                {
                                    sensHdp = "+ ";
                                    handicapType = handicap1;
                                }


                                if (handicapType != "0")
                                {
                                    handicapType = sensHdp + handicapType;
                                }
                                handicapType = $"{typeTime} {handicapType}";


                                if (handicapType == hdpToFind)
                                {
                                    string strOddsFav = rowMatch.SelectSingleNode("./td[3]/table/tbody/tr/td[2]/table/tbody/tr[1]/td[2]/span/label").InnerText;
                                    string strOddsNoFav = rowMatch.SelectSingleNode("./td[3]/table/tbody/tr/td[2]/table/tbody/tr[2]/td[2]/span/label").InnerText;

                                    strOddsFav = strOddsFav.Replace("&nbsp;", "");
                                    strOddsFav = strOddsFav.Replace("\n", "");
                                    strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");
                                    strOddsNoFav = strOddsNoFav.Replace("\n", "");

                                    if (winOddFav)
                                    {
                                        if (decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat) >= 0)
                                        {
                                            string oddPath = "/td[3]/table/tbody/tr/td[2]/table/tbody/tr[1]/td[2]/span";
                                            NLogger.Log(EventLevel.Trace, $"Click on odds : 002 {typeTime}");
                                            betResult = checkBet(oddPath, rowMatch.Attributes["oddsid"].Value, hdpToFind);
                                            return betResult;
                                        }
                                        else
                                        {
                                            betResult.BetStatus = 1;
                                            betResult.BetIsConfirmed = false;
                                            betResult.BetIsPlaced = false;
                                            betResult.BetMessage = "Odd from panel > 1";
                                            return betResult;
                                        }
                                    }
                                    else
                                    {
                                        if (decimal.Parse(strOddsNoFav, CultureInfo.InvariantCulture.NumberFormat) >= 0)
                                        {
                                            string oddPath = "/td[3]/table/tbody/tr/td[2]/table/tbody/tr[2]/td[2]/span";
                                            NLogger.Log(EventLevel.Trace, $"Click on odds : 001 {typeTime}");
                                            betResult = checkBet(oddPath, rowMatch.Attributes["oddsid"].Value, hdpToFind);
                                            return betResult;
                                        }
                                        else
                                        {
                                            betResult.BetStatus = 1;
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
            betResult.BetStatus = 1;
            betResult.BetIsConfirmed = false;
            betResult.BetIsPlaced = false;
            betResult.BetMessage = "Match and odd not founded";
            return betResult;
        }
    }
}
