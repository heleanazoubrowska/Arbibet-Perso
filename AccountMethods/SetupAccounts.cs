using ArbibetProgram.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArbibetProgram.ApiRest;
using ArbibetProgram.Models;
using Newtonsoft.Json;

namespace ArbibetProgram
{
	public static partial class AccountMethods
	{
        public static void SetupAccounts()
        {
            // ----------------------------------------------------------------------
            // Get Account List
            // ----------------------------------------------------------------------
            MainClass.listAccounts = BackendArbApi.getAccounts(MainClass.gs_DemoMode ? "1" : "0");

            // ----------------------------------------------------------------------
            // first create Bookmaker instances used for polling odds
            // we only add a bookmaker once, if the account is active
            // ----------------------------------------------------------------------
            int prv_BookmakerIndex = 0;

            foreach (AccountData prv_AccountData in MainClass.listAccounts)
            {
                if (prv_AccountData.AccountActive == 0 || prv_AccountData.AccountBookmakerActive == 0)
                {
                    Console.WriteLine("case 1 : " + prv_AccountData.AccountName);
                    continue;
                }

                int prv_Index = MainClass.bookmakers.FindIndex(prv_Item => prv_Item.BookmakerName == prv_AccountData.AccountBookmakerName);
                if (prv_Index > -1) // we've added the bookmaker already
                {
                    Console.WriteLine("case 2 : " + prv_AccountData.AccountName);
                    continue;
                }

                
                if(prv_AccountData.AccountBookmakerName != "")
                {
                    MainClass.bookmakers.Add(new Bookmaker()
                    {
                        BookmakerName = prv_AccountData.AccountBookmakerName,
                        BookmakerIndex = prv_BookmakerIndex++,
                        BookmakerUrl = prv_AccountData.AccountBookmakerUrl,
                        BookmakerActive = true,
                        BookmakerAccountActive = true,
                        BookmakerUser = prv_AccountData.AccountUsername,
                        BookmakerPass = prv_AccountData.AccountPassword,
                        BookmakerAccountBalance = prv_AccountData.AccountBalance,
                        BookmakerShowPage = false
                    });
                }

            }
            MainClass.gs_NumBookmakers = MainClass.bookmakers.Count;    // keep the number of bookmakers in a global variable for convenience
        }
    }
}
