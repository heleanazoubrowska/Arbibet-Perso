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
    public partial class Cambo88UG : CasinoAPI
    {
        public override CasinoBetStatus getBetStatus(string match)
        {

            CasinoBetStatus casinoBetStatus = new CasinoBetStatus();
            decimal odd = 0;

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(5));

            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 1 ) CHECK BET HISTORY FIRST, IF NOT FOUND CHECK WAITING LIST
            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


            // ++++++++++++++++++++++++++++++++
            // We get content of bet history, first we need to refresh the list
            // ++++++++++++++++++++++++++++++++

            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='divMemBets']/div[1]/div/div/div/div[2]")));
                chromeDriver.FindElement(By.XPath("//*[@id='divMemBets']/div[1]/div/div/div/div[2]")).Click();
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot click on refresh Cambo88 UG ");
            }

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(5));
            string bet = "";

            // ++++++++++++++++++++++++++++++++
            // List is refresh, we wait for element inside the list
            // ++++++++++++++++++++++++++++++++

            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id='betContainer']/div[1]/div[1]")));
                bet = chromeDriver.FindElementById("betContainer").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : Bet history empty Cambo88 UG ");
            }


            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);

            try
            {
                var prv_Rows = htmlDoc.DocumentNode.SelectNodes("./div");
                int prv_NumRows = prv_Rows.Count;

                // foreach (HtmlNode rowBet in htmlDoc.DocumentNode.SelectNodes("./div"))
                for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
                {
                    var rowBet = prv_Rows[Loopy];
                    string teamFav = rowBet.SelectSingleNode(".//div[1]/div[2]/div[3]/span[1]").InnerText;
                    string teamNoFav = rowBet.SelectSingleNode(".//div[1]/div[2]/div[3]/span[2]").InnerText;
                    string oddStr = rowBet.SelectSingleNode(".//div[1]/div[2]/div[5]/span").InnerText;

                    teamFav = Common.cleanTeamName(teamFav);
                    teamNoFav = Common.cleanTeamName(teamNoFav);

                    odd = (decimal.Parse(oddStr, CultureInfo.InvariantCulture.NumberFormat));

                    if (match.Contains(teamFav) && match.Contains(teamNoFav))
                    {
                        casinoBetStatus.NewStatus = "running";
                        casinoBetStatus.ActualOdd = odd;
                        return casinoBetStatus;
                    }
                }

            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot loop to bet list CAMBO88 UG");
            }


            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            // 2 ) BET RUNNING NOT FOUND, THEN WE CHECK IN WAITING LIST
            // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


            // ++++++++++++++++++++++++++++++++
            // We get content of bet waiting list, first we need to refresh the list
            // ++++++++++++++++++++++++++++++++

            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='divMemBets']/div[1]/div/div/div/div[1]")));
                chromeDriver.FindElement(By.XPath("//*[@id='divMemBets']/div[1]/div/div/div/div[1]")).Click();
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot click on refresh Cambo88 UG ");
            }

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(5));
            bet = "";

            // ++++++++++++++++++++++++++++++++
            // List is refresh, we wait for element inside the list
            // ++++++++++++++++++++++++++++++++

            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id='betContainer']/div[1]/div[1]")));
                bet = chromeDriver.FindElementById("betContainer").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : Bet history empty Cambo88 UG ");

                casinoBetStatus.NewStatus = "void";

            }


            htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);

            try
            {
                var prv_Rows = htmlDoc.DocumentNode.SelectNodes("./div");
                int prv_NumRows = prv_Rows.Count;

                // foreach (HtmlNode rowBet in htmlDoc.DocumentNode.SelectNodes("./div"))
                for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
                {
                    var rowBet = prv_Rows[Loopy];
                    string teamFav = rowBet.SelectSingleNode(".//div[1]/div[2]/div[3]/span[1]").InnerText;
                    string teamNoFav = rowBet.SelectSingleNode(".//div[1]/div[2]/div[3]/span[2]").InnerText;
                    string oddStr = rowBet.SelectSingleNode(".//div[1]/div[2]/div[5]/span").InnerText;

                    teamFav = Common.cleanTeamName(teamFav);
                    teamNoFav = Common.cleanTeamName(teamNoFav);

                    odd = (decimal.Parse(oddStr, CultureInfo.InvariantCulture.NumberFormat));

                    if (match.Contains(teamFav) && match.Contains(teamNoFav))
                    {
                        casinoBetStatus.NewStatus = "waiting";
                        casinoBetStatus.ActualOdd = odd;
                        return casinoBetStatus;
                    }
                }

                casinoBetStatus.NewStatus = "void";

            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot loop to bet list CAMBO88 UG");
                casinoBetStatus.NewStatus = "void";
            }

            casinoBetStatus.ActualOdd = odd;
            return casinoBetStatus;

        }
    }
}
