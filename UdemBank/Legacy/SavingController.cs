using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemBank.Controllers
{
    internal class SavingController
    {
        public static Saving? AddSaving(User user, SavingGroup SavingGroup, bool affiliation)
        {
            using var db = new UdemBankContext(); // Conexión a la base de datos

            // Verificar si el usuario y el grupo de ahorro existen en la base de datos
            var existingUser = db.Users.FirstOrDefault(u => u.Id == user.Id);
            var existingSavingGroup = db.SavingGroups.FirstOrDefault(sg => sg.Id == SavingGroup.Id);

            if (existingUser != null && existingSavingGroup != null)
            {
                var newSaving = new Saving
                {
                    UserId = existingUser.Id,
                    SavingGroupId = existingSavingGroup.Id,
                    Affiliation = affiliation,
                    Investment = 0
                };

                db.Savings.Add(newSaving);
                db.SaveChanges();
                Console.WriteLine();
                Console.ReadLine();
                AnsiConsole.Clear();
                return newSaving;
            }
            else
            {
                Console.WriteLine("Grave...");
                Console.ReadLine();
                return null;
            }
        }
        public static List<Saving> GetSavingsByUserSavingGroupName(string savingGroupName)
        {
            using var db = new UdemBankContext();

            // Buscar el SavingGroup por nombre
            var savingGroup = db.SavingGroups.FirstOrDefault(g => g.Name == savingGroupName);

            if (savingGroup == null)
            {
                Console.WriteLine("Grave...");
                return new List<Saving>();
            }

            // Obtener los Savings con el SavingGroupId encontrado y Affiliation en true, incluyendo la entidad User
            var savingsInGroup = db.Savings
                .Include(s => s.User) // Incluir la entidad User
                .Where(s => s.SavingGroupId == savingGroup.Id && s.Affiliation)
                .ToList();

            return savingsInGroup;
        }
        public static Saving? GetSavingByUserAndSavingGroup(User user, SavingGroup savingGroup)
        {
            using var db = new UdemBankContext();

            var saving = db.Savings.FirstOrDefault(s =>
                s.UserId == user.Id && s.SavingGroupId == savingGroup.Id && s.Affiliation == true
            );
            db.SaveChanges();
            return saving;
        }
        public static void AddInvestmentToSaving(int savingId, double amountToAdd)
        {
            using var db = new UdemBankContext();

            var saving = db.Savings.FirstOrDefault(s => s.Id == savingId);

            if (saving != null)
            {
                saving.Investment += amountToAdd;
                db.SaveChanges();
            }
        }
        public static void DeduceInvestmentToSaving(int savingId, int amountToAdd)
        {
            using var db = new UdemBankContext();

            var saving = db.Savings.FirstOrDefault(s => s.Id == savingId);

            if (saving != null)
            {
                saving.Investment -= amountToAdd;
                db.SaveChanges();
            }
        }
        public static List<Saving> GetSavingsBySavingGroupAndAffiliation(SavingGroup savingGroup)
        {
            using (var db = new UdemBankContext())
            {
                return db.Savings
                    .Where(s => s.SavingGroupId == savingGroup.Id && s.Affiliation)
                    .ToList();
            }
        }
        public static List<Saving> GetSavingsBySavingGroup(SavingGroup savingGroup)
        {
            using (var db = new UdemBankContext())
            {
                return db.Savings
                    .Where(s => s.SavingGroupId == savingGroup.Id)
                    .Include(s => s.User)          // Incluye el usuario relacionado
                    .Include(s => s.SavingGroup)  // Incluye el grupo de ahorro relacionado
                    .ToList();
            }
        }
        public static List<User> GetUsersBySavings(SavingGroup savingGroup)
        {
            List<Saving> savings = GetSavingsBySavingGroupAndAffiliation(savingGroup);

            using (var db = new UdemBankContext())
            {
                // Obtener los IDs de los Savings en la lista
                var savingIds = savings.Select(s => s.UserId).ToList();

                // Obtener los Users cuyo ID está en la lista de Savings
                var users = db.Users
                    .Where(u => savingIds.Contains(u.Id))
                    .ToList();

                return users;
            }
        }
    }
}
