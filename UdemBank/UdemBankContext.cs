using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemBank
{
    internal class UdemBankContext:DbContext
    {
        public DbSet<Saving> Savings { get; set; }
        public DbSet<SavingGroup> SavingGroups { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql("Server=127.0.0.1;Port=3306;Database=clean_code;User=root;Password=Guernica1603;", new MySqlServerVersion(new Version(8, 0, 34)));
        }
    }
}
