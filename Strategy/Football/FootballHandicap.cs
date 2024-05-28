using ArbibetProgram.Models;
using ArbibetProgram.Functions;
using System;
using Newtonsoft.Json;
using ArbibetProgram.ApiRest;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ArbibetProgram.Telegram;

namespace ArbibetProgram.Strategy
{
    public static partial class Football
    {
        public static decimal BetValue = MainClass.betValueDecimal;

        //public static string oddType = "";
        //public static decimal profitTeam1 = 0;
        //public static decimal profitTeam2 = 0;
        //public static string teamWinner1 = "";
        //public static string teamWinner2 = "";
        //public static decimal winOdd1 = 0;
        //public static decimal winOdd2 = 0;
        //public static string casetype = "";
        public static bool canTest = true;


        public static void checkHandicapMatch(int prm_Index_BM1, int prm_Index_BM2, Match bookmaker1, Match bookmaker2, string bMName1, string bMName2, int bmI1, int bmI2, string account1, string account2)
        {
            string oddType = "";
            decimal profitTeam1 = 0;
            decimal profitTeam2 = 0;
            string teamWinner1 = "";
            string teamWinner2 = "";
            bool winOddIsFav1 = true;
            bool winOddIsFav2 = true;
            decimal winOdd1 = 0;
            decimal winOdd2 = 0;
            string casetype = "";
            bool arbFound = false;

            for (int i = 0; i < bookmaker1.Odds.Count; i++)
            {
                for (int x = 0; x < bookmaker2.Odds.Count; x++)
                {
                    if (bookmaker1.Odds[i].oddType == bookmaker2.Odds[x].oddType)
                    {
                        MainClass.NbOddCheck++;

                        oddType = "";
                        profitTeam1 = 0;
                        profitTeam2 = 0;
                        teamWinner1 = "";
                        teamWinner2 = "";
                        winOdd1 = 0;
                        winOdd2 = 0;
                        casetype = "";
                        arbFound = false;

                        // First check if an opportunity can exist
                        if (bookmaker1.Odds[i].oddFav < 1 && bookmaker1.Odds[i].oddNoFav < 1.05m)
                        {
                            continue;
                        }

                        if (bookmaker2.Odds[x].oddFav < 1 && bookmaker2.Odds[x].oddNoFav < 1.05m)
                        {
                            continue;
                        }

                        // -----------------------------------------------------------------------
                        // what to do if the favorite is different, bm1.MatchName = bm2.MatchName2
                        // -----------------------------------------------------------------------
                        // if (bookmaker1.TeamFav.TeamName == bookmaker2.TeamNoFav.TeamName)
                        if (bookmaker1.MatchName != bookmaker2.MatchName)
                        {
                            continue;

                            //if (bookmaker1.Odds[i].oddFav > 1 && bookmaker2.Odds[x].oddFav > 1)
                            //{
                            //    NLogger.Log(EventLevel.Debug,"CASE 1");
                            //    arbFound = true;
                            //    profitTeam1 = calculHdpProfit(bookmaker1.Odds[i].oddFav);
                            //    profitTeam2 = calculHdpProfit(bookmaker2.Odds[x].oddFav);

                            //    teamWinner1 = bookmaker1.TeamFav.TeamName;
                            //    teamWinner2 = bookmaker2.TeamFav.TeamName;
                            //    winOdd1 = bookmaker1.Odds[i].oddFav;
                            //    winOdd2 = bookmaker2.Odds[x].oddNoFav;
                            //    oddType = bookmaker1.Odds[i].oddType;
                            //    casetype = "case 1";
                            //}

                            //if (bookmaker1.Odds[i].oddNoFav > 1 && bookmaker2.Odds[x].oddNoFav > 1)
                            //{
                            //    NLogger.Log(EventLevel.Debug,"CASE 2");
                            //    arbFound = true;
                            //    profitTeam1 = calculHdpProfit(bookmaker1.Odds[i].oddNoFav);
                            //    profitTeam2 = calculHdpProfit(bookmaker2.Odds[x].oddNoFav);

                            //    teamWinner1 = bookmaker1.TeamFav.TeamName;
                            //    teamWinner2 = bookmaker2.TeamFav.TeamName;
                            //    winOdd1 = bookmaker1.Odds[i].oddFav;
                            //    winOdd2 = bookmaker2.Odds[x].oddNoFav;
                            //    oddType = bookmaker1.Odds[i].oddType;
                            //    casetype = "case 1";
                            //}

                        }

                        // -----------------------------------------------------------------------
                        // what to do if the favorite is same, bm1.MatchName = bm2.MatchName
                        // -----------------------------------------------------------------------
                        //if (bookmaker1.TeamFav.TeamName == bookmaker2.TeamFav.TeamName)
                        {
                            if (bookmaker1.Odds[i].oddFav > 1 && bookmaker2.Odds[x].oddNoFav > 1.05m)
                            {
                                arbFound = true;
                                profitTeam1 = calculHdpProfit(bookmaker1.Odds[i].oddFav);
                                profitTeam2 = calculHdpProfit(bookmaker2.Odds[x].oddNoFav);

                                teamWinner1 = bookmaker1.TeamFav.TeamName;
                                teamWinner2 = bookmaker2.TeamNoFav.TeamName;

                                winOddIsFav1 = true;
                                winOddIsFav2 = false;
                                winOdd1 = bookmaker1.Odds[i].oddFav;
                                winOdd2 = bookmaker2.Odds[x].oddNoFav;
                                oddType = bookmaker1.Odds[i].oddType;
                            }

                            if (bookmaker1.Odds[i].oddNoFav > 1 && bookmaker2.Odds[x].oddFav > 1.05m)
                            {
                                arbFound = true;
                                profitTeam1 = calculHdpProfit(bookmaker1.Odds[i].oddNoFav);
                                profitTeam2 = calculHdpProfit(bookmaker2.Odds[x].oddFav);

                                teamWinner1 = bookmaker1.TeamNoFav.TeamName;
                                teamWinner2 = bookmaker2.TeamFav.TeamName;
                                winOddIsFav1 = false;
                                winOddIsFav2 = true;
                                winOdd1 = bookmaker1.Odds[i].oddNoFav;
                                winOdd2 = bookmaker2.Odds[x].oddFav;
                                oddType = bookmaker1.Odds[i].oddType;
                            }

                            //if (canTest)
                            //{
                            //    arbFound = true;
                            //    profitTeam1 = calculHdpProfit(bookmaker1.Odds[i].oddFav);
                            //    profitTeam2 = calculHdpProfit(bookmaker2.Odds[x].oddNoFav);

                            //    teamWinner1 = bookmaker1.TeamFav.TeamName;
                            //    teamWinner2 = bookmaker2.TeamNoFav.TeamName;

                            //    winOddIsFav1 = true;
                            //    winOddIsFav2 = false;
                            //    winOdd1 = bookmaker1.Odds[i].oddFav;
                            //    winOdd2 = bookmaker2.Odds[x].oddNoFav;
                            //    oddType = bookmaker1.Odds[i].oddType;
                            //    casetype = "case 1";
                            //}

                        }

                        if (arbFound)
                        {
                            //string match = $"{teamWinner1} - {teamWinner2}";

                            string match = bookmaker1.MatchName;

                            NLogger.Log(EventLevel.Notice, $"HANDICAP Opportunity found with : {bMName1} - {bMName2}");
                            NLogger.Log(EventLevel.Notice, $"Match : {match}");
                            NLogger.Log(EventLevel.Notice, $"Odd type : {oddType}");
                            NLogger.Log(EventLevel.Notice, $"Odd 1 : {winOdd1}");
                            NLogger.Log(EventLevel.Notice, $"Odd 2 : {winOdd2}");
                            string arbGui = Common.generateArbId(bMName1, bMName2, teamWinner1, teamWinner2);
                            string matchGui = Common.generateMatchId(teamWinner1, teamWinner2);

                            bool macthBetted = false;
                            bool canPlaceBet = true;


                            // ++++++++++++++++
                            // Check if bet placed
                            // ++++++++++++++++

                            if (bookmakerHasBetMatch(bmI1, match))
                            {
                                canPlaceBet = false;
                                NLogger.Log(EventLevel.Critical, $"Match already bet on {bookmaker1}");
                            }

                            if (bookmakerHasBetMatch(bmI2, match))
                            {
                                canPlaceBet = false;
                                NLogger.Log(EventLevel.Critical, $"Match already bet on {bookmaker2}");
                            }

                            // ++++++++++++++++
                            // Check if opportunity exist
                            // ++++++++++++++++

                            bool bm1Hasopportunity = false;
                            bool bm2Hasopportunity = false;
                            if (bookmakerHasOpportunity(bmI1, match, bMName2))
                            {
                                bm1Hasopportunity = true;
                                bookmakerUpdateDurationOpportunity(bmI1, match, bMName2);
                            }
                            else
                            {
                                bookmakerAddOpportunity(bmI1, match, bMName2);
                            }

                            if (bookmakerHasOpportunity(bmI2, match, bMName1))
                            {
                                bm2Hasopportunity = true;
                                bookmakerUpdateDurationOpportunity(bmI2, match, bMName1);
                            }
                            else
                            {
                                bookmakerAddOpportunity(bmI2, match, bMName1);
                            }



                            // ++++++++++++++++
                            // Placing bet
                            // ++++++++++++++++

                            //canPlaceBet = false;

                            if (canPlaceBet)
                            {

                                //Task<bool> statusPlaceBet1 =  PlaceBet.placeBet(bMName1, match, oddType, winOdd1, winOddIsFav1);
                                //Task<bool> statusPlaceBet2 =  PlaceBet.placeBet(bMName2, match, oddType, winOdd2, winOddIsFav2);

                                BetResult statusPlaceBet2 = new BetResult();

                                // REPLACE LINE
                                BetResult statusPlaceBet1 = Football.placeBet(prm_Index_BM1, bMName1, match, oddType, winOdd1, winOddIsFav1, false);



                                NLogger.Log(EventLevel.Debug,JsonConvert.SerializeObject(statusPlaceBet1, Formatting.Indented));
                                try
                                {
                                    if (statusPlaceBet1.BetIsPlaced)
                                    {
                                        NLogger.Log(EventLevel.Debug, $"statusPlaceBet1.BetHdpSens : {statusPlaceBet1.BetHdpSens}");
                                        NLogger.Log(EventLevel.Warn, $"bet placed for {bMName1}");
                                        MainClass.maxBet1 = statusPlaceBet1.BetMaxBet; 

                                        // REPLACE LINE
                                        statusPlaceBet2 = Football.placeBet(prm_Index_BM2, bMName2, match, oddType, winOdd2, winOddIsFav2, false);




                                        NLogger.Log(EventLevel.Debug,JsonConvert.SerializeObject(statusPlaceBet2, Formatting.Indented));
                                        if (statusPlaceBet2.BetIsPlaced)
                                        {
                                            NLogger.Log(EventLevel.Debug, $"statusPlaceBet2.BetHdpSens : {statusPlaceBet2.BetHdpSens}");
                                            NLogger.Log(EventLevel.Warn, $"bet placed for {bMName2}");
                                            MainClass.maxBet2 = statusPlaceBet1.BetMaxBet;
                                        }
                                        else
                                        {
                                            NLogger.Log(EventLevel.Error, $"Error place bet for {bMName2} : {statusPlaceBet2.BetMessage}");
                                        }
                                    }
                                    else
                                    {
                                        NLogger.Log(EventLevel.Error, $"Error place bet for {bMName1} : {statusPlaceBet1.BetMessage}");
                                    }
                                }
                                catch(Exception e)
                                {
                                    NLogger.Log(EventLevel.Error,"MAIN PLACEMENT ERROR");
                                    //NLogger.LogError(e);

                                }

                                //await Task.WhenAll(statusPlaceBet1, statusPlaceBet2);

                                if (statusPlaceBet1.BetIsPlaced && statusPlaceBet2.BetIsPlaced)
                                {

                                    // ++++++++++++++++
                                    // Save Opportunity
                                    // ++++++++++++++++

                                    addOpportunities(
                                        bMName1,
                                        bMName2,
                                        "football",
                                        bookmaker1,
                                        bookmaker2,
                                        bookmaker1.Odds[i],
                                        bookmaker2.Odds[x],
                                        arbGui,
                                        profitTeam1,
                                        profitTeam2,
                                        matchGui,
                                        "0",
                                        "0"
                                    );


                                    canTest = false;
                                    NLogger.Log(EventLevel.Critical,"Both of bet are placed");

                                    // Check if odd are opposite

                                    if(checkTeamName(statusPlaceBet1.BetTeamFav, statusPlaceBet1.BetTeamNoFav, match) &&
                                       checkTeamName(statusPlaceBet2.BetTeamFav, statusPlaceBet2.BetTeamNoFav, match))
                                    {
                                        if (checkHdpSens(oddType, statusPlaceBet1.BetHdpSens, statusPlaceBet2.BetHdpSens))
                                        {

                                            // ++++++++++++++++++++++++++++++
                                            // Get Bet value and use the smalest value;
                                            // ++++++++++++++++++++++++++++++
                                            bool isTesting = checkIftestingBookmaker(bMName1, bMName2);

                                            // ++++++++++++++++++++++++++++++
                                            // Get Bet value and use the smalest value;
                                            // ++++++++++++++++++++++++++++++

                                            bool matchIsLive = false;
                                            if (bookmaker1.MatchIsLive || bookmaker2.MatchIsLive)
                                            {
                                                matchIsLive = true;
                                            }

                                            getBetPrice(isTesting, matchIsLive);



                                            NLogger.Log(EventLevel.Critical, "++++++++++++++++++++++++++");
                                            NLogger.Log(EventLevel.Critical, $"PRICE BET : {MainClass.betPriceStr}");
                                            NLogger.Log(EventLevel.Critical, "++++++++++++++++++++++++++");

                                            if (checkAccountBalance(prm_Index_BM1, prm_Index_BM2))
                                            {
                                                BetResult statusConfirmBet1 = new BetResult();
                                                BetResult statusConfirmBet2 = new BetResult();


                                                // +++++++++++++++++++++++++++++
                                                // Place over first ( need to figure out for HDP )
                                                // +++++++++++++++++++++++++++++
                                                if (winOddIsFav1)
                                                {
                                                    NLogger.Log(EventLevel.Critical, "We place Fav first");
                                                    statusConfirmBet1 = Football.confirmBet(prm_Index_BM1, bMName1);
                                                    if (statusConfirmBet1.BetIsConfirmed)
                                                    {
                                                        TelegramBot.sendBetAlert("success", bMName1, match, teamWinner1, oddType, winOdd1, MainClass.betPriceStr);
                                                        statusConfirmBet2 = Football.confirmBet(prm_Index_BM2, bMName2);
                                                        if (statusConfirmBet2.BetIsConfirmed)
                                                        {
                                                            TelegramBot.sendBetAlert("success", bMName2, match, teamWinner2, oddType, winOdd2, MainClass.betPriceStr);
                                                        }
                                                        else
                                                        {
                                                            TelegramBot.sendBetAlert("fail", bMName2, match, teamWinner2, oddType, winOdd2, MainClass.betPriceStr);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    NLogger.Log(EventLevel.Critical, "We place NoFav first");
                                                    statusConfirmBet2 = Football.confirmBet(prm_Index_BM2, bMName2);
                                                    if (statusConfirmBet2.BetIsConfirmed)
                                                    {
                                                        TelegramBot.sendBetAlert("success", bMName2, match, teamWinner2, oddType, winOdd2, MainClass.betPriceStr);
                                                        statusConfirmBet1 = Football.confirmBet(prm_Index_BM1, bMName1);
                                                        if (statusConfirmBet1.BetIsConfirmed)
                                                        {
                                                            TelegramBot.sendBetAlert("success", bMName1, match, teamWinner1, oddType, winOdd1, MainClass.betPriceStr);
                                                        }
                                                        else
                                                        {
                                                            TelegramBot.sendBetAlert("fail", bMName1, match, teamWinner1, oddType, winOdd1, MainClass.betPriceStr);
                                                        }
                                                    }

                                                }

                                                //await Task.WhenAll(statusConfirmBet1, statusConfirmBet2);

                                                int statusBet1 = 0;
                                                int statusBet2 = 0;

                                                if (statusConfirmBet1.BetIsConfirmed)
                                                {
                                                    NLogger.Log(EventLevel.Critical, $"bet Confirmed for {bMName1}");
                                                    statusBet1 = 1;
                                                    bookmakerAddMatchBetted(bmI1, match);
                                                    MainClass.BetPlacedList.Add(new BetPlaced()
                                                    {
                                                        ArbitrageGui = arbGui,
                                                        AccountName = account1,
                                                        MatchName = match,
                                                        BookmakerIndex = prm_Index_BM1,
                                                        BookmakerName = bMName1,
                                                        TeamFav = teamWinner1,
                                                        TeamNoFav = teamWinner2,
                                                        League = "",
                                                        BetFound = false,
                                                        BetReject = false,
                                                        BetConfirmed = false,
                                                        LastStatusSent = ""
                                                    });

                                                }
                                                else
                                                {
                                                    NLogger.Log(EventLevel.Error, $"Error confirm bet for {bMName1} : {statusPlaceBet1.BetMessage}");
                                                }
                                                if (statusConfirmBet2.BetIsConfirmed)
                                                {
                                                    NLogger.Log(EventLevel.Critical, $"bet Confirmed for {bMName2}");
                                                    statusBet2 = 1;
                                                    bookmakerAddMatchBetted(bmI2, match);
                                                    MainClass.BetPlacedList.Add(new BetPlaced()
                                                    {
                                                        ArbitrageGui = arbGui,
                                                        AccountName = account2,
                                                        MatchName = match,
                                                        BookmakerIndex = prm_Index_BM2,
                                                        BookmakerName = bMName2,
                                                        TeamFav = teamWinner1,
                                                        TeamNoFav = teamWinner2,
                                                        League = "",
                                                        BetFound = false,
                                                        BetReject = false,
                                                        BetConfirmed = false,
                                                        LastStatusSent = ""
                                                    });

                                                }
                                                else
                                                {
                                                    NLogger.Log(EventLevel.Error, $"Error confirm bet for {bMName2} : {statusPlaceBet2.BetMessage}");
                                                }

                                                if (statusConfirmBet1.BetIsConfirmed && statusConfirmBet2.BetIsConfirmed)
                                                {
                                                    Console.Beep(1500, 220);
                                                    Console.Beep(1500, 220);
                                                    NLogger.Log(EventLevel.Critical, "++++++++++++++++++++++++++");
                                                    NLogger.Log(EventLevel.Critical, "Arbitrage is successfull !");
                                                    NLogger.Log(EventLevel.Critical, "++++++++++++++++++++++++++");

                                                    addBets(
                                                        account1,
                                                        account2,
                                                        arbGui,
                                                        bMName1,
                                                        bMName2,
                                                        match,
                                                        teamWinner1,
                                                        teamWinner2,
                                                        oddType,
                                                        winOdd1,
                                                        winOdd2,
                                                        MainClass.betPriceDecimal,
                                                        statusBet1,
                                                        statusBet2,
                                                        bookmaker1.MatchTime
                                                    );

                                                }
                                                else if (statusConfirmBet1.BetIsConfirmed || statusConfirmBet2.BetIsConfirmed)

                                                {
                                                    Console.Beep(1000, 196);
                                                    Console.Beep(1000, 196);
                                                    Console.Beep(1000, 196);
                                                    NLogger.Log(EventLevel.Error, "++++++++++++++++++++++++++");
                                                    NLogger.Log(EventLevel.Error, " !!! ALERT !!! ");
                                                    NLogger.Log(EventLevel.Error, " Bet only place in one side ");
                                                    NLogger.Log(EventLevel.Error, "++++++++++++++++++++++++++");

                                                    addBets(
                                                        account1,
                                                        account2,
                                                        arbGui,
                                                        bMName1,
                                                        bMName2,
                                                        match,
                                                        teamWinner1,
                                                        teamWinner2,
                                                        oddType,
                                                        winOdd1,
                                                        winOdd2,
                                                        MainClass.betPriceDecimal,
                                                        statusBet1,
                                                        statusBet2,
                                                        bookmaker1.MatchTime
                                                    );
                                                }

                                                else
                                                {
                                                    //PlaceBet.backToFrame(bMName1);
                                                    //PlaceBet.backToFrame(bMName2);
                                                    Football.backToFrame(prm_Index_BM1);
                                                    Football.backToFrame(prm_Index_BM2);
                                                    Console.WriteLine("After place bet");
                                                }
                                            }
                                            else
                                            {
                                                NLogger.Log(EventLevel.Error, "Balance too small  : ");
                                            }
                                        }
                                        else
                                        {
                                            NLogger.Log(EventLevel.Error, "ERROR  SENS OF ODD DIFFERENT  : ");
                                        }
                                    }
                                    else
                                    {
                                        NLogger.Log(EventLevel.Error, "ERROR TEAM NAME DIFFERENT  : ");
                                    }
                                }
                                else
                                {
                                    //PlaceBet.backToFrame(bMName1);
                                    //PlaceBet.backToFrame(bMName2);
                                    Football.backToFrame(prm_Index_BM1);
                                    Football.backToFrame(prm_Index_BM2);
                                    Console.WriteLine("After place bet");
                                }

                            }
                        }
                    }
                }
            }
        }

        public static bool checkTeamName(string teamFav, string teamNoFav, string match)
        {

            teamFav = Common.cleanTeamName(teamFav);
            teamNoFav = Common.cleanTeamName(teamNoFav);

            if (match.Contains(teamFav) && match.Contains(teamNoFav))
            {
                NLogger.Log(EventLevel.Fatal, $"Check team name true");
                return true;
            }
            NLogger.Log(EventLevel.Fatal, $"Check team name false");
            return false;
        }

        public static bool checkIftestingBookmaker(string bookmakerName1, string bookmakerName2)
        {
            if(bookmakerName1 == "Nba369" ||
               bookmakerName2 == "Nba369" ||

               bookmakerName1 == "Va2888" ||
               bookmakerName2 == "Va2888" ||

               bookmakerName1 == "Va2888 AFB" ||
               bookmakerName2 == "Va2888 AFB"
               )
            {
                return true;
            }

            return false;
        }

        public static void getBetPrice(bool isTesting, bool isLive)
        {
            string betPriceStr = "";
            decimal basePrice = 0;

            if (isTesting)
            {
                if (isLive)
                {
                    betPriceStr = MainClass.betValueLiveTestingLive;
                    basePrice = MainClass.betValueDecimalTestingLive;
                }
                else
                {
                    betPriceStr = MainClass.betValueLiveTesting;
                    basePrice = MainClass.betValueDecimalTesting;
                }

                MainClass.betPriceDecimal = basePrice;
                MainClass.betPriceStr = betPriceStr;
            }
            else
            {

                if (MainClass.maxBet1 >= MainClass.maxBet2)
                {
                    basePrice = MainClass.maxBet2;
                }
                else
                {
                    basePrice = MainClass.maxBet1;
                }

                basePrice = decimal.Floor(basePrice);

                if (isLive)
                {
                    if (basePrice >= MainClass.betValueDecimalLive)
                    {
                        basePrice = MainClass.betValueDecimalLive;
                    }
                }
                else
                {
                    if (basePrice >= MainClass.betValueDecimal)
                    {
                        basePrice = MainClass.betValueDecimal;
                    }
                }

                betPriceStr = basePrice.ToString("0.00").Replace(".00", String.Empty);
            }

            MainClass.betPriceDecimal = basePrice;
            MainClass.betPriceStr = betPriceStr;
        }

        public static void addOpportunities
        (

            string arbBookmaker1,
            string arbBookmaker2,
            string arbGame,
            Match bookmaker1,
            Match bookmaker2,
            Odd odd1,
            Odd odd2,
            string arbGui,
            decimal arbProfitTeamFav,
            decimal arbProfitTeamNoFav,
            string matchId,
            string errorStatus1,
            string errorStatus2
            )
        {

            OpportunitiesApi opportunityApi = new OpportunitiesApi
            {
                ArbBookmaker1 = arbBookmaker1,
                ArbBookmaker2 = arbBookmaker2,
                ArbDuration = 0,
                ArbGame = arbGame,
                ArbType = odd1.oddType,
                ArbGui = arbGui,
                ArbPotentialProfitTeamFav = arbProfitTeamFav,
                ArbPotentialProfitTeamNoFav = arbProfitTeamNoFav,
                ErrorStatus1 = errorStatus1,
                ErrorStatus2 = errorStatus2,

                ArbMatch = new Models.MatchApi
                {
                    MatchGui = matchId,
                    MatchTeamFav = bookmaker1.TeamFav.TeamName,
                    MatchTeamNoFav = bookmaker1.TeamNoFav.TeamName,
                    MatchLeague = bookmaker1.Matchleague,
                    MatchDate = "no date"

                },
                OddBookmaker1 = new Models.OddApi
                {
                    OddFav = odd1.oddFav,
                    OddNoFav = odd1.oddNoFav
                },
                OddBookmaker2 = new Models.OddApi
                {
                    OddFav = odd2.oddFav,
                    OddNoFav = odd2.oddNoFav
                }
            };

            MainClass.DataApi.ListOpportunities.Add(opportunityApi);

        }

        public static void addBets(
            string account1,
            string account2,
            string arbitrageGui,
            string bookmaker1,
            string bookmaker2,
            string match,
            string team1,
            string team2,
            string typeOdd,
            decimal odd1,
            decimal odd2,
            decimal price,
            int status1,
            int status2,
            string matchTime
            )
        {
            BetsApi betsApi = new BetsApi
            {
                Bet1 = new Models.BetApi
                {
                    BetAccount = account1,
                    BetBoomaker = bookmaker1,
                    BetMatch = match,
                    BetTeam = team1,
                    BetTypeOdd = typeOdd,
                    BetOdd = odd1,
                    BetPrice = price,
                    BetStatus = status1

                },

                Bet2 = new Models.BetApi
                {
                    BetAccount = account2,
                    BetBoomaker = bookmaker2,
                    BetMatch = match,
                    BetTeam = team2,
                    BetTypeOdd = typeOdd,
                    BetOdd = odd2,
                    BetPrice = price,
                    BetStatus = status2

                },

                ArbitrageGui = arbitrageGui,
                MatchTime = matchTime
            };

            MainClass.DataApi.ListBets.Add(betsApi);


            try
            {
                Task<string> resultAccounts = BackendArbApi.sendOpportunity(betsApi);
            }
            catch(Exception e)
            {
                NLogger.Log(EventLevel.Error, "ERROR  sendOpportunity  : ");
                NLogger.LogError(e);
            }
        }

        public static bool DataApiHasOpportunity(int i, string match, string secondBookmakerName)
        {
            foreach (OpportunitiesApi opp in MainClass.DataApi.ListOpportunities)
            {
                if (opp.ArbMatch.MatchName == match && opp.ArbBookmaker1 == secondBookmakerName)
                {
                    return true;
                }
            }
            return false;
        }

        public static void DataApiUpdateDuration(string match, string bm1, string bm2){
            foreach (OpportunitiesApi opp in MainClass.DataApi.ListOpportunities)
            {
                if (opp.ArbMatch.MatchName == match && opp.ArbBookmaker1 == bm1 && opp.ArbBookmaker1 == bm2)
                {
                    int currentTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    int newDuration = currentTimestamp - opp.ArbDuration;
                    opp.ArbDuration = newDuration;
                }
            }

        }


        public static decimal calculHdpProfit(decimal odd)
        {
            decimal result = (BetValue * odd) - BetValue;
            return result;
        }

        public static bool checkHdpSens(string oddType, string BetHdpSens1, string BetHdpSens2 )
        {
            if (!Common.isOverUnder(oddType))
            {
                if(oddType.Contains("ft 0") || oddType.Contains("fh 0"))
                {
                    if(BetHdpSens1 == "0" && BetHdpSens2 == "0")
                    {
                        NLogger.Log(EventLevel.Fatal, $"checkHdpSens true for 0");
                        return true;
                    }
                    else
                    {
                        NLogger.Log(EventLevel.Fatal, $"checkHdpSens false for 0");
                        return false;
                    }
                    
                }

                if (
                    (BetHdpSens1 == "+" && BetHdpSens2 == "-") ||
                    (BetHdpSens1 == "-" && BetHdpSens2 == "+")
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public static bool bookmakerHasBetMatch(int i, string match)
        {
            foreach (string str in MainClass.bookmakers[i].BookmakerMatchBetted)
            {
                if (match.Contains(str))
                {
                    return true;
                }
            }
            return false;
        }

        public static void bookmakerAddMatchBetted(int i, string match)
        {
            MainClass.bookmakers[i].BookmakerMatchBetted.Add(match);
        }

        public static bool bookmakerHasOpportunity(int i, string match, string secondBookmakerName)
        {
            foreach (Opportunity opp in MainClass.bookmakers[i].BookmakerOpportunities)
            {
                if (opp.SecondBookmakerName == secondBookmakerName && opp.MatchName == match)
                {
                    return true;
                }
            }
            return false;
        }

        public static void bookmakerAddOpportunity(int i, string match, string secondBookmakerName)
        {
            MainClass.bookmakers[i].BookmakerOpportunities.Add(new Opportunity()
            {
                SecondBookmakerName = secondBookmakerName,
                MatchName = match,
                Duration = 0,
                Timestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
                HasSentApi = false
            });
        }

        public static void bookmakerUpdateDurationOpportunity(int i, string match, string secondBookmakerName)
        {
            foreach (Opportunity opp in MainClass.bookmakers[i].BookmakerOpportunities)
            {
                if (opp.SecondBookmakerName == secondBookmakerName && opp.MatchName == match)
                {
                    int currentTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    int newDuration = currentTimestamp - opp.Timestamp;
                    opp.Duration = newDuration;
                    opp.HasSentApi = false;
                }
            }
        }

        public static bool checkAccountBalance(int index_bm1, int index_bm2)
        {
            if(MainClass.bookmakers[index_bm1].ActiveAccount.AccountInfo.AccountBalance < MainClass.betPriceDecimal ||
               MainClass.bookmakers[index_bm2].ActiveAccount.AccountInfo.AccountBalance < MainClass.betPriceDecimal)
            {
                return false;
            }

            return true;
        }

    }
}
