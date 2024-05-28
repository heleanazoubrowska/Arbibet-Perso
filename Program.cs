using System;
using System.Collections.Generic;

using ArbibetProgram.Models;
using ArbibetProgram.Crawling;
using ArbibetProgram.Strategy;
using ArbibetProgram.Functions;
using ArbibetProgram.Telegram;

using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using ArbibetProgram.ApiRest;
using Newtonsoft.Json.Linq;

using Microsoft.Owin.Hosting;
using ArbibetProgram.SignalR;
using System.Collections.Concurrent;
using NLog.Internal;

using System.Collections.Specialized;
using OpenQA.Selenium;

namespace ArbibetProgram
{
    class MainClass
    {
        // ------------------------------------------------------
        #region Global Static Variables
        // ------------------------------------------------------

        public static bool noLiveMatch =  false;
        public static decimal minOddRequired = 1.05m;
        public static decimal minArbTarget = 2.1m;

        public static bool sendTelegram = true;


        public static string betValue = "100";
        public static decimal betValueDecimal = 100;

        public static string betValueLive = "100";
        public static decimal betValueDecimalLive = 100;

        public static string betValueLiveTesting = "100";
        public static decimal betValueDecimalTesting = 100;

        public static string betValueLiveTestingLive = "100";
        public static decimal betValueDecimalTestingLive = 100;



        public static ConcurrentDictionary<string, Game> gs_Games = new ConcurrentDictionary<string, Game>();
        public static object gs_GamesLocker = new object();

        public static List<Bookmaker> bookmakers = new List<Bookmaker>();
        public static int gs_NumBookmakers;
        public static List<AccountData> listAccounts;
        public static List<BetPlaced> BetPlacedList { get; set; } = new List<BetPlaced>();
        
        public static int timeCheckApi = 60;
        public static DataApi DataApi = new DataApi();
        public static ReportApi ReportApi = new ReportApi();

        public static string betPriceStr { get; set; }
        public static decimal betPriceDecimal { get; set; }

        //public static ConcurrentDictionary<string, Casino> gs_Casinos = new ConcurrentDictionary<string, Casino>();
        public static int nbCheckAccount { get; set; }  = 0;

        public static int NbMatchCheck = 0;
        public static int NbOddCheck = 0;

        public static decimal maxBet1 { get; set; }
        public static decimal maxBet2 { get; set; }

        private static bool prv_ExitFlag = false;

        public static bool gs_DemoMode = false;

        public static ParallelOptions gs_ParallelOptions = new ParallelOptions()

        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };
        // ------------------------------------------------------
		#endregion Global Static Variables
		// ------------------------------------------------------

        public static void Main()
        {
	        var prv_DemoMode = System.Configuration.ConfigurationManager.AppSettings.Get("DemoMode");
	        gs_DemoMode = (prv_DemoMode == "1"); 

            NLogger.SetupLogger();
            NLogger.Log(EventLevel.Warn, "ArbiBet started!");

            SignalR_Client.SetupSignalR();
            AccountMethods.SetupAccounts();
            SetupRecentBets();
            AccountMethods.TurnOnAccounts(listAccounts, false);

            //NLogger.Log(EventLevel.Info, JsonConvert.SerializeObject(bookmakers, Formatting.Indented));

            //sendReport();

            NbMatchCheck = 0;
            NbOddCheck = 0;

            NLogger.Log(EventLevel.Info, $"Check {NbMatchCheck} Matches and {NbOddCheck} odds");
            Thread.Sleep(5000);

            // ----------------------------------------------------------------------------
            // run arb execution loop in a separate thread while main thread waits for exit
            // ----------------------------------------------------------------------------
            Task.Run(startLoop);

            // -----------------------------------------
            #region graceful exit
            // -----------------------------------------
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            prv_ExitFlag = true;    // startLoop will begin exit procedure after finishing the current round

            while (Console.KeyAvailable)
            {
                Console.ReadKey(false);
            }

            // gracefully stop SignalR server

            //prv_SignalR.Dispose();

            // cause the startLoop thread to complete current loop and terminate
            
            Console.WriteLine("Please wait for all current loop to complete [or press Enter 3 times for immediate emergency exit]");

            Console.ReadLine();
            Console.Write("Exit 1/3");

            Console.ReadLine();
            Console.Write("Exit 2/3");

            Console.ReadLine();
            Console.WriteLine("Exit 3/3");

            // before emergency exit, close all casino browser windows
            Parallel.For(0, gs_NumBookmakers, gs_ParallelOptions, (prv_Iteration) =>
            {
	            try
	            {
		            bookmakers[prv_Iteration].ActiveAccount.API.quitBrowser();
                }
                catch
	            {

	            }
            });

            NLogger.Log(EventLevel.Warn, "ArbiBetProgram emergency exit");
            Environment.Exit(0);
            // -----------------------------------------
            #endregion graceful exit
            // -----------------------------------------
        }

        // -----------------------------------------
        #region Setup Functions
        // -----------------------------------------


        /// <summary>
        /// Check if Recent bets have been placed
        /// </summary>
        private static void SetupRecentBets()
        {
	        List<ResultBets> prv_RecentBets = BackendArbApi.getLastBets();

	        foreach (ResultBets prv_BetResult in prv_RecentBets)
	        {

                //bool BetFound = (prv_BetResult.BetFound == "1");
                //bool BetWaiting = (prv_BetResult.BetWaiting == "1");
                //bool BetReject = (prv_BetResult.BetRejected == "1");
                //bool BetConfirmed = (prv_BetResult.BetConfirmed == "1");

                //BetPlacedList.Add(new BetPlaced()
                //{
                //    AccountName = prv_BetResult.AccountName,
                //    MatchName = prv_BetResult.BetMatch,
                //    BookmakerName = prv_BetResult.BookmakerName,
                //    BetFound = BetFound,
                //    BetReject = BetReject,
                //    BetConfirmed = BetConfirmed,
                //});

                int prv_Index = bookmakers.FindIndex(prv_Item => prv_Item.BookmakerName == prv_BetResult.BookmakerName);
		        if (prv_Index > -1) // -1 = not found, 0 ~ N = position in list
		        {
			        bookmakers[prv_Index].BookmakerMatchBetted.Add(prv_BetResult.BetMatch);
		        }
	        }
        }

        

        
        // -----------------------------------------
        #endregion Setup Functions
        // -----------------------------------------

        // -----------------------------------------
        #region Arbitrage Execution
        // -----------------------------------------
        public static void startLoop()
        {
	        int fDate = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            while (!prv_ExitFlag)   // loop while true [until prv_ExitFlag set to true in Main above]
            {
		        var watch = System.Diagnostics.Stopwatch.StartNew();

                // ------------------------------------------------
                // key execution method; formerly named updateOdds
                // ------------------------------------------------
                Execute();

                watch.Stop();
		        NLogger.Log(EventLevel.Debug, $"Check {NbMatchCheck} Matches and {NbOddCheck} odds in {watch.ElapsedMilliseconds} ms");

                // ------------------------------------------------------------
                // call AccountMethods.checkAccounts every timeCheckApi seconds
                // ------------------------------------------------------------
                int nDate = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		        if (nDate - fDate > timeCheckApi)
		        {
			        fDate = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    AccountMethods.GetBalance(gs_NumBookmakers);

                    if (AccountMethods.checkAccounts())
			        {
				        DataApi = new DataApi();
			        }
			        else
			        {
				        NLogger.Log(EventLevel.Error, "Error while getting CheckAccount");
			        }

                }
	        }

            // before exiting, close all casino browser windows
            Parallel.For(0, gs_NumBookmakers, gs_ParallelOptions, (prv_Iteration) =>
            {
	            try
	            {
		            bookmakers[prv_Iteration].ActiveAccount.API.quitBrowser();
	            }
	            catch
	            {
	            }
            });

            NLogger.Log(EventLevel.Warn, "ArbiBetProgram exit");
	        Environment.Exit(0);
        }


        public static void Execute()
        {
            NbMatchCheck = 0;   // reset the count of matches checked
            NbOddCheck = 0;     // reset the count of odds checked

            // --------------------------------------------------
            // update matches and odds of all casinos in parallel
            // --------------------------------------------------
            Parallel.For(0, gs_NumBookmakers, gs_ParallelOptions, (Counter) =>
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var prv_Bookmaker = bookmakers[Counter];
                prv_Bookmaker.BookmakerMatches.Clear();

                try
                {
                    prv_Bookmaker.ActiveAccount.API.udpateOdd(prv_Bookmaker, false);
                }
                catch (Exception e)
                {
                    NLogger.Log(EventLevel.Error, $"{prv_Bookmaker.BookmakerName} Error update : {e}");
                }

				watch.Stop();
                NLogger.Log(EventLevel.Info, $"Update {prv_Bookmaker.BookmakerName} in {watch.ElapsedMilliseconds} ms with : {prv_Bookmaker.BookmakerMatches.Count} match");

                //Console.WriteLine(JsonConvert.SerializeObject(bookmakers, Formatting.Indented));

            });

            // --------------------------------------------------------------------
            // check the dictionary for conflicts [same teams in different leagues]
            // --------------------------------------------------------------------
            var prv_Conflicts_Flag = Game.CheckDictionary();
            if (prv_Conflicts_Flag)
            {
	            throw new NotFoundException();
	            Environment.Exit(-42);
            }

            checkBetsPlaced();

            if (!gs_DemoMode)
            {
	            Football.CheckForArbitrage();
            }

            Game.ResetDictionary(); // turn off active flag, clear home & away teams
        }

        public static void checkBetsPlaced()
        {
            foreach (BetPlaced prv_Bet in BetPlacedList)
            {
                if (!prv_Bet.BetFound)
                {
                    CasinoBetStatus result = new CasinoBetStatus();
                    string newStatus = "";

                    try
                    {
                        result = Football.GetBetStatus(prv_Bet.BookmakerIndex, prv_Bet.BookmakerName, prv_Bet.MatchName);

                        switch (result.NewStatus)
                        {
                            case "running":
                                prv_Bet.BetFound = true;
                                prv_Bet.BetConfirmed = true;
                                NLogger.Log(EventLevel.Warn, $"Bet is Found on {prv_Bet.BookmakerName} for match : {prv_Bet.MatchName}");

                                // ++++++++++++++++++++++++++++++++
                                // If not sent yet, we update the bet status on back end
                                // ++++++++++++++++++++++++++++++++
                                if (prv_Bet.LastStatusSent != "running")
                                {
                                    newStatus = "Running";
                                    prv_Bet.LastStatusSent = "running";
                                }

                                break;

                            case "waiting":
                                //prv_Bet.BetFound = true;
                                prv_Bet.BetWaiting = true;
                                NLogger.Log(EventLevel.Warn, "!!!!!!!!!");
                                NLogger.Log(EventLevel.Warn, $"WAITING Bet on {prv_Bet.BookmakerName} for match : {prv_Bet.MatchName}");
                                NLogger.Log(EventLevel.Warn, "!!!!!!!!!");

                                if (prv_Bet.LastStatusSent != "waiting")
                                {
                                    newStatus = "Waiting";
                                    prv_Bet.LastStatusSent = "waiting";
                                }

                                break;

                            case "void":
                                prv_Bet.BetFound = false;
                                prv_Bet.BetReject = true;
                                NLogger.Log(EventLevel.Warn, "!!!!!!!!!");
                                NLogger.Log(EventLevel.Warn, $"REJECT / ERROR / NOT FOUND Bet on {prv_Bet.BookmakerName} for match : {prv_Bet.MatchName}");
                                NLogger.Log(EventLevel.Warn, "!!!!!!!!!");

                                if (prv_Bet.LastStatusSent != "void")
                                {
                                    newStatus = "Void";
                                    prv_Bet.LastStatusSent = "void";
                                }

                                break;

                            case "not found":
                                prv_Bet.BetFound = false;
                                prv_Bet.BetReject = true;
                                NLogger.Log(EventLevel.Warn, "!!!!!!!!!");
                                NLogger.Log(EventLevel.Warn, $"NOT FOUND Bet on {prv_Bet.BookmakerName} for match : {prv_Bet.MatchName}");
                                NLogger.Log(EventLevel.Warn, "!!!!!!!!!");

                                if (prv_Bet.LastStatusSent != "not found")
                                {
                                    newStatus = "Not found";
                                    prv_Bet.LastStatusSent = "not found";
                                }

                                break;

                        }

                        // ++++++++++++++++++++++++++++++++
                        // If not sent yet, we update the bet status on back end
                        // ++++++++++++++++++++++++++++++++
                        if (newStatus != "")
                        {
                            BetStatus betStatus = new BetStatus();

                            betStatus.AribtrageGui = prv_Bet.ArbitrageGui;
                            //betStatus.AribtrageGui = "guiteteteetest30";

                            betStatus.BetNewStatus = newStatus;
                            betStatus.ActualOdd = result.ActualOdd;
                            betStatus.BookmakerName = prv_Bet.BookmakerName;

                            NLogger.Log(EventLevel.Warn, $"New Bet Status to send : {newStatus}");

                            try
                            {
                                Task<string> resultReport = BackendArbApi.sendBetStatus(betStatus);
                            }
                            catch
                            {
                                NLogger.Log(EventLevel.Error, "Error while sending BET STATUS");
                            }


                        }
                        NLogger.Log(EventLevel.Info, $"result checkbetPlaced {result.NewStatus}");
                    }
                    catch(Exception e)
                    {
                        NLogger.Log(EventLevel.Error, $"error find bet on : {prv_Bet.BookmakerName} for : {prv_Bet.MatchName}");
                        NLogger.Log(EventLevel.Error, $"Error : {e}");
                        //result.NewStatus = $"error find bet on : {prv_Bet.BookmakerName} for : {prv_Bet.MatchName}";
                    }
                }
            }
        }


        // -----------------------------------------
        #endregion Arbitrage Execution
        // -----------------------------------------

        // -----------------------------------------
        #region Misc
        // -----------------------------------------
        public static void sendReport()
        {
            // +++++++++++++++++++++++++++
            // REPORT
            // +++++++++++++++++++++++++++
            ReportApi ReportApi = new ReportApi();

            ////Parallel.For(0, gs_NumBookmakers, gs_ParallelOptions, (iReport) =>
            //for (int iReport = 0; iReport < gs_NumBookmakers; iReport++)
            //{
            //    var prv_Bookmaker = bookmakers[iReport];
            //     prv_Bookmaker.ActiveAccount.API.quitBrowser();
            //    //for (int AccountCounter = 0; AccountCounter < bookmakers[iReport].Accounts.Count; AccountCounter++)
            //    for (int AccountCounter = 0; AccountCounter < prv_Bookmaker.Accounts.Count; AccountCounter++)
            //    {
            //        bookmakers[iReport].Accounts[AccountCounter].API.Report();
            //    }
            //}
            ////});


            Console.WriteLine("gs_NumBookmakers : "+ gs_NumBookmakers);

            for (int iReport = 0; iReport < gs_NumBookmakers; iReport++)
            {
                var prv_Bookmaker = bookmakers[iReport];

                bookmakers[iReport].ActiveAccount.API.Report();
            }
            Console.WriteLine("Here we are, just after report");
            

            try
            {
                Task<string> resultReport = BackendArbApi.sendReport();
                Console.WriteLine(resultReport);
            }
            catch (Exception e)
            {
                NLogger.Log(EventLevel.Error, "ERROR  sendReport  : ");
                //NLogger.LogError(e);
            }

            
        }
        // -----------------------------------------
        #endregion Misc
        // -----------------------------------------

    }
}
