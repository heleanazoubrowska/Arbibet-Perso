using System;
using HtmlAgilityPack;
using OpenQA.Selenium;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using ArbibetProgram.Models;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using System.Globalization;

namespace ArbibetProgram.Crawling
{
    public partial class Nba369 : CasinoAPI
    {
        public override CasinoBetStatus getBetStatus(string match)
        {

            CasinoBetStatus casinoBetStatus = new CasinoBetStatus();
            casinoBetStatus.ActualOdd = 0;

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("aRefreshLastBets")));
                chromeDriver.FindElement(By.Id("aRefreshLastBets")).Click();
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot click on refresh NBA ");
            }

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            string bet = "";
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("lasted10Bet")));
                bet = chromeDriver.FindElementById("lasted10Bet").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find lasted10Bet NBA ");
                casinoBetStatus.NewStatus = "void";
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);

            try
            {

                var prv_Rows = htmlDoc.DocumentNode.SelectNodes("./tr");
                int prv_NumRows = prv_Rows.Count;

                //foreach (HtmlNode rowBet in htmlDoc.DocumentNode.SelectNodes("./tr"))
                for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
                {
                    var rowBet = prv_Rows[Loopy];

                    string teamFav = rowBet.SelectSingleNode("./td/table/tbody/tr[2]/td/span").InnerText;
                    string teamNoFav = rowBet.SelectSingleNode("./td/table/tbody/tr[3]/td/span").InnerText;

                    string oddStr = rowBet.SelectSingleNode("./td/table/tbody/tr[5]/td[1]/font/b").InnerText;

                    teamFav = Common.cleanTeamName(teamFav);
                    teamNoFav = Common.cleanTeamName(teamNoFav);

                    decimal odd = (decimal.Parse(oddStr, CultureInfo.InvariantCulture.NumberFormat));

                    //string matchName = teamFav + " - " + teamNoFav;

                    if (match.Contains(teamFav) && match.Contains(teamNoFav))
                    {
                        NLogger.Log(EventLevel.Debug, "!!! WELL DONE, BET IS FOUND NBA !!!!");
                        casinoBetStatus.NewStatus = "running";
                        casinoBetStatus.ActualOdd = odd;
                        return casinoBetStatus;
                    }
                }

                if(casinoBetStatus.NewStatus != "running")
                {
                    casinoBetStatus.NewStatus = "void";
                }
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot loop to bet list NBA");
            }

            

            return casinoBetStatus;
        }
    }
}
