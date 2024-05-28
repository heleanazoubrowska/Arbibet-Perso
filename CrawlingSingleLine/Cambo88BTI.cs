using System;
using HtmlAgilityPack;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;
using OpenQA.Selenium.Support.UI;
using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Interactions;

namespace ArbibetProgram.CrawlingSingleLine
{
    public class Cambo88BTI
    {
        ChromeOptions chromeOptions { get; set; }
        ChromeDriver chromeDriver { get; set; }

        public Cambo88BTI()
        {
        }

        public void ConnectCambo88BTI(Bookmaker cambo88BTI)
        {
            NLogger.Log(EventLevel.Info,"Connect to Cambo88 BTI ...");

            chromeOptions = new ChromeOptions();
            if (cambo88BTI.BookmakerShowPage)
            {
                chromeOptions.AddArguments("headless");
            }

            chromeDriver = new ChromeDriver(chromeOptions);
            chromeDriver.Url = cambo88BTI.BookmakerUrl;


            bool isSingleLine = false;

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//html/body/div[1]/div[2]/div/sb-block[1]/div/div/div[5]/div/div[1]/div[2]/div/div[2]/span[1]")));
            NLogger.Log(EventLevel.Debug,"W 1");

            IWebElement btnLine = chromeDriver.FindElementByXPath("//html/body/div[1]/div[2]/div/sb-block[1]/div/div/div[5]/div/div[1]/div[2]/div/div/a");
            Actions actions = new Actions(chromeDriver);
            IWebElement menuHoverLink = chromeDriver.FindElement(By.XPath("//html/body/div[1]/div[2]/div/sb-block[1]/div/div/div[5]/div/div[1]/div[2]/div"));
            NLogger.Log(EventLevel.Debug,"W 2");
            actions.MoveToElement(menuHoverLink).Build().Perform();
            NLogger.Log(EventLevel.Debug,"W 3");
            actions.MoveToElement(btnLine).Build().Perform();
            NLogger.Log(EventLevel.Debug,"W 4");
            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementToBeClickable(btnLine));
            bool canContinue = false;
            try
            {
                btnLine.Click();
                canContinue = true;
            }

            catch
            {
                try
                {
                    if (chromeDriver.FindElementByXPath("//html/body/div[1]/div[2]/div/sb-block[1]/div/div/div[5]/div/div[1]/div[2]/div/div[2]/span[1]").Text == "Single View")
                    {
                        canContinue = true;
                    }
                }

                catch
                {
                    return;
                }
            }

            if (!canContinue)
            {
                NLogger.Log(EventLevel.Debug,"Click error for Cambo88 BTI");
                return;
            }

            NLogger.Log(EventLevel.Debug,"W 5");
            // wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));


            //while (chromeDriver.FindElementByXPath("//html/body/div[1]/div[2]/div/sb-block[1]/div/div/div[5]/div/div[1]/div[2]/div/div[2]/span[1]").Text != "Single View")
            //{

            //chromeDriver.ExecuteScript("$('#hr-bot-Top_ResponsiveHeader_19064-page-header-right2 > div > div.page-header-dropdown-inner > a').click();");

            //Actions actions = new Actions(chromeDriver);
            //IWebElement menuHoverLink = chromeDriver.FindElement(By.XPath("//html/body/div[1]/div[2]/div/sb-block[1]/div/div/div[5]/div/div[1]/div[2]/div"));
            //actions.MoveToElement(menuHoverLink);
            //actions.Build().Perform();

            //wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            //chromeDriver.FindElementByXPath("//html/body/div[1]/div[2]/div/sb-block[1]/div/div/div[5]/div/div[1]/div[2]/div/div/a").Click();
            //wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//html/body/div[1]/div[2]/div/sb-block[1]/div/div/div[5]/div/div[1]/div[2]/div/div/a")));
            //try
            //{
            //    chromeDriver.FindElementByXPath("//html/body/div[1]/div[2]/div/sb-block[1]/div/div/div[5]/div/div[1]/div[2]/div/div/a").Click();
            //    isSingleLine = true;
            //}
            //catch
            //{
            //    if (chromeDriver.FindElementByXPath("//html/body/div[1]/div[2]/div/sb-block[1]/div/div/div[5]/div/div[1]/div[2]/div/div[2]/span[1]").Text == "Single View")
            //    {
            //        isSingleLine = true;
            //    }
            //    else
            //    {
            //        NLogger.Log(EventLevel.Error,"Cannot select single line");
            //    }

            //}


            //}

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.XPath("//html/body/div[1]/div[4]/div[1]/main/div/sb-comp/div")));

            bool oddTableFounded = false;
            try
            {
                oddTableFounded = true;

                int xpathCount = chromeDriver.FindElements(By.XPath("//html/body/div[1]/div[4]/div[1]/main/div/sb-comp/div")).Count;
                int col = 0;
                if (xpathCount < 2)
                {
                    col = 1;
                }
                else
                {
                    col = 2;
                }

                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementExists(By.XPath("//html/body/div[1]/div[4]/div[1]/main/div/sb-comp/div/div/div[3]/div[1]/div[2]/div/div[1]")));

                IWebElement tableTodayN = chromeDriver.FindElementByXPath("//html/body/div[1]/div[4]/div[1]/main/div/sb-comp/div[" + col + "]/div/div[3]");

                string html = tableTodayN.GetAttribute("innerHTML");

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                getOdds(cambo88BTI, htmlDoc);
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error,"Va2888 odds table not found : " + elemNotFound);
            }

            if (!oddTableFounded)
            {
                return;
            }
        }

        public void udpateOdd(Bookmaker ibc05)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                int xpathCount = chromeDriver.FindElements(By.XPath("//html/body/div[1]/div[4]/div[1]/main/div/sb-comp/div")).Count;
                int col = 0;
                if (xpathCount < 2)
                {
                    col = 1;
                }
                else
                {
                    col = 2;
                }

                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementExists(By.XPath("//html/body/div[1]/div[4]/div[1]/main/div/sb-comp/div/div/div[3]/div[1]/div[2]/div/div[1]")));

                IWebElement tableTodayN = chromeDriver.FindElementByXPath("//html/body/div[1]/div[4]/div[1]/main/div/sb-comp/div[" + col + "]/div/div[3]");

                string html = tableTodayN.GetAttribute("innerHTML");

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                getOdds(ibc05, htmlDoc);
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error,"Va2888 odds table not found : " + elemNotFound);
            }
        }

        private void getOdds(Bookmaker cambo88BTI, HtmlDocument htmlDoc)
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
            string classFirstTeam = "";
            string classSecondTeam = "";

            List<Odd> listOdds = new List<Odd>();
            bool hasOdd = false;

            List<string> listHdp = new List<string>();

            bool canUseCurrentLeague = true;
            string currentLeague = "";

            try
            {
                foreach (HtmlNode rowLeague in htmlDoc.DocumentNode.SelectNodes(".//div[@class='rj-asian-events__single-league']"))
                {
                    league = rowLeague.SelectSingleNode(".//div[1]/h4").InnerText;
                    league = currentLeague.ToUpper();

                    canUseCurrentLeague = Config.Config.CheckForUselessLeague(league);

                    try
                    {
                        foreach (HtmlNode rowList in rowLeague.SelectNodes(".//div[2]/div[@class='rj-asian-events__single-event']"))
                        {
                            //NLogger.Log(EventLevel.Debug,"++++++++++++++++");
                            //NLogger.Log(EventLevel.Debug,rowList.InnerText);
                            //NLogger.Log(EventLevel.Debug,"++++++++++++++++");

                            foreach (HtmlNode rowMatch in rowList.SelectNodes(".//div[@class='rj-asian-events__row']"))
                            {

                                teamFav = rowMatch.SelectSingleNode(".//div[2]/div/div[1]").InnerText;
                                teamNoFav = rowMatch.SelectSingleNode(".//div[2]/div/div[2]").InnerText;

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
                                {

                                    if (hasOdd)
                                    {
                                        iCurrentMatch++;
                                        cambo88BTI.BookmakerMatches.Add(new Match()
                                        {
                                            MatchName = currentRawMatch,
                                            MatchDate = matchDate,
                                            MatchTime = matchTime,
                                            Matchleague = currentLeague
                                        });

                                        cambo88BTI.BookmakerMatches[iCurrentMatch].TeamFav.TeamName = currentTeamFav;
                                        cambo88BTI.BookmakerMatches[iCurrentMatch].TeamNoFav.TeamName = currentTeamNoFav;

                                        cambo88BTI.BookmakerMatches[iCurrentMatch].Odds = listOdds;
                                    }
                                    currentRawMatch = matchName;
                                    currentTeamFav = teamFav;
                                    currentTeamNoFav = teamNoFav;
                                    currentLeague = league;
                                    hasOdd = false;
                                    listOdds = new List<Odd>();
                                    listHdp = new List<string>();
                                }

                                string handicapType = rowMatch.SelectSingleNode(".//div[3]/div/div[1]/div").InnerText;
                                if (handicapType == "")
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

                                string strOddsFav = rowMatch.SelectSingleNode(".//div[3]/div/div[2]/div").InnerText;
                                string strOddsNoFav = rowMatch.SelectSingleNode(".//div[3]/div/div[3]/div").InnerText;

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
                    catch
                    {
                        NLogger.Log(EventLevel.Error,"Cannot loop cambo88 BTI");
                    }
                }
            }
            catch
            {
                NLogger.Log(EventLevel.Error,"Cannot loop cambo88 BTI");
            }
        }
    }
}
