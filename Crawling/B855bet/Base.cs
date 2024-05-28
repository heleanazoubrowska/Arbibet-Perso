using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System;

namespace ArbibetProgram.Crawling
{
    public partial class B855bet : CasinoAPI
    {
        public B855bet()
        {
	        prv_NumUselessLeagues = Config.Config.uselessLeague.Count;
        }


        public override void backToFrame()
        {
            try
            {
                if (focusOnBet)
                {
                    WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(5));
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    try
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnBPCancel")));
                        chromeDriver.FindElement(By.Id("btnBPCancel")).Click();
                    }
                    catch
                    {
                        NLogger.Log(EventLevel.Error, "Cennot click on btn cancel bet on 855");
                    }
                    chromeDriver.SwitchTo().DefaultContent();
                    chromeDriver.SwitchTo().Frame("mainFrame");
                    focusOnBet = false;
                    watch.Stop();
                    NLogger.Log(EventLevel.Info, $"{base.accountAPI.AccountBookmakerName} back frame : {watch.ElapsedMilliseconds} ms");
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
