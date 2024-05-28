using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Nba369 : CasinoAPI
    {

        public Nba369()
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
                    chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
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
