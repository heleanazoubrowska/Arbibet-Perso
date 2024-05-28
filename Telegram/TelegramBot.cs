using System;
using Telegram.Bot;

namespace ArbibetProgram.Telegram
{
    public class TelegramBot
    {
        public static async void sendMessage(string message)
        {

            if (MainClass.sendTelegram)
            {
                var botClient = new TelegramBotClient(Config.Config.telegrameToken);
                var me = botClient.GetMeAsync().Result;

                await botClient.SendTextMessageAsync(
                  chatId: Config.Config.chatIdGroup,
                  text: message
                );

                await botClient.SendTextMessageAsync(
                  chatId: Config.Config.chatIdKenGroup,
                  text: message
                );
            }
        }

        public static async void sendMessageTest()
        {
            var botClient = new TelegramBotClient(Config.Config.telegrameToken);
            var me = botClient.GetMeAsync().Result;

            string message = "Auto message TEST : Money is coming now!";

            await botClient.SendTextMessageAsync(
              chatId: Config.Config.chatIdKenGroup,
              text: message
            );
        }


        public static void sendBetAlert(string status,string bookmaker, string matchName, string teamWinner, string oddType, decimal winOdd, string betPrice)
        {
            string messageStatus = "";
            string betInfo = $"Bookmaker : {bookmaker} \n Match : {matchName} \n Team Bet : {teamWinner} \n Odd Type : {oddType} \n Odd : {winOdd} \n Price : {betPrice}";

            switch (status)
            {
                case "success":
                    messageStatus = " ------ BET PLACED ------\n";
                    break;

                case "fail":
                    messageStatus = "------ !!! BET FAILED !!! ------\n";
                    break;

                case "not_found":
                    messageStatus = "------ !!! BET NOT FOUND !!! ------\n";
                    break;
            }

            string message = $"{messageStatus}{betInfo}";

            sendMessage(message);
        }
    }
}
//https://api.telegram.org/bot1373599129:AAGsabs850V0vAJNR-oI__ucNDTonzZcY5M/getUpdates