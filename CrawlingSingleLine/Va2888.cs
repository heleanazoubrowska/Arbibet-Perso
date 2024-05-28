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
using OpenQA.Selenium.Support.UI;
using ArbibetProgram.Models;
using System.Globalization;
using System.Linq;

using ArbibetProgram.Functions;

namespace ArbibetProgram.CrawlingSingleLine
{
    public class Va2888
    {
        ChromeOptions chromeOptions { get; set; }
        ChromeDriver chromeDriver { get; set; }

        public Va2888()
        {
        }

        // mainIframe2 = left menu
        // mainIframe3 = odd Iframe

        public void ConnectVa2888(Bookmaker va2888)
        {
            NLogger.Log(EventLevel.Info,"Connect to Va2888 ...");

            chromeOptions = new ChromeOptions();
            if (va2888.BookmakerShowPage)
            {
                chromeOptions.AddArguments("headless");
            }
            chromeDriver = new ChromeDriver(chromeOptions);
            chromeDriver.Url = va2888.BookmakerUrl;
            //chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            bool loginFormFounded = false;
            bool oddTablesFounded = false;
            try
            {
                IWebElement form = chromeDriver.FindElementByClassName("form-login");
                loginFormFounded = true;
                // Connect From Login Page

                form.FindElement(By.Id("login_account")).SendKeys(va2888.BookmakerUser);
                form.FindElement(By.Id("login_password")).SendKeys(va2888.BookmakerPass);
                form.FindElement(By.Id("remeber_me")).Click();
                Thread.Sleep(1000);
                form.FindElement(By.Name("save")).Click();


                // Get Odds Data after connected

                chromeDriver.SwitchTo().Frame(chromeDriver.FindElement(By.XPath("//html/body/div[3]/div/iframe")));
                chromeDriver.SwitchTo().Frame("mainIframe2");


                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='SoccerMenu']/div[2]/a")));
                chromeDriver.FindElementByXPath("//*[@id='SoccerMenu']/div[2]/a").Click();


                chromeDriver.SwitchTo().ParentFrame();
                chromeDriver.SwitchTo().Frame("mainIframe3");
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30));
                chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tableTodayN")));

                try
                {
                    IWebElement tableTodayN = chromeDriver.FindElementById("tableTodayN");
                    oddTablesFounded = true;
                    try
                    {
                        string html = tableTodayN.GetAttribute("innerHTML");

                        HtmlDocument htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(html);
                        getOdds(va2888, htmlDoc);
                    }

                    catch
                    {

                    }

                }
                catch (NoSuchElementException elemNotFound)
                {
                    NLogger.Log(EventLevel.Error,"Va2888 odds table not found : " + elemNotFound);
                }
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error,"Va2888 login not found : " + elemNotFound);
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

        public void udpateOdd(Bookmaker va2888)
        {
            try
            {
                IWebElement tableTodayN = chromeDriver.FindElementById("tableTodayN");
                string html = tableTodayN.GetAttribute("innerHTML");

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                getOdds(va2888, htmlDoc);
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error,"Va2888 odds table not found : " + elemNotFound);
            }
        }

        private void getOdds(Bookmaker va2888, HtmlDocument htmlDoc)
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

            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            bool canUseCurrentLeague = true;
            string league = "";
            string currentLeague = "";

            foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes("//tr"))
            {

                if (rowMatch.Attributes["class"].Value == "GridHeader" || rowMatch.Attributes["class"].Value == "y_titlebg")
                {
                    continue;
                }

                if (rowMatch.SelectNodes(".//td").Count == 1) // is line league
                {
                    canUseCurrentLeague = true;
                    league = rowMatch.SelectSingleNode(".//td/span[1]").InnerText.ToUpper();
                    canUseCurrentLeague = Config.Config.CheckForUselessLeague(currentLeague);
                }
                else
                {
                    if (!rowMatch.SelectSingleNode(".//td[4]/span").Descendants("span").Any())
                    {
                        continue;
                    }

                    // Check if match name is on this line
                    if (rowMatch.SelectSingleNode(".//td[1]").Descendants("a").Any())
                    {
                        try
                        {
                            teamFav = rowMatch.SelectSingleNode(".//td[3]/div[1]/a").InnerText;
                            teamNoFav = rowMatch.SelectSingleNode(".//td[3]/div[2]/a").InnerText;

                            teamFav = Common.cleanTeamName(teamFav);
                            teamNoFav = Common.cleanTeamName(teamNoFav);

                            matchName = teamFav + " - " + teamNoFav;

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
                                    va2888.BookmakerMatches.Add(new Match()
                                    {
                                        MatchName = currentRawMatch,
                                        MatchDate = matchDate,
                                        MatchTime = matchTime,
                                        Matchleague = currentLeague
                                    });

                                    va2888.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = currentTeamFav;
                                    va2888.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = currentTeamNoFav;

                                    va2888.BookmakerMatches[iCurrentMatch].Odds = listOdds;
                                }
                                currentRawMatch = matchName;
                                currentTeamFav = teamFav;
                                currentTeamNoFav = teamNoFav;
                                currentLeague = league;
                                hasOdd = false;
                                listOdds = new List<Odd>();
                                listHdp = new List<string>();
                            }

                            currentRawMatch = matchName;
                        }

                        catch
                        {

                        }
                    }

                    string handicapType = rowMatch.SelectSingleNode(".//td[4]/span/span").InnerText;
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

                    try {
                        string strOddsFav = rowMatch.SelectSingleNode(".//td[5]/span/label").InnerText;
                        string strOddsNoFav = rowMatch.SelectSingleNode(".//td[6]/span/label").InnerText;

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

                    catch
                    {

                    }
                }
                //NLogger.Log(EventLevel.Debug,JsonConvert.SerializeObject(va2888.BookmakerMatches[iCurrentMatch], Formatting.Indented));
            }
        }
    }
}
