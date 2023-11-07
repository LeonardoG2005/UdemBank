using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemBank.Controllers;

namespace UdemBank
{
    internal class PayLoanService
    {
        public static User? ShowInfoAndPayLoan(User user, Loan loan)
        {
            Console.WriteLine("Deuda a pagar: ");
            Console.WriteLine(loan.Amount.ToString());
            Console.WriteLine("");

            Console.WriteLine("Fecha de vencimiento: ");
            Console.WriteLine(loan.DueDate.ToString("yyyy-MM-dd"));
            Console.WriteLine("");

            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadLine();

            return PayLoan(user, loan);
        }

        public static User? PayLoan (User user, Loan loan)
        {
            SavingGroup? savingGroup = SavingGroupController.GetSavingGroupById(loan.Saving.SavingGroupId);

            if (user.Account < loan.Amount)
            {
                Console.WriteLine("El usuario no tiene suficiente cash para pagar (se le embargará la casa) ");
                Console.ReadLine();
                Console.Clear();
                return null;
            }

            // Meter la cantidad en el savingGroup
            SavingGroupController.AddAmountToSavingGroup(savingGroup, loan.Amount);

            // Se deduce la cantidad de la cuenta del usuario...
            user = UserController.RemoveAmount(user, loan.Amount);

            // se debe obtener el Saving asociado al usuario y al savingGroups
            Saving? saving = SavingController.GetSavingByUserAndSavingGroup(user, savingGroup);

            // La cantidad se regresa al Saving.Invesment...
            SavingController.AddInvestmentToSaving(saving.Id, loan.Amount);

            // indicar que el préstamo ya se pagó...
            LoanController.UpdateLoanPaidStatus(loan, true);

            Console.WriteLine("Se pagó el préstamo correctamente :) ...");
            Console.ReadLine();
            AnsiConsole.Clear();
            return user;
        }

    }
}
