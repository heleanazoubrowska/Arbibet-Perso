using System;
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
    public partial class Cambo88SBO : CasinoAPI
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


            chromeDriver.SwitchTo().Window(chromeDriver.WindowHandles[0]);

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='js_headerNav']/a[14]")));
            chromeDriver.FindElementByXPath("//*[@id='js_headerNav']/a[14]").Click();


            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='switchTab']/div[1]")));
            chromeDriver.FindElement(By.XPath("//*[@id='switchTab']/div[1]")).Click();

            Thread.Sleep(3000);

            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("yestaday_record")));
            chromeDriver.FindElement(By.Id("yestaday_record")).Click();


            Thread.Sleep(10000);

            IWebElement searchBtn = chromeDriver.FindElementByXPath("//*[@id='tr_history']/div[1]/div[7]/div");

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.TextToBePresentInElement(searchBtn, "Search"));



            string statement = chromeDriver.FindElementByXPath("//*[@id='tr_history']/div[2]/table/tbody[1]").GetAttribute("innerHTML");

            Console.WriteLine(statement);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(statement);
            processStatement(htmlDoc, report);

            MainClass.ReportApi.ListReport.Add(report);
            Console.WriteLine(JsonConvert.SerializeObject(report, Formatting.Indented));

        }

        public void processStatement(HtmlDocument htmlDoc, Report report)
        {

            string investmentValue = chromeDriver.FindElementByXPath("//*[@id='tr_history']/div[2]/table/tbody[2]/tr/td[6]").Text;
            string profitValue = chromeDriver.FindElementByXPath("//*[@id='tr_history']/div[2]/table/tbody[2]/tr/td[7]").Text;

            decimal investmentDecimal = decimal.Parse(investmentValue, CultureInfo.InvariantCulture.NumberFormat);
            decimal profitDecimal = decimal.Parse(profitValue, CultureInfo.InvariantCulture.NumberFormat);


            report.ReportTotal = profitDecimal - investmentDecimal;


            var prv_Rows = htmlDoc.DocumentNode.SelectNodes("./tr");
            int prv_NumRows = prv_Rows.Count;


            for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
            {

                var rowMatch = prv_Rows[Loopy];

                if (rowMatch.SelectSingleNode("./td[5]/div/div[1]") == null)
                {
                    continue;
                }


                // Get data from Table
                string teamBetAndOdd = rowMatch.SelectSingleNode("./td[5]/div/div[1]").InnerText;
                //string oddTypeValue = rowMatch.SelectSingleNode(".//td[3]/span[2]").InnerText;
                string oddBetted = rowMatch.SelectSingleNode("./td[5]/div/div[2]").InnerText;
                string winLose = rowMatch.SelectSingleNode("./td[9]").InnerText;
                string priceBet = rowMatch.SelectSingleNode("./td[6]/span").InnerText;
                string winLosePrice = rowMatch.SelectSingleNode("./td[7]/span").InnerText;

                // Proccess Data

                string match = "NO NAME FOUND";


                var o = Regex.Match(teamBetAndOdd, "^.*(?=(:))");
                string teamBet = o.Value;
                teamBet = teamBet.Replace(" ", "");

                var o2 = Regex.Match(teamBetAndOdd, "[^:]*$");
                string oddTypeValue = o2.Value;


                oddBetted = oddBetted.Replace(" ", "");
                winLose = winLose.Replace(" ", "");

                priceBet = priceBet.Replace(" ", "");
                winLosePrice = winLosePrice.Replace(" ", "");

                decimal priceBetDecimal = decimal.Parse(priceBet, CultureInfo.InvariantCulture.NumberFormat);
                decimal winLosePriceDecimal = decimal.Parse(winLosePrice, CultureInfo.InvariantCulture.NumberFormat);

                decimal resultBet = winLosePriceDecimal - priceBetDecimal;

                report.ReportBetList.Add(new ReportBetList()
                {
                    ReportBetMatch = match,
                    ReportBetTeam = teamBet,
                    ReportBetOddBet = oddBetted,
                    ReportBetWinLose = winLose,
                    ReportBetWinLosePrice = resultBet,
                });

            }
        }
    }
}
