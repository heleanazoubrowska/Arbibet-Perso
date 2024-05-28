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
    public partial class Nba369 : CasinoAPI
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


            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

            wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("tabstatement")));
            chromeDriver.FindElement(By.Id("tabstatement")).Click();

            Thread.Sleep(2000);

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(5));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tbetlist")));

            int days = chromeDriver.FindElementsByXPath("//tbody[@id='tbetlist']/tr").Count;

            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"//tbody[@id='tbetlist']/tr[{(days - 1)}]/td[4]/a")));
            chromeDriver.FindElement(By.XPath($"//tbody[@id='tbetlist']/tr[{(days - 1)}]/td[4]/a")).Click();

            Thread.Sleep(3000);

            try
            {

                string totalValue = chromeDriver.FindElement(By.XPath($"//*[@id='tbl-winlose_by_game_body']/tr[1]/td[6]/b")).Text;
                decimal totalDecimal = decimal.Parse(totalValue, CultureInfo.InvariantCulture.NumberFormat);

                report.ReportTotal = totalDecimal;

                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"//*[@id='tbl-winlose_by_game_body']/tr[1]/td[1]/a")));
                chromeDriver.FindElement(By.XPath($"//*[@id='tbl-winlose_by_game_body']/tr[1]/td[1]/a")).Click();


                string statement = chromeDriver.FindElementById("tbl-winlose_detail_by_game_body").GetAttribute("innerHTML");


                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(statement);
                processStatement(htmlDoc, report);
            }
            catch
            {

            }

            MainClass.ReportApi.ListReport.Add(report);
            Console.WriteLine(JsonConvert.SerializeObject(report, Formatting.Indented));



        }

        private void processStatement(HtmlDocument htmlDoc, Report report)
        {


            var prv_Rows = htmlDoc.DocumentNode.SelectNodes(".//tr");
            int prv_NumRows = prv_Rows.Count;


            for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
            {
                var rowMatch = prv_Rows[Loopy];
                // Get data from Table
                string teamBet = rowMatch.SelectSingleNode(".//td[3]/div[1]/span/span[1]").InnerText;
                string oddTypeValue = rowMatch.SelectSingleNode(".//td[3]/div[1]/span/span[2]").InnerText;
                string oddType = rowMatch.SelectSingleNode(".//td[3]/div[2]/span/span").InnerHtml;
                string teamFav = rowMatch.SelectSingleNode(".//td[3]/div[3]/span/span[1]").InnerText;
                string teamNoFav = rowMatch.SelectSingleNode(".//td[3]/div[3]/span/span[2]").InnerText;
                string oddBetted = rowMatch.SelectSingleNode(".//td[4]/span[1]").InnerText;
                string winLose = rowMatch.SelectSingleNode(".//td[6]/span/span[1]/font").InnerText;
                string winLosePrice = rowMatch.SelectSingleNode(".//td[7]/font").InnerText;

                teamFav = Common.cleanTeamName(teamFav);
                teamNoFav = Common.cleanTeamName(teamNoFav);

                string match = $"{teamFav} - {teamNoFav}";

                var m1 = Regex.Match(oddTypeValue, "[^&nbsp;&nbsp;]*$");
                oddTypeValue = m1.Value;

                oddTypeValue = oddTypeValue.Replace("-", "");
                oddTypeValue = oddTypeValue.Replace(" ", "");
                oddTypeValue = Common.getQuarterBet(oddTypeValue);

                string timeBet = "";
                string hdpType = "";
                if (oddType.Contains("1ST Half"))
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

                winLosePrice = winLosePrice.Replace(" ", "");
                decimal winLosePriceDecimal = decimal.Parse(winLosePrice, CultureInfo.InvariantCulture.NumberFormat);

                report.ReportBetList.Add(new ReportBetList()
                {
                    ReportBetMatch = match,
                    ReportBetTeam = teamBet,
                    ReportBetOddType = finalBetOddType,
                    ReportBetOddBet = oddBetted,
                    ReportBetWinLose = winLose,
                    ReportBetWinLosePrice = winLosePriceDecimal,
                });

            }

        }
    }
}
