using System;
using System.Globalization;
using System.Linq;
using System.Threading;

using HtmlAgilityPack;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;

using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ArbibetProgram.Crawling
{
    public partial class Cambo88SBO : CasinoAPI
    {
        protected override BetResult findBet(HtmlDocument htmlDoc, string matchToFind, string hdpToFind, decimal winOddToFind, bool winOddFav, bool hedgeBet)
        {
            string teamFav = "";
            string teamNoFav = "";
            string matchName = "";
            string league = "";
            bool canUseCurrentLeague = true;
            string typeLeague = "";
            string currentLeague = "";
            string currentLeagueType = "";

            int line1I = 0;

            BetResult betResult = new BetResult();

            var prv_RowMatches = htmlDoc.DocumentNode.SelectNodes("//tbody");
            int prv_NumRowMatches = prv_RowMatches.Count;

            //foreach (HtmlNode rowMatch in htmlDoc.DocumentNode.SelectNodes("//tbody"))
            for (int RowMatchCounter = 0; RowMatchCounter < prv_NumRowMatches; RowMatchCounter++)
            {
	            var rowMatch = prv_RowMatches[RowMatchCounter];
                line1I++;

                if (rowMatch.SelectNodes(".//tr") == null)
                {
                    continue;
                }

                if (rowMatch.SelectNodes(".//tr").Count == 1) // Is line League
                {

                    if (rowMatch.SelectSingleNode(".//tr[1]").Attributes["class"] == null)
                    {
                        continue;
                    }

                    if (rowMatch.SelectSingleNode(".//tr[1]").Attributes["class"].Value == "league-row") // Is line League
                    {
                        canUseCurrentLeague = true;


                        if (rowMatch.SelectSingleNode(".//tr/td/span/span[2]") == null)
                        {
                            league = rowMatch.SelectSingleNode(".//tr/td/span/span/span[2]").InnerText;
                        }
                        else
                        {
                            league = rowMatch.SelectSingleNode(".//tr/td/span/span[2]").InnerText;
                        }

                        league = league.ToUpper();

                        //foreach (string str in Config.Config.uselessLeague)
                        for (int UselessLeagueCounter = 0; UselessLeagueCounter < prv_NumUselessLeagues; UselessLeagueCounter++)
                        {
	                        if (league.Contains(Config.Config.uselessLeague[UselessLeagueCounter]))
	                        {
		                        //NLogger.Log(EventLevel.Debug,currentLeague);
		                        canUseCurrentLeague = false;
		                        break;
	                        }
                        }

                        typeLeague = Common.getLeagueType(league);
                    }
                }
                if (rowMatch.SelectNodes(".//tr").Count == 3) // Is line odd
                {
                    int iC = 0;
                    if (rowMatch.SelectSingleNode(".//tr[1]").Attributes["id"].Value.Contains("row-live"))
                    {
                        iC = 1;
                    }

                    if (!canUseCurrentLeague)
                    {
                        continue;
                    }

                    teamFav = rowMatch.SelectSingleNode(".//tr[1]/td[3]/span[1]").InnerText;
                    teamNoFav = rowMatch.SelectSingleNode(".//tr[1]/td[3]/span[2]").InnerText;
                    teamFav = Common.cleanTeamName(teamFav) + typeLeague;
                    teamNoFav = Common.cleanTeamName(teamNoFav) + typeLeague;
                    matchName = $"{teamFav} - {teamNoFav}";

                    if (matchName == matchToFind)
                    {

                        NLogger.Log(EventLevel.Debug, $"To find : {matchName}");

                        // ++++++++++++++++++++
                        // OVER UNDER ODD FT & FH
                        // ++++++++++++++++++++


                        int iColL1 = 0;
                        int iColL2 = 0;
                        string typeTime = "";

                        for (int i = 0; i < 2; i++)
                        {
                            if (i == 0)
                            {
                                iColL1 = 5;
                                iColL2 = 2;
                                typeTime = "ft";
                            }
                            else
                            {
                                iColL1 = 8;
                                iColL2 = 5;
                                typeTime = "fh";
                            }

                            int prv_ColIndex_Over = iColL1 + iC;
                            int prv_ColIndex_Under = iColL2 + iC;
                            //try
                            //{
                            if (rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[1]") != null)
                            {
                                string overUnder = rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[1]").InnerText;
                                overUnder = overUnder.Replace("&nbsp;", "");
                                overUnder = overUnder.Replace(" ", "");
                                if (overUnder == "" || overUnder.Contains("&nbsp;"))
                                {
                                    continue;
                                }

                                if (rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[3]/span/span") == null)
                                {
                                    continue;
                                }
                                string strOddsOver = rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[3]/span/span").InnerText;
                                string strOddsUnder = rowMatch.SelectSingleNode($".//tr[2]/td[{prv_ColIndex_Under}]/a/span[2]/span/span").InnerText;
                                overUnder = overUnder.Replace("/", "-");
                                overUnder = $"{typeTime} o/u {overUnder}";
                                strOddsOver = strOddsOver.Replace("&nbsp;", "");
                                strOddsUnder = strOddsUnder.Replace("&nbsp;", "");
                                if (overUnder == hdpToFind)
                                {

                                    if (winOddFav)
                                    {
                                        NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsOver}");

                                        if (decimal.Parse(strOddsOver, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                        {
                                            NLogger.Log(EventLevel.Trace, $"Click on odds : 004 {typeTime}");

                                            // sn: not sure whether oddId refers to oddPath or line1I... guessing line1I?
                                            //betResult = checkBet(rowMatch.SelectSingleNode(".//tr[1]/td[" + (iColL1 + iC) + "]/a/span[3]/span/span").Attributes["id"].Value, hdpToFind);
                                            betResult = checkBet("", rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[3]/span/span").Attributes["id"].Value, hdpToFind);
                                            return betResult;
                                        }
                                        else
                                        {
                                            betResult.BetStatus = 1;
                                            betResult.BetIsConfirmed = false;
                                            betResult.BetIsPlaced = false;
                                            betResult.BetMessage = "Odd from panel > 1";
                                            return betResult;
                                        }
                                    }
                                    else
                                    {
                                        NLogger.Log(EventLevel.Debug, $"strOddsUnder : {strOddsUnder}");
                                        NLogger.Log(EventLevel.Debug, $"id : {rowMatch.SelectSingleNode($".//tr[2]/td[{prv_ColIndex_Under}]/a/span[2]/span/span").Attributes["id"].Value}");
                                        if (decimal.Parse(strOddsUnder, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                        {
                                            NLogger.Log(EventLevel.Debug, "FDP 1");
                                            // sn: not sure whether oddId refers to oddPath or line1I... guessing line1I?
                                            //betResult = checkBet(rowMatch.SelectSingleNode(".//tr[2]/td[" + (iColL2 + iC) + "]/a/span[2]/span/span").Attributes["id"].Value, hdpToFind);
                                            betResult = checkBet("", rowMatch.SelectSingleNode($".//tr[2]/td[{prv_ColIndex_Under}]/a/span[2]/span/span").Attributes["id"].Value, hdpToFind);
                                            NLogger.Log(EventLevel.Trace, $"Click on odds : 003 {typeTime}");
                                            return betResult;
                                        }
                                        else
                                        {
                                            NLogger.Log(EventLevel.Debug, "FDP 2");
                                            betResult.BetStatus = 1;
                                            betResult.BetIsConfirmed = false;
                                            betResult.BetIsPlaced = false;
                                            betResult.BetMessage = "Odd from panel > 1";
                                            return betResult;
                                        }
                                    }
                                }
                            }
                            //}
                            //catch
                            //{

                            //}
                        }

                        // ++++++++++++++++++++
                        // HANDICAP ODD
                        // ++++++++++++++++++++

                        for (int i = 0; i < 2; i++)
                        {
                            if (i == 0)
                            {
                                iColL1 = 4;
                                iColL2 = 1;
                                typeTime = "ft";
                            }
                            else
                            {
                                iColL1 = 7;
                                iColL2 = 4;
                                typeTime = "fh";
                            }

                            int prv_ColIndex_Over = iColL1 + iC;
                            int prv_ColIndex_Under = iColL2 + iC;

                            //try
                            //{
                            string sensHdp = "";
                            int posHdp = 0;

                            if (rowMatch.SelectSingleNode($".//tr[2]/td[{prv_ColIndex_Under}]/a/span[1]").InnerText == "&nbsp;")
                            {
                                continue;
                            }

                            string handicapType = "";
                            // Get the position of the favorite
                            if (rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[1]").InnerText == "" 
                                || 
                                rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[1]").InnerText == "&nbsp;")
                            {
                                sensHdp = "- ";
                                posHdp = 2;
                                handicapType = rowMatch.SelectSingleNode($".//tr[2]/td[{prv_ColIndex_Under}]/a/span[1]").InnerText;
                            }
                            else
                            {
                                sensHdp = "+ ";
                                posHdp = 1;
                                handicapType = rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[1]").InnerText;
                            }

                            handicapType = handicapType.Replace("&nbsp;", "");
                            handicapType = handicapType.Replace("/", "-");

                            if (handicapType != "0")
                            {
                                handicapType = sensHdp + handicapType;
                            }

                            handicapType = $"{typeTime} {handicapType}";

                            if (handicapType == hdpToFind)
                            {
                                string strOddsFav = rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[2]/span/span").InnerText;
                                string strOddsNoFav = rowMatch.SelectSingleNode($".//tr[2]/td[{prv_ColIndex_Under}]/a/span[2]/span/span").InnerText;
                                strOddsFav = strOddsFav.Replace("&nbsp;", "");
                                strOddsNoFav = strOddsNoFav.Replace("&nbsp;", "");

                                NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsFav}");

                                if (winOddFav)
                                {
                                    NLogger.Log(EventLevel.Debug, $"strOddsOver : {strOddsFav}");

                                    if (decimal.Parse(strOddsFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                    {
                                        NLogger.Log(EventLevel.Trace, $"Click on odds : 002 {handicapType}");
                                        // sn: not sure whether oddId refers to oddPath or line1I... guessing line1I?
                                        // betResult = checkBet(rowMatch.SelectSingleNode(".//tr[1]/td[" + (iColL1 + iC) + "]/a/span[2]/span/span").Attributes["id"].Value, hdpToFind);
                                        betResult = checkBet("",rowMatch.SelectSingleNode($".//tr[1]/td[{prv_ColIndex_Over}]/a/span[2]/span/span").Attributes["id"].Value, hdpToFind);
                                        return betResult;
                                    }
                                    else
                                    {
                                        betResult.BetStatus = 1;
                                        betResult.BetIsConfirmed = false;
                                        betResult.BetIsPlaced = false;
                                        betResult.BetMessage = "Odd from panel > 1";
                                        return betResult;
                                    }
                                }
                                else
                                {
                                    NLogger.Log(EventLevel.Debug, $"strOddsNoFav : {strOddsNoFav}");

                                    if (decimal.Parse(strOddsNoFav, CultureInfo.InvariantCulture.NumberFormat) >= MainClass.minOddRequired)
                                    {
                                        NLogger.Log(EventLevel.Trace, $"Click on odds : 001 {handicapType}");
                                        // sn: not sure whether oddId refers to oddPath or line1I... guessing line1I?
                                        // betResult = checkBet(rowMatch.SelectSingleNode(".//tr[2]/td[" + (iColL2 + iC) + "]/a/span[2]/span/span").Attributes["id"].Value, hdpToFind);
                                        betResult = checkBet("",rowMatch.SelectSingleNode($".//tr[2]/td[{prv_ColIndex_Under}]/a/span[2]/span/span").Attributes["id"].Value, hdpToFind);
                                        return betResult;
                                    }
                                    else
                                    {
                                        betResult.BetStatus = 1;
                                        betResult.BetIsConfirmed = false;
                                        betResult.BetIsPlaced = false;
                                        betResult.BetMessage = "Odd from panel > 1";
                                        return betResult;
                                    }
                                }

                            }
                            //}
                            //catch
                            //{

                            //}

                        }

                    }

                }
            }
            betResult.BetStatus = 1;
            betResult.BetErrorStatus = 2;
            betResult.BetIsConfirmed = false;
            betResult.BetIsPlaced = false;
            betResult.BetMessage = "Match and odd not founded";
            return betResult;
        }
    }
}
