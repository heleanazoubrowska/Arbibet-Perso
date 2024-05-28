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
    public partial class Cambo88BTI : CasinoAPI
    {
        public override CasinoBetStatus getBetStatus(string match)
        {

            CasinoBetStatus casinoBetStatus = new CasinoBetStatus();
            decimal odd = 0;
            casinoBetStatus.ActualOdd = odd;

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            //try
            //{
            //    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("betslip-tab-header")));
            //    chromeDriver.FindElement(By.Id("betslip-tab-header")).Click();
            //}
            //catch
            //{
            //    NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot click on betslip-tab-header Cambo88 BTI ");
            //}
            //wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

            //try
            //{
            //    wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='asian-view-mybets']/button[1]")));
            //    chromeDriver.FindElement(By.XPath("//*[@id='asian-view-mybets']/button[1]")).Click();
            //}
            //catch
            //{
            //    NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot click on button bet Cambo88 BTI ");
            //}


            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            string bet = "";
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@data-uat='my-bets-list']")));
                bet = chromeDriver.FindElementByXPath("//*[@data-uat='my-bets-list']").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find my-bets-list Cambo88 BTI ");
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);


            try
            {

                var prv_Rows = htmlDoc.DocumentNode.SelectNodes("./div");
                int prv_NumRows = prv_Rows.Count;

                //foreach (HtmlNode rowBet in htmlDoc.DocumentNode.SelectNodes("./div"))
                for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
                {
                    var rowBet = prv_Rows[Loopy];
                    string teamFav = rowBet.SelectSingleNode(".//div[1]/div[2]/div[1]/span[1]").InnerText;
                    string teamNoFav = rowBet.SelectSingleNode(".//div[1]/div[2]/div[1]/span[3]").InnerText;
                    string oddStr = rowBet.SelectSingleNode(".//div[1]/div[1]/div/span").InnerText;


                    teamFav = Common.cleanTeamName(teamFav);
                    teamNoFav = Common.cleanTeamName(teamNoFav);

                    string matchName = $"{teamFav} - {teamNoFav}";

                    oddStr = oddStr.Replace(" ", "").Replace("@", "");

                    odd = (decimal.Parse(oddStr, CultureInfo.InvariantCulture.NumberFormat));

                    if (match.Contains(teamFav) && match.Contains(teamNoFav))
                    {
                        NLogger.Log(EventLevel.Notice, "!!! WELL DONE, BET IS FOUND Cambo bti !!!!");
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
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot loop to bet list CAMBO88 BTI");
            }

            return casinoBetStatus;
        }
    }
}
