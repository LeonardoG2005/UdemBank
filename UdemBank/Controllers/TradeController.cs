using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static UdemBank.Enums;

namespace UdemBank.Controllers
{
    internal class TradeController
    {
        public static void AddTrade(User user, SavingGroup savingGroup, TradeType type, int amount, DateTime date)
        {
            using var db = new UdemBankContext();

            var saving = db.Savings.FirstOrDefault(s => s.UserId == user.Id && s.SavingGroupId == savingGroup.Id);

            if (saving == null)
            {
                Console.WriteLine("No se encontró el grupo de ahorro para el usuario.");
                return;
            }

            var trade = new Trade
            {
                SavingId = saving.Id,
                Amount = amount,
                Date = date,
                Type = type
            };

            db.Trades.Add(trade);
            db.SaveChanges();
        }


        public static Trade? GetTradeById(int Id)
        {
            using var db = new UdemBankContext();
            var trade = db.Trades.SingleOrDefault(b => b.Id == Id);
            return trade;
        }
        public static List<Trade> GetTradesForUserSavings(int knownUserId)
        {
            using (var context = new UdemBankContext()) 
            {
                var trades = context.Trades
                    .Where(trade => context.Savings
                        .Where(saving => saving.UserId == knownUserId)
                        .Select(saving => saving.Id)
                        .Contains(trade.SavingId))
                    .ToList();

                return trades;
            }
        }
        public static List<Trade> GetTradeHistoryForUser(User user)
        {
            using (var db = new UdemBankContext())
            {
                var tradeHistory = db.Trades
                    .Where(trade => db.Savings
                        .Where(saving => saving.UserId == user.Id)
                        .Select(saving => saving.Id)
                        .Contains(trade.SavingId))
                    .ToList();

                return tradeHistory;
            }
        }
    }
}
