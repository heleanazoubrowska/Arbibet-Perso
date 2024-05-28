using System;
using System.Threading;

using HtmlAgilityPack;

using OpenQA.Selenium;
using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class B855bet : CasinoAPI
    {
        public override void Report()
        {

            //if (!base.isLogin)
            //{
            //    Connect();
            //}

            Report report = new Report()
            {
                RepportAccount = base.accountAPI.AccountName,
                RepportBookmaker = base.accountAPI.AccountBookmakerName,
                ReportDate = "",
                ReportTotal = 0
            };


            chromeDriver.SwitchTo().DefaultContent();
            chromeDriver.SwitchTo().Frame("topFrame");


            chromeDriver.FindElement(By.XPath("//*[@id='logonTxt']/a[2]")).Click();

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

            // Get number of day report and check the last 2

            //Thread.Sleep(1000);

            //chromeDriver.SwitchTo().ParentFrame();

            //wait.Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.Id("mainFrame")));
            chromeDriver.SwitchTo().DefaultContent();
            chromeDriver.SwitchTo().Frame("mainFrame");


            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//tbody[@id='statementContainer']/tr")));

            int days = chromeDriver.FindElementsByXPath("//tbody[@id='statementContainer']/tr").Count;

            // Click on previous day

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"//tbody[@id='statementContainer']/tr[{(days - 1)}]/td[1]/span")));
            chromeDriver.FindElement(By.XPath($"//tbody[@id='statementContainer']/tr[{(days - 1)}]/td[1]/span")).Click();


            // Click on sport book

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//tbody[@id='statementContainer']/tr[1]/td[1]/a")));
            chromeDriver.FindElement(By.XPath("//tbody[@id='statementContainer']/tr[1]/td[1]/a")).Click();

            // get Statement table

            Thread.Sleep(1000);
            string statement = chromeDriver.FindElementById("statementContainer").GetAttribute("innerHTML");


            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(statement);
            processStatement(htmlDoc, report);

            MainClass.ReportApi.ListReport.Add(report);
            NLogger.Log(EventLevel.Debug, JsonConvert.SerializeObject(report, Formatting.Indented));

        }

        public void processStatement(HtmlDocument htmlDoc, Report report)
        {

            
            var prv_Rows = htmlDoc.DocumentNode.SelectNodes(".//tr");
            int prv_NumRows = prv_Rows.Count;

            //foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes(".//tr"))
            for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
            {
	            var rowMatch = prv_Rows[Loopy];
                // Get data from Table
                string teamBet = rowMatch.SelectSingleNode(".//td[3]/span[1]").InnerText;
                string oddTypeValue = rowMatch.SelectSingleNode(".//td[3]/span[2]").InnerText;
                string oddType = rowMatch.SelectSingleNode(".//td[3]/div[1]/span[1]").InnerText;
                string rawMatch = rowMatch.SelectSingleNode(".//td[3]/span[3]").InnerText;
                string oddBetted = rowMatch.SelectSingleNode(".//td[5]/span[1]").InnerText;
                string winLose = rowMatch.SelectSingleNode(".//td[6]/span[1]").InnerText;


                // Proccess Data
                var m = Regex.Match(rawMatch, "^.*(?=(-VS-))");
                string teamFav = m.Value;

                var m2 = Regex.Match(rawMatch, "[^-VS-]*$");
                string teamNoFav = m2.Value;

                teamFav = Common.cleanTeamName(teamFav);
                teamNoFav = Common.cleanTeamName(teamNoFav);

                string match = $"{teamFav} - {teamNoFav}";

                oddTypeValue = oddTypeValue.Replace("-", "");
                oddTypeValue = oddTypeValue.Replace(" ", "");
                oddTypeValue = Common.getQuarterBet(oddTypeValue);

                string timeBet = "";
                string hdpType = "";
                if (oddType.Contains("First half"))
                {
                    timeBet = "fh";
                }
                else
                {
                    timeBet = "ft";
                }

                if (oddType.Contains("Handicap"))
                {
                    hdpType = "";
                }
                else
                {
                    hdpType = " o/u";
                }

                string finalBetOddType = $"{timeBet}{hdpType} {oddTypeValue}";

                if (teamBet == "Over")
                {
                    teamBet = teamFav;
                }

                if (teamBet == "Under")
                {
                    teamBet = teamNoFav;
                }

                oddBetted = oddBetted.Replace(" ", "");
                winLose = winLose.Replace(" ", "");

                report.ReportBetList.Add(new ReportBetList()
                {
                    ReportBetMatch = match,
                    ReportBetTeam = teamBet,
                    ReportBetOddType = finalBetOddType,
                    ReportBetOddBet = oddBetted,
                    ReportBetWinLose = winLose
                });

            }
        }
    }
}
