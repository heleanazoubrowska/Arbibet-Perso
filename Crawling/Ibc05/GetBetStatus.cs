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
    public partial class Ibc05 : CasinoAPI
    {
        static Regex prv_RegEx_GetBetStatus_m1 = new Regex("^(.*?)&nbsp;&nbsp;", RegexOptions.Compiled);
        static Regex prv_RegEx_GetBetStatus_m2 = new Regex("^(.*?)&nbsp;&nbsp;", RegexOptions.Compiled);

        public override CasinoBetStatus getBetStatus(string match)
        {

            CasinoBetStatus casinoBetStatus = new CasinoBetStatus();
            decimal odd = 0;
            casinoBetStatus.NewStatus = "void";
            casinoBetStatus.ActualOdd = odd;

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

            try
            {
                chromeDriver.SwitchTo().DefaultContent();
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Cannot switch to default content");
            }
            try
            {
                chromeDriver.SwitchTo().Frame(0);
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Cannot switch to Frame 0");
            }
            try
            {
                chromeDriver.SwitchTo().Frame("mainIframe2");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Cannot switch to mainIframe2");
            }

            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("tableStake2")));
                chromeDriver.FindElement(By.Id("tableStake2")).Click();
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot click on refresh IBC05 ");
            }

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            string bet = "";
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.Id("tableStake")));
                bet = chromeDriver.FindElementById("tableStake").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find lasted10Bet IBC05 ");
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);

            var prv_Rows = htmlDoc.DocumentNode.SelectNodes("./table");
            int prv_NumRows = prv_Rows.Count;

            //foreach (HtmlNode rowBet in htmlDoc.DocumentNode.SelectNodes("./table"))
            for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
            {
                var rowBet = prv_Rows[Loopy];
                string teamFav = "";
                string teamNoFav = "";
                string oddStr = "";
                try
                {
                    teamFav = rowBet.SelectSingleNode("./tbody/tr/td/table/tbody/tr[1]/td/font[1]").InnerText;
                    teamNoFav = rowBet.SelectSingleNode("./tbody/tr/td/table/tbody/tr[1]/td/font[2]").InnerText;
                    oddStr = rowBet.SelectSingleNode("./tbody/tr/td/table/tbody/tr[1]/td/font[3]").InnerText;

                    //var m = Regex.Match(teamFav, "^(.*?)&nbsp;&nbsp;");
                    teamFav = prv_RegEx_GetBetStatus_m1.Match(teamFav).Value;

                    //var m2 = Regex.Match(teamNoFav, "^(.*?)&nbsp;&nbsp;");
                    teamNoFav = prv_RegEx_GetBetStatus_m2.Match(teamNoFav).Value;

                    teamFav = Common.cleanTeamName(teamFav);
                    teamNoFav = Common.cleanTeamName(teamNoFav);

                    NLogger.Log(EventLevel.Warn, $"Watch found : {teamFav} - {teamNoFav}");

                    oddStr = oddStr.Replace("&nbsp;", "");
                    NLogger.Log(EventLevel.Warn, $"oddStr : {oddStr}");
                    odd = (decimal.Parse(oddStr, CultureInfo.InvariantCulture.NumberFormat));
                    NLogger.Log(EventLevel.Warn, $"odd : {odd}");
                    //string matchName = teamFav + " - " + teamNoFav;

                    if (match.Contains(teamFav) && match.Contains(teamNoFav))
                    {
                        NLogger.Log(EventLevel.Debug, "!!! WELL DONE, BET IS FOUND IBC05 !!!!");

                        casinoBetStatus.NewStatus = "running";
                        casinoBetStatus.ActualOdd = odd;
                        return casinoBetStatus;
                    }

                }
                catch(Exception e)
                {
                    NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find TEAM NAME IN LOOP : "+ e);
                }

            }
            chromeDriver.SwitchTo().DefaultContent();
            chromeDriver.SwitchTo().Frame(0);
            chromeDriver.SwitchTo().Frame("mainIframe3");

            if (casinoBetStatus.NewStatus != "running")
            {
                casinoBetStatus.NewStatus = "void";
            }

            return casinoBetStatus;

        }
    }
}
