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
    public partial class B855bet : CasinoAPI
    {
        static Regex prv_RegEx_GetBetStatus_m1 = new Regex("^(.*?)vs", RegexOptions.Compiled);
        static Regex prv_RegEx_GetBetStatus_m2 = new Regex("[^vs]*$", RegexOptions.Compiled);

        public override CasinoBetStatus getBetStatus(string match)
        {

            CasinoBetStatus casinoBetStatus = new CasinoBetStatus();
            decimal odd = 0;
            casinoBetStatus.ActualOdd = odd;

            try
            {
                chromeDriver.SwitchTo().DefaultContent();
                chromeDriver.SwitchTo().Frame("leftFrame");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot switch frame 855BET ");
            }

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            string bet = "";
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.Id("betlistdiv")));
                bet = chromeDriver.FindElementById("betlistdiv").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find betlistdiv 855BET ");
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);

            if (htmlDoc.DocumentNode.SelectSingleNode(".//tbody[1]") == null)
            {
                casinoBetStatus.NewStatus = "void";
            }

            try
            {

                var prv_Rows = htmlDoc.DocumentNode.SelectNodes(".//tbody");
                int prv_NumRows = prv_Rows.Count;

                //foreach (HtmlNode rowBet in htmlDoc.DocumentNode.SelectNodes(".//tbody"))
                for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
                {
                    var rowBet = prv_Rows[Loopy];

                    string rowMatch = rowBet.SelectSingleNode(".//tr[4]/td[1]").InnerText;

                    //var m1 = Regex.Match(rowMatch, "^(.*?)vs");
                    string teamFav = prv_RegEx_GetBetStatus_m1.Match(rowMatch).Value;

                    //var m2 = Regex.Match(rowMatch, "[^vs]*$");
                    string teamNoFav = prv_RegEx_GetBetStatus_m2.Match(rowMatch).Value;

                    string oddStr = rowBet.SelectSingleNode(".//tr[6]/td[1]/span[2]").InnerText;


                    teamFav = Common.cleanTeamName(teamFav);
                    teamNoFav = Common.cleanTeamName(teamNoFav);

                    odd = (decimal.Parse(oddStr, CultureInfo.InvariantCulture.NumberFormat));

                    //string matchName = teamFav + " - " + teamNoFav;

                    if (match.Contains(teamFav) && match.Contains(teamNoFav))
                    {
                        NLogger.Log(EventLevel.Notice, "!!! WELL DONE, BET IS FOUND 855bet !!!!");
                        if (rowBet.SelectSingleNode(".//tr[5]/td[2]/span/span").InnerText.Contains("Running"))
                        {
                            casinoBetStatus.NewStatus = "running";
                            casinoBetStatus.ActualOdd = odd;
                            return casinoBetStatus;
                        }
                        else
                        {
                            casinoBetStatus.NewStatus = "waiting";
                            casinoBetStatus.ActualOdd = odd;
                            return casinoBetStatus;
                        }
                    }
                }

                if (casinoBetStatus.NewStatus != "running")
                {
                    casinoBetStatus.NewStatus = "void";
                }
            }
            catch
            {
                casinoBetStatus.NewStatus = "void";
            }
            
            return casinoBetStatus;

        }
    }
}
