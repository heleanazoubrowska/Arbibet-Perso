using System;
using ArbibetProgram.Functions;
using ArbibetProgram.Models;

namespace ArbibetProgram.Strategy
{
    public static partial class Football
    {
        public static CasinoBetStatus GetBetStatus(int prm_Index_Bookmaker, string bookmaker, string match)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CasinoBetStatus result = MainClass.bookmakers[prm_Index_Bookmaker].ActiveAccount.API.getBetStatus(match);
            watch.Stop();
            NLogger.Log(EventLevel.Info, $"Search bet {bookmaker} in {watch.ElapsedMilliseconds} ms");

            return result;
        }
    }
}
