using System;
using HtmlAgilityPack;
using OpenQA.Selenium;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using ArbibetProgram.Models;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Ibet789 : CasinoAPI
    {
        public override CasinoBetStatus getBetStatus(string match)
        {
            CasinoBetStatus casinoBetStatus = new CasinoBetStatus();
            return casinoBetStatus;
        }
    }
}
