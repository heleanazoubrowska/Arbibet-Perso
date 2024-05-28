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
    public partial class Cambo88SBO : CasinoAPI
    {
        //protected override BetResult checkBet(string oddId, string hdpToFind)
        protected override BetResult checkBet(string oddPath, string line1I, string hdpToFind, CheckBet_ExtraParams prm_ExtraParams = null)
        {
            NLogger.Log(EventLevel.Debug, "Do we click here ?");
            bool betPlaced = false;

            BetResult betResult = new BetResult();
            string betMessage = "";
            decimal maxBet = 0;
            string hdpSens = "";

            // +++++++++++++++++++++++
            // Click The Odd
            // +++++++++++++++++++++++

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            NLogger.Log(EventLevel.Debug, "case 1");
            try
            {
                // sn: not sure whether oddId refers to oddPath or line1I... guessing line1I?
                //wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@id='" + oddId + "']")));
                //chromeDriver.FindElementByXPath("//*[@id='" + oddId + "']").Click();
                wait.Until(ExpectedConditions.ElementExists(By.XPath($"//*[@id='{line1I}']")));
                chromeDriver.FindElementByXPath($"//*[@id='{line1I}']").Click();
                focusOnBet = true;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - Odd click not found : ");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 8;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Odd click not found ";
                return betResult;
            }
            NLogger.Log(EventLevel.Debug, "case 2");
            Thread.Sleep(1000);
            try
            {
                IAlert alertOdd = chromeDriver.SwitchTo().Alert();
                alertOdd.Accept();
            }
            catch (NoAlertPresentException Ex)
            {

            }


            Thread.Sleep(3000);


            // +++++++++++++++++++++++
            // Parse bet panel HTML
            // +++++++++++++++++++++++

            //wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30));
            //string bet = "";
            //try
            //{
            //    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("DesktopQuickBet")));
            //    bet = chromeDriver.FindElementById("DesktopQuickBet").GetAttribute("innerHTML");
            //}
            //catch
            //{
            //    NLogger.Log(EventLevel.Error,"CHECK BET LIST : cannot find betlistdiv 855BET ");
            //}

            //HtmlDocument htmlDoc = new HtmlDocument();
            //htmlDoc.LoadHtml(bet);

            NLogger.Log(EventLevel.Debug, "case 3");

            // +++++++++++++++++++++++
            // Control the odd is > 1
            // +++++++++++++++++++++++

            string oddToBet = "";

            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("DesktopQuickBetOddsSection")));
                oddToBet = chromeDriver.FindElementById("DesktopQuickBetOddsSection").Text;
                //oddToBet = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='DesktopQuickBetOddsSection']").InnerText;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - Odd not founded");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 5;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Error - odd not founded";
                return betResult;
            }

            NLogger.Log(EventLevel.Debug, $"odd to bet : {oddToBet}");
            oddToBet = oddToBet.Replace("&nbsp;", "");
            decimal oddDecimal = decimal.Parse(oddToBet, CultureInfo.InvariantCulture.NumberFormat);
            NLogger.Log(EventLevel.Debug, $"odd to bet DECIMAL : {oddDecimal}");
            if (oddDecimal < MainClass.minOddRequired)
            {
                NLogger.Log(EventLevel.Error, "Odd is under than 1, cannot place the bet");
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Odd is under than 1, cannot place the bet";
                return betResult;
            }


            // +++++++++++++++++++++++++++
            // Control Type Odd is Correct
            // +++++++++++++++++++++++++++

            string oddType = "";

            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("hdp-point-info-for-quickBet")));
                oddType = chromeDriver.FindElementById("hdp-point-info-for-quickBet").Text;
                //oddType = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='hdp-point-info-for-quickBet']").InnerText;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - odd type not found");
            }
            var mO = Regex.Match(oddType, "^(.*?)@");
            oddType = mO.Value;

            if (Common.isOverUnder(hdpToFind))
            {

            }
            else
            {
                oddType = oddType.Replace("HDP:", "");

                if (oddType.Substring(0, 1) == "-")
                {
                    hdpSens = "-";
                }
                else
                {
                    hdpSens = "+";
                }
            }

            oddType = Common.cleanOddPlaceBet(oddType);
            string hdpToFindClean = Common.cleanOddPlaceBet(hdpToFind);

            if (oddType == "0")
            {
                hdpSens = "0";
                NLogger.Log(EventLevel.Debug, $"hdpSens = 0");
            }

            if (oddType != hdpToFindClean)
            {
                NLogger.Log(EventLevel.Error, $"Error - odd type Cambo88SBO is different : oddType : {oddType} - hdpToFind : {hdpToFind}");
                //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 3;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "odd type Cambo88SBO is different";
                return betResult;
            }


            // +++++++++++++++++++++++
            // Get Max Bet
            // +++++++++++++++++++++++


            string brutMaxBet = "";
            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            NLogger.Log(EventLevel.Debug, "case 4");
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("DesktopQuickBetBetLimit")));
                brutMaxBet = chromeDriver.FindElementById("DesktopQuickBetBetLimit").Text;
                //brutMaxBet = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='DesktopQuickBetBetLimit']").InnerText;

            }
            catch (NoSuchElementException Ex)
            {
                NLogger.Log(EventLevel.Error, $"Error - MaxBet not found : {Ex}");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 9;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "MaxBet not found ";
                return betResult;
            }
            NLogger.Log(EventLevel.Debug, "case 5");
            var m = Regex.Match(brutMaxBet, "[^-]*$");
            string maxBetStr = m.Value;
            maxBetStr = maxBetStr.Replace("&nbsp;", "");
            maxBetStr = maxBetStr.Replace(",", "");
            maxBet = (decimal.Parse(maxBetStr, CultureInfo.InvariantCulture.NumberFormat));
            NLogger.Log(EventLevel.Debug, $"Max bet : {maxBet}");

            // +++++++++++++++++++++++
            // Get Team Name
            // +++++++++++++++++++++++

            string teamFav = "";
            string teamNoFav = "";

            try
            {
                teamFav = chromeDriver.FindElementByXPath("//*[@id='tk:tk']/div[2]/div[1]/span[1]").Text;
                teamNoFav = chromeDriver.FindElementByXPath("//*[@id='tk:tk']/div[2]/div[1]/span[3]").Text;
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
            betResult.BetIsConfirmed = true;
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
