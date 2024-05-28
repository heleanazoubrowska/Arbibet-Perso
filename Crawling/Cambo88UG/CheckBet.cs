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
	public partial class Cambo88UG : CasinoAPI
	{
        //protected override BetResult checkBet(string oddPath, string line1I, string hdpToFind)
        protected override BetResult checkBet(string oddPath, string line1I, string hdpToFind, CheckBet_ExtraParams prm_ExtraParams = null)
        {
            bool betPlaced = false;
            BetResult betResult = new BetResult();
            string betMessage = "";
            decimal maxBet = 0;
            string hdpSens = "";

            string teamFav = "";
            string teamNoFav = "";

            try
            {
                // +++++++++++++++++++++++
                // Click The Odd
                // +++++++++++++++++++++++
                WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

                try
                {
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                    chromeDriver.FindElementByXPath($"//*[@id='{line1I}']{oddPath}").Click();
                    focusOnBet = true;
                }
                catch (ElementNotVisibleException e)
                {
                    NLogger.Log(EventLevel.Error, $"Error - Odd click not found : {e}");
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 8;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "Click not found";
                    return betResult;

                }

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

                string oddToBet = chromeDriver.FindElementById("betOdds").Text;
                NLogger.Log(EventLevel.Debug, $"odd to bet : {oddToBet}");
                oddToBet = oddToBet.Replace("&nbsp;", "");
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
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("betHdp")));
                    oddType = chromeDriver.FindElementById("betHdp").Text;

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
                    NLogger.Log(EventLevel.Error,  $"Error - odd type UG is different : oddType : {oddType} - hdpToFind : {hdpToFind}");
                    //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 3;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "odd type UG is different";
                    return betResult;
                }

                // +++++++++++++++++++++++
                // Get Max Bet
                // +++++++++++++++++++++++
                string maxBetStr = "";
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                try
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.Id("MaxBet")));
                    maxBetStr = chromeDriver.FindElementById("MaxBet").Text;

                }
                catch (ElementNotVisibleException e)
                {
                    NLogger.Log(EventLevel.Error, $"Error - MaxBet not found : {e}");
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 9;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "MaxBet not found";
                    return betResult;
                }

                maxBetStr = maxBetStr.Replace("&nbsp;", "");
                maxBetStr = maxBetStr.Replace(" ", "");
                maxBet = (decimal.Parse(maxBetStr, CultureInfo.InvariantCulture.NumberFormat));
                NLogger.Log(EventLevel.Debug, $"Max bet : {maxBet}");

                // +++++++++++++++++++++++
                // Get Team Name
                // +++++++++++++++++++++++



                try
                {
                    teamFav = chromeDriver.FindElementById("betHome").Text;
                    teamNoFav = chromeDriver.FindElementById("betAway").Text;
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
            }

            catch
            {
                betPlaced = false;
                betMessage = "Error place bet (catch)";
            }


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
