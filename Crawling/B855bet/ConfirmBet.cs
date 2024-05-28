using System;
using System.Threading;

using OpenQA.Selenium;
using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class B855bet : CasinoAPI
    {
        public override BetResult confirmBet()
        {
            bool betConfirm = false;
            string messageConfirm = "";

            // +++++++++++++++++++++++
            // Input Amount
            // +++++++++++++++++++++++
            BetResult betResult = new BetResult();
            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(20));

            try
            {
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(20));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("BPstake")));
                chromeDriver.FindElement(By.Id("BPstake")).Clear();
                chromeDriver.FindElement(By.Id("BPstake")).SendKeys(MainClass.betPriceStr);
            }
            catch (ElementNotVisibleException e)
            {
                NLogger.Log(EventLevel.Error, $"Error - BPstake not found : {e}");
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "BPstake not found";
                return betResult;
            }

            Thread.Sleep(1000);

            // +++++++++++++++++++++++
            // Place the bet
            // +++++++++++++++++++++++

            try
            {

                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("cbAcceptBetterOdds")));
                chromeDriver.FindElement(By.Id("cbAcceptBetterOdds")).Click();

                Thread.Sleep(1000);

                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnBPSubmit")));
                chromeDriver.FindElement(By.Id("btnBPSubmit")).Click();

                Thread.Sleep(1000);

                try
                {
                    IAlert alert = chromeDriver.SwitchTo().Alert();
                    alert.Accept();
                }
                catch (NoAlertPresentException Ex)
                {

                }

                Thread.Sleep(500);

                // Confirm if odd changed

                try
                {
                    IAlert alert = chromeDriver.SwitchTo().Alert();
                    alert.Accept();
                }
                catch (NoAlertPresentException Ex)
                {

                }

                try
                {
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(2));
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnBPSubmit")));
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

                chromeDriver.SwitchTo().DefaultContent();

                chromeDriver.SwitchTo().Frame("mainFrame");
                
            }
            catch
            {
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
