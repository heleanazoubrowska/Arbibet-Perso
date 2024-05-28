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
        public override CasinoBetStatus getBetStatus(string match)
        {
            CasinoBetStatus casinoBetStatus = new CasinoBetStatus();
            return casinoBetStatus;
        }
    }
}
