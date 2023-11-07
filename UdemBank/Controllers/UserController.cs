using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Tls;
using Piranha;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UdemBank;

namespace UdemBank.Controllers
{
    internal class UserController
    {
        public static User? VerifyExistingUser(string username)
        {
            using var db = new UdemBankContext(); //Conexión a la BD --> contexto

            var existingUser = db.Users.SingleOrDefault(u => u.Name == username);

            if (existingUser != null)
            {
                return existingUser;
            }
            return null;
        }
        public static User? VerifyExistingUserById(int userId)
        {
            using var db = new UdemBankContext(); // Conexión a la BD --> contexto

            var existingUser = db.Users.SingleOrDefault(u => u.Id == userId);

            if (existingUser != null)
            {
                return existingUser;
            }
            Console.WriteLine("HOLY FUCK EL USUARIO ES NULO QUE");
            Console.ReadLine();
            return null;
        }
        public static void AddUser()
        {
            List<string> userInfo = UserService.GetUserLogInInput();

            string name = userInfo[0];
            string password = userInfo[1];

            var account = AnsiConsole.Ask<int>("Cantidad inicial en la cuenta:");

            using var db = new UdemBankContext(); //Conexión a la BD --> contexto

            if (!(VerifyExistingUser(name) == null))
            {
                Console.WriteLine("El usuario ya se encuentra registrado...");
                Console.ReadLine();
                Console.Clear();
                return;
            }

            //No necesitamos el Id porque entity framework lo asigna y autoincrementa --> revisar migración
            db.Add(new User { Name = name , Password = password, Account = account });

            //Surgirá un error típico porque EF no encuentra la BD en su carpeta por defecto
            //Cambiar directorio de trabajo (flechita hacia abajo del botón run -> Propiedades de depuración)
            //Agregar el full path del proyecto --> clic derecho en el proyecto, copiar ruta completa
            //Esto crea la carpeta Properties --> launchsettings.json
            db.SaveChanges();
        }
        public static User? AuthenticateUser()
        {
            List<string> userInfo = UserService.GetUserLogInInput();

            string name = userInfo[0];
            string password = userInfo[1];

            using var db = new UdemBankContext();

            // busqueda por nombre y contraseña...
            var user = db.Users.SingleOrDefault(u => u.Name == name && u.Password == password);

            if (user == null)
            {
                return null;
            }
            return user;
        }
        public static User? AddAmount(User user, double amount)
        {
            using var db = new UdemBankContext();
            if (!(user == null))
            {
                var existingUser = db.Users.SingleOrDefault(u => u.Id == user.Id);

                if (existingUser == null)
                {
                    Console.WriteLine("El existingUser es null");
                    Console.ReadLine();
                    AnsiConsole.Clear();
                    return null;
                }
                existingUser.Account += amount;
                db.SaveChanges();
                AnsiConsole.Clear();
                return existingUser;
            }
            Console.WriteLine("Al addAmount del UserController se le mandó un user == null");
            Console.ReadLine();
            AnsiConsole.Clear();
            return null;
        }

        public static User? RemoveAmount(User user, double amount) 
        {
            using var db = new UdemBankContext();

            var existingUser = db.Users.SingleOrDefault(u => u.Id == user.Id);

            if (existingUser == null)
            {
                Console.WriteLine("El existingUser es null");
                Console.ReadLine();
                AnsiConsole.Clear();
                return null;
            }

            if (existingUser.Account >= amount)
            {
                existingUser.Account -= amount; // Deduce la cantidad.
                db.SaveChanges(); // Guarda los cambios en la base de datos.
                AnsiConsole.Clear();
                return existingUser;
            }
            else
            {
                Console.WriteLine("No se puede deducir la cantidad ...");
                Console.ReadLine();
                AnsiConsole.Clear();
            }
            return null;
        }
        public static User? RewardUser(User user)
        {
            using (var db = new UdemBankContext())
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Id == user.Id);

                if (existingUser != null)
                {
                    existingUser.Rewarded = true;
                    db.SaveChanges();
                    return existingUser;
                }
                else
                {
                    Console.WriteLine("El RewardUser dice que el user es null (mero loko)");
                    Console.ReadLine();
                    AnsiConsole.Clear();
                    return user;
                }
            }
        }

        //método que devuelve todos los usuarios que pertenecen a grupos en los que está un usuario
        public static List<User>? GetUsersInEligibleSavingGroups(User user)
        {
            using (var db = new UdemBankContext())
            {
                // Paso 1 : Se obtienen todos los grupos de ahorro a los que pertenece el usuario.
                var UserSavingGroups = SavingGroupController.GetSavingGroupsByUser(user);

                if (UserSavingGroups != null)
                {
                    // Paso 2: Los Id's 
                    var userSavingGroupIds = UserSavingGroups.Select(group => group.Id).ToList();


                    // Paso 3 : Obtener todos los Savings asociados a los grupos de ahorro en los que se encuentra el usuario
                    var savingsInUserGroups = db.Savings
                        .Where(s => userSavingGroupIds.Contains(s.SavingGroupId))
                        .Include(s => s.User) // Incluir el objeto User relacionado
                        .ToList();

                    // Paso 4 : Id's again 
                    var userIdsInUserGroups = savingsInUserGroups.Select(s => s.UserId).ToList();

                    // Paso 5 : Obtener todos los usuarios que se encuentran en grupos de ahorro a los que pertenece el usuario
                    var usersInUserGroups = db.Users
                        .Where(u => userIdsInUserGroups.Contains(u.Id))
                        .ToList();

                    // Retornar la lista de usuarios que se encuentran en los grupos de ahorro a los que pertenece el usuario.
                    return usersInUserGroups;
                }
                else
                {
                    Console.WriteLine("El usuario no se encuentra en nigún grupo de ahorro...");
                    Console.ReadLine();
                    Console.Clear();
                    return null;
                }
            }
        }
        public static void DeleteUser()
        {
            throw new NotImplementedException();
        }

        public static User? GetUserById(int Id)
        {
            using var db = new UdemBankContext();
            var user = db.Users.SingleOrDefault(b => b.Id == Id);
            return user;
        }
        public static User? GetUserByName(string name)
        {
            using var db = new UdemBankContext();
            var user = db.Users.SingleOrDefault(b => b.Name == name);
            return user;
        }
        public static List<User> GetUsers()
        {
            using var db = new UdemBankContext();
            var users = db.Users.ToList();
            return users;
        }
    }
}
