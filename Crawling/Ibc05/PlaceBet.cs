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
    public partial class Ibc05 : CasinoAPI
    {
        public override BetResult placeBet(string matchName, string winOddType, decimal winOdd, bool winOddFav, bool hedgeBet)
        {

            string table1 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table1 = chromeDriver.FindElementById("tableRunN").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Debug, "tableRunN not found ");
            }
            string table2 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table2 = chromeDriver.FindElementById("tableTodayN").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Debug, "tableTodayN not found ");
            }

            string allTables = table1 + table2;

            Console.WriteLine("we are in place bet ibc");

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(allTables);
            BetResult betResult = findBet(htmlDoc, matchName, winOddType, winOdd, winOddFav, hedgeBet);
            return betResult;

            //IWebElement tableTodayN = chromeDriver.FindElementById("tableTodayN");
            //string html = tableTodayN.GetAttribute("innerHTML");

            //HtmlDocument htmlDoc = new HtmlDocument();
            //htmlDoc.LoadHtml(html);

            //return findBet(htmlDoc, matchName, winOddType, winOdd, winOddFav);


            //try
            //{
            //    IWebElement gridFutur = chromeDriver.FindElementById("tbl2");

            //    string html = gridFutur.GetAttribute("innerHTML");

            //    HtmlDocument htmlDoc = new HtmlDocument();
            //    htmlDoc.LoadHtml(html);

            //    return findBet(htmlDoc, matchName, winOddType, winOdd, winOddFav);
            //}
            //catch (NoSuchElementException elemNotFound)
            //{
            //    NLogger.Log(EventLevel.Error,"Aa2888 odds table on place bet not found");
            //    return false;
            //}
        }
    }
}
