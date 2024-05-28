using System;
using System.Globalization;
using System.Linq;
using System.Threading;

using HtmlAgilityPack;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;

namespace ArbibetProgram.CrawlingSingleLine
{
    public class Aa2888
    {

        ChromeOptions chromeOptions { get; set; }
        ChromeDriver chromeDriver { get; set; }

        public Aa2888()
        {
        }

        public void ConnectAa2888(Bookmaker aa2888)
        {
            NLogger.Log(EventLevel.Info,"Connect to Aa2888 ...");

            chromeOptions = new ChromeOptions();

            if (aa2888.BookmakerShowPage)
            {
                chromeOptions.AddArguments("headless");
            }

            chromeDriver = new ChromeDriver(chromeOptions);
            chromeDriver.Url = aa2888.BookmakerUrl;
            chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            bool loginFormFounded = false;
            bool oddTablesFounded = false;
            try
            {
                IWebElement form = chromeDriver.FindElementById("myForm");
                loginFormFounded = true;
                // Connect From Login Page
                form.FindElement(By.Id("txtAccount")).SendKeys(aa2888.BookmakerUser);
                form.FindElement(By.Id("txtPassword")).SendKeys(aa2888.BookmakerPass);
                form.FindElement(By.Id("chkRememberMe")).Click();
                Thread.Sleep(1000);
                form.FindElement(By.Id("btnLogin")).Click();


                // Get Odds Data after connected

                chromeDriver.SwitchTo().Frame("frame_main_content");
                chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                chromeDriver.SwitchTo().Frame("frame_sports_main");

                try
                {
                    WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tbl2")));

                    try
                    {
                        wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                        wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("aSetting")));
                        // Set Single line settings
                        chromeDriver.FindElement(By.Id("aSetting")).Click();

                        try
                        {
                            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("rdSingleLine")));
                            chromeDriver.FindElement(By.Id("rdSingleLine")).Click();
                            chromeDriver.FindElement(By.Id("btnSaveSetting")).Click();

                            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tbl2")));
                            try
                            {
                                IWebElement gridFutur = chromeDriver.FindElementById("tbl2");
                                oddTablesFounded = true;
                                string html = gridFutur.GetAttribute("innerHTML");

                                HtmlDocument htmlDoc = new HtmlDocument();
                                htmlDoc.LoadHtml(html);

                                getOdds(aa2888, htmlDoc);
                            }
                            catch (NoSuchElementException elemNotFound)
                            {
                                NLogger.Log(EventLevel.Error,"Aa2888 Odd Table not found 6 : " + elemNotFound);
                            }
                        }

                        catch
                        {
                            NLogger.Log(EventLevel.Error,"Aa2888 Odd Table not found 5 : ");
                        }
                    }
                    catch
                    {
                        NLogger.Log(EventLevel.Error,"Aa2888 Odd Table not found 4 : ");
                    }

                }
                catch (NoSuchElementException elemNotFound)
                {
                    NLogger.Log(EventLevel.Error,"Aa2888 Odd Table not found 3 : " + elemNotFound);
                }
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error,"Aa2888 login not found 1 : " + elemNotFound);
            }
        }

        public void ConnectAa2888Test(Bookmaker cambo88UG)
        {
            cambo88UG.BookmakerMatches.Add(new Match()
            {
                MatchName = "Red Lion - Daan",
                MatchDate = "Today",
                MatchTime = "17:00"
            });

            cambo88UG.BookmakerMatches[0].TeamNoFav.TeamName = "Red Lion";
            cambo88UG.BookmakerMatches[0].TeamFav.TeamName = "Daan";

            //cambo88UG.BookmakerMatches[0].TeamNoFav.Odds.DrawOdd = 1.10f;
            //cambo88UG.BookmakerMatches[0].TeamFav.Odds.DrawOdd = 0.90f;
        }

        public void udpateOdd(Bookmaker aa2888)
        {
            try
            {
                IWebElement gridFutur = chromeDriver.FindElementById("tbl2");

                string html = gridFutur.GetAttribute("innerHTML");

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                getOdds(aa2888, htmlDoc);
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error,"Aa2888 Odd Table not found : " + elemNotFound);
            }
        }

        private void getOdds(Bookmaker aa2888, HtmlDocument htmlDoc)
        {

            //NLogger.Log(EventLevel.Debug,htmlDoc.DocumentNode.InnerText);

            string currentRawMatch = "";
            int iCurrentMatch = -1;
            string matchName = "";
            string teamFav = "";
            string teamNoFav = "";
            string matchDate = "";
            string matchTime = "";
            string classFirstTeam = "";
            string classSecondTeam = "";

            int colOddPos = 0;
            int lineOddPos = 0;

            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            bool canUseCurrentLeague = true;
            string league = "";
            string currentLeague = "";

            foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes("//tr"))
            {

                if (rowMatch.Descendants("th").Any()) // Is line League
                {
                    league = rowMatch.SelectSingleNode(".//th[2]").InnerText;
                    league = currentLeague.ToUpper();

                    canUseCurrentLeague = Config.Config.CheckForUselessLeague(league);
                }
                else
                {
                    // Check if match name is on this line
                    if (rowMatch.SelectNodes(".//td").Count  == 15)
                    {
                        colOddPos = 3;

                        if (currentRawMatch == "")
                        {
                            currentLeague = league;
                        }
                        else
                        {   // We check if we need to add the new match
                            if (hasOdd)
                            {
                                iCurrentMatch++;
                                aa2888.BookmakerMatches.Add(new Match()
                                {
                                    MatchName = matchName,
                                    MatchDate = matchDate,
                                    MatchTime = matchTime,
                                    Matchleague = currentLeague
                                });

                                aa2888.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = teamFav;
                                aa2888.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = teamNoFav;

                                aa2888.BookmakerMatches[iCurrentMatch].Odds = listOdds;
                            }

                            colOddPos = 0;
                            lineOddPos = 0;
                            hasOdd = false;
                            listOdds = new List<Odd>();
                            listHdp = new List<string>();

                        }

                        // Get the position of the favorite

                        teamFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/span[1]").InnerText;
                        teamNoFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/span[2]").InnerText;

                        teamFav = Common.cleanTeamName(teamFav);
                        teamNoFav = Common.cleanTeamName(teamNoFav);

                        matchName = teamFav + " - " + teamNoFav;
                        currentRawMatch = matchName;
                        currentLeague = league;

                    }
                    else
                    {
                        colOddPos = 1;
                    }

                    // Get the position of the favorite
                    if (rowMatch.SelectSingleNode(".//td[" + colOddPos + "]/span") == null)
                    {
                        continue;
                    }

                    string handicapType = rowMatch.SelectSingleNode(".//td[" + colOddPos + "]/span").InnerText;
                    if(handicapType == "")
                    {
                        continue;
                    }
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

                    string strOddsFav = rowMatch.SelectSingleNode(".//td[" + (colOddPos + 1) + "]/a").InnerText;
                    string strOddsNoFav = rowMatch.SelectSingleNode(".//td[" + (colOddPos + 2) + "]/a").InnerText;

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
                //NLogger.Log(EventLevel.Debug,JsonConvert.SerializeObject(aa2888.BookmakerMatches[iCurrentMatch], Formatting.Indented));
            }
        }
    }
}
