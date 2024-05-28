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
	public partial class NewCasino : CasinoAPI
	{
        protected override void getOdds(Bookmaker prm_Bookmaker, HtmlDocument htmlDoc, bool hedgeBet)
        {
        }

        public override void udpateOdd(Bookmaker prm_Bookmaker, bool hedgeBet)
        {
        }
    }
}
