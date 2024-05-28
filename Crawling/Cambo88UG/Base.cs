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
    public partial class Cambo88UG : CasinoAPI
    {
        public Cambo88UG()
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

                    chromeDriver.FindElement(By.XPath("//*[@id='betBtnbox']/div[2]/span")).Click();

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
