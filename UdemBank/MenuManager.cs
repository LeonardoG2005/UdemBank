using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Security;
using Spectre.Console;
using UdemBank.Controllers;
using UdemBank.Services;

namespace UdemBank
{
    public class MenuManager
    {
        public enum UserType
        {
            Usuario,
            EquipoFidelizacion
        }
        public enum UserMenuOption
        {
            VerInfo,
            AgregarDineroCuenta,
            SacarDineroCuenta,
            VerOpcionesGrupoDeAhorro,
            PagarPrestamo,
            SolicitarPrestamo,
            Salir
        }
        public enum SavingGroupInitOption
        {
            CrearGrupoAhorro,
            VerOpcionesGrupoAhorro,
            Salir
        }
        public enum SavingGroupOption
        {
            SolicitarPrestamo,
            IngresarCapital,
            InvitarAmigo,
            VerMovimientosCuenta,
            VerUsuariosGrupoDeAhorro,
            DisolverGrupoDeAhorro,
            Salir
        }
        public enum FidelizacionMenuOption
        {
            PremiarGrupoAhorro,
            PremiarUsuario,
            Salir
        }
        public static void MainMenu()
        {
            while (true)
            {
                var userType = AnsiConsole.Prompt(
                    new SelectionPrompt<UserType>()
                        .Title("UdeMBank - Identificación de Usuario")
                        .PageSize(10)
                        .AddChoices(
                            UserType.Usuario,
                            UserType.EquipoFidelizacion
                        )
                );

                if (userType == UserType.Usuario)
                {
                    StartMenu();
                }
                else if (userType == UserType.EquipoFidelizacion)
                {
                    FidelizacionMenu();
                }
            }
        }
        public static void FidelizacionMenu()
        {
            while (true)
            {
                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<FidelizacionMenuOption>()
                        .Title("UdeMBank - Menú de Equipo de Fidelización")
                        .PageSize(10)
                        .AddChoices(
                            FidelizacionMenuOption.PremiarGrupoAhorro,
                            FidelizacionMenuOption.PremiarUsuario,
                            FidelizacionMenuOption.Salir
                        )
                );

                switch (option)
                {
                    case FidelizacionMenuOption.PremiarGrupoAhorro:
                        RewardService.RewardSavingGroup();
                        break;

                    case FidelizacionMenuOption.PremiarUsuario:
                        RewardService.RewardUser();
                        break;

                    case FidelizacionMenuOption.Salir:
                        return;
                }
            }
        }
        public static void StartMenu()
        {
            AnsiConsole.Clear();

            while (true)
            {
                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("UdeMBank - Menú Principal")
                        .PageSize(10)
                        .AddChoices(
                            "Iniciar Sesión",
                            "Registrarse",
                            "Salir"
                        )
                );

                switch (option)
                {
                    case "Iniciar Sesión":

                        User? user = UserController.AuthenticateUser();
                        if (user == null)
                        {
                            Console.WriteLine("El usuario no se encuentra registrado");
                            Console.ReadLine();
                            Console.Clear();
                        }
                        else
                        {
                            AnsiConsole.Clear();
                            UserMenu(user.Id);
                        }
                        break;

                    case "Registrarse":
                        UserController.AddUser();
                        AnsiConsole.Clear();
                        break;

                    case "Salir":
                        return;
                }
            }
        }
        private static void UserMenu(int Id)
        {
            AnsiConsole.Clear();

            while (true)
            {
                User? user = UserController.VerifyExistingUserById(Id);

                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<UserMenuOption>()
                        .Title("UdeMBank - Menú de Usuario")
                        .PageSize(10)
                        .AddChoices(
                            UserMenuOption.VerInfo,
                            UserMenuOption.AgregarDineroCuenta,
                            UserMenuOption.SacarDineroCuenta,
                            UserMenuOption.VerOpcionesGrupoDeAhorro,
                            UserMenuOption.PagarPrestamo,
                            UserMenuOption.SolicitarPrestamo,
                            UserMenuOption.Salir
                        )
                );

                switch (option)
                {
                    case UserMenuOption.VerInfo:
                        Console.WriteLine("Contraseña del usuario: " + user.Password);
                        Console.WriteLine("Monto en cuenta del usuario: " + user.Account);
                        Console.ReadLine();
                        AnsiConsole.Clear();
                        break;

                    case UserMenuOption.AgregarDineroCuenta:
                        var amount = AnsiConsole.Ask<int>("Ingrese la cantidad : ");
                        user = UserController.AddAmount(user, amount);
                        break;

                    case UserMenuOption.SacarDineroCuenta:
                        var DeducedAmount = AnsiConsole.Ask<int>("Ingrese la cantidad que desea deducir : ");
                        user = UserController.RemoveAmount(user, DeducedAmount);
                        break;

                    case UserMenuOption.VerOpcionesGrupoDeAhorro:
                        SavingGroupInitMenu(user.Id);
                        break;

                    case UserMenuOption.PagarPrestamo:
                        Loan? loan = ShowLoansService.GetLoanOptionInput(user);

                        if (!(loan == null))
                        {
                            user = PayLoanService.ShowInfoAndPayLoan(user, loan);
                        }
                        break;
                
                    case UserMenuOption.SolicitarPrestamo:

                        var selectedGroup = CreateLoanService.ShowAvailableSavingGroups(user);
                        if (selectedGroup != null)
                        {
                            if (UserController.IsUserInSavingGroup(user, selectedGroup))
                            {
                                CreateLoanService.CreateLoan(user, selectedGroup, 0.03);
                            }
                            else
                            {
                                CreateLoanService.CreateLoan(user, selectedGroup, 0.05);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Desde el menú ya salió que ese grupo ni existe ._.");
                        }
                        break;

                    case UserMenuOption.Salir:
                        return;
                }
            }
        }
        private static void SavingGroupInitMenu(int Id)
        {
            AnsiConsole.Clear();

            while (true)
            {
                User? user = UserController.VerifyExistingUserById(Id);

                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<SavingGroupInitOption>()
                        .Title("UdeMBank - Menú Grupo de Ahorro")
                        .PageSize(10)
                        .AddChoices(
                            SavingGroupInitOption.CrearGrupoAhorro,
                            SavingGroupInitOption.VerOpcionesGrupoAhorro,
                            SavingGroupInitOption.Salir
                        )
                );

                switch (option)
                {
                    case SavingGroupInitOption.CrearGrupoAhorro:

                        SavingGroup? SavingGroup = SavingGroupService.CheckAndAddSavingGroup(user);

                        if (!(SavingGroup == null))
                        {
                            // Aparece la relación Saving
                            SavingController.AddSaving(user, SavingGroup, true);
                        }
                        break;

                    case SavingGroupInitOption.VerOpcionesGrupoAhorro:

                        // Es necesario saber si el user es creador de un Grupo de Ahorro o no...
                        // Pero el usuario puede ser dueño de varios Grupos de Ahorro 0:
                        // AY WN

                        // Solucion --> Más menús MÁS MENÚS

                        // Solución real (sin recocha) --> hay que mostrar los grupo de ahorro en pantalla y que el usuario seleccione uno...

                        List<SavingGroup>? SavingGroups = SavingGroupController.GetSavingGroupsByUser(user);

                        if (SavingGroups == null || SavingGroups.Count == 0)
                        {
                            Console.WriteLine("El usuario no se encuentra en ningún grupo de ahorro");
                            Console.ReadLine();
                            AnsiConsole.Clear();
                            break;
                        }

                        SavingGroup = UserService.GetSavingGroupOption(user);
                        SavingGroupMenu(user.Id, SavingGroup.Id);
                        return;

                    case SavingGroupInitOption.Salir:
                        return;
                }
            }
        }
        private static void SavingGroupMenu(int Id, int savingGroupId)
        {
            while (true)
            {
                User? user = UserController.VerifyExistingUserById(Id);
                SavingGroup? SavingGroup = SavingGroupController.GetSavingGroupById(savingGroupId);

                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<SavingGroupOption>()
                        .Title("UdeMBank - Menú Grupo de Ahorro")
                        .PageSize(10)
                        .AddChoices(
                            SavingGroupOption.SolicitarPrestamo,
                            SavingGroupOption.IngresarCapital,
                            SavingGroupOption.VerMovimientosCuenta,
                            SavingGroupOption.InvitarAmigo,
                            SavingGroupOption.VerUsuariosGrupoDeAhorro,
                            SavingGroupOption.DisolverGrupoDeAhorro,
                            SavingGroupOption.Salir
                        )
                );

                switch (option)
                {
                    case SavingGroupOption.SolicitarPrestamo:

                        if (CreateLoanService.UserIsAffiliated(user, SavingGroup))
                        {
                            // si el usuario está en el grupo, el interés es del 3%
                            double interestRate = 0.03;
                            CreateLoanService.CreateLoan(user, SavingGroup, interestRate);
                            AnsiConsole.Clear();
                            break;
                        }

                        if (LoanController.CanRequestLoanFromGroup(user, SavingGroup))
                        {
                            // si el usuario NO está en el grupo, el interés es del 5%
                            double interestRate = 0.05;
                            CreateLoanService.CreateLoan(user, SavingGroup, interestRate);
                            AnsiConsole.Clear();
                            break;
                        }

                        Console.WriteLine("No se puede hacer el préstamo...");
                        Console.ReadLine();
                        break;

                    case SavingGroupOption.IngresarCapital:
                        user = TradeService.AddAmountToGroup(user, SavingGroup);
                        AnsiConsole.Clear();
                        break;

                    case SavingGroupOption.VerMovimientosCuenta:
                        //List<Trade> trades = TradeController.GetTradesForUserSavings(user.Id);
                        //UserInterface.ShowTradesHistory(trades);
                        UserInterface.ShowUserTransactionHistory(user);
                        break;

                    case SavingGroupOption.InvitarAmigo:
                        InvitationService.InviteFriend(SavingGroup);
                        break;

                    case SavingGroupOption.VerUsuariosGrupoDeAhorro:
                        UserInterface.ShowUserInfoOfSavingGroup(SavingGroup);
                        break;

                    case SavingGroupOption.DisolverGrupoDeAhorro:

                        DissolveGroupService.DissolveSavingGroup(SavingGroup);

                        return;

                    case SavingGroupOption.Salir:
                        return;
                }
            }
        }
    }
}
