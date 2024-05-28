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
    public partial class Cambo88BTI : CasinoAPI
    {
        public override BetResult placeBet(string matchName, string winOddType, decimal winOdd, bool winOddFav, bool hedgeBet)
        {
            bool placeBetSuccess = false;
            BetResult betResult = new BetResult();
            try
            {
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id='rj-asian-view-events']/sb-comp")));
                IWebElement tableTodayN = chromeDriver.FindElementByXPath("//*[@id='rj-asian-view-events']/sb-comp");

                string html = tableTodayN.GetAttribute("innerHTML");

                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                betResult = findBet(htmlDoc, matchName, winOddType, winOdd, winOddFav, hedgeBet);
            }
            catch (NoSuchElementException elemNotFound)
            {
                NLogger.Log(EventLevel.Error, $"Cambo88 BTI  odds table not found on placebet : {elemNotFound}");
                betResult.BetStatus = 1;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "odds table not found for place";
                return betResult;
            }
            return betResult;
        }
    }
}
