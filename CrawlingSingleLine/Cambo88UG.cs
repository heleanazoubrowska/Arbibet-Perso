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

namespace ArbibetProgram.CrawlingSingleLine
{
    public class Cambo88UG
    {
        ChromeOptions chromeOptions { get; set; }
        ChromeDriver chromeDriver { get; set; }

        public Cambo88UG()
        {

        }

        public void ConnectCambo88UG(Bookmaker cambo88UG)
        {
            NLogger.Log(EventLevel.Info,"Connect to Cambo88 UG ...");

            chromeOptions = new ChromeOptions();
            if (cambo88UG.BookmakerShowPage)
            {
                chromeOptions.AddArguments("headless");
            }

            chromeDriver = new ChromeDriver(chromeOptions);
            chromeDriver.Url = cambo88UG.BookmakerUrl;
            chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='doubleLine']/div[1]/span[2]/span/span")));
                chromeDriver.FindElementByXPath("//*[@id='doubleLine']/div[1]/span[2]/span/span").Click();

                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='doubleLine']/div[2]/ul/li[2]/span")));
                chromeDriver.FindElementByXPath("//*[@id='doubleLine']/div[2]/ul/li[2]/span").Click();

                Thread.Sleep(5000);

                bool oddTableFounded = false;
                chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                try
                {
                    IWebElement gridFutur = chromeDriver.FindElementByXPath("//*[@id='grid']/table/tbody");
                    oddTableFounded = true;
                    string html = gridFutur.GetAttribute("innerHTML");

                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);
                    getOdds(cambo88UG, htmlDoc);
                }
                catch (NoSuchElementException elemNotFound)
                {
                    NLogger.Log(EventLevel.Error,"Cambo88 UG odds table not found : " + elemNotFound);
                }

                if (!oddTableFounded)
                {
                    return;
                }
            }
            catch
            {
                NLogger.Log(EventLevel.Error,"Cambo88 UG odds table not found");
            }



        }

        public void ConnectCambo88UGTest(Bookmaker cambo88UG)
        {
            cambo88UG.BookmakerMatches.Add(new Match()
            {
                MatchName = "Red Lion - Daan",
                MatchDate = "Today",
                MatchTime = "17:00"
            });

            cambo88UG.BookmakerMatches[0].TeamFav.TeamName = "Red Lion";
            cambo88UG.BookmakerMatches[0].TeamNoFav.TeamName = "Daan";

            //cambo88UG.BookmakerMatches[0].TeamFav.Odds.DrawOdd = 0.95f;
            //cambo88UG.BookmakerMatches[0].TeamNoFav.Odds.DrawOdd = 1.05f;
        }

        public void udpateOdd(Bookmaker cambo88UG)
        {
            try
            {
                chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                IWebElement gridFutur = chromeDriver.FindElementByXPath("//*[@id='grid']/table/tbody");

                string html = gridFutur.GetAttribute("innerHTML");

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                getOdds(cambo88UG, htmlDoc);
            }

            catch
            {
                NLogger.Log(EventLevel.Error,"Cambo88 UG odds table not found");
            }

        }

        private void getOdds(Bookmaker cambo88UG, HtmlDocument htmlDoc)
        {
            string currentRawMatch = "";
            string currentTeamFav = "";
            string currentTeamNoFav = "";
            string league = "";

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
            string currentLeague = "";

            foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes("//tr"))
            {
                if(rowMatch.Descendants("th").Any()) // Is line League
                {
                    canUseCurrentLeague = true;
                    league = rowMatch.SelectSingleNode(".//th[2]/div").InnerText.ToUpper();

                    canUseCurrentLeague = Config.Config.CheckForUselessLeague(league);
                }
                else // is Match line
                {
                    if (!canUseCurrentLeague)
                    {
                        continue;
                    }

                    teamFav = rowMatch.SelectSingleNode(".//td[2]/p[1]/a").InnerText;
                    teamNoFav = rowMatch.SelectSingleNode(".//td[2]/p[2]/a").InnerText;
                    teamFav = Common.cleanTeamName(teamFav);
                    teamNoFav = Common.cleanTeamName(teamNoFav);
                    matchName = teamFav + " - " + teamNoFav;

                    if(currentRawMatch == "")
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
                            cambo88UG.BookmakerMatches.Add(new Match()
                            {
                                MatchName = currentRawMatch,
                                MatchDate = matchDate,
                                MatchTime = matchTime,
                                Matchleague = currentLeague
                            });

                            cambo88UG.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = currentTeamFav;
                            cambo88UG.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = currentTeamNoFav;

                            cambo88UG.BookmakerMatches[iCurrentMatch].Odds = listOdds;
                        }
                        currentRawMatch = matchName;
                        currentTeamFav = teamFav;
                        currentTeamNoFav = teamNoFav;
                        currentLeague = league;
                        hasOdd = false;
                        listOdds = new List<Odd>();
                        listHdp = new List<string>();
                    }

                    if (!rowMatch.SelectSingleNode(".//td[4]/b").Descendants("a").Any())
                    {
                        continue;
                    }
                    string handicapType = rowMatch.SelectSingleNode(".//td[4]/b/a").InnerText;
                    handicapType = handicapType.Replace("&nbsp;", "");
                    handicapType = handicapType.Replace("/", "-");

                    if (listHdp.Contains(handicapType))
                    {
                        handicapType = "-" + handicapType;
                    }

                    listHdp.Add(handicapType);

                    // Check if we need this handicap type

                    //if (handicapType != "0" && handicapType != "0.5" && handicapType != "1" && handicapType != "1.5" && handicapType != "2" && handicapType != "2.5" && handicapType != "3")
                    //{
                    //    continue;
                    //}

                    string strOddsFav = rowMatch.SelectSingleNode(".//td[5]/b/a/span").InnerText;
                    string strOddsNoFav = rowMatch.SelectSingleNode(".//td[6]/b/a/span").InnerText;
                    strOddsFav = strOddsFav.Replace("&nbsp;", "");
                    strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");
                    hasOdd = true;
                    listOdds.Add(new Odd()
                    {
                        oddType = handicapType,
                        oddFav = decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat) - 1,
                        oddNoFav = decimal.Parse(strOddsNoFav, CultureInfo.InvariantCulture.NumberFormat) - 1
                    });
                }
            }
        }
    }
}
