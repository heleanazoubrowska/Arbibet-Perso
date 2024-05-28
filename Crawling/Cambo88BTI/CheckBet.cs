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
    public partial class Cambo88BTI : CasinoAPI
    {
        //protected BetResult checkBet(string oddPath, int line0I, int line1I, int line2I, int line3I, string hdpToFind)
        protected override BetResult checkBet(string oddPath, string line1I, string hdpToFind, CheckBet_ExtraParams prm_ExtraParams = null)
        {

            bool betPlaced = false;
            BetResult betResult = new BetResult();
            string betMessage = "";
            string hdpSens = "";

            WebDriverWait wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));

            // +++++++++++++++++++++++
            // Click The Odd
            // +++++++++++++++++++++++

            //wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            //wait.Until(ExpectedConditions.ElementExists(By.XPath("//html/body/div[1]/div[4]/div[1]/main/div/sb-comp/div/div/div[3]/div[1]/div[2]/div/div[1]")));
            chromeDriver.FindElementByXPath($"//html/body/div[1]/div[4]/div[1]/main/div/sb-comp/div[{prm_ExtraParams.line0I}]/div/div[3]/div[@class='rj-asian-events__single-league'][{line1I}]/div[2]/div[@class='rj-asian-events__single-event'][{prm_ExtraParams.line2I}]/div[@class='rj-asian-events__row'][{prm_ExtraParams.line3I}]{oddPath}").Click();

            try
            {
                IAlert alertOdd = chromeDriver.SwitchTo().Alert();
                alertOdd.Accept();
            }
            catch (NoAlertPresentException Ex)
            {

            }

            Thread.Sleep(3000);

            // +++++++++++++++++++++++
            // Parse bet panel HTML
            // +++++++++++++++++++++++

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            string bet = "";
            try
            {
                bet = chromeDriver.FindElementById("idBetsSelections").GetAttribute("innerHTML");
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "CHECK BET LIST : cannot find betlistdiv 855BET ");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 8;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Error : cannot find betlistdiv 855BET";
                return betResult;
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(bet);

            // +++++++++++++++++++++++
            // Control the odd is > 1
            // +++++++++++++++++++++++

            string oddToBet = "";
            try
            {
                oddToBet = htmlDoc.DocumentNode.SelectSingleNode("./div[1]/div/div[1]/div[2]/label").InnerText;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - odd not founded");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 5;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "Error : cannot find odd";
                return betResult;
            }

            NLogger.Log(EventLevel.Debug, $"odd to bet : {oddToBet}");
            oddToBet = oddToBet.Replace("&nbsp;", "");
            decimal oddDecimal = decimal.Parse(oddToBet, CultureInfo.InvariantCulture.NumberFormat);
            NLogger.Log(EventLevel.Debug, $"odd to bet DECIMAL : {oddDecimal}");

            NLogger.Log(EventLevel.Debug, "Case 7");

            if (oddDecimal < MainClass.minOddRequired)
            {
                NLogger.Log(EventLevel.Error, "Odd is under than 1, cannot place the bet");
                betResult.BetStatus = 2;
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

                oddType = htmlDoc.DocumentNode.SelectSingleNode("./div[1]/div/div[1]/div[2]/span").InnerText;

            }
            catch (ElementNotVisibleException e)
            {

            }

            if (Common.isOverUnder(hdpToFind))
            {

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

            oddType = Common.getQuarterBet(oddType);
            oddType = Common.cleanOddPlaceBet(oddType);
            string hdpToFindClean = Common.cleanOddPlaceBet(hdpToFind);
            NLogger.Log(EventLevel.Debug, $"Odd converted : {oddType}");

            if (oddType == "0")
            {
                hdpSens = "0";
                NLogger.Log(EventLevel.Debug, $"hdpSens = 0");
            }

            if (oddType != hdpToFindClean)
            {
                NLogger.Log(EventLevel.Error, $"Error - odd type BTI is different : oddType : {oddType} - hdpToFind : {hdpToFind}");
                //chromeDriver.FindElement(By.XPath("//*[@id='divBetZone']/table/tbody/tr[12]/td/button[1]")).Click();
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 3;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "odd type BTI is different";
                return betResult;
            }

            // +++++++++++++++++++++++
            // Get Max Bet
            // +++++++++++++++++++++++

            wait = new WebDriverWait(chromeDriver, TimeSpan.FromSeconds(10));
            string brutMaxBet = "";
            try
            {
                brutMaxBet = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='sce_0']/div/div[4]/div/div/div[3]/div").InnerText;
            }
            catch
            {
                NLogger.Log(EventLevel.Error, "Error - brutMaxBet not found");
                betResult.BetStatus = 2;
                betResult.BetErrorStatus = 9;
                betResult.BetIsConfirmed = false;
                betResult.BetIsPlaced = false;
                betResult.BetMessage = "brutMaxBet not found";
                return betResult;
            }

            var m = Regex.Match(brutMaxBet, "[^- ]*$");

            string maxBetStr = m.Value;

            maxBetStr = maxBetStr.Replace("&nbsp;", "");
            maxBetStr = maxBetStr.Replace(" ", "");
            decimal maxBet = (decimal.Parse(maxBetStr, CultureInfo.InvariantCulture.NumberFormat));
            NLogger.Log(EventLevel.Debug, $"Max bet : {maxBet}");

            NLogger.Log(EventLevel.Debug, "Case 6");


            // +++++++++++++++++++++++
            // Get Team Name
            // +++++++++++++++++++++++

            string teamFav = "";
            string teamNoFav = "";

            try
            {
                teamFav = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='sce_0']/div/div[3]/div/div/div[1]/strong[1]").InnerText;
                teamNoFav = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='sce_0']/div/div[3]/div/div/div[1]/strong[2]").InnerText;
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
            betResult.BetIsConfirmed = false;
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
