using Spectre.Console;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemBank.Controllers;

namespace UdemBank.Services
{
    internal class CreateLoanService
    {
        public static bool UserIsAffiliated(User user, SavingGroup savingGroup)
        {
            // se debe obtener el Saving asociado al usuario y al savingGroups
            Saving? saving = SavingController.GetSavingByUserAndSavingGroup(user, savingGroup);

            return (saving.Affiliation);
        }
        public static void CreateLoan(User user, SavingGroup savingGroup, double interestRate)
        {
            var amount = AnsiConsole.Ask<int>("Ingrese la cantidad que desea prestar : ");
            var date = AnsiConsole.Ask<DateTime>("Ingrese el plazo máximo de pago deseado (YYYY-MM-DD): ");

            DateOnly dateOnly = DateOnly.FromDateTime(date);

            // Este sería igual a date pero es de tipo DateTime y se establece la hora como 00:00:00 ...
            DateTime dateTime = new DateTime(dateOnly.Year, dateOnly.Month, dateOnly.Day, 0, 0, 0);

            // se debe obtener el Saving asociado al usuario y al savingGroups
            Saving? saving = SavingController.GetSavingByUserAndSavingGroup(user, savingGroup);

            // El plazo de pago no debe ser menor a 2 meses...
            double months = CalculateMonths(dateOnly);

            if (months < 2)
            {
                Console.WriteLine("El plazo de pago debe ser de al menos dos meses.");
                Console.ReadLine();
                return;
            }

            // AQUÍ SE TIENE EN CUENTA LA FIDELIZACIÓN :
            if (user.Rewarded)
            {
                interestRate = interestRate - 0.01;
            }

            // Se obtiene el interés...
            double interest = CalculateInterest(amount, dateOnly, interestRate);

            // El préstamo no puede superar el monto total invertido por el usuario
            if (saving.Investment >= amount)
            {
                // Se crea el Loan (la deuda es amount + interest)
                SavingGroupController.DeduceAmountToSavingGroup(savingGroup, amount);
                LoanController.AddLoan(saving, amount + interest, dateOnly);
                
                // Hay que deducir la cantidad al Saving asociado al usuario y al savingGroups
                SavingController.DeduceInvestmentToSaving(savingGroup.Id, amount);

                // Y hay que SUMAR la cantidad al account del User.
                UserController.AddAmount(user, amount);

                Console.WriteLine("Se hizo el prestamo correctamente :) ...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("El usuario no poseé suficiente capital...");
                Console.ReadLine();
            }
        }

        public static void CreateLoanNotInSavingGroup(User user, SavingGroup savingGroup)
        {
            return;
        }
        public static double CalculateInterest(int amount, DateOnly date, double interestRate)
        {
            double months = CalculateMonths(date);
            double interest = amount * interestRate * months;

            return interest;
        }

        public static double CalculateMonths(DateOnly date)
        {
            double months = (date.Year - DateTime.Now.Year) * 12 + date.Month - DateTime.Now.Month;
            return months;
        }
    }
}
