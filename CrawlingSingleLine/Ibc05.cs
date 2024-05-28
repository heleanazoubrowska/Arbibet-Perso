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
    public class Ibc05
    {
        ChromeOptions chromeOptions { get; set; }
        ChromeDriver chromeDriver { get; set; }

        public Ibc05()
        {
        }

        public void ConnectIbc05(Bookmaker ibc05)
        {
            NLogger.Log(EventLevel.Info,"Connect to Ibc05 ...");

            chromeOptions = new ChromeOptions();
            if (ibc05.BookmakerShowPage)
            {
                chromeOptions.AddArguments("headless");
            }
            chromeDriver = new ChromeDriver(chromeOptions);
            chromeDriver.Url = ibc05.BookmakerUrl;
            //chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            bool loginFormFounded = false;
            bool oddTablesFounded = false;
            try
            {
                IWebElement form = chromeDriver.FindElementByTagName("form");
                loginFormFounded = true;
                // Connect From Login Page
                form.FindElement(By.Name("useracc")).SendKeys(ibc05.BookmakerUser);
                form.FindElement(By.Name("passwd")).SendKeys(ibc05.BookmakerPass);
                form.FindElement(By.Name("remember")).Click();
                Thread.Sleep(1000);
                form.FindElement(By.Id("btnLogin")).Click();


                // Get Odds Data after connected

                chromeDriver.SwitchTo().Frame(0);
                chromeDriver.SwitchTo().Frame("mainIframe3");
                //chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.Id("aSetting")));

                // Set Single line settings
                chromeDriver.FindElement(By.Id("aSetting")).Click();
                chromeDriver.FindElement(By.Id("chkSport2")).Click();
                chromeDriver.FindElement(By.Id("btnSave")).Click();

                try
                {
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tableTodayN")));
                    try
                    {
                        IWebElement tableTodayN = chromeDriver.FindElementById("tableTodayN");
                        oddTablesFounded = true;
                        string html = tableTodayN.GetAttribute("innerHTML");

                        HtmlDocument htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(html);

                        getOdds(ibc05, htmlDoc);
                    }
                    catch (NoSuchElementException elemNotFound)
                    {
                        NLogger.Log(EventLevel.Error,"Ibc05 Odd Table not found : " + elemNotFound);
                    }
                }
                catch (NoSuchElementException elemNotFound)
                {
                    NLogger.Log(EventLevel.Error,"Ibc05 Odd Table not found : " + elemNotFound);
                }
            }
            catch
            {
                NLogger.Log(EventLevel.Error,"Ibc05 login not found : ");
            }
        }


        public void udpateOdd(Bookmaker ibc05)
        {
            try
            {
                IWebElement tableTodayN = chromeDriver.FindElementById("tableTodayN");
                string html = tableTodayN.GetAttribute("innerHTML");

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                getOdds(ibc05, htmlDoc);
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error,"Aa2888 Odd Table not found : " + elemNotFound);
            }
        }

        private void getOdds(Bookmaker ibc05, HtmlDocument htmlDoc)
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

            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            bool canUseCurrentLeague = true;
            string currentLeague = "";

            foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes("//tr"))
            {

                if (rowMatch.Attributes["class"].Value == "GridHeader" || rowMatch.Attributes["class"].Value == "y_titlebg")
                {
                    continue;
                }

                if (rowMatch.SelectNodes(".//td").Count == 1) // is line league
                {
                    currentLeague = rowMatch.SelectSingleNode(".//td/span").InnerText.ToUpper();
                    canUseCurrentLeague = Config.Config.CheckForUselessLeague(currentLeague);
                }
                else
                {
                    if (!canUseCurrentLeague)
                    {
                        continue;
                    }

                    if (!rowMatch.SelectSingleNode(".//td[3]/span").Descendants("span").Any())
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
                                ibc05.BookmakerMatches.Add(new Match()
                                {
                                    MatchName = matchName,
                                    MatchDate = matchDate,
                                    MatchTime = matchTime,
                                    Matchleague = currentLeague
                                });

                                ibc05.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = teamFav;
                                ibc05.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = teamNoFav;

                                ibc05.BookmakerMatches[iCurrentMatch].Odds = listOdds;
                            }
                            hasOdd = false;
                            listOdds = new List<Odd>();
                            listHdp = new List<string>();
                        }

                        try
                        {
                            teamFav = rowMatch.SelectSingleNode(".//td[2]/div[1]/a").InnerText;
                            teamNoFav = rowMatch.SelectSingleNode(".//td[2]/div[2]/a").InnerText;

                            teamFav = Common.cleanTeamName(teamFav);
                            teamNoFav = Common.cleanTeamName(teamNoFav);

                            matchName = teamFav + " - " + teamNoFav;
                            currentRawMatch = matchName;
                        }
                        catch
                        {

                        }
                    }

                    string handicapType = rowMatch.SelectSingleNode(".//td[3]/span/span").InnerText;
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

                    string strOddsFav = rowMatch.SelectSingleNode(".//td[4]/span/label").InnerText;
                    string strOddsNoFav = rowMatch.SelectSingleNode(".//td[5]/span/label").InnerText;

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
                //NLogger.Log(EventLevel.Debug,JsonConvert.SerializeObject(va2888.BookmakerMatches[iCurrentMatch], Formatting.Indented));
            }
        }

    }
}
