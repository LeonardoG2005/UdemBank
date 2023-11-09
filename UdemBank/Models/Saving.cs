using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemBank
{
    public class Saving
    {
        //Data annotations
        [Key]
        public int Id { get; set; }
        /*
         * Para cada FK, agregar la columna específica de la otra tabla
         * y un atributo adicional del mismo tipo. Adicionalmente, agregar la anotación FK
         */

        [Required]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]

        public User User { get; set; }

        [Required]
        public int SavingGroupId { get; set; }
        [ForeignKey(nameof(SavingGroupId))]

        public SavingGroup SavingGroup { get; set; }

        public bool Affiliation { get; set; }

        // Se agregó el atributo Investment.
        public double Investment { get; set; }
    }
}
