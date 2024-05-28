using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArbibetProgram.Functions;
using ArbibetProgram.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
	public partial class NewCasino : CasinoAPI
	{
		public override void Connect(AccountData prm_AccountAPI = null)
		{
			if (prm_AccountAPI != null)
			{
				base.accountAPI = prm_AccountAPI;
			}


			NLogger.Log(EventLevel.Info, "Connect to NewCasino...");
		}

		protected override void removingPopup()
		{
		}

	}
}
