using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArbibetProgram.Functions;
using ArbibetProgram.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
	public partial class Ibc05 : CasinoAPI
	{
		public override void Login(int prm_WaitTime)
		{
			try
			{
				IWebElement form = chromeDriver.FindElementByTagName("form");
				// Connect From Login Page
				WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30));
				form.FindElement(By.Name("useracc")).SendKeys(accountAPI.AccountUsername);
				form.FindElement(By.Name("passwd")).SendKeys(accountAPI.AccountPassword);
				form.FindElement(By.Name("remember")).Click();
				Thread.Sleep(1000);
				form.FindElement(By.Id("btnLogin")).Click();
			}
			catch (Exception prv_Exception)
			{
				NLogger.Log(EventLevel.Error, $"{base.accountAPI.AccountBookmakerName} failed to log in");
				NLogger.LogError(prv_Exception);
			}
		}
	}
}
