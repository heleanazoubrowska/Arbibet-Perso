using System;
using System.Threading.Tasks;
using ArbibetProgram;
using ArbibetProgram.Functions;

namespace ArbiBetCore.Strategy
{
    public class HedgeBet
    {
        public HedgeBet()
        {

        }

        public static void hedgeBet()
        {
            Parallel.For(0, MainClass.gs_NumBookmakers, MainClass.gs_ParallelOptions, (Counter) =>
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var prv_Bookmaker = MainClass.bookmakers[Counter];
                prv_Bookmaker.BookmakerMatches.Clear();

                try
                {
                    prv_Bookmaker.ActiveAccount.API.udpateOdd(prv_Bookmaker, true);
                }
                catch (Exception e)
                {
                    NLogger.Log(EventLevel.Error, $"{prv_Bookmaker.BookmakerName} Error update : {e}");
                }

                watch.Stop();
                NLogger.Log(EventLevel.Info, $"Update {prv_Bookmaker.BookmakerName} in {watch.ElapsedMilliseconds} ms with : {prv_Bookmaker.BookmakerMatches.Count} match");

                //Console.WriteLine(JsonConvert.SerializeObject(bookmakers, Formatting.Indented));

            });
        }
    }
}
