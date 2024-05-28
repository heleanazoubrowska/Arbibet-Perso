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
	public partial class Va2888 : CasinoAPI
	{

        public override void udpateOdd(Bookmaker prm_Bookmaker, bool hedgeBet)
        {
            string table1 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table1 = chromeDriver.FindElementById("tableRunN").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Debug, "va tableRunN not found ");
            }
            string table2 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table2 = chromeDriver.FindElementById("tableTodayN").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Debug, "va tableTodayN not found ");
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
                NLogger.Log(EventLevel.Error, "Va2888 odds table not found : ");
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

                
                var prv_Rows = htmlDoc.DocumentNode.SelectNodes(".//tr");
                int prv_NumRows = prv_Rows.Count;

                //foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes(".//tr"))
                for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
                {
	                var rowMatch = prv_Rows[Loopy];

                    //NLogger.Log(EventLevel.Debug,rowMatch.InnerHtml);

                    //NLogger.Log(EventLevel.Debug,"Case 0");
                    if (rowMatch.Attributes["class"].Value == "GridHeader" || rowMatch.Attributes["class"].Value == "y_titlebg")
                    {
                        continue;
                    }

                    if (rowMatch.SelectNodes(".//td").Count == 1) // is line league
                    {
                        //NLogger.Log(EventLevel.Debug,"Case 1");
                        canUseCurrentLeague = true;
                        league = rowMatch.SelectSingleNode(".//td/span[1]").InnerText;
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
                        if (!canUseCurrentLeague)
                        {
                            continue;
                        }

                        nbLine++;
                        //NLogger.Log(EventLevel.Debug,"Case 2");
                        if (!rowMatch.SelectSingleNode(".//td[4]/span").Descendants("span").Any())
                        {
                            continue;
                        }

                        // Check if match name is on this line
                        if (rowMatch.SelectSingleNode(".//td[1]").Descendants("a").Any())
                        {
                            //NLogger.Log(EventLevel.Debug,"Case 2.1");

                            try
                            {
                                if (rowMatch.SelectSingleNode(".//td[3]/div[1]/*").Attributes["class"].Value == "Give")
                                {
                                    currentSensHdp = "+ ";

                                }
                                else
                                {
                                    currentSensHdp = "- ";
                                }

                                teamFav = rowMatch.SelectSingleNode(".//td[3]/div[1]/*").InnerText;
                                teamNoFav = rowMatch.SelectSingleNode(".//td[3]/div[2]/*").InnerText;

                                teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                                teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;

                                matchName = $"{teamFav} - {teamNoFav}";

                                if (rowMatch.SelectSingleNode("./td[2]/span/font") == null)
                                {
                                    // Live
                                    matchIsLive = true;
                                }

                                //NLogger.Log(EventLevel.Debug,"Case 2.2");
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
                                {   // We check if we need to add the new match
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
                                        });

                                        prm_Bookmaker.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = currentTeamFav;
                                        prm_Bookmaker.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = currentTeamNoFav;

                                        prm_Bookmaker.BookmakerMatches[iCurrentMatch].Odds = listOdds;
                                        //NLogger.Log(EventLevel.Debug,"Case 2.3");
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

                                //currentRawMatch = matchName;
                            }
                            catch
                            {
                                //NLogger.Log(EventLevel.Debug,"error va 004");
                            }

                        }


                        // -----------------------------------------
                        #region OVER UNDER ODD FT & FH
                        // -----------------------------------------
                        int iCol;
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
                                iCol = 13;
                                typeTime = "fh";
                            }

                            if (rowMatch.SelectSingleNode(".//td["+ iCol + "]/span/span") != null)
                            {
                                try
                                {
                                    string overUnder = rowMatch.SelectSingleNode(".//td[" + iCol + "]/span/span").InnerText;
                                    string strOddsOver = rowMatch.SelectSingleNode(".//td[" + (iCol + 1) + "]/span/span").InnerText;
                                    string strOddsUnder = rowMatch.SelectSingleNode(".//td[" + (iCol + 2) + "]/span/span").InnerText;
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
                                catch
                                {
                                    //NLogger.Log(EventLevel.Debug,"error va 003");
                                }
                            }
                        }
                        // -----------------------------------------
                        #endregion OVER UNDER ODD FT & FH
                        // -----------------------------------------


                        // -----------------------------------------
                        #region HANDICAP ODD
                        // -----------------------------------------


                        for (int i = 0; i < 2; i++)
                        {
                            if (i == 0)
                            {
                                iCol = 4;
                                typeTime = "ft";
                            }
                            else
                            {
                                iCol = 10;
                                typeTime = "fh";
                            }

                            if(rowMatch.SelectSingleNode(".//td[" + iCol + "]/span/span") == null)
                            {
                                continue;
                            }
                            //NLogger.Log(EventLevel.Debug,"Case 3");
                            string originalHandicapType = rowMatch.SelectSingleNode(".//td[" + iCol + "]/span/span").InnerText;
                            originalHandicapType = originalHandicapType.Replace("/", "-");

                            string handicapType = currentSensHdp + originalHandicapType;

                            if (originalHandicapType != "0")
                            {
                                //if (listHdp.Contains(handicapType))
                                //{
                                //    if (currentSensHdp == "+ ")
                                //    {
                                //        handicapType = $"- {originalHandicapType}";
                                //    }
                                //    else
                                //    {
                                //        handicapType = $"+ {originalHandicapType}";
                                //    }

                                //}
                            }
                            else
                            {
                                handicapType = originalHandicapType;
                            }

                            handicapType = $"{typeTime} {handicapType}";
                            listHdp.Add(handicapType);

                            try
                            {
                                //NLogger.Log(EventLevel.Debug,"Case 4");
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
                                //NLogger.Log(EventLevel.Debug,"Case 5");
                            }

                            catch
                            {
                                //NLogger.Log(EventLevel.Debug,"error va 002");
                            }

                        }
                        // -----------------------------------------
                        #endregion HANDICAP ODD
                        // -----------------------------------------

                    }
                    //NLogger.Log(EventLevel.Debug,JsonConvert.SerializeObject(va2888.BookmakerMatches[iCurrentMatch], Formatting.Indented));
                }
                //watch.Stop();
                //NLogger.Log(EventLevel.Info,"Va28888 update "+ nbLine +" rows in " + watch.ElapsedMilliseconds + " ms");

            }
            catch(Exception e)
            {
                NLogger.Log(EventLevel.Error, $"Error update Va28888 {e}");
            }
        }
    }
}
