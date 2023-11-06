using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemBank.Controllers;

namespace UdemBank
{
    internal class UserService
    {
        public static List<string>? GetUserLogInInput()
        {
            var name = AnsiConsole.Ask<string>("Nombre de usuario:");
            var password = AnsiConsole.Ask<string>(" Contraseña:");
            return new List<string> { name, password };
        }
        public static SavingGroup? GetSavingGroupOption(User user)
        {
            if (user == null)
            {
                Console.WriteLine("El usuario es nulo.");
                Console.ReadLine();
                AnsiConsole.Clear();
                return null;
            }

            var SavingGroups = SavingGroupController.GetSavingGroupsByUser(user);

            if (SavingGroups == null)
            {
                Console.Write("El savingGroups del usuario es null...");
                Console.ReadLine();
                return null;
            }

            if (!SavingGroups.Any())
            {
                Console.Write("El savingGroups.Count() del usuario es 0...");
                Console.ReadLine();
                return null;
            }

            foreach (var savingGroup in SavingGroups)
            {
                if (savingGroup != null)
                {
                    Console.WriteLine($"Nombre del grupo de ahorro: {savingGroup.Name}");
                }
                else
                {
                    Console.WriteLine("Nombre del grupo de ahorro: null");
                }
            }

            var SavingGroupsArray = SavingGroups
            .Where(x => x != null) // Filtrar elementos nulos
            .Select(x => x.Name)
            .ToArray();

            if (SavingGroupsArray != null && SavingGroupsArray.Length > 0)
            {
                var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("Selecciona un grupo de ahorro")
                    .AddChoices(SavingGroupsArray));

                var selectedSavingGroup = SavingGroups.SingleOrDefault(x => x.Name == option);

                if (selectedSavingGroup != null)
                {
                    return selectedSavingGroup;
                }
                else
                {
                    Console.WriteLine("Grupo de ahorro no encontrado.");
                    Console.ReadLine();
                    return null;
                }
            }
            else
            {
                Console.WriteLine("No se encontraron grupos de ahorro.");
                Console.ReadLine();
                return null;
            }
        }
    }
}
