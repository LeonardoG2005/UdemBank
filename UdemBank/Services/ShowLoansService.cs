using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemBank.Services
{
    internal class ShowLoansService
    {
        public static Loan? GetLoanOptionInput(User user)
        {
            var loans = LoanController.GetLoansByUser(user);
            if (!loans.Any())
            {
                Console.WriteLine("El usuario no ha hecho ningún préstamo...");
                Console.ReadLine();
                AnsiConsole.Clear();
                return null;
            }

            // Crear una lista de cadenas con la combinación de nombre de grupo de ahorro y cantidad del préstamo
            var loanOptions = loans.Select(loan => $"{loan.Saving.SavingGroup.Name} {loan.Amount}").ToList();

            var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Selecciona un préstamo")
                .AddChoices(loanOptions));

            // Extraer el Id del préstamo seleccionado a partir del texto seleccionado
            var selectedLoan = loans.FirstOrDefault(loan => $"{loan.Saving.SavingGroup.Name} {loan.Amount}" == option);

            if (selectedLoan != null)
            {
                return selectedLoan;
            }
            else
            {
                Console.WriteLine("Préstamo no encontrado.");
                Console.ReadLine();
                return null;
            }
        }

    }
}
