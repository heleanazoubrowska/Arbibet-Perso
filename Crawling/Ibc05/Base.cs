using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using System;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Ibc05 : CasinoAPI
    {
        public Ibc05()
        {
	        prv_NumUselessLeagues = Config.Config.uselessLeague.Count;
        }

        public override void backToFrame()
        {
            try
            {
                if (focusOnBet)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    IWebElement imgBtn = chromeDriver.FindElement(By.Id("betBtnCancel"));
                    IWebElement btn = imgBtn.FindElement(By.XPath("./.."));
                    chromeDriver.SwitchTo().DefaultContent();
                    chromeDriver.SwitchTo().Frame(0);
                    chromeDriver.SwitchTo().Frame("mainIframe3");
                    focusOnBet = false;
                    watch.Stop();
                    NLogger.Log(EventLevel.Info, $"Ibc 05 back frame : {watch.ElapsedMilliseconds} ms");
                }

            }
            catch (System.Exception prv_Exception)
            {
                NLogger.Log(EventLevel.Info, $"Error in {base.accountAPI.AccountBookmakerName} bbackToFrame: {prv_Exception.Message}");
                NLogger.LogError(prv_Exception);

                //Environment.Exit(-1);
            }
        }
    }
}
