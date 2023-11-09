using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemBank.Controllers;

namespace UdemBank.Services
{
    internal class InvitationService
    {
        public static void InviteFriend(SavingGroup SavingGroup) 
        {
            // primero hay que verificar que exista el usuario según el nombre
            var name = AnsiConsole.Ask<string>("Nombre de usuario al que desea invitar:");
            User? user = UserController.GetUserByName(name);

            if (user == null)
            {
                Console.WriteLine("Ese usuario ni existe ._.");
                Console.ReadLine();
                AnsiConsole.Clear();
                return;
            }
            // verificar que el usuario no se encuentre en el grupo de ahorro
            if (UserController.IsUserInSavingGroup(user, SavingGroup))
            {
                Console.WriteLine("El usuario ya se encuentra en el grupo :)");
                Console.ReadLine();
                AnsiConsole.Clear();
            }

            if (user != null)
            {
                List<SavingGroup> SavingGroups = SavingGroupController.GetSavingGroupsByUser(user);
                if (!(SavingGroups == null))
                {
                    if (SavingGroups.Count < 3)
                    {
                        // Debe aparecer una relación Saving
                        SavingController.AddSaving(user, SavingGroup, true);
                        Console.WriteLine("Se invitó al usuario correctamente... :)");
                        Console.ReadLine();
                        AnsiConsole.Clear();
                    }
                    else
                    {
                        Console.WriteLine("El usuario ya se encuentra en 3 grupos de ahorro...");
                        Console.ReadLine();
                        AnsiConsole.Clear();
                    }
                }
                else
                {
                    SavingController.AddSaving(user, SavingGroup, true);
                    Console.WriteLine("Se invitó al usuario correctamente... :)");
                    Console.ReadLine();
                    AnsiConsole.Clear();
                }
            }
            else
            {
                Console.WriteLine("No se encontró el usuario...");
                Console.ReadLine();
                AnsiConsole.Clear();
            }
        }
    }
}
