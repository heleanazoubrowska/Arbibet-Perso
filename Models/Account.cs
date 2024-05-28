using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ArbibetProgram.Crawling;

namespace ArbibetProgram.Models
{
    public class AccountData
    {
        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("account_user")]
        public string AccountUsername { get; set; }

        [JsonProperty("account_pass")]
        public string AccountPassword { get; set; }

        [JsonProperty("account_balance")]
        public decimal AccountBalance { get; set; }

        [JsonProperty("bookmaker_name")]
        public string AccountBookmakerName { get; set; }

        [JsonProperty("bookmaker_url")]
        public string AccountBookmakerUrl { get; set; }

        [JsonProperty("account_active")]
        public int AccountActive { get; set; }

        [JsonProperty("bookmaker_active")]
        public int AccountBookmakerActive { get; set; }

        public bool BookmakerShowPage { get; set; }

        public AccountData()
        {
            BookmakerShowPage = false;
        }
    }

    public class AccountInfo
    {
        public string AccountName { get; set; }
        public string AccountUsername { get; set; }
        public decimal AccountBalance { get; set; }
        public bool AccountActive { get; set; }
    }

    

    


    //public class Cambo88UGAccount
    //{
    //    public AccountInfo AccountInfo { get; set; }
    //    public Cambo88UG Cambo88UG { get; set; }
    //}

    //public class Cambo88BTIAccount
    //{
    //    public AccountInfo AccountInfo { get; set; }
    //    public Cambo88BTI Cambo88BTI { get; set; }
    //}

    //public class Cambo88SBOAccount
    //{
    //    public AccountInfo AccountInfo { get; set; }
    //    public Cambo88SBO Cambo88SBO { get; set; }
    //}

    //public class Aa2888Account
    //{
    //    public AccountInfo AccountInfo { get; set; }
    //    public Aa2888 Aa2888 { get; set; }
    //}

    //public class B855betAccount
    //{
    //    public AccountInfo AccountInfo { get; set; }
    //    public B855bet B855bet { get; set; }
    //}

    //public class Va2888Account
    //{
    //    public AccountInfo AccountInfo { get; set; }
    //    public Va2888 Va2888 { get; set; }
    //}

    //public class Va2888AFBAccount
    //{
    //    public AccountInfo AccountInfo { get; set; }
    //    public Va2888AFB Va2888AFB { get; set; }
    //}

    //public class Nba369Account
    //{
    //    public AccountInfo AccountInfo { get; set; }
    //    public Nba369 Nba369 { get; set; }
    //}

    //public class Ibc05Account
    //{
    //    public AccountInfo AccountInfo { get; set; }
    //    public Ibc05 Ibc05 { get; set; }
    //}

    //public class Ibet789Account
    //{
    //    public AccountInfo AccountInfo { get; set; }
    //    public Ibet789 Ibet789 { get; set; }
    //}

    //public class Ph2888Account
    //{
    //    public AccountInfo AccountInfo { get; set; }
    //    public Ph2888 Ph2888 { get; set; }
    //}
}
