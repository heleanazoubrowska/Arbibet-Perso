using System;
using System.Threading.Tasks;
using ArbibetProgram.Functions;
using ArbibetProgram.Models;

namespace ArbibetProgram
{
    public static partial class AccountMethods
    {
        public static void GetBalance(int gs_NumBookmakers)
        {
            Parallel.For(0, gs_NumBookmakers, MainClass.gs_ParallelOptions, (Counter) =>
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var prv_Bookmaker = MainClass.bookmakers[Counter];
                prv_Bookmaker.BookmakerMatches.Clear();

                try
                {
                    decimal balance = prv_Bookmaker.ActiveAccount.API.getBalance();
                    prv_Bookmaker.ActiveAccount.AccountInfo.AccountBalance = balance;

                    AccountInfoApi AccountInfo = new AccountInfoApi
                    {
                        Accountname = prv_Bookmaker.ActiveAccount.AccountInfo.AccountName,
                        AccountBalance = balance
                    };

                    MainClass.DataApi.ListAccountInfoApi.Add(AccountInfo);

                    NLogger.Log(EventLevel.Info, $"Balance for {prv_Bookmaker.BookmakerName} : {balance} $");

                }
                catch (Exception e)
                {
                    NLogger.Log(EventLevel.Error, $"{prv_Bookmaker.BookmakerName} Error get balance : {e}");
                }

                watch.Stop();
                NLogger.Log(EventLevel.Info, $"Get Balance {prv_Bookmaker.BookmakerName} in {watch.ElapsedMilliseconds} ms with : {prv_Bookmaker.BookmakerMatches.Count} match");
            });

            //NLogger.Log(EventLevel.Notice, $"Check {NbMatchCheck} Matches and {NbOddCheck} odds");

        }
    }
}
