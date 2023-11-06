using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemBank.Controllers;

namespace UdemBank.Services
{
    internal class DissolveGroupService
    {
        public static void DissolveSavingGroup(SavingGroup savingGroup)
        {
            // Obtener los Savings afiliados al SavingGroup
            List<Saving> savingsOfSavingGroup = SavingController.GetSavingsBySavingGroup(savingGroup);

            if (savingsOfSavingGroup.Count == 0)
            {
                Console.WriteLine("Imposible...");
                return;
            }

            // Calcular la comisión y el monto restante
            double totalAmount = savingGroup.TotalAmount;
            double commission = 0.05 * totalAmount; // Comisión del 5%
            double remainingAmount = totalAmount - commission;

            // Calcular el porcentaje de ahorro de cada usuario y transferir el monto a sus cuentas
            foreach (var saving in savingsOfSavingGroup)
            {
                double userShare = (double)saving.Investment / totalAmount;
                double amountToTransfer = userShare * remainingAmount;

                // Transferir el monto a la cuenta del usuario
                UserController.AddAmount(saving.User, amountToTransfer);
            }

            // Registrar la comisión en algún lugar si es necesario? (Nahhh )

            // Disolver el grupo de ahorro (eliminarlo de la base de datos) y también todos los savings...
            using (var db = new UdemBankContext())
            {
                db.SavingGroups.Remove(savingGroup);
                db.Savings.RemoveRange(savingsOfSavingGroup);
                db.SaveChanges();
            }

            Console.WriteLine("El grupo de ahorro se ha disuelto y los montos se han transferido a las cuentas de los usuarios.");
            Console.ReadLine();
            AnsiConsole.Clear();
        }
    }
}
