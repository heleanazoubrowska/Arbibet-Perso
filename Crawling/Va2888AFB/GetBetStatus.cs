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
using System.Text.RegularExpressions;

namespace ArbibetProgram.Crawling
{
    public partial class Va2888AFB : CasinoAPI
    {

        static Regex prv_RegEx_GetMatch = new Regex(@"(?<=font><br>)(.*?)(?=<br>)", RegexOptions.Compiled);

        static Regex prv_RegEx_GetBetStatus_m1 = new Regex("^.*?(?= v )", RegexOptions.Compiled);
        static Regex prv_RegEx_GetBetStatus_m2 = new Regex(@"(?<= v\s).*", RegexOptions.Compiled);

        static Regex prv_RegEx_GetOdds = new Regex(@"(?<=\)&nbsp;)(.*?)(?=<span)", RegexOptions.Compiled);

        public override CasinoBetStatus getBetStatus(string match)
        {
            CasinoBetStatus casinoBetStatus = new CasinoBetStatus();
            casinoBetStatus.ActualOdd = 0;

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            string bet = "";

            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='sblistbtn']/div[2]")));
                chromeDriver.FindElementByXPath("//*[@id='sblistbtn']/div[2]").Click();
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find lasted10Bet AFB ");
                casinoBetStatus.NewStatus = "void";
            }

            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.Id("betsInfo")));
                bet = chromeDriver.FindElementById("betsInfo").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find lasted10Bet AFB ");
                casinoBetStatus.NewStatus = "void";
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);

            var prv_Rows = htmlDoc.DocumentNode.SelectNodes(".//table");
            int prv_NumRows = prv_Rows.Count;

            //foreach (HtmlNode rowBet in htmlDoc.DocumentNode.SelectNodes("./tr"))
            for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
            {
                try
                {
                    var rowBet = prv_Rows[Loopy];

                    string rowMatch = rowBet.SelectSingleNode("./tbody/tr/td/table/tbody/tr[1]/td/span[3]").InnerHtml;

                    string fullMatch = prv_RegEx_GetMatch.Match(rowMatch).Value;

                    //var m = Regex.Match(rowMatch, "^(.*?)-vs-");
                    string teamFav = prv_RegEx_GetBetStatus_m1.Match(fullMatch).Value;

                    //var m2 = Regex.Match(rowMatch, "[^-vs-]*$");
                    string teamNoFav = prv_RegEx_GetBetStatus_m2.Match(fullMatch).Value;

                    string rawOddStr = rowBet.SelectSingleNode("./tbody/tr/td/table/tbody/tr[1]/td/b").InnerHtml;

                    teamFav = Common.cleanTeamName(teamFav);
                    teamNoFav = Common.cleanTeamName(teamNoFav);

                    NLogger.Log(EventLevel.Warn, $"Watch found : {teamFav} - {teamNoFav}");
                    NLogger.Log(EventLevel.Warn, $"Watch to find : {match}");

                    string oddStr = prv_RegEx_GetOdds.Match(rawOddStr).Value;
                    NLogger.Log(EventLevel.Warn, $"odd str : {oddStr}");
                    oddStr = oddStr.Replace("&nbsp;", "");
                    decimal odd = (decimal.Parse(oddStr, CultureInfo.InvariantCulture.NumberFormat));

                    

                    //string matchName = teamFav + " - " + teamNoFav;

                    if (match.Contains(teamFav) && match.Contains(teamNoFav))
                    {
                        NLogger.Log(EventLevel.Debug, "!!! WELL DONE, BET IS FOUND AFB !!!!");
                        casinoBetStatus.NewStatus = "running";
                        casinoBetStatus.ActualOdd = odd;
                        return casinoBetStatus;
                    }
                }
                catch(Exception e)
                {
                    NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot loop to bet list AFB : "+e);
                }
            }

            if (casinoBetStatus.NewStatus != "running")
            {
                casinoBetStatus.NewStatus = "void";
            }

            return casinoBetStatus;

        }
    }
}
