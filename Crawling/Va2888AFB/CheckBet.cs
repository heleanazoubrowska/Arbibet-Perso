﻿using System;
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
	public partial class Va2888AFB : CasinoAPI
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

            string teamFav = "";
            string teamNoFav = "";
            try
            {
                // +++++++++++++++++++++++
                // Click The Odd
                // +++++++++++++++++++++++

                try
                {
                    wait.Until(ExpectedConditions.ElementExists(By.XPath("//*[@abet='" + line1I + "']")));
                    chromeDriver.FindElementByXPath("//*[@abet='" + line1I + "']").Click();
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

                // +++++++++++++++++++++++
                // Control the odd is > 1
                // +++++++++++++++++++++++
                string oddToBet = "";
                oddToBet = chromeDriver.FindElementById("socBetOdds").Text;

                oddToBet = oddToBet.Replace("&nbsp;", "");
                oddToBet = oddToBet.Replace("@", "");
                decimal oddDecimal = decimal.Parse(oddToBet, CultureInfo.InvariantCulture.NumberFormat);
                NLogger.Log(EventLevel.Debug, "odd to bet : " + oddDecimal);
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

                    wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='socBetHdp']/span")));
                    oddType = chromeDriver.FindElementByXPath("//*[@id='socBetHdp']/span").Text;

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
                NLogger.Log(EventLevel.Debug, "AFB oddType : " + oddType);
                NLogger.Log(EventLevel.Debug, "AFB hdpToFindClean : " + hdpToFindClean);

                if (oddType == "0")
                {
                    hdpSens = "0";
                    NLogger.Log(EventLevel.Debug, $"hdpSens = 0");
                }

                if (oddType != hdpToFindClean)
                {
                    NLogger.Log(EventLevel.Error, $"Error - odd type AFB is different : oddType : {oddType} - hdpToFind : {hdpToFind}");
                    //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                    betResult.BetStatus = 2;
                    betResult.BetErrorStatus = 3;
                    betResult.BetIsConfirmed = false;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = "odd type AFB is different";
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

                // +++++++++++++++++++++++
                // Get Team Name
                // +++++++++++++++++++++++

                try
                {
                    teamFav = chromeDriver.FindElementById("socClsHome").Text;
                    teamNoFav = chromeDriver.FindElementById("socClsAway").Text;
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

                Thread.Sleep(1000);
                betMessage = "Bet is Placed";
                betPlaced = true;
            }
            catch (NoSuchElementException e)
            {
                NLogger.Log(EventLevel.Error, "Error while placing bet on Va2888 AFB: " + e);

                betPlaced = false;
                betMessage = "Error while placing bet on Va2888 AFB";
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