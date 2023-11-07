using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemBank.Controllers;

namespace UdemBank
{
    internal class SavingGroupController
    {
        public static SavingGroup? AddSavingGroup(User user)
        {
            using (var db = new UdemBankContext())
            {
                var name = AnsiConsole.Ask<string>("Ingrese el nombre que tendrá el grupo: ");

                // Verificar si el usuario ya está rastreado en el contexto
                var existingUser = db.Users.FirstOrDefault(u => u.Id == user.Id);

                if (existingUser == null)
                {
                    Console.WriteLine("how the fuck");
                    return null;
                }
                // Crear un nuevo grupo de ahorro con TotalAmount inicial en 0
                var newSavingGroup = new SavingGroup
                {
                    UserId = existingUser.Id,
                    Name = name,
                    TotalAmount = 0
                };

                // Agregar el nuevo grupo de ahorro a la base de datos
                db.SavingGroups.Add(newSavingGroup);
                db.SaveChanges();

                Console.WriteLine("El grupo de ahorro se ha creado con éxito.");
                Console.ReadLine();
                Console.Clear();

                return newSavingGroup;
            }
        }
        public static void AddAmountToSavingGroup(SavingGroup? savingGroup, double Amount)
        {
            using var db = new UdemBankContext();

            if (savingGroup == null)
            {
                Console.WriteLine("Grave...");
            }
            if (!(savingGroup == null))
            {
                savingGroup.TotalAmount += Amount;

                db.SavingGroups.Update(savingGroup);
            }
            db.SaveChanges();
        }
        public static void DeduceAmountToSavingGroup(SavingGroup savingGroup, double Amount)
        {
            using var db = new UdemBankContext();

            if (savingGroup == null)
            {
                Console.WriteLine("Grave...");
            }

            savingGroup.TotalAmount -= Amount;

            db.SavingGroups.Update(savingGroup);
            db.SaveChanges();
        }
        public static SavingGroup? GetSavingGroupWithBiggestIncome()
        {
            using (var db = new UdemBankContext())
            {
                // Obtén el SavingGroup con el TotalAmount más alto
                var savingGroupWithBiggestIncome = db.SavingGroups
                    .OrderByDescending(sg => sg.TotalAmount)
                    .FirstOrDefault();

                return savingGroupWithBiggestIncome;
            }
        }
        public static List<SavingGroup>? GetSavingGroupsByUser(User user)
        {
            using var db = new UdemBankContext();

            // Verificar si el usuario ya está rastreado en el contexto
            User? existingUser = db.Users.FirstOrDefault(u => u.Id == user.Id);

            if (existingUser == null)
            {
                Console.WriteLine("El usuario no existe en el contexto.");
                return null;
            }

            var userId = existingUser.Id; // Copiar el valor en una variable local

            Console.WriteLine(userId);
            Console.ReadLine();

            var savings = db.Savings
                .Include(s => s.SavingGroup) // Cargar las entidades SavingGroup relacionadas
                .Where(s => s.UserId == userId)
                .ToList();

            if (savings == null)
            {
                Console.WriteLine("El conjunto de ahorros del usuario es nulo.");
                Console.ReadLine();
                return null;
            }

            if (!savings.Any())
            {
                Console.WriteLine("El conjunto de ahorros del usuario está vacío.");
                Console.ReadLine();
                return null;
            }

            foreach (var saving in savings)
            {
                Console.WriteLine($"El saving tiene userId :{saving.UserId}");
            }

            var savingGroups = savings.Select(s => s.SavingGroup).ToList();

            foreach (var savinggroup in savingGroups)
            {
                Console.WriteLine($"El savingGroup tiene userId :{savinggroup.UserId}");
            }

            return savingGroups;
        }

        //método que devuelve todos los usuarios que pertenecen a grupos en los que está un usuario
        public static List<User> GetUsersInEligibleSavingGroups(User user)
        {
            using (var db = new UdemBankContext())
            {
                // Paso 1: Obtener todos los Savings asociados al usuario con inclusión de objetos relacionados
                var userSavings = db.Savings
                    .Where(s => s.UserId == user.Id)
                    .Include(s => s.SavingGroup) // Incluir el objeto SavingGroup relacionado
                    .ToList();

                // Paso 2: Obtener todos los grupos de ahorro a los que pertenece el usuario
                var userSavingGroupIds = userSavings.Select(s => s.SavingGroup.Id).ToList();
                var userSavingGroups = db.SavingGroups
                    .Where(g => userSavingGroupIds.Contains(g.Id))
                    .ToList();

                // Paso 3: Obtener todos los Savings asociados a los grupos de ahorro en los que se encuentra el usuario
                var savingsInUserGroups = db.Savings
                    .Where(s => userSavingGroupIds.Contains(s.SavingGroupId))
                    .Include(s => s.User) // Incluir el objeto User relacionado
                    .ToList();

                // Paso 4: Obtener todos los usuarios que se encuentran en grupos de ahorro a los que pertenece el usuario
                var userIdsInUserGroups = savingsInUserGroups.Select(s => s.UserId).ToList();
                var usersInUserGroups = db.Users
                    .Where(u => userIdsInUserGroups.Contains(u.Id))
                    .ToList();

                // Retornar la lista de grupos de ahorro a los que el usuario podría solicitar un préstamo
                return usersInUserGroups;
            }
        }

        public static List<SavingGroup> GetEligibleSavingGroupsForUser(User user)
        {
            using (var db = new UdemBankContext())
            {
                // Obtener la lista de usuarios que pertenecen a grupos de ahorro a los que el usuario actual también pertenece
                var usersInUserGroups = SavingGroupController.GetUsersInEligibleSavingGroups(user);

                // Paso 1: Obtener todos los Savings asociados a los usuarios
                var savingsAssociatedWithUsers = db.Savings
                    .Include(s => s.SavingGroup) // Incluir la relación SavingGroup
                    .Where(s => usersInUserGroups.Select(u => u.Id).Contains(s.UserId))
                    .ToList();

                // Paso 2: A partir de los Savings, obtener los Grupos de Ahorro
                var eligibleSavingGroups = db.SavingGroups
                    .Where(g => savingsAssociatedWithUsers
                        .Any(s => s.SavingGroupId == g.Id))
                    .ToList();

                return eligibleSavingGroups;
            }
        }

        public static SavingGroup? GetSavingGroupById(int Id)
        {
            using var db = new UdemBankContext();
            var SavingGroup = db.SavingGroups.SingleOrDefault(b => b.Id == Id);
            return SavingGroup;
        }
    }
}
