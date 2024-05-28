﻿using System;
using System.Threading;

using OpenQA.Selenium;
using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Cambo88BTI : CasinoAPI
    {
        public override BetResult confirmBet()
        {
            BetResult betResult = new BetResult();
            // +++++++++++++++++++++++
            // Input Amount
            // +++++++++++++++++++++++

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));


            try
            {
                //wait.Until(ExpectedConditions.ElementIsVisible(By.Id("stake_0")));
                chromeDriver.FindElement(By.Id("stake_0")).Clear();
                chromeDriver.FindElement(By.Id("stake_0")).SendKeys(MainClass.betPriceStr);

            }
            catch (ElementNotVisibleException e)
            {
                NLogger.Log(EventLevel.Error, $"Error - stake_0 not found : {e}");
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "stake_0 not found";
                return betResult;
            }

            // +++++++++++++++++++++++
            // Place the bet
            // +++++++++++++++++++++++
            bool betConfirm = false;
            string messageConfirm = "";
            try
            {
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("PlaceBetButton")));
                chromeDriver.FindElement(By.Id("PlaceBetButton")).Click();

                NLogger.Log(EventLevel.Debug, "Case 8");

                Thread.Sleep(1000);
                try
                {
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                    wait.Until(ExpectedConditions.AlertIsPresent());

                    IAlert alert = chromeDriver.SwitchTo().Alert();

                    alert.Accept();
                }
                catch (NoAlertPresentException Ex)
                {

                }

                try
                {
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(2));
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("PlaceBetButton")));
                    //chromeDriver.FindElement(By.Id("btnBPSubmit")).Click();
                    NLogger.Log(EventLevel.Debug, "bet still present");

                    betConfirm = false;
                    messageConfirm = "bet failed : still present";
                    betResult.BetStatus = 2;
                    betResult.BetIsConfirmed = betConfirm;
                    betResult.BetIsPlaced = false;
                    betResult.BetMessage = messageConfirm;
                    messageConfirm = "bet is still present";
                    return betResult;


                }
                catch
                {
                    messageConfirm = "bet is confirmed";
                    betConfirm = true;
                }

                NLogger.Log(EventLevel.Debug, "Case 9");

            }
            catch
            {
                messageConfirm = "bet not confirmed";
                betConfirm = false;
            }
            betResult.BetStatus = 2;
            betResult.BetIsConfirmed = betConfirm;
            betResult.BetIsPlaced = false;
            betResult.BetMessage = messageConfirm;
            return betResult;
        }
    }
}
