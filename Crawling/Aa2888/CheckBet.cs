using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using HtmlAgilityPack;
using System;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Aa2888 : CasinoAPI
    {
	    protected override BetResult checkBet(string oddPath, string line1I, string hdpToFind, CheckBet_ExtraParams prm_ExtraParams = null)
        {
            bool betPlaced = false;

            BetResult betResult = new BetResult();
            string betMessage = "";
            string hdpSens = "";

            // +++++++++++++++++++++++
            // Click The Odd
            // +++++++++++++++++++++++
            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));


            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.XPath($"//*[@id='{line1I}']{oddPath}")));
                chromeDriver.FindElementByXPath($"//*[@id='{line1I}']{oddPath}").Click();
                focusOnBet = true;
            }
            catch (NoSuchElementException e)
            {
                NLogger.Log(EventLevel.Error, $"Error - Click not founded : {e}");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 8;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Click not found";
                return betResult;
            }

            Thread.Sleep(1000);

            // +++++++++++++++++++++++
            // Parse bet panel HTML
            // +++++++++++++++++++++++

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            string bet = "";
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.Id("divBetZone")));
                bet = chromeDriver.FindElementById("divBetZone").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find betlistdiv 855BET ");
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);

            // +++++++++++++++++++++++
            // Control the odd is > 1
            // +++++++++++++++++++++++

            string oddToBet = "";
            try
            {
                oddToBet = htmlDoc.DocumentNode.SelectSingleNode(".//table/tbody/tr[7]/td[2]/span[1]").InnerText;
                oddToBet = oddToBet.Replace("&nbsp;", "");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - Click not founded ");
            }


            decimal oddDecimal = decimal.Parse(oddToBet, CultureInfo.InvariantCulture.NumberFormat);
            NLogger.Log(EventLevel.Debug, $"odd to bet DECIMAL : {oddDecimal}");
            if (oddDecimal < MainClass.minOddRequired)
            {
                NLogger.Log(EventLevel.Error, "Odd is under than 1, cannot place the bet");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 5;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Odd is under than 1, cannot place the bet";
                return betResult;
            }


            // +++++++++++++++++++++++
            // Control Type Odd is Correct
            // +++++++++++++++++++++++

            string oddType = "";

            try
            {

                oddType = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='trBZHandicap']/td[2]/span").InnerText;

            }
            catch (ElementNotVisibleException e)
            {

            }

            if (Common.isOverUnder(hdpToFind))
            {
                // Should get Over / Under ( if needed )
            }
            else
            {
                if (oddType.Substring(0, 1) == "-")
                {
                    hdpSens = "-";
                }
                else
                {
                    hdpSens = "+";
                }
            }

            oddType = Common.cleanOddPlaceBet(oddType);
            string hdpToFindClean = Common.cleanOddPlaceBet(hdpToFind);
            NLogger.Log(EventLevel.Debug, $"Odd converted : {oddType}");

            if(oddType == "0")
            {
                hdpSens = "0";
                NLogger.Log(EventLevel.Debug, $"hdpSens = 0");
            }

            if (oddType != hdpToFindClean)
            {
                NLogger.Log(EventLevel.Error,  $"Error - odd type Aa222 is different : oddType : {oddType} - hdpToFind : {hdpToFind}");
                //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 3;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "odd type Aa222 is different";
                return betResult;
            }

            // +++++++++++++++++++++++
            // Get Max Bet
            // +++++++++++++++++++++++

            string brutMaxBet = "";
            try
            {
                brutMaxBet = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='tdBetLimit']").InnerText;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Max bet not founded");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 9;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Error - Max bet not founded";
                return betResult;
            }

            NLogger.Log(EventLevel.Debug, $"brutMaxBet : {brutMaxBet}");
            Regex regex = new Regex(@"[^~]*$");
            System.Text.RegularExpressions.Match match = regex.Match(brutMaxBet);

            string maxBetStr = match.Value;
            decimal maxBet = (decimal.Parse(maxBetStr, CultureInfo.InvariantCulture.NumberFormat));
            NLogger.Log(EventLevel.Debug, $"maxBet decimal : {maxBet}");

            // +++++++++++++++++++++++
            // Get Team Name
            // +++++++++++++++++++++++

            string teamFav = "";
            string teamNoFav = "";
            try
            {
                teamFav = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='divBetZone']/table/tbody/tr[3]/td/span[1]").InnerText;
                teamNoFav = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='divBetZone']/table/tbody/tr[3]/td/span[2]").InnerText;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Team name bet not founded");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 9;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Error - Team name not founded";
                return betResult;
            }

            betPlaced = true;
            betMessage = "Bet is placed";

            betResult.BetStatus = 0;
            betResult.BetIsConfirmed = true;
            betResult.BetIsPlaced = betPlaced;
            betResult.BetMessage = betMessage;
            betResult.BetMaxBet = maxBet;
            betResult.BetHdpSens = hdpSens;
            betResult.BetTeamFav = teamFav;
            betResult.BetTeamNoFav = teamNoFav;
            return betResult;
        }
    }
}
