using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Ibet789 : CasinoAPI
    {
        public Ibet789()
        {
	        prv_NumUselessLeagues = Config.Config.uselessLeague.Count;
        }

        public override void backToFrame()
        {
            if (focusOnBet)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                chromeDriver.FindElement(By.Id("betBtnCancel")).Click();
                chromeDriver.SwitchTo().ParentFrame();
                chromeDriver.SwitchTo().Frame("fraMain");
                focusOnBet = false;
                watch.Stop();
                NLogger.Log(EventLevel.Info, $"Ibet789 back frame :: {watch.ElapsedMilliseconds} ms");
            }
        }
    }
}
