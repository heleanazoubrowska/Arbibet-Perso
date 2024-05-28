using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using HtmlAgilityPack;
using System;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Ibet789 : CasinoAPI
    {
	    protected override BetResult checkBet(string oddPath, string line1I, string hdpToFind, CheckBet_ExtraParams prm_ExtraParams = null)
	    {
            bool betPlaced = false;

            BetResult betResult = new BetResult();
            string betMessage = "";
            string hdpSens = "";

            // +++++++++++++++++++++++
            // Click The Odd
            // +++++++++++++++++++++++
            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

            NLogger.Log(EventLevel.Debug, $"oddPath : {oddPath}");
            NLogger.Log(EventLevel.Debug, $"line1I : {line1I}");
            NLogger.Log(EventLevel.Debug, $"Xpth full : //tr[@oddsid='{line1I}']{oddPath}");

            try
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath($"//tr[@oddsid='{line1I}']{oddPath}")));
                chromeDriver.FindElementByXPath($"//tr[@oddsid='{line1I}']{oddPath}").Click();
                focusOnBet = true;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - Click not founded");
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Click not found";
                return betResult;
            }


            try
            {
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
                wait.Until(ExpectedConditions.AlertIsPresent());

                IAlert alert = chromeDriver.SwitchTo().Alert();

                alert.Accept();
            }
            catch
            {

            }


            NLogger.Log(EventLevel.Debug, "case 1");

            focusOnBet = true;

            try
            {
                chromeDriver.SwitchTo().DefaultContent();
                focusOnBet = true;
            }
            catch (NoSuchFrameException e)
            {
                NLogger.Log(EventLevel.Error, $"Error - DefaultContent cannot switch : {e}");
            }

            NLogger.Log(EventLevel.Debug, "case 2");

            try
            {
                chromeDriver.SwitchTo().Frame("fraPanel");
            }
            catch (NoSuchFrameException e)
            {
                NLogger.Log(EventLevel.Error, $"Error - Second frame cannot switch : {e}");
            }

            NLogger.Log(EventLevel.Debug, "case 3");
            // +++++++++++++++++++++++
            // Parse bet panel HTML
            // +++++++++++++++++++++++

            Thread.Sleep(1000);

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            string bet = "";
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tableBetBox")));
                bet = chromeDriver.FindElementById("tableBetBox").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find tableBetBox on Ibet789");
            }

            NLogger.Log(EventLevel.Debug, "case 4");

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);

            // +++++++++++++++++++++++
            // Control the odd is > 1
            // +++++++++++++++++++++++

            string oddToBet = "";
            try
            {
                oddToBet = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='socBetOdds']/span/span").InnerText;
                oddToBet = oddToBet.Replace("&nbsp;", "");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - socBetOdds not founded ");
            }


            NLogger.Log(EventLevel.Debug, "case 5");

            decimal oddDecimal = decimal.Parse(oddToBet, CultureInfo.InvariantCulture.NumberFormat);
            NLogger.Log(EventLevel.Debug, $"odd to bet DECIMAL : {oddDecimal}");
            if (oddDecimal < 1)
            {
                //NLogger.Log(EventLevel.Error,"Odd is under than 1, cannot place the bet");
                //betResult.BetStatus = 2;
                //betResult.BetIsConfirmed = false;
                //betResult.BetIsPlaced = false;
                //betResult.BetMessage = "Odd is under than 1, cannot place the bet";
                //return betResult;
            }
            NLogger.Log(EventLevel.Debug, "case 6");
            // +++++++++++++++++++++++
            // Control Type Odd is Correct
            // +++++++++++++++++++++++

            string oddType = "";

            try
            {

                oddType = htmlDoc.DocumentNode.SelectSingleNode("//label[@id='socBetHdp']").InnerText;

            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - socBetHdp not founded");
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "socBetHdp not found";
                return betResult;
            }

            NLogger.Log(EventLevel.Debug, "case 7");


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


            oddType = Common.cleanOddPlaceBet(oddType);
            string hdpToFindClean = Common.cleanOddPlaceBet(hdpToFind);
            NLogger.Log(EventLevel.Debug, $"Odd converted : {oddType}");

            if (oddType != hdpToFindClean)
            {
                NLogger.Log(EventLevel.Error,  $"Error - odd type Ibet789 is different : oddType : {oddType} - hdpToFind : {hdpToFind}");
                //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "odd type is different";
                return betResult;
            }

            NLogger.Log(EventLevel.Debug, "case 8");
            // +++++++++++++++++++++++
            // Get Max Bet
            // +++++++++++++++++++++++

            string brutMaxBet = "";
            try
            {
                brutMaxBet = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='betMaxLimit']").InnerText;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Max bet not founded");
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Error - Max bet not founded";
                return betResult;
            }

            NLogger.Log(EventLevel.Debug, $"brutMaxBet : {brutMaxBet}");
            Regex regex = new Regex(@"[^~]*$");
            System.Text.RegularExpressions.Match match = regex.Match(brutMaxBet);

            string maxBetStr = match.Value;
            decimal maxBet = (decimal.Parse(maxBetStr, CultureInfo.InvariantCulture.NumberFormat));
            NLogger.Log(EventLevel.Debug, $"maxBet decimal : {maxBet}");


            betPlaced = true;
            betMessage = "Bet is placed";

            betResult.BetStatus = 0;
            betResult.BetIsConfirmed = true;
            betResult.BetIsPlaced = betPlaced;
            betResult.BetMessage = betMessage;
            betResult.BetMaxBet = maxBet;
            betResult.BetHdpSens = hdpSens;
            return betResult;
        }
    }
}
