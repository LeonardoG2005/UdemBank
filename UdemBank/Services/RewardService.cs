using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemBank.Controllers;

namespace UdemBank.Services
{
    internal class RewardService
    {
        public static void RewardSavingGroup()
        {
            SavingGroup? savingGroup = SavingGroupController.GetSavingGroupWithBiggestIncome();

            // inyectarle el 10%
            double percent = savingGroup.TotalAmount * 0.10;
            SavingGroupController.AddAmountToSavingGroup(savingGroup, percent);
            Console.WriteLine("Se premió al grupo de ahorro : " + savingGroup.Name + "con una transacción de valor : " + percent.ToString());
            Console.ReadLine();
            Console.Clear();
        }

        public static void RewardUser()
        {
            var userName = AnsiConsole.Ask<string>("Ingrese el nombre del usuario : ");

            User user = UserController.GetUserByName(userName);

            if (user == null)
            {
                Console.WriteLine("No hay un usuario con ese nombre ... TwT");
                Console.ReadLine();
                AnsiConsole.Clear();
                return;
            }

            user = UserController.RewardUser(user);
            Console.WriteLine("Se premió al usuario de nombre : " + user.Name + " con un 1% menos de comisión a la hora de hacer préstamos.");
            Console.ReadLine();
            Console.Clear();
        }

    }
}
