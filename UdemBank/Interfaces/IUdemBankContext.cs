using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemBank
{
    public interface IUdemBankContext
    {
        DbSet<Saving> Savings { get; set; }
        DbSet<SavingGroup> SavingGroups { get; set; }
        DbSet<Trade> Trades { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Loan> Loans { get; set; }

        int SaveChanges();
    }

}
