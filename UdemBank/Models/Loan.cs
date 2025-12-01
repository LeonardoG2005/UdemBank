using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UdemBank.Enums;

namespace UdemBank
{
    public class Loan : ITransaction 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SavingId { get; set; }
        [ForeignKey(nameof(SavingId))]

        public Saving Saving { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public DateTimeOffset Date { get; set; }

        public TradeType Type { get; set; }

        [Required]
        public DateOnly DueDate { get; set; }

        public double CurrentBalance { get; set; }

        [Required]
        public bool Paid { get; set; }
    }
}
