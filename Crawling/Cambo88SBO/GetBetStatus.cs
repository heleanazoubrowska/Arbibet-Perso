using System;
using HtmlAgilityPack;
using OpenQA.Selenium;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using ArbibetProgram.Models;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Cambo88SBO : CasinoAPI
    {
        static Regex prv_RegEx_GetBetStatus_m1 = new Regex("^(.*?)-vs-", RegexOptions.Compiled);
        static Regex prv_RegEx_GetBetStatus_m2 = new Regex("[^-vs-]*$", RegexOptions.Compiled);

        public override CasinoBetStatus getBetStatus(string match)
        {

            CasinoBetStatus casinoBetStatus = new CasinoBetStatus();
            decimal odd = 0;

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            string bet = "";
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.Id("mb:pb:data")));
                bet = chromeDriver.FindElementById("mb:pb:data").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find mb:pb:data CAMBO88 SBO ");
            }



            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);



            var prv_Rows = htmlDoc.DocumentNode.SelectNodes(".//my-bet-item");
            int prv_NumRows = prv_Rows.Count;

            //foreach (HtmlNode rowBet in htmlDoc.DocumentNode.SelectNodes(".//my-bet-item"))
            for (int Loopy = 0; Loopy < prv_NumRows; Loopy++)
            {
                var rowBet = prv_Rows[Loopy];

                string rowMatch = rowBet.SelectSingleNode("./div[2]").InnerText;

                //var m = Regex.Match(rowMatch, "^(.*?)-vs-");
                string teamFav = prv_RegEx_GetBetStatus_m1.Match(rowMatch).Value;

                //var m2 = Regex.Match(rowMatch, "[^-vs-]*$");
                string teamNoFav = prv_RegEx_GetBetStatus_m2.Match(rowMatch).Value;

                teamFav = Common.cleanTeamName(teamFav);
                teamNoFav = Common.cleanTeamName(teamNoFav);

                if (match.Contains(teamFav) && match.Contains(teamNoFav))
                {
                    NLogger.Log(EventLevel.Debug, "!!! WELL DONE, BET IS FOUND CAMBO88 SBO!!!!");
                }

                string matchName = $"{teamFav} - {teamNoFav}";

            }

            casinoBetStatus.ActualOdd = odd;

            return casinoBetStatus;
        }
    }
}
