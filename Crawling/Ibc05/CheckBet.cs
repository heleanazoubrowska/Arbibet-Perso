using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using HtmlAgilityPack;
using System;
using System.Threading;
using System.Globalization;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Ibc05 : CasinoAPI
    {
        //private BetResult checkBet(string oddPath, int line2I, string idRowLeague, string hdpToFind)
        protected override BetResult checkBet(string oddPath, string line1I, string hdpToFind, CheckBet_ExtraParams prm_ExtraParams = null)
        {

            NLogger.Log(EventLevel.Debug, "here 11");
            //string panSportsAttr = chromeDriver.FindElement(By.Id("panSports")).GetAttribute("style");
            //NLogger.Log(EventLevel.Debug,"panSportsAttr : " + panSportsAttr);
            bool betPlaced = false;
            BetResult betResult = new BetResult();
            string betMessage = "";
            string hdpSens = "";

            string teamFav = "";
            string teamNoFav = "";

            // +++++++++++++++++++++++
            // Click The Odd
            // +++++++++++++++++++++++

            var watch = System.Diagnostics.Stopwatch.StartNew();
            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

            IWebElement line;
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.XPath($"//tbody[@id='{prm_ExtraParams.idRowLeague}']/tr[{prm_ExtraParams.line2I}]{oddPath}")));
                chromeDriver.FindElementByXPath($"//tbody[@id='{prm_ExtraParams.idRowLeague}']/tr[{prm_ExtraParams.line2I}]{oddPath}").Click();
                focusOnBet = true;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - line find odd ");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 8;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Click not found";
                return betResult;
            }

            watch = System.Diagnostics.Stopwatch.StartNew();


            watch.Stop();
            NLogger.Log(EventLevel.Info, $"LOL 0 :{watch.ElapsedMilliseconds} ms");

            watch = System.Diagnostics.Stopwatch.StartNew();
            chromeDriver.SwitchTo().DefaultContent();
            focusOnBet = true;
            watch.Stop();
            NLogger.Log(EventLevel.Info, $"LOL 1 :{watch.ElapsedMilliseconds} ms");
            watch = System.Diagnostics.Stopwatch.StartNew();
            chromeDriver.SwitchTo().Frame(0);
            watch.Stop();
            NLogger.Log(EventLevel.Info, $"LOL 2 :{watch.ElapsedMilliseconds} ms");
            watch = System.Diagnostics.Stopwatch.StartNew();
            chromeDriver.SwitchTo().Frame("mainIframe2");
            watch.Stop();
            NLogger.Log(EventLevel.Info, $"LOL 3 :{watch.ElapsedMilliseconds} ms");

            Thread.Sleep(1000);
            try
            {
                IAlert alertOdd = chromeDriver.SwitchTo().Alert();
                alertOdd.Accept();
            }
            catch (NoAlertPresentException Ex)
            {

            }


            // +++++++++++++++++++++++
            // Control the odd is > 1
            // +++++++++++++++++++++++

            string oddToBet = chromeDriver.FindElementById("socBetOdds").Text;
            oddToBet = oddToBet.Replace("&nbsp;", "");
            oddToBet = oddToBet.Replace("@", "");
            decimal oddDecimal = decimal.Parse(oddToBet, CultureInfo.InvariantCulture.NumberFormat);
            NLogger.Log(EventLevel.Debug, $"odd to bet DECIMAL : {oddDecimal}");
            if (oddDecimal < MainClass.minOddRequired)
            {
                NLogger.Log(EventLevel.Error, "Odd is under than 1, cannot place the bet");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 5;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Odd is under than 1, cannot place the bet";
                return betResult;
            }


            // +++++++++++++++++++++++
            // Control Type Odd is Correct
            // +++++++++++++++++++++++

            string oddType = "";

            try
            {

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("socBetHdp")));
                oddType = chromeDriver.FindElementById("socBetHdp").Text;

            }
            catch (ElementNotVisibleException e)
            {

            }

            if (Common.isOverUnder(hdpToFind))
            {

            }
            else
            {
                if (oddType.Substring(0, 1) == "-")
                {
                    hdpSens = "-";
                }
                else
                {
                    hdpSens = "+";
                }
            }

            oddType = Common.getQuarterBet(oddType);
            oddType = Common.cleanOddPlaceBet(oddType);
            string hdpToFindClean = Common.cleanOddPlaceBet(hdpToFind);
            NLogger.Log(EventLevel.Debug, $"Odd converted : {oddType}");

            if (oddType == "0")
            {
                hdpSens = "0";
                NLogger.Log(EventLevel.Debug, $"hdpSens = 0");
            }

            if (oddType != hdpToFindClean)
            {
                NLogger.Log(EventLevel.Error, $"Error - odd type IBC is different : oddType : {oddType} - hdpToFind : {hdpToFind}");
                //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 3;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "odd type IBC is different";
                return betResult;
            }

            betMessage = "Bet is placed";
            betPlaced = true;


            // +++++++++++++++++++++++
            // Get Max Bet
            // +++++++++++++++++++++++

            watch = System.Diagnostics.Stopwatch.StartNew();
            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("betMaxLimit")));
            string maxBetStr = chromeDriver.FindElementById("betMaxLimit").Text;
            watch.Stop();
            NLogger.Log(EventLevel.Info, $"LOL 4 :{watch.ElapsedMilliseconds} ms");
            maxBetStr = maxBetStr.Replace("&nbsp;", "");
            maxBetStr = maxBetStr.Replace(" ", "");
            decimal maxBet = (decimal.Parse(maxBetStr, CultureInfo.InvariantCulture.NumberFormat));
            NLogger.Log(EventLevel.Debug, $"maxBet : {maxBet}");


            // +++++++++++++++++++++++
            // Get Team Name
            // +++++++++++++++++++++++



            try
            {
                teamFav = chromeDriver.FindElementById("socHome").Text;
                teamNoFav = chromeDriver.FindElementById("socAway").Text;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Team name bet not founded");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 9;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Error - Team name not founded";
                return betResult;
            }

            betPlaced = true;
            betMessage = "Bet is placed";


            betResult.BetStatus = 0;
            betResult.BetIsConfirmed = false;
            betResult.BetIsPlaced = betPlaced;
            betResult.BetMessage = betMessage;
            betResult.BetMaxBet = maxBet;
            betResult.BetHdpSens = hdpSens;
            betResult.BetTeamFav = teamFav;
            betResult.BetTeamNoFav = teamNoFav;
            return betResult;
        }
    }
}
