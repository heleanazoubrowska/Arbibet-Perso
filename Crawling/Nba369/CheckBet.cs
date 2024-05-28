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
    public partial class Nba369 : CasinoAPI
    {
        //protected override BetResult checkBet(string idClick, string hdpToFind)
        protected override BetResult checkBet(string oddPath, string line1I, string hdpToFind, CheckBet_ExtraParams prm_ExtraParams = null)
        {
            bool betPlaced = false;
            BetResult betResult = new BetResult();
            string betMessage = "";
            decimal maxBet = 0;
            string hdpSens = "";
            //try
            //{
            // +++++++++++++++++++++++
            // Click The Odd
            // +++++++++++++++++++++++

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(2));
            IWebElement odd;
            try
            {
                //odd = chromeDriver.FindElementById(idClick);
                odd = chromeDriver.FindElementById(oddPath);
                focusOnBet = true;
            }
            catch (NoSuchElementException e)
            {
                NLogger.Log(EventLevel.Error, $"Error - Cannot find odd : {e}");
                //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 8;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Odd path not found";
                return betResult;
            }


            try
            {
                IJavaScriptExecutor ex = (IJavaScriptExecutor)chromeDriver;
                ex.ExecuteScript("arguments[0].click();", odd);
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - Cannot JS click odd ");
                //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 8;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Click not found";
                return betResult;
            }

            try
            {
                IAlert alert = chromeDriver.SwitchTo().Alert();
                alert.Accept();
            }
            catch (NoAlertPresentException Ex)
            {

            }


            // +++++++++++++++++++++++
            // Control the odd is > 1
            // +++++++++++++++++++++++

            var watch = System.Diagnostics.Stopwatch.StartNew();

            string oddToBet = "";
            watch = System.Diagnostics.Stopwatch.StartNew();
            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(2));

            try
            {

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("spanBetZoneOdds")));
                oddToBet = chromeDriver.FindElementById("spanBetZoneOdds").Text;
            }
            catch (ElementNotVisibleException e)
            {
                NLogger.Log(EventLevel.Error, $"Error - spanBetZoneOdds not found : {e}");
                try
                {

                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("socBetOdds")));
                    oddToBet = chromeDriver.FindElementById("socBetOdds").Text;
                }
                catch (ElementNotVisibleException eE)
                {
                    NLogger.Log(EventLevel.Error, $"Error - socBetOdds not found : {eE}");
                }
            }


            watch.Stop();
            NLogger.Log(EventLevel.Info, $"Case 1 :{watch.ElapsedMilliseconds} ms");

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(2));

            NLogger.Log(EventLevel.Debug, $"odd to bet : {oddToBet}");
            oddToBet = oddToBet.Replace("&nbsp;", "");
            oddToBet = oddToBet.Replace("@", "");
            decimal oddDecimal = decimal.Parse(oddToBet, CultureInfo.InvariantCulture.NumberFormat);
            NLogger.Log(EventLevel.Debug, $"odd to bet DECIMAL : {oddDecimal}");
            if (oddDecimal < MainClass.minOddRequired)
            {
                //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                NLogger.Log(EventLevel.Error, "Odd is under than 1, cannot place the bet");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 5;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Odd is under than 1, cannot place the bet";
                return betResult;
            }


            // +++++++++++++++++++++++
            // Get Max Bet
            // +++++++++++++++++++++++
            Thread.Sleep(1000);

            string brutMaxBet = "";
            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tdBetLimit")));
                brutMaxBet = chromeDriver.FindElementById("tdBetLimit").Text;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Max bet not founded");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 9;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Error - Max bet not founded";
                return betResult;
            }

            NLogger.Log(EventLevel.Debug, $"brutMaxBet : {brutMaxBet}");
            Regex regex = new Regex(@"[^~]*$");
            System.Text.RegularExpressions.Match match = regex.Match(brutMaxBet);

            string maxBetStr = match.Value;
            maxBet = (decimal.Parse(maxBetStr, CultureInfo.InvariantCulture.NumberFormat));
            NLogger.Log(EventLevel.Debug, $"maxBet decimal : {maxBet}");


            watch.Stop();
            NLogger.Log(EventLevel.Info, $"Case 0 : {watch.ElapsedMilliseconds} ms");


            // +++++++++++++++++++++++
            // Control Type Odd is Correct
            // +++++++++++++++++++++++

            string oddType = "";

            try
            {

                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("spanBetZoneHdp")));
                oddType = chromeDriver.FindElementById("spanBetZoneHdp").Text;

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

            oddType = Common.cleanOddPlaceBet(oddType);
            string hdpToFindClean = Common.cleanOddPlaceBet(hdpToFind);

            if (oddType == "0")
            {
                hdpSens = "0";
                NLogger.Log(EventLevel.Debug, $"hdpSens = 0");
            }

            if (oddType != hdpToFindClean)
            {
                NLogger.Log(EventLevel.Error,  $"Error - odd type Nba is different : oddType : {oddType} - hdpToFind : {hdpToFind}");
                //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 3;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "odd type Nba is different";
                return betResult;
            }


            // +++++++++++++++++++++++
            // Get Team Name
            // +++++++++++++++++++++++

            string teamFav = "";
            string teamNoFav = "";

            try
            {
                teamFav = chromeDriver.FindElementById("bet_sport_blueFont").Text;
                teamNoFav = chromeDriver.FindElementById("bet_sport_redFont").Text;
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

            betMessage = "Bet is placed";
            betPlaced = true;
            //}
            //catch
            //{
            //    NLogger.Log(EventLevel.Error,"Error Confirm bet Nba369  : ");
            //    //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
            //    betMessage = "Error confirm bet";
            //    betPlaced = false;
            //}

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
