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
    public partial class Ibet789 : CasinoAPI
    {
        public override BetResult placeBet(string matchName, string winOddType, decimal winOdd, bool winOddFav, bool hedgeBet)
        {

            string table1 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table1 = chromeDriver.FindElementByXPath("//*[@id='tableRun']/table[1]/tbody/tr[2]/td/table/tbody/tr[2]/td/table").GetAttribute("innerHTML");
            }
            catch
            {
                //NLogger.Log(EventLevel.Debug,"oddsTable_1_1_3_r not found ");
            }
            string table2 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table2 = chromeDriver.FindElementByXPath("//*[@id='tableToday']/table[1]/tbody/tr[2]/td/table/tbody/tr[2]/td/table").GetAttribute("innerHTML");
            }
            catch
            {
                //NLogger.Log(EventLevel.Debug,"oddsTable_1_1_3_t not found ");
            }

            string allTables = table1 + table2;

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(allTables);
            BetResult betResult = findBet(htmlDoc, matchName, winOddType, winOdd, winOddFav, hedgeBet);
            return betResult;

        }
    }
}
