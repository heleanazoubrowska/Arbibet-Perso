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
    public partial class Aa2888 : CasinoAPI
    {
        public override void Report()
        {

            //if (!base.isLogin)
            //{
            //    Login(30);
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

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));


            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//li[@id='menu9']/a")));
            chromeDriver.FindElement(By.XPath("//li[@id='menu9']/a")).Click();

            chromeDriver.SwitchTo().DefaultContent();
            chromeDriver.SwitchTo().Frame("frame_main_content");

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(5));
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[@id='divReport']/table/tbody")));

            int days = chromeDriver.FindElementsByXPath("//div[@id='divReport']/table/tbody/tr").Count;

            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"//div[@id='divReport']/table/tbody/tr[{(days - 1)}]/td[4]/a")));
                chromeDriver.FindElement(By.XPath($"//div[@id='divReport']/table/tbody/tr[{(days - 1)}]/td[4]/a")).Click();

                Thread.Sleep(1000);
                string statement = chromeDriver.FindElementById("tblMain").GetAttribute("innerHTML");


                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(statement);
                processStatement(htmlDoc, report);
            }
            catch
            {

            }


            MainClass.ReportApi.ListReport.Add(report);

            NLogger.Log(EventLevel.Debug, JsonConvert.SerializeObject(report, Formatting.Indented));

        }

        public void processStatement(HtmlDocument htmlDoc, Report report)
        {

            string profitValue = chromeDriver.FindElementByXPath("//*[@id='tblMain']/tfoot/tr/td[5]/font").Text;
            decimal investmentDecimal = decimal.Parse(profitValue, CultureInfo.InvariantCulture.NumberFormat);

            report.ReportTotal = investmentDecimal;

            var prv_Rows = htmlDoc.DocumentNode.SelectNodes(".//tr[@class='main_table_odd']");
            int prv_NumRows = prv_Rows.Count;


            for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
            {
                var rowMatch = prv_Rows[Loopy];
                // Get data from Table
                string teamBet = rowMatch.SelectSingleNode(".//td[7]/span").InnerText;
                string oddTypeValue = rowMatch.SelectSingleNode(".//td[7]").InnerText;
                string oddType = rowMatch.SelectSingleNode(".//td[7]").InnerHtml;
                string teamFav = rowMatch.SelectSingleNode(".//td[6]/span[2]").InnerText;
                string teamNoFav = rowMatch.SelectSingleNode(".//td[6]/span[3]").InnerText;
                string oddBetted = rowMatch.SelectSingleNode(".//td[10]/span[1]").InnerText;
                string winLose = rowMatch.SelectSingleNode(".//td[14]/font").InnerText;

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
                var m2 = Regex.Match(oddType, "(?<=>)(.*)(?=<br>)");
                oddType = m2.Value;
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
