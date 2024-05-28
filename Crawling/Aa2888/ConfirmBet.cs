using System;
using System.Threading;

using OpenQA.Selenium;
using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Aa2888 : CasinoAPI
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
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("txtBetAmount")));
                chromeDriver.FindElement(By.Id("txtBetAmount")).Clear();
                chromeDriver.FindElement(By.Id("txtBetAmount")).SendKeys(MainClass.betPriceStr);
            }
            catch (ElementNotVisibleException e)
            {
                NLogger.Log(EventLevel.Error, $"Error - txtBetAmount not found : {e}");
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "txtBetAmount not found";
                return betResult;
            }

            // +++++++++++++++++++++++
            // Place the bet
            // +++++++++++++++++++++++
            bool betConfirm = false;
            string messageConfirm = "";
            Thread.Sleep(1000);
            try
            {
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnBet")));
                chromeDriver.FindElement(By.Id("btnBet")).Click();

                Thread.Sleep(500);

                try
                {
                    IAlert alertOdd = chromeDriver.SwitchTo().Alert();
                    NLogger.Log(EventLevel.Debug, $"Aa22 Text Alert 1 {alertOdd.Text}");
                    alertOdd.Accept();
                }
                catch (NoAlertPresentException Ex)
                {
                    NLogger.Log(EventLevel.Info, " No alert confirmation found ");
                }

                try
                {
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(2));
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnBet")));
                    //chromeDriver.FindElement(By.Id("btnBet")).Click();
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

                try
                {
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("cleanblue_state0_buttonOK")));
                    chromeDriver.FindElement(By.Id("cleanblue_state0_buttonOK")).Click();
                }
                catch (NoSuchElementException e)
                {
                    NLogger.Log(EventLevel.Info, $"Cannot click cleanblue_state0_buttonOK {e}");
                }

                


            }
            catch (InvalidCastException e)
            {
                NLogger.Log(EventLevel.Error, $"Error Confirm bet on Aa2888 : {e}");
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
