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
		public override BetResult placeBet(string matchName, string winOddType, decimal winOdd, bool winOddFav, bool hedgeBet)
		{
			string table1 = "";
			WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
			wait.Until(ExpectedConditions.ElementIsVisible(By.Id("oddsTable")));
			string html = chromeDriver.FindElementById("oddsTable").GetAttribute("innerHTML");

			HtmlDocument htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(html);

			//return true;

			BetResult betResult = findBet(htmlDoc, matchName, winOddType, winOdd, winOddFav, hedgeBet);
			return betResult;

			//try
			//{
			//    IWebElement gridFutur = chromeDriver.FindElementById("tbl2");

			//    string html = gridFutur.GetAttribute("innerHTML");

			//    HtmlDocument htmlDoc = new HtmlDocument();
			//    htmlDoc.LoadHtml(html);

			//    return findBet(htmlDoc, matchName, winOddType, winOdd, winOddFav);
			//}
			//catch (NoSuchElementException elemNotFound)
			//{
			//    NLogger.Log(EventLevel.Error,"Aa2888 odds table on place bet not found");
			//    return false;
			//}		}
		}
	}
}
