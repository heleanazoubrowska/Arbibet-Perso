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
    public partial class Va2888 : CasinoAPI
    {

        static Regex prv_RegEx_GetMatch = new Regex(@"(?<=<br>)(.*?)(?=<br><span)", RegexOptions.Compiled);

        static Regex prv_RegEx_GetBetStatus_m1 = new Regex("^.*?(?= x )", RegexOptions.Compiled);
        static Regex prv_RegEx_GetBetStatus_m2 = new Regex(@"(?<= x\s).*", RegexOptions.Compiled);

        static Regex prv_RegEx_GetOdd = new Regex(@"(?<=&nbsp;&nbsp;)(.*?)(?=&nbsp;\(HK\))", RegexOptions.Compiled);

        public override CasinoBetStatus getBetStatus(string match)
        {
            CasinoBetStatus casinoBetStatus = new CasinoBetStatus();
            decimal odd = 0;
            casinoBetStatus.ActualOdd = odd;

            try
            {
                chromeDriver.SwitchTo().DefaultContent();
                chromeDriver.SwitchTo().Frame(chromeDriver.FindElement(By.XPath("//html/body/div[3]/div/iframe")));
                chromeDriver.SwitchTo().Frame("mainIframe2");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot switch frame Va2888 ");
            }

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            string bet = "";

            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("divBetList")));
                chromeDriver.FindElementById("divBetList").Click();
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find lasted10Bet AFB ");
                casinoBetStatus.NewStatus = "void";
            }

            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.Id("tableStake")));
                bet = chromeDriver.FindElementById("tableStake").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find table list Va2888 ");
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);


            var prv_Rows = htmlDoc.DocumentNode.SelectNodes("./table");
            int prv_NumRows = prv_Rows.Count;

            //foreach (HtmlNode rowBet in htmlDoc.DocumentNode.SelectNodes(".//tbody"))
            for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
            {
                try
                {
                    var rowBet = prv_Rows[Loopy];

                    string rowMatch = rowBet.SelectSingleNode("./tbody/tr[1]/td").InnerHtml;

                    string fullMatch = prv_RegEx_GetMatch.Match(rowMatch).Value;

                    string teamFav = prv_RegEx_GetBetStatus_m1.Match(fullMatch).Value;

                    string teamNoFav = prv_RegEx_GetBetStatus_m2.Match(fullMatch).Value;

                    string oddStr = prv_RegEx_GetOdd.Match(rowMatch).Value;

                    oddStr = oddStr.Replace("&nbsp;", "");

                    teamFav = Common.cleanTeamName(teamFav);
                    teamNoFav = Common.cleanTeamName(teamNoFav);

                    NLogger.Log(EventLevel.Warn, $"Watch found : {teamFav} - {teamNoFav}");
                    NLogger.Log(EventLevel.Warn, $"Watch to find : {match}");


                    odd = (decimal.Parse(oddStr, CultureInfo.InvariantCulture.NumberFormat));

                    //string matchName = teamFav + " - " + teamNoFav;

                    if (match.Contains(teamFav) && match.Contains(teamNoFav))
                    {
                        NLogger.Log(EventLevel.Notice, "!!! WELL DONE, BET IS FOUND Va2888 !!!!");

                        casinoBetStatus.NewStatus = "running";
                        casinoBetStatus.ActualOdd = odd;
                        return casinoBetStatus;

                        //if (rowBet.SelectSingleNode("./tbody/tr[2]/td[1]/span").InnerText.Contains("Accepted"))
                        //{
                        //    casinoBetStatus.NewStatus = "running";
                        //    casinoBetStatus.ActualOdd = odd;
                        //    return casinoBetStatus;
                        //}
                        //else
                        //{
                        //    casinoBetStatus.NewStatus = "waiting";
                        //    casinoBetStatus.ActualOdd = odd;
                        //}
                    }
                }

                catch(Exception e)
                {
                    NLogger.Log(EventLevel.Error, "Error find bet on  VA2888 : "+ e);
                }
            }

            if (casinoBetStatus.NewStatus != "running")
            {
                casinoBetStatus.NewStatus = "void";
            }

            chromeDriver.SwitchTo().DefaultContent();
            chromeDriver.SwitchTo().Frame(chromeDriver.FindElement(By.XPath("//html/body/div[3]/div/iframe")));
            chromeDriver.SwitchTo().Frame("fraMain");

            return casinoBetStatus;

        }
    }
}
