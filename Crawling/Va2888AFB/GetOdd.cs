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
	public partial class Va2888AFB : CasinoAPI
	{

        public override void udpateOdd(Bookmaker prm_Bookmaker, bool hedgeBet)
        {
            string table1 = "";
            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("oddsTable")));
            table1 = chromeDriver.FindElementById("oddsTable").GetAttribute("innerHTML");

            if (table1 != "")
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(table1);
                getOdds(prm_Bookmaker, htmlDoc, hedgeBet);
            }
            else
            {
                NLogger.Log(EventLevel.Debug, "No table found on VA28888AFB ");
                //chromeDriver.SwitchTo().Frame(0);
            }
        }

        protected override void getOdds(Bookmaker prm_Bookmaker, HtmlDocument htmlDoc, bool hedgeBet)
        {
            string currentRawMatch = "";
            int currentTeamFavPos = 0;
            int currentTeamNoFavPos = 0;
            int iCurrentMatch = -1;
            string matchName = "";
            string teamFav = "";
            string teamNoFav = "";
            string matchDate = "";
            string matchTime = "";
            string currentTeamFav = "";
            string currentTeamNoFav = "";
            bool matchIsLive = false;

            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            string league = "";
            string typeLeague = "";
            string currentLeague = "";
            string currentLeagueType = "";
            string currentSensHdp = "";
            bool canUseCurrentLeague = true;
            bool currentIsLive = false;

            try
            {
                //var watch = System.Diagnostics.Stopwatch.StartNew();
                int nbLine = 0;
                foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes(".//tr"))
                {

                    if (rowMatch.Attributes["class"] == null)
                    {
                        continue;
                    }

                    if (rowMatch.Attributes["class"].Value.Contains("oddsLeague"))
                    {
                        canUseCurrentLeague = true;
                        league = rowMatch.SelectSingleNode(".//td[2]/span[2]").InnerText;
                        league.ToUpper();

                        foreach (string str in Config.Config.uselessLeague)
                        {
                            if (league.Contains(str))
                            {
                                canUseCurrentLeague = false;
                                continue;
                            }
                        }

                        typeLeague = Common.getLeagueType(league);

                    }
                    else if (rowMatch.Attributes["class"].Value.Contains("oddsRow"))
                    {
                        if (!canUseCurrentLeague)
                        {
                            continue;
                        }


                        teamFav = rowMatch.SelectSingleNode(".//td[3]/div[2]/span/span[@class='HomeNamePC']").InnerText;
                        teamNoFav = rowMatch.SelectSingleNode(".//td[3]/div[3]/span/span[@class='AwayNamePC']").InnerText;

                        teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                        teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;

                        matchName = $"{teamFav} - {teamNoFav}";

                        try
                        {
                            if (rowMatch.SelectSingleNode("./td[2]/div/span/font") == null)
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
                            currentLeagueType = typeLeague;
                            currentIsLive = matchIsLive;
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
                                    Matchleague = currentLeague,
                                    MatchIsLive = currentIsLive
                                });

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
                        // OVER UNDER ODD
                        // ++++++++++++++++++++

                        int iCol;
                        string typeTime = "";

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

                            if (rowMatch.SelectSingleNode(".//td["+ iCol + "]/div[1]/span[1]/span[2]") != null)
                            {

                                string overUnder = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[1]/span[1]/span[2]").InnerText;
                                string strOddsOver = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[1]/span[2]/span[1]").InnerText;
                                string strOddsUnder = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[2]/span[2]/span[1]").InnerText;
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
                                iCol = 5;
                                typeTime = "ft";
                            }
                            else
                            {
                                iCol = 8;
                                typeTime = "fh";
                            }

                            string sensHdp = "";
                            //if (rowMatch.SelectSingleNode(".//td[" + colOddPos + "]/span[1]") == null && rowMatch.SelectSingleNode(".//td[" + colOddPos + "]/span[2]") == null)
                            //{
                            //    continue;
                            //}

                            // Get the position of the favorite

                            string handicapType = "";
                            if (handicapType == "")
                            {
                                continue;
                            }

                            if (rowMatch.SelectSingleNode(".//td["+ iCol + "]/div[1]/span[1]/span") != null)
                            {
                                handicapType = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[1]/span[1]/span").InnerText;
                                sensHdp = "+ ";
                            }
                            else if (rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[2]span[1]/span") != null)
                            {
                                handicapType = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[2]span[1]/span").InnerText;
                                sensHdp = "- ";
                            }
                            else
                            {
                                continue;
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
                            if (rowMatch.SelectSingleNode(".//td[" + (iCol + 1) + "]/div[2]/a[1]") == null)
                            {
                                continue;
                            }

                            string strOddsFav = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[1]/span[2]/span").InnerText;
                            string strOddsNoFav = rowMatch.SelectSingleNode(".//td[" + iCol + "]/div[2]/span[2]/span").InnerText;

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
                //watch.Stop();
                //NLogger.Log(EventLevel.Info,"Va28888 update "+ nbLine +" rows in " + watch.ElapsedMilliseconds + " ms");

            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error update Va28888");
            }
        }
    }
}
