using System;
using System.Globalization;

using HtmlAgilityPack;

using System.Collections.Generic;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using OpenQA.Selenium;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Nba369 : CasinoAPI
    {
        public override void udpateOdd(Bookmaker nba369In, bool hedgeBet)
        {

            string url = chromeDriver.Url;

            if (url == "http://www.nba369.com/")
            {
                Connect();
                return;
            }
            else if (url == "http://www.nba369.com/home/")
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("tabsport")));
                chromeDriver.FindElementById("tabsport").Click();
                return;
            }

            string table1 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table1 = chromeDriver.FindElementByXPath("//*[@id='main-content']/div[1]/div[4]/div[2]").GetAttribute("innerHTML");
                //table1 = chromeDriver.FindElementById("today-content").GetAttribute("innerHTML");
                //
            }
            catch
            {
                //NLogger.Log(EventLevel.Debug,"oddsTable_1_1_3_r not found ");
            }

            if (table1 != "")
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(table1);
                getOdds(nba369In, htmlDoc, hedgeBet);
            }
            else
            {
                NLogger.Log(EventLevel.Error, "Nba369 Odd Table not found : ");
            }

            //try
            //{
            //    IWebElement gridFutur = chromeDriver.FindElementById("tbltoday-content");

            //    string html = gridFutur.GetAttribute("innerHTML");

            //    HtmlDocument htmlDoc = new HtmlDocument();
            //    htmlDoc.LoadHtml(html);

            //    getOdds(ibc05, htmlDoc);
            //}

            //catch (NoSuchElementException elemNotFound)
            //{
            //    NLogger.Log(EventLevel.Error,"Nba369 Odd Table not found : " + elemNotFound);
            //}
        }

        protected override void getOdds(Bookmaker nba369, HtmlDocument htmlDoc, bool hedgeBet)
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
            bool matchIsLive = false;

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
            bool currentIsLive = false;

            //var watch = System.Diagnostics.Stopwatch.StartNew();
            int nbLine = 0;

            
            var prv_RowMatches = htmlDoc.DocumentNode.SelectNodes("//tr");
            int prv_NumRowMatches = prv_RowMatches.Count;

            //foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes("//tr"))
            for (int RowMatchCounter = 0; RowMatchCounter < prv_NumRowMatches; RowMatchCounter++)
            {
	            var rowMatch = prv_RowMatches[RowMatchCounter];

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

                    nbLine++;
                    // Check if match name is on this line
                    if (rowMatch.SelectNodes(".//td").Count == 7)
                    {
                        teamFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/span[1]").InnerText;
                        teamNoFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/span[3]").InnerText;
                        teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                        teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;
                        matchName = $"{teamFav} - {teamNoFav}";

                        if (rowMatch.SelectSingleNode("./td[1]/span") != null)
                        {
                            // Live
                            matchIsLive = true;
                        }

                        colOddPos = 3;

                        if (currentRawMatch == "")
                        {
                            currentRawMatch = matchName;
                            currentTeamFav = teamFav;
                            currentTeamNoFav = teamNoFav;
                            currentLeague = league;
                            currentIsLive = matchIsLive;
                        }
                        if (currentRawMatch != matchName)
                        {   // We check if we need to add the new match
                            if (hasOdd)
                            {
                                iCurrentMatch++;
                                nba369.BookmakerMatches.Add(new Models.Match()
                                {
                                    MatchName = currentRawMatch,
                                    MatchName2 = $"{teamNoFav} - {teamFav}",
                                    MatchDate = matchDate,
                                    MatchTime = matchTime,
                                    Matchleague = currentLeague,
                                    MatchIsLive = currentIsLive
                                });

                                nba369.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = currentTeamFav;
                                nba369.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = currentTeamNoFav;

                                nba369.BookmakerMatches[iCurrentMatch].Odds = listOdds;
                            }
                            currentRawMatch = matchName;
                            currentTeamFav = teamFav;
                            currentTeamNoFav = teamNoFav;
                            currentLeague = league;
                            currentIsLive = matchIsLive;
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

                        string sensHdp = "";
                        int posHdp = 0;

                        int prv_ColIndex = colOddPos + iCol;

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

                        // Check if we need this handicap type
                        //if (handicapType != "0" && handicapType != "0.5" && handicapType != "1" && handicapType != "1.5" && handicapType != "2" && handicapType != "2.5" && handicapType != "3")
                        //{
                        //    continue;
                        //}

                        string strOddsFav = rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/a[1]").InnerText;
                        string strOddsNoFav = rowMatch.SelectSingleNode($".//td[{prv_ColIndex}]/a[2]").InnerText;

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
            }
            //watch.Stop();
            //NLogger.Log(EventLevel.Info,"Nba369 update " + nbLine + " rows in " + watch.ElapsedMilliseconds + " ms");
        }
    }
}
