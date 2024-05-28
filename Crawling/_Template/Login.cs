﻿using System;
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
	    public override void Login(int prm_WaitTime)
	    {
		    try
		    {

		    }
		    catch (Exception prv_Exception)
		    {
			    NLogger.Log(EventLevel.Error, $"{base.accountAPI.AccountBookmakerName} failed to log in");
			    NLogger.LogError(prv_Exception);
		    }
		}
    }
}