using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UdemBank.Enums;

namespace UdemBank
{
    internal class LoanController
    {
        public static void AddLoan(Saving saving, double amount, DateOnly dueDate)
        {
            using var db = new UdemBankContext();

            // Verificar si el ahorro existe en la base de datos
            var existingSaving = db.Savings.FirstOrDefault(s => s.Id == saving.Id);

            if (existingSaving != null)
            {
                var newLoan = new Loan
                {
                    SavingId = existingSaving.Id,
                    Saving = existingSaving,
                    Amount = amount,
                    Date = DateTimeOffset.Now,
                    DueDate = dueDate,
                    Type = TradeType.PrestamoGrupoAhorro
                };

                db.Loans.Add(newLoan);
                db.SaveChanges();

                Console.WriteLine("El préstamo se ha registrado con éxito.");
            }
            else
            {
                Console.WriteLine("El ahorro al que intentas asociar el préstamo no existe.");
            }
        }
        public static bool CanRequestLoanFromGroup(User user, SavingGroup savingGroup)
        {
            using var db = new UdemBankContext();

            // Obtener los IDs de todos los grupos de ahorro a los que pertenece el usuario
            var userGroupIds = db.Savings
                .Where(s => s.UserId == user.Id && s.Affiliation)
                .Select(s => s.SavingGroupId)
                .ToList();

            // Verificar si hay algún usuario en esos grupos de ahorro que también esté en el grupo de ahorro ingresado
            bool canRequestLoan = db.Savings
                .Any(s => userGroupIds.Contains(s.SavingGroupId) && s.SavingGroupId == savingGroup.Id);

            return canRequestLoan;
        }
        public static List<Loan> GetLoanHistoryForUser(User user)
        {
            using (var db = new UdemBankContext())
            {
                var loanHistory = db.Loans
                    .Where(loan => db.Savings
                        .Where(saving => saving.UserId == user.Id)
                        .Select(saving => saving.Id)
                        .Contains(loan.SavingId))
                    .ToList();

                return loanHistory;
            }
        }
        public static List<Loan> GetLoansByUser(User user)
        {
            using (var db = new UdemBankContext())
            {
                // Obtener los Savings del usuario con la propiedad SavingGroup cargada
                var userSavings = db.Savings
                    .Include(s => s.SavingGroup)
                    .Where(s => s.UserId == user.Id)
                    .ToList();

                // Obtener los Loans cuyos SavingId estén en la lista de Savings del usuario con la propiedad Saving cargada
                var loans = db.Loans
                    .Include(loan => loan.Saving)
                    .Where(loan => userSavings.Select(s => s.Id).Contains(loan.SavingId))
                    .ToList();

                return loans;
            }
        }


        public static Loan? GetLoanById(int Id)
        {
            using var db = new UdemBankContext();
            var loan = db.Loans.SingleOrDefault(u => u.Id == Id);
            return loan;
        }
        public static List<Loan> GetLoans()
        {
            using var db = new UdemBankContext();
            var loans = db.Loans.ToList();
            return loans;
        }
    }

}
