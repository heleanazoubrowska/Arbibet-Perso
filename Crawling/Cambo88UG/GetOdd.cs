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

        public override void udpateOdd(Bookmaker prm_Bookmaker, bool hedgeBet)
        {
            string url = chromeDriver.Url;
            if (url == "https://www.cambo88.com/index.html")
            {
                Connect();
                return;
            }
            else if (url == "https://www.cambo88.com/index.html#/live")
            {
                removingPopup();
                return;
            }

            if (isLogin)
            {
                string table = "";
                try
                {
                    WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                    wait.Until(ExpectedConditions.ElementExists(By.Id("oddGridArae")));
                    table = chromeDriver.FindElementById("oddGridArae").GetAttribute("innerHTML");

                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(table);
                    getOdds(prm_Bookmaker, htmlDoc, hedgeBet);
                }
                catch
                {
                    NLogger.Log(EventLevel.Debug, "UG grid no Live not found ");
                }
            }
            else
            {
                Login(10);
            }
        }


        protected override void getOdds(Bookmaker prm_Bookmaker, HtmlDocument htmlDoc, bool hedgeBet)
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

                    nbLine++;
                    if (!canUseCurrentLeague)
                    {
                        continue;
                    }

                    teamFav = rowMatch.SelectSingleNode(".//td[2]/p[1]/a").InnerText;
                    teamNoFav = rowMatch.SelectSingleNode(".//td[2]/p[2]/a").InnerText;
                    teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                    teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;
                    matchName = $"{teamFav} - {teamNoFav}";

                    try
                    {
                        if (rowMatch.SelectSingleNode("./td[1]/b[1]/a").InnerText.Contains("live"))
                        {
                            // Live
                            matchIsLive = true;
                        }
                    }
                    catch
                    {

                    }


                    if (currentRawMatch == "")
                    {
                        currentRawMatch = matchName;
                        currentTeamFav = teamFav;
                        currentTeamNoFav = teamNoFav;
                        currentLeague = league;
                        currentIsLive = matchIsLive;
                    }

                    if (currentRawMatch != matchName)
                    {
                        if (hasOdd)
                        {
                            iCurrentMatch++;
                            prm_Bookmaker.BookmakerMatches.Add(new Match()
                            {
                                MatchName = currentRawMatch,
                                MatchName2 = $"{teamNoFav} - {teamFav}",
                                MatchDate = matchDate,
                                MatchTime = matchTime,
                                Matchleague = currentLeague,
                                MatchIsLive = currentIsLive
                            }); ;

                            prm_Bookmaker.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = currentTeamFav;
                            prm_Bookmaker.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = currentTeamNoFav;

                            prm_Bookmaker.BookmakerMatches[iCurrentMatch].Odds = listOdds;
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
                            && 
                            rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[1]/span/a[2]") == null)
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

                        string strOddsFav = rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[2]/span/b[1]/a/span").InnerText;
                        string strOddsNoFav = rowMatch.SelectSingleNode($".//td[{iCol}]/div[1]/div[2]/span/b[2]/a/span").InnerText;
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
            //NLogger.Log(EventLevel.Info,"Cambo88 UG update " + nbLine + " rows in " + watch.ElapsedMilliseconds + " ms");
        }

    }
}
