using System;
using System.Globalization;
using System.Linq;
using System.Threading;

using HtmlAgilityPack;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class B855bet : CasinoAPI
    {
        public override BetResult placeBet(string matchName, string winOddType, decimal winOdd, bool winOddFav, bool hedgeBet)
        {

            string table1 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table1 = chromeDriver.FindElementById("oTableContainer_L").GetAttribute("innerHTML");
            }
            catch
            {

            }
            string table2 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table2 = chromeDriver.FindElementById("oTableContainer_C").GetAttribute("innerHTML");
            }
            catch
            {

            }

            string table3 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table3 = chromeDriver.FindElementById("oTableContainer_D").GetAttribute("innerHTML");
            }
            catch
            {

            }

            string allTables = $"{table1}{table2}{table3}";

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(allTables);
            BetResult betResult = findBet(htmlDoc, matchName, winOddType, winOdd, winOddFav, hedgeBet);
            return betResult;

        }
    }
}
