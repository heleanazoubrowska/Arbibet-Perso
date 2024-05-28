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
    public partial class Va2888AFB : CasinoAPI
    {
        public Va2888AFB()
        {
	        prv_NumUselessLeagues = Config.Config.uselessLeague.Count;
        }

		public override void backToFrame()
        {
	        if (focusOnBet)
	        {
		        var watch = System.Diagnostics.Stopwatch.StartNew();
		        chromeDriver.FindElement(By.XPath("//*[@id='tbBetBox']/tbody/tr[2]/td/table/tbody/tr[8]/td/span[1]")).Click();
		        focusOnBet = false;
		        watch.Stop();
		        NLogger.Log(EventLevel.Info, "Va2888 AFB back frame ::" + watch.ElapsedMilliseconds + " ms");
	        }
        }

    }
}
