using Org.BouncyCastle.Crypto.Tls;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemBank.Controllers;
using static UdemBank.Enums;

namespace UdemBank.Services
{
    internal class TradeService
    {
        public static User? AddAmountToGroup(User user, SavingGroup savingGroup)
        {
            var amount = AnsiConsole.Ask<int>("Ingrese la cantidad: ");
            DateTime currentDateTime = DateTime.Now;

            if (user.Account >= amount)
            {
                // Se crea el Trade
                SavingGroupController.AddAmountToSavingGroup(savingGroup, amount);
                TradeController.AddTrade(user, savingGroup, TradeType.TransferenciaGrupoAhorro, amount, currentDateTime);

                // Hay que agregar la cantidad al Saving asociado al usuario y al savingGroups
                Saving? saving = SavingController.GetSavingByUserAndSavingGroup(user, savingGroup);
                SavingController.AddInvestmentToSaving(saving.Id, amount);

                // Y hay que deducir la cantidad del account del User.
                user = UserController.RemoveAmount(user, amount);

                Console.WriteLine("Se ingreso el capital correctamente :) ...");
                Console.ReadLine();
                AnsiConsole.Clear();
                return user;
            }
            else
            {
                Console.WriteLine("El usuario no poseé suficiente capital...");
                Console.ReadLine();
                AnsiConsole.Clear();
                return user;
            }
        }
    }
}
