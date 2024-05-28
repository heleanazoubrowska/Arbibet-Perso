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
		public override BetResult placeBet(string matchName, string winOddType, decimal winOdd, bool winOddFav, bool hedgeBet)
		{
			string table = "";
			try
			{
				WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
				wait.Until(ExpectedConditions.ElementExists(By.Id("oddGridArae")));
				table = chromeDriver.FindElementById("oddGridArae").GetAttribute("innerHTML");
			}
			catch
			{
				NLogger.Log(EventLevel.Debug, "UG grid no Live not found ");
			}


			HtmlDocument htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(table);
			BetResult betResult = findBet(htmlDoc, matchName, winOddType, winOdd, winOddFav, hedgeBet);
			return betResult;
		}
    }
}
