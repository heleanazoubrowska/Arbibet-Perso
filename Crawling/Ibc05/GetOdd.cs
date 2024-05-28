using System;
using System.Globalization;

using HtmlAgilityPack;

using System.Collections.Generic;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Ibc05 : CasinoAPI
    {
        public override void udpateOdd(Bookmaker prm_Bookmaker, bool hedgeBet)
        {

            string url = chromeDriver.Url;

            if (url == "http://www.ibc05.com/index.php")
            {
                Connect();
                return;
            }

            string table1 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table1 = chromeDriver.FindElementById("tableRunN").GetAttribute("innerHTML");
            }
            catch
            {
                //NLogger.Log(EventLevel.Debug,"oddsTable_1_1_3_r not found ");
            }
            string table2 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table2 = chromeDriver.FindElementById("tableTodayN").GetAttribute("innerHTML");
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
                try
                {
                    chromeDriver.SwitchTo().DefaultContent();
                }
                catch
                {
                    NLogger.Log(EventLevel.Error, "Cannot switch to default content");
                }
                try
                {
                    chromeDriver.SwitchTo().Frame(0);
                }
                catch
                {
                    NLogger.Log(EventLevel.Error, "Cannot switch to Frame 0");
                }
                try
                {
                    chromeDriver.SwitchTo().Frame("mainIframe3");
                }
                catch
                {
                    NLogger.Log(EventLevel.Error, "Cannot switch to mainIframe3");
                }

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
            string classFirstTeam = "";
            string classSecondTeam = "";
            bool matchIsLive = false;

            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            bool canUseCurrentLeague = true;
            string league = "";
            string typeLeague = "";
            string currentLeague = "";
            string currentLeagueType = "";
            string currentSensHdp = "";
            bool currentIsLive = false;

            //try
            //{
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            int nbLine = 0;
            //if(htmlDoc.DocumentNode.SelectNodes("//tr").Count == 0)
            //{
            //    return;
            //}

            
            var prv_RowMatches = htmlDoc.DocumentNode.SelectNodes("//tr");
            int prv_NumRowMatches = prv_RowMatches.Count;

            //foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes("//tr"))
            for (int RowMatchCounter = 0; RowMatchCounter < prv_NumRowMatches; RowMatchCounter++)
            {
	            var rowMatch = prv_RowMatches[RowMatchCounter];

                if (rowMatch.Attributes["class"].Value == "GridHeader" || rowMatch.Attributes["class"].Value == "y_titlebg")
                {
                    continue;
                }

                if (rowMatch.SelectNodes(".//td").Count == 1) // is line league
                {
                    canUseCurrentLeague = true;
                    league = rowMatch.SelectSingleNode(".//td/span").InnerText;
                    league.ToUpper();

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
                    nbLine++;
                    if (!canUseCurrentLeague)
                    {
                        continue;
                    }

                    // Check if match name is on this line
                    if (rowMatch.SelectSingleNode(".//td[1]").Descendants("span").Any())
                    {
                        if (currentRawMatch == "")
                        {

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
                                    MatchIsLive = currentIsLive
                                });

                                prm_Bookmaker.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = teamFav;
                                prm_Bookmaker.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = teamNoFav;

                                prm_Bookmaker.BookmakerMatches[iCurrentMatch].Odds = listOdds;
                            }
                            hasOdd = false;
                            listOdds = new List<Odd>();
                            listHdp = new List<string>();
                        }

                        try
                        {

                            if (rowMatch.SelectSingleNode(".//td[2]/div[1]/a").Attributes["class"].Value == "Give")
                            {
                                currentSensHdp = "+ ";
                            }
                            else
                            {
                                currentSensHdp = "- ";
                            }


                            teamFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/a").InnerText;
                            teamNoFav = rowMatch.SelectSingleNode(".//td[2]/div[2]/a").InnerText;

                            teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                            teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;

                            matchName = teamFav + " - " + teamNoFav;
                            currentRawMatch = matchName;
                            currentLeague = league;

                            if (rowMatch.SelectSingleNode("./td[1]/span/font") == null)
                            {
                                // Live
                                matchIsLive = true;
                            }
                            currentIsLive = matchIsLive;

                        }
                        catch
                        {

                        }
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
                            iCol = 6;
                            typeTime = "ft";
                        }
                        else
                        {
                            iCol = 12;
                            typeTime = "fh";
                        }

                        if (rowMatch.SelectSingleNode(".//td[" + iCol + "]/span/span") != null)
                        {
                            string overUnder = rowMatch.SelectSingleNode(".//td[" + iCol + "]/span/span").InnerText;
                            string strOddsOver = rowMatch.SelectSingleNode(".//td[" + (iCol + 1) + "]/span/span/label").InnerText;
                            string strOddsUnder = rowMatch.SelectSingleNode(".//td[" + (iCol + 2) + "]/span/span/label").InnerText;
                            overUnder = overUnder.Replace("/", "-");
                            overUnder = typeTime + " o/u " + overUnder;
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
                            iCol = 3;
                            typeTime = "ft";
                        }
                        else
                        {
                            iCol = 9;
                            typeTime = "fh";
                        }


                        if (rowMatch.SelectSingleNode(".//td[" + iCol + "]/span/span") == null)
                        {
                            continue;
                        }

                        string originalHandicapType = rowMatch.SelectSingleNode(".//td[" + iCol + "]/span/span").InnerText;
                        originalHandicapType = originalHandicapType.Replace("/", "-");

                        string handicapType = currentSensHdp + originalHandicapType;

                        if (originalHandicapType == "")
                        {
                            continue;
                        }

                        if (originalHandicapType != "0")
                        {
                            handicapType = currentSensHdp + originalHandicapType;
                        }
                        else
                        {
                            handicapType = originalHandicapType;
                        }

                        handicapType = typeTime + " " + handicapType;

                        if (rowMatch.SelectSingleNode(".//td[" + (iCol + 1) + "]/span/label") == null)
                        {
                            continue;
                        }

                        string strOddsFav = rowMatch.SelectSingleNode(".//td[" + (iCol + 1) + "]/span/label").InnerText;
                        string strOddsNoFav = rowMatch.SelectSingleNode(".//td[" + (iCol + 2) + "]/span/label").InnerText;

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
                //NLogger.Log(EventLevel.Debug,JsonConvert.SerializeObject(va2888.BookmakerMatches[iCurrentMatch], Formatting.Indented));
            }
            //watch.Stop();
            //NLogger.Log(EventLevel.Info,"Ibc05 update " + nbLine + " rows in " + watch.ElapsedMilliseconds + " ms");
            //}
            //catch
            //{
            //    NLogger.Log(EventLevel.Error,"Error update Ibc05 update");
            //}

        }
    }
}
