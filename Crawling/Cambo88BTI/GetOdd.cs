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
    public partial class Cambo88BTI : CasinoAPI
    {
        public override void udpateOdd(Bookmaker cambo88bti, bool hedgeBet)
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


            try
            {

                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id='rj-asian-view-events']/sb-comp")));
                IWebElement tableTodayN = chromeDriver.FindElementByXPath("//*[@id='rj-asian-view-events']/sb-comp");

                string html = tableTodayN.GetAttribute("innerHTML");

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                getOdds(cambo88bti, htmlDoc, hedgeBet);
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error, $"Cambo88 BTI  odds table not found : {elemNotFound}");
            }
        }

        protected override void getOdds(Bookmaker cambo88BTI, HtmlDocument htmlDoc, bool hedgeBet)
        {
            string currentRawMatch = "";
            string currentTeamFav = "";
            string currentTeamNoFav = "";
            string league = "";
            int currentTeamFavPos = 0;
            int currentTeamNoFavPos = 0;
            int iCurrentMatch = -1;
            string matchName = "";
            string teamFav = "";
            string teamNoFav = "";
            string matchDate = "";
            string matchTime = "";
            bool matchIsLive = false;
            string classFirstTeam = "";
            string classSecondTeam = "";
            string typeLeague = "";

            bool currentIsLive = false;

            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            bool canUseCurrentLeague = true;
            string currentLeague = "";

            //try
            //{
            //var watch = System.Diagnostics.Stopwatch.StartNew();

            
            var prv_EventLists = htmlDoc.DocumentNode.SelectNodes("./div[@class='rj-asian-events__events-list']");
            int prv_NumEventLists = prv_EventLists.Count;

            //foreach (HtmlNode rowEventList in htmlDoc.DocumentNode.SelectNodes("./div[@class='rj-asian-events__events-list']"))
            for (int Loopy = 0; Loopy < prv_NumEventLists; Loopy++)
            {
	            var rowEventList = prv_EventLists[Loopy];

                int nbLine = 0;
                
                var prv_RowLeagues = rowEventList.SelectNodes("./div[@class='rj-asian-events__events-block']/div[3]/div[@class='rj-asian-events__single-league']");
                int prv_NumRowLeagues = prv_RowLeagues.Count;

                //foreach (HtmlNode rowLeague in rowEventList.SelectNodes("./div[@class='rj-asian-events__events-block']/div[3]/div[@class='rj-asian-events__single-league']"))
                for (int LeagueCounter = 0; LeagueCounter < prv_NumRowLeagues; LeagueCounter++)
                {
	                var rowLeague = prv_RowLeagues[LeagueCounter];

                    canUseCurrentLeague = true;
                    league = rowLeague.SelectSingleNode(".//div[1]/h4").InnerText;
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

                    //NLogger.Log(EventLevel.Debug,rowLeague.InnerText);
                    try
                    {
                        var prv_RowLists = rowLeague.SelectNodes("./div[2]/div[@class='rj-asian-events__single-event']");
                        int prv_NumRowLists = prv_RowLists.Count;

                        //foreach (HtmlNode rowList in rowLeague.SelectNodes("./div[2]/div[@class='rj-asian-events__single-event']"))
                        for (int RowListCounter = 0; RowListCounter < prv_NumRowLists; RowListCounter++)
                        {
	                        var rowList = prv_RowLists[RowListCounter];
                            //NLogger.Log(EventLevel.Debug,"++++++++++++++++");
                            //NLogger.Log(EventLevel.Debug,rowList.InnerText);
                            //NLogger.Log(EventLevel.Debug,"++++++++++++++++");

                            
                            var prv_RowMatches = rowList.SelectNodes(".//div[@class='rj-asian-events__row']");
                            int prv_NumRowMatches = prv_RowMatches.Count;

                            //foreach (HtmlNode rowMatch in rowList.SelectNodes(".//div[@class='rj-asian-events__row']"))
                            for (int RowMatchCounter = 0; RowMatchCounter < prv_NumRowMatches; RowMatchCounter++)
                            {
	                            var rowMatch = prv_RowMatches[RowMatchCounter];
	                            nbLine++;
                                //NLogger.Log(EventLevel.Debug,"CASE 1");

                                //NLogger.Log(EventLevel.Debug,rowMatch.InnerHtml);
                                teamFav = rowMatch.SelectSingleNode(".//div[2]/div/div[1]").InnerText;
                                teamNoFav = rowMatch.SelectSingleNode(".//div[2]/div/div[2]").InnerText;

                                teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                                teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;

                                matchName = $"{teamFav} - {teamNoFav}";

                                bool isLive = false;
                                try
                                {
                                    if (rowMatch.SelectSingleNode("./div[1]/div[1]/div[1]").Attributes["class"].Value.Contains("rj-asian-events__event-score"))
                                    {
                                        // Live
                                        matchIsLive = true;

                                    }
                                }
                                catch
                                {

                                }


                                //NLogger.Log(EventLevel.Debug,"CASE 2");
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
                                        cambo88BTI.BookmakerMatches.Add(new Models.Match()
                                        {
                                            MatchName = currentRawMatch,
                                            MatchName2 = $"{teamNoFav} - {teamFav}",
                                            MatchDate = matchDate,
                                            MatchTime = matchTime,
                                            Matchleague = currentLeague,
                                            MatchIsLive = currentIsLive
                                        });

                                        cambo88BTI.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = currentTeamFav;
                                        cambo88BTI.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = currentTeamNoFav;

                                        cambo88BTI.BookmakerMatches[iCurrentMatch].Odds = listOdds;
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


                                //NLogger.Log(EventLevel.Debug,"CASE 3");
                                string sensHdp = "";
                                int posHdp = 0;


                                // ++++++++++++++++++++
                                // OVER UNDER ODD FT & FH
                                // ++++++++++++++++++++


                                int iCol = 0;
                                string typeTime = "";

                                for (int i = 0; i < 2; i++)
                                {
                                    if (i == 0)
                                    {
                                        iCol = 3;
                                        typeTime = "ft";
                                    }
                                    else
                                    {
                                        iCol = 4;
                                        typeTime = "fh";
                                    }


                                    if (rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[2]/div[1]/div[1]") != null)
                                    {
                                        string overUnder = rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[2]/div[1]/div[1]").InnerText;
                                        string strOddsOver = rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[2]/div[1]/div[2]").InnerText;
                                        string strOddsUnder = rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[2]/div[2]/div[2]").InnerText;
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
                                        iCol = 4;
                                        typeTime = "fh";
                                    }


                                    if (rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[1]/div[1]/div[1]") == null)
                                    {
                                        //NLogger.Log(EventLevel.Debug,"Case 3.1");
                                        continue;
                                    }
                                    //NLogger.Log(EventLevel.Debug,"CASE 3111");
                                    // Get the position of the favorite
                                    if (rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[1]/div[1]/div[1]").InnerText == "" 
                                        || 
                                        rowMatch.SelectSingleNode(".//div[3]/div/div[1]/div[1]/div[1]").InnerText == "&nbsp;")
                                    {
                                        //NLogger.Log(EventLevel.Debug,"Case 3.2");
                                        sensHdp = "- ";
                                        posHdp = 2;
                                    }
                                    else
                                    {
                                        //NLogger.Log(EventLevel.Debug,"Case 3.3");
                                        sensHdp = "+ ";
                                        posHdp = 1;
                                    }


                                    //NLogger.Log(EventLevel.Debug,"CASE 3222");
                                    string handicapType = rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[1]/div[{posHdp}]/div[1]").InnerText;
                                    //NLogger.Log(EventLevel.Debug,"Case 4");
                                    if (handicapType == "")
                                    {
                                        //NLogger.Log(EventLevel.Debug,"Case 4.1");
                                        continue;
                                    }
                                    handicapType = handicapType.Replace("/", "-");

                                    if (handicapType != "0")
                                    {
                                        handicapType = sensHdp + handicapType;
                                    }

                                    handicapType = $"{typeTime} {handicapType}";

                                    string strOddsFav = rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[1]/div[1]/div[2]").InnerText;
                                    string strOddsNoFav = rowMatch.SelectSingleNode($".//div[{iCol}]/div/div[1]/div[2]/div[2]").InnerText;
                                    //NLogger.Log(EventLevel.Debug,"Case 5");
                                    strOddsFav = strOddsFav.Replace("&nbsp;", "");
                                    strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");
                                    //NLogger.Log(EventLevel.Debug,"++++++++++++++");
                                    //NLogger.Log(EventLevel.Debug,handicapType);
                                    //NLogger.Log(EventLevel.Debug,strOddsFav);
                                    //NLogger.Log(EventLevel.Debug,strOddsNoFav);
                                    //NLogger.Log(EventLevel.Debug,"++++++++++++++");
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
                    catch
                    {

                    }
                }
                //watch.Stop();
                //NLogger.Log(EventLevel.Info,"Cambo88 BTI update " + nbLine + " rows in " + watch.ElapsedMilliseconds + " ms");
            }
            //}

            //catch
            //{
            //    NLogger.Log(EventLevel.Error,"Error update Cambo88 BTI");
            //}
        }
    }
}
