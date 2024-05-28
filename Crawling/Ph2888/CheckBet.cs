using System;
using HtmlAgilityPack;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using System.Threading;
using Newtonsoft.Json;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
	public partial class Ph2888 : CasinoAPI
	{
		protected override BetResult checkBet(string oddPath, string line1I, string hdpToFind, CheckBet_ExtraParams prm_ExtraParams = null)
        {
            bool betPlaced = false;
            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            var watch = System.Diagnostics.Stopwatch.StartNew();
            BetResult betResult = new BetResult();
            string betMessage = "";
            decimal maxBet = 0;
            string hdpSens = "";
            // +++++++++++++++++++++++
            // Click The Odd
            // +++++++++++++++++++++++

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            IWebElement line;
            try
            {
                line = chromeDriver.FindElementByCssSelector("tr[oddsid='" + line1I + "']");
            }
            catch (NoSuchElementException e)
            {
                NLogger.Log(EventLevel.Error, "Error - line find odd : " + e);
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Line odd not found";
                return betResult;
            }

            watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                line.FindElement(By.XPath("./" + oddPath)).Click();

            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - click find odd : ");
                //chromeDriver.FindElement(By.XPath("//*[@id='tbBetBox']/tbody/tr[2]/td/table/tbody/tr[8]/td/span[1]")).Click();
                //chromeDriver.SwitchTo().ParentFrame();
                //chromeDriver.SwitchTo().Frame("fraMain");
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Click not found";
                return betResult;
            }



            watch.Stop();
            NLogger.Log(EventLevel.Info, "Time step 1 " + watch.ElapsedMilliseconds + " ms");

            try
            {
                chromeDriver.SwitchTo().DefaultContent();
                focusOnBet = true;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - DefaultContent cannot switch : ");
            }

            try
            {
                chromeDriver.SwitchTo().Frame(chromeDriver.FindElement(By.XPath("//html/body/section/div/div/iframe")));
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - Second frame cannot switch : ");
            }

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(5));

            try
            {
                chromeDriver.SwitchTo().Frame("fraPanel");
            }
            catch
            {

                NLogger.Log(EventLevel.Error, "Error - Third frame cannot switch : ");
            }


            // +++++++++++++++++++++++
            // Control the odd is > 1
            // +++++++++++++++++++++++
            Thread.Sleep(1000);


            string oddToBet = chromeDriver.FindElementById("socBetOdds").Text;

            oddToBet = oddToBet.Replace("&nbsp;", "");
            oddToBet = oddToBet.Replace("@", "");
            decimal oddDecimal = decimal.Parse(oddToBet, CultureInfo.InvariantCulture.NumberFormat);
            NLogger.Log(EventLevel.Debug, "odd to bet : " + oddDecimal);
            if (oddDecimal < 1)
            {
                //NLogger.Log(EventLevel.Error,"Odd is under than 1, cannot place the bet");
                //betResult.BetStatus = 2;
                //betResult.BetIsConfirmed = false;
                //betResult.BetIsPlaced = false;
                //betResult.BetMessage = "Odd is under than 1, cannot place the bet";
                //return betResult;
            }


            // +++++++++++++++++++++++
            // Control Type Odd is Correct
            // +++++++++++++++++++++++

            string oddType = "";

            try
            {

                oddType = chromeDriver.FindElementByXPath("//*[@id='socBetHdp']/span").Text;

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
            NLogger.Log(EventLevel.Debug, "Odd converted : " + oddType);

            if (oddType != hdpToFindClean)
            {
                NLogger.Log(EventLevel.Error, "Error - odd type Ibet789 is different : oddType : " + oddType + " - hdpToFind : " + hdpToFind);
                //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "odd type is different";
                return betResult;
            }

            // +++++++++++++++++++++++
            // Get Max Bet
            // +++++++++++++++++++++++

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("betMaxLimit")));
            string maxBetStr = chromeDriver.FindElementById("betMaxLimit").Text;
            maxBetStr = maxBetStr.Replace("&nbsp;", "");
            maxBetStr = maxBetStr.Replace(" ", "");
            maxBet = (decimal.Parse(maxBetStr, CultureInfo.InvariantCulture.NumberFormat));
            NLogger.Log(EventLevel.Debug, "Max bet : " + maxBet);

            Thread.Sleep(1000);
            betMessage = "Bet is Placed";
            betPlaced = true;


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
