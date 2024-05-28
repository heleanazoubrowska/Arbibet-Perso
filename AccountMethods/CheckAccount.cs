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
        public static bool checkAccounts()
        {
            //NLogger.Log(EventLevel.Info,"Check " + NbMatchCheck + " Matches and " + NbOddCheck);
            NLogger.Log(EventLevel.Critical, "CHECK ACCOUNT");
            MainClass.nbCheckAccount++;
            Console.WriteLine("Check account : " + MainClass.nbCheckAccount);
            bool isApiChecked = false;
            try
            {
                Task<string> resultAccounts = BackendArbApi.checkApi();
                resultAccounts.Wait();

                //NLogger.Log(EventLevel.Debug,"resultAccounts.Result" + resultAccounts.Result);
                //string resultAccounts = BackendArbApi.getAccounts();

                try
                {
                    ResultApi ResultApi = JsonConvert.DeserializeObject<ResultApi>(resultAccounts.Result);
                    //(List<AccountApi>)JsonConvert.DeserializeObject<IEnumerable<AccountApi>>(resultAccounts.Result);

                    if (ResultApi.AccountsApi.Count == 0)
                    {
                        return false;
                    }

                    List<AccountData> accountsToRemove = new List<AccountData>();
                    List<AccountData> accountsToTurnOn = new List<AccountData>();
                    List<AccountData> accountsToAdd = new List<AccountData>();

                    foreach (AccountData newAccount in ResultApi.AccountsApi)
                    {
                        bool accountExist = false;
                        foreach (AccountData account in MainClass.listAccounts)
                        {
                            if (account.AccountBookmakerName == newAccount.AccountBookmakerName)
                            {
                                if (account.AccountName == newAccount.AccountName)
                                {
                                    accountExist = true;
                                    if (account.AccountActive == 1 && account.AccountBookmakerActive == 1 && (newAccount.AccountActive != 1 || newAccount.AccountBookmakerActive != 1)) // Account is now disable
                                    {
                                        account.AccountActive = 0;
                                        accountsToRemove.Add(newAccount);
                                    }

                                    if (account.AccountActive == 0 && newAccount.AccountActive == 1 && newAccount.AccountBookmakerActive == 1) // Account is now active
                                    {
                                        account.AccountActive = 1;
                                        accountsToTurnOn.Add(newAccount);
                                    }
                                }
                            }
                        }

                        if (!accountExist && newAccount.AccountActive == 1 && newAccount.AccountActive == 1)
                        {
                            accountsToTurnOn.Add(newAccount);
                        }
                    }

                    //NLogger.Log(EventLevel.Debug,"accountToRemove");
                    //NLogger.Log(EventLevel.Debug,JsonConvert.SerializeObject(accountToRemove, Formatting.Indented));
                    //NLogger.Log(EventLevel.Debug,"accountToTurnOn");
                    //NLogger.Log(EventLevel.Debug,JsonConvert.SerializeObject(accountToTurnOn, Formatting.Indented));
                    //NLogger.Log(EventLevel.Debug,"accountToAdd");
                    //NLogger.Log(EventLevel.Debug,JsonConvert.SerializeObject(accountToAdd, Formatting.Indented));

                    if (accountsToRemove.Count > 0)
                    {
                        removeAccount(accountsToRemove);
                    }

                    if (accountsToTurnOn.Count > 0)
                    {
                        //turnOnAccount(accountToTurnOn);
                        TurnOnAccounts(accountsToTurnOn, true);
                    }

                    //if (accountToAdd.Count > 0)
                    //{
                    //    TurnOnAccount.turnOnAccount(accountToTurnOn);
                    //}

                    foreach (AccountData account in accountsToTurnOn)
                    {
	                    MainClass.listAccounts.Add(account);
                    }

                    isApiChecked = true;
                }
                catch
                {
                    isApiChecked = false;
                }

            }
            catch
            {
                isApiChecked = false;
            }

            return isApiChecked;

        }
    }
}
