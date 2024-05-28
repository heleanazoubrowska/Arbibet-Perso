using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;

namespace ArbibetProgram.Crawling
{
    public partial class Aa2888 : CasinoAPI
    {
        public Aa2888()
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

                    // insert code here
                    chromeDriver.FindElement(By.Id("btnCancelBet")).Click();

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
