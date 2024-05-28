using ArbibetProgram.Models;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using ArbibetProgram.Crawling;

namespace ArbibetProgram.Models
{
	public abstract class CasinoAPI
	{
		// -------------------------------------
		#region common variables
		// -------------------------------------
		public ChromeOptions chromeOptions { get; set; }
		public ChromeDriver chromeDriver { get; set; }

		public AccountData accountAPI { get; set; }

		public bool isLogin { get; set; }
		public bool focusOnBet = false;

		protected int prv_NumUselessLeagues;
		// -------------------------------------
		#endregion common variables
		// -------------------------------------



		// -------------------------------------
		#region abstract methods
		// -------------------------------------
		/// <summary>
		/// Connect to the casino using the current accountAPI
		/// </summary>
		public abstract void Connect(AccountData prm_AccountAPI = null);
		public abstract void Login(int prm_WaitTime);

		protected abstract void removingPopup();

		public abstract void udpateOdd(Bookmaker prm_Bookmaker, bool hedgeBet);
		protected abstract void getOdds(Bookmaker prm_Bookmaker, HtmlDocument htmlDoc, bool hedgeBet);
		public abstract BetResult placeBet(string matchName, string winOddType, decimal winOdd, bool winOddFav, bool hedgeBet);
		protected abstract BetResult findBet(HtmlDocument htmlDoc, string matchToFind, string hdpToFind, decimal winOddToFind, bool winOddFav, bool hedgeBet);
		protected abstract BetResult checkBet(string oddPath, string line1I, string hdpToFind, CheckBet_ExtraParams prm_ExtraParams = null);
		public abstract BetResult confirmBet();

		public abstract decimal getBalance();
		public abstract void backToFrame();
		public abstract CasinoBetStatus getBetStatus(string match);

		public abstract void Report();
		// -------------------------------------
		#endregion abstract methods
		// -------------------------------------



		// -------------------------------------
		#region common methods
		// -------------------------------------
		public void quitBrowser()
		{
			chromeDriver.Quit();
		}
		// -------------------------------------
		#endregion common methods
		// -------------------------------------


		public static Type GetImplementationByName(string prm_BookmakerName)
		{
			switch (prm_BookmakerName)
			{
				case "Aa2888":
					return typeof(Aa2888);
				case "855bet":
					return typeof(B855bet);
				case "Cambo88 BTI":
					return typeof(Cambo88BTI);
				case "Cambo88 SBO":
					return typeof(Cambo88SBO);
				case "Cambo88 UG":
					return typeof(Cambo88UG);
				case "Ibc05":
					return typeof(Ibc05);
				case "Ibet789":
					return typeof(Ibet789);
				case "Nba369":
					return typeof(Nba369);
				case "Ph2888":
					return typeof(Ph2888);
				case "Va2888":
					return typeof(Va2888);
				case "Va2888 AFB":
					return typeof(Va2888AFB);
				default:
					throw new NotImplementedException();
			}
		}
	}
}
