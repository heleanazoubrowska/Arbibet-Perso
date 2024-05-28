using System;
using System.Collections.Generic;
using ArbibetProgram.Functions;
using ArbibetProgram.Models;

namespace ArbibetProgram
{
    public static partial class AccountMethods
    {
        public static void removeAccount(List<AccountData> prm_AccountsToRemove)
        {
            NLogger.Log(EventLevel.Critical,"REMOVE ACCOUNT");
           
            foreach (AccountData prv_AccountData in prm_AccountsToRemove)
            {
                int iR = 0;
                int prv_NumAccounts;

                Bookmaker prv_Bookmaker = new Bookmaker();

                for (var i = 0; i < MainClass.gs_NumBookmakers; i++)
                {
	                if (prv_AccountData.AccountBookmakerName == MainClass.bookmakers[i].BookmakerName)
	                {
		                prv_Bookmaker = MainClass.bookmakers[i];
		                break;
	                }
                }

                int prv_Index = prv_Bookmaker.Accounts.FindIndex(prv_Account => prv_Account.AccountInfo.AccountName == prv_AccountData.AccountUsername);
                prv_Bookmaker.Accounts.RemoveAt(prv_Index);

                /*
                switch (account.AccountBookmakerName)
                {
                    case "Cambo88 UG":
                        prv_NumAccounts = MainClass.Cambo88UG_Accounts.Count;
                        for (var i = 0; i < prv_NumAccounts; i++)
                        {
                            if (MainClass.Cambo88UG_Accounts[i].AccountInfo.AccountUsername == account.AccountUsername)
                            {
                                iR = i;
                                MainClass.Cambo88UG_Accounts[i].Cambo88UG.quitBrowser();
                                break;
                            }
                        }
                        MainClass.Cambo88UG_Accounts.RemoveAt(iR);
                        
                        if (MainClass.Cambo88UG_Accounts.Count == 0)
                        {
                            int bmR = 0;
                            for (var j = 0; j < MainClass.bookmakers.Count; j++)
                            {
                                if (MainClass.bookmakers[j].BookmakerName == "Cambo88 UG")
                                {
                                    bmR = j;
                                    break;
                                }
                            }
                            MainClass.bookmakers.RemoveAt(bmR);
                        }
                        break;

                    case "Cambo88 BTI":
                        prv_NumAccounts = MainClass.Cambo88BTI_Accounts.Count;
                        for (var i = 0; i < prv_NumAccounts; i++)
                        {
                            if (MainClass.Cambo88BTI_Accounts[i].AccountInfo.AccountUsername == account.AccountUsername)
                            {
                                iR = i;
                                MainClass.Cambo88BTI_Accounts[i].Cambo88BTI.quitBrowser();
                                break;
                            }
                        }
                        MainClass.Cambo88BTI_Accounts.RemoveAt(iR);
                        if (MainClass.Cambo88BTI_Accounts.Count == 0)
                        {
                            int bmR = 0;
                            for (var j = 0; j < MainClass.bookmakers.Count; j++)
                            {
                                if (MainClass.bookmakers[j].BookmakerName == "Cambo88 BTI")
                                {
                                    bmR = j;
                                    break;
                                }
                            }
                            MainClass.bookmakers.RemoveAt(bmR);
                        }
                        break;

                    case "Cambo88 SBO":
                        prv_NumAccounts = MainClass.Cambo88SBO_Accounts.Count;
                        for (var i = 0; i < prv_NumAccounts; i++)
                        {
                            if (MainClass.Cambo88SBO_Accounts[i].AccountInfo.AccountUsername == account.AccountUsername)
                            {
                                iR = i;
                                MainClass.Cambo88SBO_Accounts[i].Cambo88SBO.quitBrowser();
                                break;
                            }
                        }
                        MainClass.Cambo88SBO_Accounts.RemoveAt(iR);
                        if (MainClass.Cambo88SBO_Accounts.Count == 0)
                        {
                            int bmR = 0;
                            for (var j = 0; j < MainClass.bookmakers.Count; j++)
                            {
                                if (MainClass.bookmakers[j].BookmakerName == "Cambo88 SBO")
                                {
                                    bmR = j;
                                    break;
                                }
                            }
                            MainClass.bookmakers.RemoveAt(bmR);
                        }
                        break;

                    case "Aa2888":
                        prv_NumAccounts = MainClass.Cambo88BTI_Accounts.Count;
                        for (var i = 0; i < prv_NumAccounts; i++)
                        {
                            if (MainClass.Aa2888_Accounts[i].AccountInfo.AccountUsername == account.AccountUsername)
                            {
                                iR = i;
                                MainClass.Aa2888_Accounts[i].Aa2888.quitBrowser();
                                break;
                            }
                        }
                        MainClass.Aa2888_Accounts.RemoveAt(iR);
                        if (MainClass.Aa2888_Accounts.Count == 0)
                        {
                            int bmR = 0;
                            for (var j = 0; j < MainClass.bookmakers.Count; j++)
                            {
                                if (MainClass.bookmakers[j].BookmakerName == "Cambo88 Aa2888")
                                {
                                    bmR = j;
                                    break;
                                }
                            }
                            MainClass.bookmakers.RemoveAt(bmR);
                        }
                        break;


                    case "855bet":
                        prv_NumAccounts = MainClass.B855bet_Accounts.Count;
                        for (var i = 0; i < prv_NumAccounts; i++)
                        {
                            if (MainClass.B855bet_Accounts[i].AccountInfo.AccountUsername == account.AccountUsername)
                            {
                                iR = i;
                                MainClass.B855bet_Accounts[i].B855bet.quitBrowser();
                                break;
                            }
                        }
                        MainClass.B855bet_Accounts.RemoveAt(iR);
                        if (MainClass.B855bet_Accounts.Count == 0)
                        {
                            int bmR = 0;
                            for (var j = 0; j < MainClass.bookmakers.Count; j++)
                            {
                                if (MainClass.bookmakers[j].BookmakerName == "855bet")
                                {
                                    bmR = j;
                                    break;
                                }
                            }
                            MainClass.bookmakers.RemoveAt(bmR);
                        }
                        break;

                    case "Va2888":
                        prv_NumAccounts = MainClass.Va2888_Accounts.Count;
                        for (var i = 0; i < prv_NumAccounts; i++)
                        {
                            if (MainClass.Va2888_Accounts[i].AccountInfo.AccountUsername == account.AccountUsername)
                            {
                                iR = i;
                                MainClass.Va2888_Accounts[i].Va2888.quitBrowser();
                                break;
                            }
                        }
                        MainClass.Va2888_Accounts.RemoveAt(iR);
                        if (MainClass.Va2888_Accounts.Count == 0)
                        {
                            int bmR = 0;
                            for (var j = 0; j < MainClass.bookmakers.Count; j++)
                            {
                                if (MainClass.bookmakers[j].BookmakerName == "Va2888")
                                {
                                    bmR = j;
                                    break;
                                }
                            }
                            MainClass.bookmakers.RemoveAt(bmR);
                        }
                        break;

                    case "Va2888AFB":
                        prv_NumAccounts = MainClass.Va2888AFB_Accounts.Count;
                        for (var i = 0; i < prv_NumAccounts; i++)
                        {
                            if (MainClass.Va2888AFB_Accounts[i].AccountInfo.AccountUsername == account.AccountUsername)
                            {
                                iR = i;
                                MainClass.Va2888AFB_Accounts[i].Va2888AFB.quitBrowser();
                                break;
                            }
                        }
                        MainClass.Va2888AFB_Accounts.RemoveAt(iR);
                        if (MainClass.Va2888AFB_Accounts.Count == 0)
                        {
                            int bmR = 0;
                            for (var j = 0; j < MainClass.bookmakers.Count; j++)
                            {
                                if (MainClass.bookmakers[j].BookmakerName == "Va2888AFB")
                                {
                                    bmR = j;
                                    break;
                                }
                            }
                            MainClass.bookmakers.RemoveAt(bmR);
                        }
                        break;

                    case "Nba369":
                        prv_NumAccounts = MainClass.Nba369_Accounts.Count;
                        for (var i = 0; i < prv_NumAccounts; i++)
                        {
                            if (MainClass.Nba369_Accounts[i].AccountInfo.AccountUsername == account.AccountUsername)
                            {
                                iR = i;
                                MainClass.Nba369_Accounts[i].Nba369.quitBrowser();
                                break;
                            }
                        }
                        MainClass.Nba369_Accounts.RemoveAt(iR);
                        if (MainClass.Nba369_Accounts.Count == 0)
                        {
                            int bmR = 0;
                            for (var j = 0; j < MainClass.bookmakers.Count; j++)
                            {
                                if (MainClass.bookmakers[j].BookmakerName == "Nba369")
                                {
                                    bmR = j;
                                    break;
                                }
                            }
                            MainClass.bookmakers.RemoveAt(bmR);
                        }
                        break;

                    case "Ibc05":
                        prv_NumAccounts = MainClass.Ibc05_Accounts.Count;
                        for (var i = 0; i < prv_NumAccounts; i++)
                        {
                            if (MainClass.Ibc05_Accounts[i].AccountInfo.AccountUsername == account.AccountUsername)
                            {
                                iR = i;
                                MainClass.Ibc05_Accounts[i].Ibc05.quitBrowser();
                                break;
                            }
                        }
                        MainClass.Ibc05_Accounts.RemoveAt(iR);
                        if (MainClass.Ibc05_Accounts.Count == 0)
                        {
                            int bmR = 0;
                            for (var j = 0; j < MainClass.bookmakers.Count; j++)
                            {
                                if (MainClass.bookmakers[j].BookmakerName == "Ibc05")
                                {
                                    bmR = j;
                                    break;
                                }
                            }
                            MainClass.bookmakers.RemoveAt(bmR);
                        }
                        break;
                }
                */
            }
        }

  //      private void ExecuteRemove(int prm_NumAccounts)
  //      {

		//}

    }
}
