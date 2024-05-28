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
	public partial class Ibet789 : CasinoAPI
	{
		public override void Login(int prm_WaitTime)
		{
			try
			{
				IWebElement form = chromeDriver.FindElementByTagName("form");
				//loginFormFounded = true;
				// Connect From Login Page
				WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(prm_WaitTime));
				chromeDriver.FindElementById("txtUserName").SendKeys(accountAPI.AccountUsername);
				chromeDriver.FindElementById("password").SendKeys(accountAPI.AccountPassword);
				Thread.Sleep(1000);
				chromeDriver.FindElementById("btnSignIn").Click();

				wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(30));
				wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("btnAgree")));

				chromeDriver.FindElementById("btnAgree").Click();
			}
			catch (Exception prv_Exception)
			{
				NLogger.Log(EventLevel.Error, $"{base.accountAPI.AccountBookmakerName} failed to log in");
				NLogger.LogError(prv_Exception);
			}
		}
	}
}
