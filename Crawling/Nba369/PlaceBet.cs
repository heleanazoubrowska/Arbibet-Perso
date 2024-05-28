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
    public partial class Nba369 : CasinoAPI
    {
        public override BetResult placeBet(string matchName, string hdpToFind, decimal winOdd, bool winOddFav, bool hedgeBet)
        {


            string table1 = "";
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                table1 = chromeDriver.FindElementByXPath("//*[@id='main-content']/div[1]/div[4]/div[2]").GetAttribute("innerHTML");
            }
            catch
            {
                //NLogger.Log(EventLevel.Debug,"oddsTable_1_1_3_r not found ");
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(table1);
            BetResult betResult = findBet(htmlDoc, matchName, hdpToFind, winOdd, winOddFav, hedgeBet);
            return betResult;


            //IWebElement gridFutur = chromeDriver.FindElementById("tbltoday-content");

            //string html = gridFutur.GetAttribute("innerHTML");

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
