using System;
using System.Globalization;

using HtmlAgilityPack;

using System.Collections.Generic;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System.Threading;
using System.Linq;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Aa2888 : CasinoAPI
    {
        public override void udpateOdd(Bookmaker prm_Bookmaker, bool hedgeBet)
        {

            if (isLogin)
            {
                string table1 = "";

                if (!MainClass.noLiveMatch)
                {
                    try
                    {
                        IWebElement gridFutur = chromeDriver.FindElementByXPath("//*[@id='tbl3']/tbody");
                        table1 = gridFutur.GetAttribute("innerHTML");
                    }
                    catch
                    {
                        NLogger.Log(EventLevel.Debug, "Aa2888 tbl3 not found ");
                    }
                }

                string table2 = "";
                try
                {
                    IWebElement gridFutur = chromeDriver.FindElementByXPath("//*[@id='tbl2']/tbody");
                    table2 = gridFutur.GetAttribute("innerHTML");
                }
                catch
                {
                    NLogger.Log(EventLevel.Debug, "Aa2888 tbl2 not found ");
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
                    NLogger.Log(EventLevel.Error, "Aa2888 odds table not found");
                }


                //try
                //{

                //    IWebElement gridFutur = chromeDriver.FindElementByXPath("//*[@id='tbl2']/tbody");

                //    string html = gridFutur.GetAttribute("innerHTML");

                //    HtmlDocument htmlDoc = new HtmlDocument();
                //    htmlDoc.LoadHtml(html);

                //    getOdds(aa2888, htmlDoc);
                //}
                //catch (NoSuchElementException elemNotFound)
                //{
                //    NLogger.Log(EventLevel.Error,"Aa2888 Update Odd Table not found : " + elemNotFound);
                //}
            }
            else
            {
                try
                {
	                Login(2);

	                chromeDriver.SwitchTo().Frame("frame_main_content");
                    chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                    chromeDriver.SwitchTo().Frame("frame_sports_main");
                    isLogin = true;
                }
                catch
                {
                    NLogger.Log(EventLevel.Error, "Aa2888 Update Odd Login not found");
                }
            }
        }

        protected override void getOdds(Bookmaker prm_Bookmaker, HtmlDocument htmlDoc, bool hedgeBet)
        {

            //NLogger.Log(EventLevel.Debug,htmlDoc.DocumentNode.InnerText);

            string currentRawMatch = "";
            int iCurrentMatch = -1;
            string matchName = "";
            string teamFav = "";
            string teamNoFav = "";
            string matchDate = "";
            string matchTime = "";
            bool matchIsLive = false;
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
            bool currentIsLive = false;

            //var watch = System.Diagnostics.Stopwatch.StartNew();
            int nbLine = 0;
            var prv_Rows = htmlDoc.DocumentNode.SelectNodes("//tr");
            int prv_NumRows = prv_Rows.Count;

            //foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes("//tr"))
            for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
            {
	            var rowMatch = prv_Rows[Loopy];
                if (rowMatch.Descendants("th").Any()) // Is line League
                {
                    canUseCurrentLeague = true;
                    league = rowMatch.SelectSingleNode(".//th[2]").InnerText;
                    league = league.Replace("&nbsp;", "");

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
                else
                {
                    if (!canUseCurrentLeague)
                    {
                        continue;
                    }

                    try
                    {
                        nbLine++;
                        // Check if match name is on this line
                        if (rowMatch.SelectNodes(".//td").Count == 7)
                        {
                            colOddPos = 3;

                            if (currentRawMatch == "")
                            {
                                currentLeague = league;
                                currentLeagueType = typeLeague;
                            }
                            else
                            {   // We check if we need to add the new match
                                if (hasOdd)
                                {
                                    iCurrentMatch++;
                                    prm_Bookmaker.BookmakerMatches.Add(new Models.Match()
                                    {
                                        MatchName = matchName,
                                        MatchName2 = $"{teamNoFav} - {teamFav}",

                                        MatchDate = matchDate,
                                        MatchTime = matchTime,
                                        Matchleague = currentLeague,
                                        MatchIsLive = matchIsLive
                                    });

                                    prm_Bookmaker.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = teamFav;
                                    prm_Bookmaker.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = teamNoFav;

                                    prm_Bookmaker.BookmakerMatches[iCurrentMatch].Odds = listOdds;
                                }

                                hasOdd = false;
                                listOdds = new List<Odd>();
                                listHdp = new List<string>();

                            }

                            // Get the position of the favorite

                            teamFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/span[1]").InnerText;
                            teamNoFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/span[2]").InnerText;

                            teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                            teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;

                            matchName = $"{teamFav} - {teamNoFav}";

                            matchTime = rowMatch.SelectSingleNode(".//td[1]").InnerHtml;

                            try
                            {
                                if (matchTime.Contains("span"))
                                {
                                    matchIsLive = true;
                                }
                            }
                            catch
                            {

                            }



                            //NLogger.Log(EventLevel.Debug,matchName);
                            currentRawMatch = matchName;
                            currentLeague = league;
                            currentLeagueType = typeLeague;
                        }
                        else
                        {
                            colOddPos = 1;
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
                            if (rowMatch.SelectSingleNode($".//td[{colTarget}]/span[1]") == null 
                                && rowMatch.SelectSingleNode($".//td[{colTarget}]/span[2]") == null)
                            {
                                continue;
                            }

                            // Get the position of the favorite
                            if (rowMatch.SelectSingleNode($".//td[{colTarget}]/span[1]").InnerText == "" 
                                || rowMatch.SelectSingleNode($".//td[{colTarget}]/span[1]").InnerText == "&nbsp;")
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

                            string strOddsFav = rowMatch.SelectSingleNode($".//td[{colTarget}]/a[1]").InnerText;
                            string strOddsNoFav = rowMatch.SelectSingleNode($".//td[{colTarget}]/a[2]").InnerText;

                            strOddsFav = strOddsFav.Replace("&nbsp;", "");
                            strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");

                            hasOdd = true;
                            listOdds.Add(new Odd()
                            {
                                oddType = handicapType,
                                oddFav = decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat),
                                oddNoFav = decimal.Parse(strOddsNoFav, CultureInfo.InvariantCulture.NumberFormat)
                            });
                        }
                    }
                    catch
                    {

                    }
                }
                //NLogger.Log(EventLevel.Debug,JsonConvert.SerializeObject(aa2888.BookmakerMatches[iCurrentMatch], Formatting.Indented));
            }
            //watch.Stop();
            //NLogger.Log(EventLevel.Info,"Aa2888 update " + nbLine + " rows in " + watch.ElapsedMilliseconds + " ms");

        }
    }
}
