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
	public partial class Ph2888 : CasinoAPI
	{
        public override BetResult confirmBet()
        {
            // +++++++++++++++++++++++
            // Input Amount
            // +++++++++++++++++++++++

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(20));
            string oddToBet = "";
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
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id='tbBetBox']/tbody/tr[2]/td/table/tbody/tr[8]/td/span[2]")));
                chromeDriver.FindElement(By.XPath("//*[@id='tbBetBox']/tbody/tr[2]/td/table/tbody/tr[8]/td/span[2]")).Click();
                Thread.Sleep(500);

                try
                {
                    IAlert alertOdd = chromeDriver.SwitchTo().Alert();
                    NLogger.Log(EventLevel.Debug, "Ph2888 Text Alert 1" + alertOdd.Text);
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
                    NLogger.Log(EventLevel.Debug, "Ph2888 Text Alert 2" + alertOdd.Text);
                    alertOdd.Accept();
                }
                catch (NoAlertPresentException Ex)
                {
                    NLogger.Log(EventLevel.Info, " No alert confirmation found " + Ex);
                }

                betConfirm = true;
                messageConfirm = "bet is confirmed";
            }
            catch (InvalidCastException e)
            {
                NLogger.Log(EventLevel.Error, "Error Confirm bet on Ph2888 : " + e);
                messageConfirm = "bet not confirmed";
            }

            chromeDriver.SwitchTo().ParentFrame();

            chromeDriver.SwitchTo().Frame("fraMain");

            BetResult betResult = new BetResult();
            betResult.BetStatus = 2;
            betResult.BetIsConfirmed = betConfirm;
            betResult.BetIsPlaced = false;
            betResult.BetMessage = messageConfirm;
            return betResult;
        }

    }
}
