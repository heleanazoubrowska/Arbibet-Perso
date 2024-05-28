using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArbibetProgram.Crawling;
using ArbibetProgram.Functions;
using ArbibetProgram.Models;

namespace ArbibetProgram
{
	public static partial class AccountMethods
	{
		public static void TurnOnAccounts(List<AccountData> prm_AccountsToTurnOn, bool prm_AddNewBookmaker_Flag)
		{
			//for (int loopy = 0; loopy < listAccounts.Count; loopy++)
			//{
			//    TurnOnAccount.AddAccount(listAccounts[loopy], false);
			//}
			//Parallel.ForEach(MainClass.listAccounts, (prv_AccountData) =>
			//Parallel.ForEach(prm_AccountsToTurnOn, (prv_AccountData) =>
			int prv_NumAccounts = prm_AccountsToTurnOn.Count;
			Parallel.For(0, prv_NumAccounts, (prv_Counter) =>
			{
				var prv_AccountData = prm_AccountsToTurnOn[prv_Counter];
				AddAccount(prv_AccountData, prm_AddNewBookmaker_Flag);
			});
		}

		public static void AddAccount(AccountData prm_AccountData, bool prm_AddNewBookmaker_Flag)
        {
            try
            {
                //if (prv_AccountData.AccountBookmakerActive == 1)
                Bookmaker prv_Bookmaker = null;

                int prv_Index = MainClass.bookmakers.FindIndex(prv_Item => prv_Item.BookmakerName == prm_AccountData.AccountBookmakerName);
                
                if (prv_Index > -1) // -1 = not found, 0 ~ N = position in list
                {
                    prv_Bookmaker = MainClass.bookmakers[prv_Index];
                }
                else if (prm_AddNewBookmaker_Flag)
                {
	                prv_Bookmaker = new Bookmaker()
	                {
		                BookmakerName = prm_AccountData.AccountBookmakerName,
		                BookmakerUrl = prm_AccountData.AccountBookmakerUrl,
		                BookmakerActive = true,
		                BookmakerAccountActive = true,
		                BookmakerUser = prm_AccountData.AccountUsername,
		                BookmakerPass = prm_AccountData.AccountPassword,
		                BookmakerAccountBalance = prm_AccountData.AccountBalance,
		                BookmakerShowPage = false
	                };
                }
                else
                {
                    return;
                }

                // ---------------------------------------------------------
                // get the implementation class type by its string name
                // ---------------------------------------------------------
                Type prv_ImplementationType = CasinoAPI.GetImplementationByName(prv_Bookmaker.BookmakerName);

                // ---------------------------------------------------------
                // the account class encapsulates casino's API
                // ---------------------------------------------------------
                var prv_Account = new Models.Account()
                {
                    AccountInfo = new AccountInfo()
                    {
                        AccountName = prm_AccountData.AccountName,
                        AccountUsername = prm_AccountData.AccountUsername,
                        AccountBalance = prm_AccountData.AccountBalance
                    },
                    API = Activator.CreateInstance(prv_ImplementationType) as CasinoAPI // magic!
                };
                prv_Bookmaker.Accounts.Add(prv_Account);

                // -------------------------------------
                // assign active account and connect
                // -------------------------------------
                if (prm_AccountData.AccountActive == 1)
                {
	                //if (prv_Bookmaker.ActiveAccount != null)
	                //{
	                //    prv_Bookmaker.ActiveAccount.API.quitBrowser();
	                //}
	                prv_Bookmaker.ActiveAccount?.API.quitBrowser(); // ? = null propagation, ie if ActiveAccount not null then quitBrowser

	                prv_Bookmaker.ActiveAccount = prv_Account;
	                prv_Bookmaker.ActiveAccount.API.Connect(prm_AccountData);
                }

                if (prv_Index == -1) // -1 = bookmaker not found, 0 ~ N = position in list
                {
	                MainClass.bookmakers.Add(prv_Bookmaker);
	                MainClass.gs_NumBookmakers = MainClass.bookmakers.Count;
                }
            }
            catch (Exception e)
            {
                NLogger.Log(EventLevel.Error, $"Error in TurnOnAccount.AddAccount : {e}");
            }
        }

		/*
        public static void turnOnAccount(List<AccountData> prm_AccountsToTurnOn)
        {
            NLogger.Log(EventLevel.Critical,"TurnOn ACCOUNT");

            foreach (AccountData prv_AccountData in prm_AccountsToTurnOn)
            {
	            AddAccount(prv_AccountData, true);

			//	int iR = 0;
			//	switch (account.AccountBookmakerName)
			//	{
			//		case "Cambo88 UG":

			//			MainClass.Cambo88UG_Accounts.Add(new Cambo88UGAccount()
			//			{
			//				AccountInfo = new AccountInfo()
			//				{
			//					AccountName = account.AccountName,
			//					AccountUsername = account.AccountUsername,
			//					AccountBalance = account.AccountBalance
			//				},
			//				Cambo88UG = new Cambo88UG(account)
			//			});
			//			break;

			//		case "Cambo88 BTI":

			//			MainClass.Cambo88BTI_Accounts.Add(new Cambo88BTIAccount()
			//			{
			//				AccountInfo = new AccountInfo()
			//				{
			//					AccountName = account.AccountName,
			//					AccountUsername = account.AccountUsername,
			//					AccountBalance = account.AccountBalance
			//				},
			//				Cambo88BTI = new Cambo88BTI(account)
			//			});
			//			break;

			//		case "Cambo88 SBO":

			//			MainClass.Cambo88SBO_Accounts.Add(new Cambo88SBOAccount()
			//			{
			//				AccountInfo = new AccountInfo()
			//				{
			//					AccountName = account.AccountName,
			//					AccountUsername = account.AccountUsername,
			//					AccountBalance = account.AccountBalance
			//				},
			//				Cambo88SBO = new Cambo88SBO(account)
			//			});
			//			break;

			//		case "Aa2888":

			//			MainClass.Aa2888_Accounts.Add(new Aa2888Account()
			//			{
			//				AccountInfo = new AccountInfo()
			//				{
			//					AccountName = account.AccountName,
			//					AccountUsername = account.AccountUsername,
			//					AccountBalance = account.AccountBalance
			//				},
			//				Aa2888 = new Aa2888(account)
			//			});

			//			break;

			//		case "855bet":

			//			MainClass.B855bet_Accounts.Add(new B855betAccount()
			//			{
			//				AccountInfo = new AccountInfo()
			//				{
			//					AccountName = account.AccountName,
			//					AccountUsername = account.AccountUsername,
			//					AccountBalance = account.AccountBalance
			//				},
			//				B855bet = new B855bet(account)
			//			});

			//			break;
			//		case "Va2888":

			//			MainClass.Va2888_Accounts.Add(new Va2888Account()
			//			{
			//				AccountInfo = new AccountInfo()
			//				{
			//					AccountName = account.AccountName,
			//					AccountUsername = account.AccountUsername,
			//					AccountBalance = account.AccountBalance
			//				},
			//				Va2888 = new Va2888(account)
			//			});

			//			break;

			//		case "Va2888AFB":

			//			MainClass.Va2888AFB_Accounts.Add(new Va2888AFBAccount()
			//			{
			//				AccountInfo = new AccountInfo()
			//				{
			//					AccountName = account.AccountName,
			//					AccountUsername = account.AccountUsername,
			//					AccountBalance = account.AccountBalance
			//				},
			//				Va2888AFB = new Va2888AFB(account)
			//			});

			//			break;

			//		case "Nba369":

			//			MainClass.Nba369_Accounts.Add(new Nba369Account()
			//			{
			//				AccountInfo = new AccountInfo()
			//				{
			//					AccountName = account.AccountName,
			//					AccountUsername = account.AccountUsername,
			//					AccountBalance = account.AccountBalance
			//				},
			//				Nba369 = new Nba369(account)
			//			});

			//			break;

			//		case "Ibc05":

			//			MainClass.Ibc05_Accounts.Add(new Ibc05Account()
			//			{
			//				AccountInfo = new AccountInfo()
			//				{
			//					AccountName = account.AccountName,
			//					AccountUsername = account.AccountUsername,
			//					AccountBalance = account.AccountBalance
			//				},
			//				Ibc05 = new Ibc05(account)
			//			});

			//			break;
			//	}

			//	bool bmFounded = false;
			//	for (var j = 0; j < MainClass.bookmakers.Count; j++)
			//	{

			//		if (MainClass.bookmakers[j].BookmakerName == account.AccountBookmakerName)
			//		{
			//			bmFounded = true;
			//			break;
			//		}
			//	}
			//	if (!bmFounded)
			//	{
			//		MainClass.bookmakers.Add(new Bookmaker()
			//		{
			//			BookmakerName = account.AccountBookmakerName,
			//			BookmakerUrl = account.AccountBookmakerUrl,
			//			BookmakerActive = true,
			//			BookmakerAccountActive = true,
			//			BookmakerUser = account.AccountUsername,
			//			BookmakerPass = account.AccountPassword,
			//			BookmakerAccountBalance = account.AccountBalance,
			//			BookmakerShowPage = false
			//		});
			//	}
			}

			// update number of bookmakers
			//MainClass.gs_NumBookmakers = MainClass.bookmakers.Count;    
		}
		*/
    }
}
