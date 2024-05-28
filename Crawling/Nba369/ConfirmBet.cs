using System;
using System.Threading;

using OpenQA.Selenium;
using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Nba369 : CasinoAPI
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
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - txtBetAmount not found : ");
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "txtBetAmount not found";
                return betResult;
            }

            Thread.Sleep(1000);

            // +++++++++++++++++++++++
            // Place the bet
            // +++++++++++++++++++++++
            bool betConfirm = false;
            string messageConfirm = "";
            try
            {
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnBet")));
                chromeDriver.FindElement(By.Id("btnBet")).Click();

                try
                {
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//html/body/div[3]/div[2]/form/div[2]/div/div[3]/button")));
                    chromeDriver.FindElement(By.XPath("//html/body/div[3]/div[2]/form/div[2]/div/div[3]/button")).Click();

                    try
                    {
                        IAlert alert = chromeDriver.SwitchTo().Alert();
                        NLogger.Log(EventLevel.Debug, $"Nba Text Alert 1{alert.Text}");
                        alert.Accept();
                    }
                    catch (NoAlertPresentException e)
                    {
                        NLogger.Log(EventLevel.Debug, "No alrt Confirm bet on Nba : ");
                    }

                    try
                    {
                        IAlert alert = chromeDriver.SwitchTo().Alert();
                        NLogger.Log(EventLevel.Debug, $"Nba Text Alert 2{alert.Text}");
                        alert.Accept();
                    }
                    catch (NoAlertPresentException e)
                    {
                        NLogger.Log(EventLevel.Debug, "No alrt Confirm bet on Nba : ");
                    }

                    Thread.Sleep(1000);

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
                        return betResult;


                    }
                    catch
                    {
                        betConfirm = true;
                    }

                    betConfirm = true;
                    messageConfirm = "bet is confirmed";
                }
                catch (ElementNotVisibleException e)
                {
                    NLogger.Log(EventLevel.Error, $"Error - Alert popup bet confirmed not found : {e}");
                }

            }
            catch
            {
                betConfirm = false;
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
