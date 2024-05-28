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
		public override BetResult placeBet(string matchName, string winOddType, decimal winOdd, bool winOddFav, bool hedgeBet)
		{
			string table1 = "";
			try
			{
				WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
				table1 = chromeDriver.FindElementById("tableRunN").GetAttribute("innerHTML");
			}
			catch
			{
				//NLogger.Log(EventLevel.Debug,"oddsTable_1_1_3_r not found ");
			}

			string table2 = "";
			try
			{
				WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(1));
				table2 = chromeDriver.FindElementById("tableTodayN").GetAttribute("innerHTML");
			}
			catch
			{
				//NLogger.Log(EventLevel.Debug,"oddsTable_1_1_3_t not found ");
			}

			string allTables = table1 + table2;

			HtmlDocument htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(allTables);
			BetResult betResult = findBet(htmlDoc, matchName, winOddType, winOdd, winOddFav, hedgeBet);
			return betResult;

			//IWebElement gridFutur = chromeDriver.FindElementById("tableTodayN");

			//string html = gridFutur.GetAttribute("innerHTML");

			//HtmlDocument htmlDoc = new HtmlDocument();
			//htmlDoc.LoadHtml(html);

			////return true;

			//return findBet(htmlDoc, matchName, winOddType, winOdd, winOddFav);


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
