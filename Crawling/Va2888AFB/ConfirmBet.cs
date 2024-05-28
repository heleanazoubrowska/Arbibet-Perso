using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArbibetProgram.Functions;
using ArbibetProgram.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
	public partial class Va2888AFB : CasinoAPI
	{
        public override BetResult confirmBet()
        {


            // +++++++++++++++++++++++
            // Input Amount
            // +++++++++++++++++++++++

            Thread.Sleep(1000);
            BetResult betResult = new BetResult();
            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(20));

            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("betTxtAmount")));
                chromeDriver.FindElement(By.Id("betTxtAmount")).Clear();
                chromeDriver.FindElement(By.Id("betTxtAmount")).SendKeys(MainClass.betPriceStr);
            }
            catch (NoSuchElementException e)
            {
                NLogger.Log(EventLevel.Error, "Error - betTxtAmount not founded : " + e);
            }

            // +++++++++++++++++++++++
            // Place the bet
            // +++++++++++++++++++++++
            bool betConfirm = false;
            string messageConfirm = "";
            try
            {
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("submitBetT")));
                chromeDriver.FindElement(By.Id("submitBetT")).Click();
                Thread.Sleep(500);

                try
                {
                    IAlert alertOdd = chromeDriver.SwitchTo().Alert();
                    NLogger.Log(EventLevel.Debug, "AFB Text Alert 1" + alertOdd.Text);
                    alertOdd.Accept();
                }
                catch (NoAlertPresentException Ex)
                {
                    NLogger.Log(EventLevel.Info, " No alert confirmation found " + Ex);
                }

                Thread.Sleep(500);

                try
                {
                    IAlert alertOdd = chromeDriver.SwitchTo().Alert();
                    NLogger.Log(EventLevel.Debug, "Va Text Alert 2" + alertOdd.Text);
                    alertOdd.Accept();
                }
                catch (NoAlertPresentException Ex)
                {
                    NLogger.Log(EventLevel.Info, " No alert confirmation found " + Ex);
                }

                try
                {
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(2));
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("submitBetT")));
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
            }
            catch (InvalidCastException e)
            {
                NLogger.Log(EventLevel.Error, "Error Confirm bet on AFB : " + e);
                messageConfirm = "bet not confirmed";
            }

            betResult.BetStatus = 2;
            betResult.BetIsConfirmed = betConfirm;
            betResult.BetIsPlaced = false;
            betResult.BetMessage = messageConfirm;
            return betResult;
        }

    }
}
