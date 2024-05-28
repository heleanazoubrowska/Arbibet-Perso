using System;
using HtmlAgilityPack;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

using System.Globalization;
using System.Linq;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using System.Threading;
using Newtonsoft.Json;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Ph2888 : CasinoAPI
    {
	    // mainIframe2 = left menu
	    // mainIframe3 = odd Iframe

        public Ph2888()
        {
	        prv_NumUselessLeagues = Config.Config.uselessLeague.Count;
        }

		public override void backToFrame()
        {
	        if (focusOnBet)
	        {
		        var watch = System.Diagnostics.Stopwatch.StartNew();
		        chromeDriver.FindElement(By.XPath("//*[@id='tbBetBox']/tbody/tr[2]/td/table/tbody/tr[8]/td/span[1]")).Click();
		        chromeDriver.SwitchTo().ParentFrame();
		        chromeDriver.SwitchTo().Frame("fraMain");
		        focusOnBet = false;
		        watch.Stop();
		        NLogger.Log(EventLevel.Info, "Va2888 back frame ::" + watch.ElapsedMilliseconds + " ms");
	        }
        }

    }
}
