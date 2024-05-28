using System;
using System.Globalization;

using HtmlAgilityPack;

using System.Collections.Generic;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Ibet789 : CasinoAPI
    {
        public override void udpateOdd(Bookmaker prm_Bookmaker, bool hedgeBet)
        {

            string url = chromeDriver.Url;

            if (url.Contains("ibet789.com/Default1_0.aspx"))
            {
                Connect();
                return;
            }

            string table1 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table1 = chromeDriver.FindElementByXPath("//*[@id='tableRun']/table[1]/tbody/tr[2]/td/table/tbody/tr[2]/td/table").GetAttribute("innerHTML");
            }
            catch
            {
                //NLogger.Log(EventLevel.Debug,"oddsTable_1_1_3_r not found ");
            }
            string table2 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table2 = chromeDriver.FindElementByXPath("//*[@id='tableToday']/table[1]/tbody/tr[2]/td/table/tbody/tr[2]/td/table").GetAttribute("innerHTML");
            }
            catch
            {
                //NLogger.Log(EventLevel.Debug,"oddsTable_1_1_3_t not found ");
            }

            string allTables = table1 + table2;

            if (allTables != "")
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(allTables);
                getOdds(prm_Bookmaker, htmlDoc, hedgeBet);
            }
            else
            {
                NLogger.Log(EventLevel.Error, "IBC05 odds table not found");
                chromeDriver.SwitchTo().Frame("fraMain");
            }

        }

        protected override void getOdds(Bookmaker prm_Bookmaker, HtmlDocument htmlDoc, bool hedgeBet)
        {
            //NLogger.Log(EventLevel.Debug,htmlDoc.DocumentNode.InnerText);

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


            var prv_RowLeagues = htmlDoc.DocumentNode.SelectNodes("./tbody");
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

                            if (hasOdd)
                            {
                                iCurrentMatch++;
                                prm_Bookmaker.BookmakerMatches.Add(new Models.Match()
                                {
                                    MatchName = currentRawMatch,
                                    MatchName2 = $"{teamNoFav} - {teamFav}",
                                    MatchDate = matchDate,
                                    MatchTime = matchTime,
                                    Matchleague = currentLeague
                                });

                                prm_Bookmaker.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = currentTeamFav;
                                prm_Bookmaker.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = currentTeamNoFav;

                                prm_Bookmaker.BookmakerMatches[iCurrentMatch].Odds = listOdds;
                            }
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
                                listOdds.Add(new Odd()
                                {
                                    oddType = overUnder,
                                    oddFav = decimal.Parse(strOddsOver, CultureInfo.InvariantCulture.NumberFormat),
                                    oddNoFav = decimal.Parse(strOddsUnder, CultureInfo.InvariantCulture.NumberFormat)
                                });
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
                                string strOddsFav = rowMatch.SelectSingleNode("./td[3]/table/tbody/tr/td[2]/table/tbody/tr[1]/td[2]/span/label").InnerText;
                                string strOddsNoFav = rowMatch.SelectSingleNode("./td[3]/table/tbody/tr/td[2]/table/tbody/tr[2]/td[2]/span/label").InnerText;

                                strOddsFav = strOddsFav.Replace("&nbsp;", "");
                                strOddsFav = strOddsFav.Replace("\n", "");
                                strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");
                                strOddsNoFav = strOddsNoFav.Replace("\n", "");

                                hasOdd = true;
                                listOdds.Add(new Odd()
                                {
                                    oddType = handicapType,
                                    oddFav = decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat),
                                    oddNoFav = decimal.Parse(strOddsNoFav, CultureInfo.InvariantCulture.NumberFormat)
                                });
                            }
                        }

                    }
                }

            }

        }
    }
}
