using System;

using ArbibetProgram.Models;
using System.Threading;

using HtmlAgilityPack;

using OpenQA.Selenium;
using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Globalization;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Ibc05 : CasinoAPI
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
            chromeDriver.SwitchTo().Frame(0);
            chromeDriver.SwitchTo().Frame(0);

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));


            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='navigator']/ul/li[5]/a")));
            chromeDriver.FindElement(By.XPath("//*[@id='navigator']/ul/li[5]/a")).Click();

            chromeDriver.SwitchTo().DefaultContent();
            chromeDriver.SwitchTo().Frame(0);
            chromeDriver.SwitchTo().Frame("mainIframe3");

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(5));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl00_grdHistory")));

            int days = chromeDriver.FindElementsByXPath("//*[@id='ctl00_grdHistory']/tbody/tr").Count;

            string totalValue = chromeDriver.FindElement(By.XPath($"//*[@id='ctl00_grdHistory']/tbody/tr[{(days - 1)}]/td[4]/span/span")).Text;
            decimal totalDecimal = decimal.Parse(totalValue, CultureInfo.InvariantCulture.NumberFormat);
            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"//*[@id='ctl00_grdHistory']/tbody/tr[{(days - 4)}]/td[2]/a")));
                chromeDriver.FindElement(By.XPath($"//*[@id='ctl00_grdHistory']/tbody/tr[{(days - 1)}]/td[2]/a")).Click();

                Thread.Sleep(1000);
                string statement = chromeDriver.FindElementById("AccMatchWL_mb1_g").GetAttribute("innerHTML");

                Console.WriteLine("Has link");

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(statement);
                processStatement(htmlDoc, report);

            }
            catch
            {
                Console.WriteLine("NO link");
            }


            MainClass.ReportApi.ListReport.Add(report);
            NLogger.Log(EventLevel.Debug, JsonConvert.SerializeObject(report, Formatting.Indented));

        }

        public void processStatement(HtmlDocument htmlDoc, Report report)
        {

            string profitValue = chromeDriver.FindElementByXPath("//*[@id='tblMain']/tfoot/tr/td[5]/font").Text;
            decimal investmentDecimal = decimal.Parse(profitValue, CultureInfo.InvariantCulture.NumberFormat);

            var prv_Rows = htmlDoc.DocumentNode.SelectNodes(".//tr");
            int prv_NumRows = prv_Rows.Count;


            for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
            {
                var rowMatch = prv_Rows[Loopy];
                // Get data from Table

                string rawMatch = rowMatch.SelectSingleNode(".//td[3]/a").InnerText;

                string winLose = rowMatch.SelectSingleNode(".//td[5]/span").InnerText;


                // Proccess Data
                var m = Regex.Match(rawMatch, "^.*(?=(vs))");
                string teamFav = m.Value;

                var m2 = Regex.Match(rawMatch, "[^vs]*$");
                string teamNoFav = m2.Value;


                teamFav = Common.cleanTeamName(teamFav);
                teamNoFav = Common.cleanTeamName(teamNoFav);

                string match = $"{teamFav} - {teamNoFav}";

                winLose = winLose.Replace(" ", "");

                report.ReportBetList.Add(new ReportBetList()
                {
                    ReportBetMatch = match,
                    ReportBetWinLosePrice = decimal.Parse(winLose, CultureInfo.InvariantCulture.NumberFormat)
            });

            }

        }
    }
}
