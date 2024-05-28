using System;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

using ArbibetProgram.Models;
using System.Globalization;
using System.Linq;

using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;

namespace ArbibetProgram.CrawlingSingleLine
{
    public class Nba369
    {
        public Nba369()
        {
        }

        ChromeOptions chromeOptions { get; set; }
        ChromeDriver chromeDriver { get; set; }

        public void ConnectNba369(Bookmaker nba369)
        {
            NLogger.Log(EventLevel.Info,"Connect to Nba369 ...");

            chromeOptions = new ChromeOptions();
            if (nba369.BookmakerShowPage)
            {
                chromeOptions.AddArguments("headless");
            }
            chromeDriver = new ChromeDriver(chromeOptions);
            chromeDriver.Url = nba369.BookmakerUrl;
            chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            bool loginFormFounded = false;
            bool oddTablesFounded = false;
            try
            {
                IWebElement form = chromeDriver.FindElementByClassName("login");
                loginFormFounded = true;
                // Connect From Login Page
                form.FindElement(By.Id("txtAccount")).SendKeys(nba369.BookmakerUser);
                form.FindElement(By.Id("txtPassword")).SendKeys(nba369.BookmakerPass);
                form.FindElement(By.ClassName("checkmark")).Click();
                Thread.Sleep(1000);
                form.FindElement(By.Id("btnLogin")).Click();

                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("tabsport")));
                chromeDriver.FindElementById("tabsport").Click();

                // Get Odds Data after connected

                chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

                try
                {

                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(15));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tbltoday-content")));
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(15));
                    try
                    {
                        wait.Until(ExpectedConditions.ElementExists(By.Id("aSetting")));
                        // Set Single line settings
                        chromeDriver.FindElement(By.Id("aSetting")).Click();
                        wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                        wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("rdSingleLine")));
                        chromeDriver.FindElement(By.Id("rdSingleLine")).Click();
                        chromeDriver.FindElement(By.Id("btnSaveSetting")).Click();

                        wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tbltoday-content")));

                        try
                        {
                            IWebElement gridFutur = chromeDriver.FindElementById("tbltoday-content");
                            oddTablesFounded = true;

                            string html = gridFutur.GetAttribute("innerHTML");

                            HtmlDocument htmlDoc = new HtmlDocument();
                            htmlDoc.LoadHtml(html);

                            getOdds(nba369, htmlDoc);
                        }

                        catch
                        {
                            NLogger.Log(EventLevel.Error,"Nba369 table not found");
                        }
                    }
                    catch
                    {
                        NLogger.Log(EventLevel.Error,"Nba369 table not found");
                    }

                }

                catch (NoSuchElementException elemNotFound)
                {
                    NLogger.Log(EventLevel.Error,"Nba369 Odd Table not found : " + elemNotFound);
                }
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error,"Nba369 login not found : " + elemNotFound);
            }

            if (!loginFormFounded)
            {
                return;
            }

            if (!oddTablesFounded)
            {
                return;
            }

        }

        public void udpateOdd(Bookmaker ibc05)
        {
            try
            {
                IWebElement gridFutur = chromeDriver.FindElementById("tbltoday-content");

                string html = gridFutur.GetAttribute("innerHTML");

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                getOdds(ibc05, htmlDoc);
            }

            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error,"Nba369 Odd Table not found : " + elemNotFound);
            }
        }

        private void getOdds(Bookmaker nba369, HtmlDocument htmlDoc)
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
                    canUseCurrentLeague = Config.Config.CheckForUselessLeague(currentLeague);
                }
                else
                {
                    // Check if match name is on this line
                    if (rowMatch.SelectNodes(".//td").Count == 15)
                    {

                        teamFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/span[1]").InnerText;
                        teamNoFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/span[3]").InnerText;
                        teamFav = Common.cleanTeamName(teamFav);
                        teamNoFav = Common.cleanTeamName(teamNoFav);
                        matchName = teamFav + " - " + teamNoFav;

                        colOddPos = 3;

                        if (currentRawMatch == "")
                        {
                            currentRawMatch = matchName;
                            currentTeamFav = teamFav;
                            currentTeamNoFav = teamNoFav;
                            currentLeague = league;
                        }
                        if (currentRawMatch != matchName)
                        {   // We check if we need to add the new match
                            if (hasOdd)
                            {
                                iCurrentMatch++;
                                nba369.BookmakerMatches.Add(new Match()
                                {
                                    MatchName = currentRawMatch,
                                    MatchDate = matchDate,
                                    MatchTime = matchTime,
                                    Matchleague = currentLeague
                                });

                                nba369.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = currentTeamFav;
                                nba369.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = currentTeamNoFav;

                                nba369.BookmakerMatches[iCurrentMatch].Odds = listOdds;
                            }
                            currentRawMatch = matchName;
                            currentTeamFav = teamFav;
                            currentTeamNoFav = teamNoFav;
                            currentLeague = league;
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
