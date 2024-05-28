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
    public partial class Aa2888 : CasinoAPI
    {


        public override BetResult placeBet(string matchName, string winOddType, decimal winOdd, bool winOddFav, bool hedgeBet)
        {


            string table1 = "";
            try
            {
                IWebElement gridFutur = chromeDriver.FindElementByXPath("//*[@id='tbl3']/tbody");
                table1 = gridFutur.GetAttribute("innerHTML");
            }
            catch
            {
                //NLogger.Log(EventLevel.Debug,"oddsTable_1_1_3_r not found ");
            }
            string table2 = "";
            try
            {
                IWebElement gridFutur = chromeDriver.FindElementByXPath("//*[@id='tbl2']/tbody");
                table2 = gridFutur.GetAttribute("innerHTML");
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
