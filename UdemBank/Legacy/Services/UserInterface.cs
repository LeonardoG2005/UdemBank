using AutoMapper.Internal;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemBank.Controllers;

namespace UdemBank.Services
{
    internal class UserInterface
    {
        public static void ShowTradesHistory(List<Trade> trades)
        {
            var table = new Table();
            table.AddColumn("Cantidad");
            table.AddColumn("Fecha");
            table.AddColumn("Tipo de transacción");
            foreach (var trade in trades)
            {
                table.AddRow(trade.Amount.ToString(), trade.Date.ToString(), trade.Type.ToString());
            }
            AnsiConsole.Write(table);
            Console.ReadLine();
            Console.Clear();
        }
        public static void ShowUserInfoOfSavingGroup(SavingGroup savingGroup)
        {
            string SavingGroupName = savingGroup.Name;
            List<Saving> savings = SavingController.GetSavingsByUserSavingGroupName(SavingGroupName);

            var table = new Table();
            table.AddColumn("Nombre del usuario: ");

            foreach (var saving in savings)
            {
                table.AddRow(saving.User.Name.ToString());
            }
            AnsiConsole.Write(table);
            Console.ReadLine();
            AnsiConsole.Clear();
        }
        public static void ShowUserTransactionHistory(User user)
        {
            var tradeHistory = TradeController.GetTradeHistoryForUser(user);
            var loanHistory = LoanController.GetLoanHistoryForUser(user);

            var transactionHistory = new List<ITransaction>();
            transactionHistory.AddRange(tradeHistory);
            transactionHistory.AddRange(loanHistory);

            transactionHistory = transactionHistory
                .OrderBy(transaction => transaction.Date)
                .ToList();

            var table = new Table();
            table.Border = TableBorder.Rounded;
            table.AddColumn("Fecha");
            table.AddColumn("Hora");
            table.AddColumn("Tipo");
            table.AddColumn("Monto");
            table.AddColumn("Saldo en cuenta");

            foreach (var transaction in transactionHistory)
            {
                string date = "";
                string time = "";

                date = transaction.Date.Date.ToString();
                time = transaction.Date.TimeOfDay.ToString();

                table.AddRow(date, time, transaction.GetType().Name, transaction.Amount.ToString(), transaction.CurrentBalance.ToString());
            }
            AnsiConsole.Render(table);
            Console.ReadLine();
            AnsiConsole.Clear();
        }
    }
}
