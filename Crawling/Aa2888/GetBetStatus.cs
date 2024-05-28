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
    public partial class Aa2888 : CasinoAPI
    {
        public override CasinoBetStatus getBetStatus(string match)
        {

            CasinoBetStatus casinoBetStatus = new CasinoBetStatus();
            decimal odd = 0;
            casinoBetStatus.ActualOdd = 0;

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("aRefreshLastBets")));
                chromeDriver.FindElement(By.Id("aRefreshLastBets")).Click();
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot click on refresh Aa2888 ");
                casinoBetStatus.NewStatus = "void";
            }

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            string bet = "";
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.Id("divBetList")));
                bet = chromeDriver.FindElementById("divBetList").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find lasted10Bet Aa2888 ");
                casinoBetStatus.NewStatus = "void";
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);

            try
            {
                var prv_Rows = htmlDoc.DocumentNode.SelectNodes("//*[@id='last10betlist']/tbody/tr");
                int prv_NumRows = prv_Rows.Count;
                casinoBetStatus.NewStatus = "not found";
                //foreach (HtmlNode rowBet in htmlDoc.DocumentNode.SelectNodes("./tr"))
                for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
                {
                    var rowBet = prv_Rows[Loopy];
                    string teamFav = rowBet.SelectSingleNode("./td/table/tbody/tr[1]/td/span").InnerText;
                    string teamNoFav = rowBet.SelectSingleNode("./td/table/tbody/tr[2]/td/span").InnerText;
                    string oddStr = rowBet.SelectSingleNode("./td/table/tbody/tr[4]/td[1]/font/b").InnerText;

                    teamFav = Common.cleanTeamName(teamFav);
                    teamNoFav = Common.cleanTeamName(teamNoFav);

                    odd = (decimal.Parse(oddStr, CultureInfo.InvariantCulture.NumberFormat));

                    string matchName = teamFav + " - " + teamNoFav;
                    //NLogger.Log(EventLevel.Debug, "matchName : "+ matchName);
                    if (match.Contains(teamFav) && match.Contains(teamNoFav))
                    {
                        NLogger.Log(EventLevel.Debug, "!!! WELL DONE, BET IS FOUND Aa2888 !!!!");
                        casinoBetStatus.NewStatus = "running";
                        casinoBetStatus.ActualOdd = odd;
                        return casinoBetStatus;
                    }
                }

                if (casinoBetStatus.NewStatus != "running")
                {
                    casinoBetStatus.NewStatus = "void";
                }

            }
            catch(Exception e)
            {
                NLogger.Log(EventLevel.Error, "Cannot loop into Aa2888 ");
                NLogger.LogError(e);
                casinoBetStatus.NewStatus = "void";
            }

            

            return casinoBetStatus;

        }
    }
}
