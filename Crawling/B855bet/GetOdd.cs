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
    public partial class B855bet : CasinoAPI
    {
        public override void udpateOdd(Bookmaker b855bet, bool hedgeBet)
        {
            chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            string table1 = "";

            if (!MainClass.noLiveMatch)
            {
                try
                {
                    WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                    table1 = chromeDriver.FindElementById("oTableContainer_L").GetAttribute("innerHTML");
                }
                catch
                {
                    NLogger.Log(EventLevel.Debug, "oTableContainer_L not found ");
                }
            }

            string table2 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table2 = chromeDriver.FindElementById("oTableContainer_C").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Debug,"oTableContainer_C not found ");
            }
            string table3 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table3 = chromeDriver.FindElementById("oTableContainer_D").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Debug,"oTableContainer_D not found ");
            }

            string allTables = $"{table1}{table2}{table3}";

            if (allTables == "")
            {
                chromeDriver.SwitchTo().DefaultContent();
                chromeDriver.SwitchTo().Frame("mainFrame");
            }
            else
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(allTables);
                getOdds(b855bet, htmlDoc, hedgeBet);
            }
        }

        protected override void getOdds(Bookmaker b855bet, HtmlDocument htmlDoc, bool hedgeBet)
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


            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();
            string league = "";
            string typeLeague = "";
            string currentLeague = "";
            string currentLeagueType = "";
            bool canUseCurrentLeague = true;
            bool currentIsLive = false;

            //var watch = System.Diagnostics.Stopwatch.StartNew();
            int nbLine = 0;

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
                        for (int LeagueCounter = 0; LeagueCounter < prv_NumUselessLeagues; LeagueCounter++)
                        {
	                        if (league.Contains(Config.Config.uselessLeague[LeagueCounter]))
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

                    nbLine++;
                    if (rowMatch.SelectNodes(".//td").Count > 1)
                    {

                        teamFav = rowMatch.SelectSingleNode(".//td[3]/div[1]").InnerText;
                        teamNoFav = rowMatch.SelectSingleNode(".//td[3]/div[2]").InnerText;

                        teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                        teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;

                        matchName = $"{teamFav} - {teamNoFav}";

                        if(rowMatch.SelectSingleNode("./td[1]/div") != null)
                        {
                            // Is live
                            matchIsLive = true;
                        }

                        if (currentRawMatch == "")
                        {
                            currentRawMatch = matchName;
                            currentTeamFav = teamFav;
                            currentTeamNoFav = teamNoFav;
                            currentLeague = league;
                            currentLeagueType = typeLeague;
                            currentIsLive = matchIsLive;
                        }

                        if (currentRawMatch != matchName)
                        {

                            if (hasOdd)
                            {
                                iCurrentMatch++;
                                b855bet.BookmakerMatches.Add(new Models.Match()
                                {
                                    MatchName = currentRawMatch,
                                    MatchName2 = $"{teamNoFav} - {teamFav}",
                                    MatchDate = matchDate,
                                    MatchTime = matchTime,
                                    Matchleague = currentLeague,
                                    MatchIsLive = currentIsLive
                                });

                                b855bet.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = currentTeamFav;
                                b855bet.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = currentTeamNoFav;

                                b855bet.BookmakerMatches[iCurrentMatch].Odds = listOdds;
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
                                        hasOdd = true;
                                        listOdds.Add(new Odd()
                                        {
                                            oddType = overUnder,
                                            oddFav = decimal.Parse(strOddsOver, CultureInfo.InvariantCulture.NumberFormat),
                                            oddNoFav = decimal.Parse(strOddsUnder, CultureInfo.InvariantCulture.NumberFormat)
                                        });
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
                                if (rowMatch.SelectSingleNode($".//td[{iCol}]/div[2]/a[1]") != null)
                                {
                                    string strOddsFav = rowMatch.SelectSingleNode($".//td[{iCol}]/div[2]/a[1]").InnerText;
                                    string strOddsNoFav = rowMatch.SelectSingleNode($".//td[{iCol}]/div[2]/a[2]").InnerText;

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
            //watch.Stop();
            //NLogger.Log(EventLevel.Info,"855bet update " + nbLine + " rows in " + watch.ElapsedMilliseconds + " ms");
        }
    }
}
