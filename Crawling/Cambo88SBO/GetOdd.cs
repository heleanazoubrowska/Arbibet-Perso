using System;
using System.Globalization;

using HtmlAgilityPack;

using System.Collections.Generic;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Cambo88SBO : CasinoAPI
    {
        public override void udpateOdd(Bookmaker cambo88SBO, bool hedgeBet)
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
                try
                {


                    string table1 = "";
                    try
                    {
                        //WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                        table1 = chromeDriver.FindElementById("module-odds-display").GetAttribute("innerHTML");
                    }
                    catch
                    {
                        //NLogger.Log(EventLevel.Debug,"oddsTable_1_1_3_r not found ");
                    }

                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(table1);
                    getOdds(cambo88SBO, htmlDoc, hedgeBet);

                }

                catch (NoSuchElementException Ex)
                {
                    NLogger.Log(EventLevel.Error, $"Cambo88 SBO odds table not found{Ex}");
                }
            }
            else
            {
	            Login(10);
            }
        }

        protected override void getOdds(Bookmaker cambo88SBO, HtmlDocument htmlDoc, bool hedgeBet)
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


            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            bool canUseCurrentLeague = true;
            string league = "";
            string typeLeague = "";
            string currentLeague = "";
            string currentLeagueType = "";

            //var watch = System.Diagnostics.Stopwatch.StartNew();
            int nbLine = 0;

            
            var prv_RowMatches = htmlDoc.DocumentNode.SelectNodes("//tbody");
            int prv_NumRowMatches = prv_RowMatches.Count;

            //foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes("//tbody"))
            for (int RowMatchCounter = 0; RowMatchCounter < prv_NumRowMatches; RowMatchCounter++)
            {
	            var rowMatch = prv_RowMatches[RowMatchCounter];

                if (rowMatch.SelectNodes(".//tr") == null)
                {
                    continue;
                }

                if (rowMatch.SelectNodes(".//tr").Count == 1) // Is line League
                {

                    if (rowMatch.SelectSingleNode(".//tr[1]").Attributes["class"] == null)
                    {
                        continue;
                    }

                    if (rowMatch.SelectSingleNode(".//tr[1]").Attributes["class"].Value == "league-row") // Is line League
                    {
                        canUseCurrentLeague = true;

                        if (rowMatch.SelectSingleNode(".//tr/td/span/span[2]") == null)
                        {
                            league = rowMatch.SelectSingleNode(".//tr/td/span/span/span[2]").InnerText;
                        }
                        else
                        {
                            league = rowMatch.SelectSingleNode(".//tr/td/span/span[2]").InnerText;
                        }

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
                if (rowMatch.SelectNodes(".//tr").Count == 3) // Is line odd
                {

                    //if (rowMatch.SelectSingleNode(".//tr[1]").Attributes["class"] == null)
                    //{
                    //    continue;
                    //}

                    int iC = 0;
                    if (rowMatch.SelectSingleNode(".//tr[1]").Attributes["id"].Value.Contains("row-live"))
                    {
                        iC = 1;
                    }

                    nbLine++;
                    if (!canUseCurrentLeague)
                    {
                        continue;
                    }

                    teamFav = rowMatch.SelectSingleNode(".//tr[1]/td[3]/span[1]").InnerText;
                    teamNoFav = rowMatch.SelectSingleNode(".//tr[1]/td[3]/span[2]").InnerText;
                    teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                    teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;
                    matchName = $"{teamFav} - {teamNoFav}";
                    //NLogger.Log(EventLevel.Debug,matchName);

                    if (currentRawMatch == "")
                    {
                        currentRawMatch = matchName;
                        currentTeamFav = teamFav;
                        currentTeamNoFav = teamNoFav;
                        currentLeague = league;
                    }

                    if (currentRawMatch != matchName)
                    {
                        if (hasOdd)
                        {
                            iCurrentMatch++;
                            cambo88SBO.BookmakerMatches.Add(new Models.Match()
                            {
                                MatchName = currentRawMatch,
                                MatchName2 = $"{teamNoFav} - {teamFav}",
                                MatchDate = matchDate,
                                MatchTime = matchTime,
                                Matchleague = currentLeague
                            });

                            cambo88SBO.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = currentTeamFav;
                            cambo88SBO.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = currentTeamNoFav;

                            cambo88SBO.BookmakerMatches[iCurrentMatch].Odds = listOdds;
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


                    int iColL1 = 0;
                    int iColL2 = 0;
                    string typeTime = "";

                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            iColL1 = 5;
                            iColL2 = 2;
                            typeTime = "ft";
                        }
                        else
                        {
                            iColL1 = 8;
                            iColL2 = 5;
                            typeTime = "fh";
                        }

                        int prv_ColIndex_Over = iColL1 + iC;
                        int prv_ColIndex_Under = iColL2 + iC;

                        if (rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[1]") != null)
                        {
                            string overUnder = rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[1]").InnerText;
                            overUnder = overUnder.Replace("&nbsp;", "");
                            overUnder = overUnder.Replace(" ", "");
                            if (overUnder == "" || overUnder.Contains("&nbsp;"))
                            {
                                continue;
                            }

                            if (rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[3]/span/span") == null)
                            {
                                continue;
                            }

                            string strOddsOver = rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[3]/span/span").InnerText;
                            string strOddsUnder = rowMatch.SelectSingleNode($".//tr[2]/td[{prv_ColIndex_Under}]/a/span[2]/span/span").InnerText;
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
                            iColL1 = 4;
                            iColL2 = 1;
                            typeTime = "ft";
                        }
                        else
                        {
                            iColL1 = 7;
                            iColL2 = 4;
                            typeTime = "fh";
                        }

                        int prv_ColIndex_Over = iColL1 + iC;
                        int prv_ColIndex_Under = iColL2 + iC;

                        string sensHdp = "";
                        int posHdp = 0;

                        if (rowMatch.SelectSingleNode($".//tr[2]/td[{prv_ColIndex_Under}]/a/span[1]").InnerText == "&nbsp;")
                        {
                            continue;
                        }

                        string handicapType = "";
                        // Get the position of the favorite
                        if (rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[1]").InnerText == "" 
                            || 
                            rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[1]").InnerText == "&nbsp;")
                        {
                            sensHdp = "- ";
                            posHdp = 2;
                            handicapType = rowMatch.SelectSingleNode($".//tr[2]/td[{prv_ColIndex_Under}]/a/span[1]").InnerText;
                        }
                        else
                        {
                            sensHdp = "+ ";
                            posHdp = 1;
                            handicapType = rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[1]").InnerText;
                        }

                        handicapType = handicapType.Replace("&nbsp;", "");
                        handicapType = handicapType.Replace("/", "-");

                        if (handicapType != "0")
                        {
                            handicapType = sensHdp + handicapType;
                        }

                        handicapType = $"{typeTime} {handicapType}";

                        string strOddsFav = rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[2]/span/span").InnerText;
                        string strOddsNoFav = rowMatch.SelectSingleNode($".//tr[2]/td[{prv_ColIndex_Under}]/a/span[2]/span/span").InnerText;
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
        }
    }
}
