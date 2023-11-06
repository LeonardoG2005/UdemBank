using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemBank.Controllers;

namespace UdemBank
{
    public class SavingGroupService
    {
        public static SavingGroup? CheckAndAddSavingGroup(User user)
        {
            List<SavingGroup> savingGroups = SavingGroupController.GetSavingGroupsByUser(user); 

            if (savingGroups != null)
            {
                if (!(savingGroups.Count < 3))
                {
                    Console.WriteLine("El usuario ya pertenece a 3 grupos de ahorro y no se puede unir a más.");
                    Console.ReadLine();
                    Console.Clear();
                    return null;
                }
            }
            SavingGroup SavingGroup = SavingGroupController.AddSavingGroup(user);
            return SavingGroup;
        }
    }
}
