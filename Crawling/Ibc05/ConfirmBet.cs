using System;
using System.Threading;

using OpenQA.Selenium;
using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Ibc05 : CasinoAPI
    {
        public override BetResult confirmBet()
        {
            BetResult betResult = new BetResult();
            // +++++++++++++++++++++++
            // Input Amount
            // +++++++++++++++++++++++

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(20));
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("betTxtAmount")));
                chromeDriver.FindElement(By.Id("betTxtAmount")).Clear();
                chromeDriver.FindElement(By.Id("betTxtAmount")).SendKeys(MainClass.betPriceStr);
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - betTxtAmount not found : ");
                betResult.BetStatus = 2;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "betTxtAmount not found";
                return betResult;
            }

            // +++++++++++++++++++++++
            // Place the bet
            // +++++++++++++++++++++++
            bool betConfirm = false;
            string messageConfirm = "";
            try
            {

                var watch = System.Diagnostics.Stopwatch.StartNew();
                wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("betBtnBet")));
                IWebElement imgBtn = chromeDriver.FindElement(By.Id("betBtnBet"));
                IWebElement btn = imgBtn.FindElement(By.XPath("./.."));

                btn.Click();

                Thread.Sleep(1000);

                try
                {
                    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(2));
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("betBtnBet")));
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


                //string panSportsAttr = chromeDriver.FindElement(By.Id("panSports")).GetAttribute("style");
                //NLogger.Log(EventLevel.Debug,"panSportsAttr : " + panSportsAttr);

                //if (panSportsAttr == "")
                //{
                //    btn.Click();

                //    Thread.Sleep(1000);

                //    if (panSportsAttr == "")
                //    {
                //        betConfirm = false;
                //        messageConfirm = "Bet failed at second time";
                //    }
                //    else
                //    {
                //        chromeDriver.SwitchTo().DefaultContent();
                //        chromeDriver.SwitchTo().Frame(0);

                //        wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                //        wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='test2']/div/ul[3]/li/a")));
                //        chromeDriver.FindElement(By.XPath("//*[@id='test2']/div/ul[3]/li/a")).Click();

                //        watch.Stop();
                //        NLogger.Log(EventLevel.Info,"LOL 999 :" + watch.ElapsedMilliseconds + " ms");

                //        betConfirm = true;
                //        messageConfirm = "bet is confirmed";
                //    }

                //}
                //else
                //{
                //    chromeDriver.SwitchTo().DefaultContent();
                //    chromeDriver.SwitchTo().Frame(0);

                //    wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
                //    wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id='test2']/div/ul[3]/li/a")));
                //    chromeDriver.FindElement(By.XPath("//*[@id='test2']/div/ul[3]/li/a")).Click();

                //    watch.Stop();
                //    NLogger.Log(EventLevel.Info,"LOL 999 :" + watch.ElapsedMilliseconds + " ms");

                //    betConfirm = true;
                //    messageConfirm = "bet is confirmed";
                //}

            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error Confirm bet Ibc  : ");
                betConfirm = false;
                messageConfirm = "bet not confirmed";
            }
            chromeDriver.SwitchTo().DefaultContent();
            chromeDriver.SwitchTo().Frame(0);
            chromeDriver.SwitchTo().Frame("mainIframe3");

            betResult.BetStatus = 2;
            betResult.BetIsConfirmed = betConfirm;
            betResult.BetIsPlaced = true;
            betResult.BetMessage = messageConfirm;
            return betResult;
        }
    }
}
