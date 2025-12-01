using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemBank
{
    public class UdemBankContext : DbContext
    {
        public UdemBankContext(DbContextOptions<UdemBankContext> options) : base(options)
        {
        }

        public DbSet<Saving> Savings { get; set; }
        public DbSet<SavingGroup> SavingGroups { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=udem_bank.db");
            }
        }
    }
}
