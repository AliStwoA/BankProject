using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Q2_Bank.Models
{
    public class Card
    {
        [Key]
        [Required]
        [MaxLength(16), MinLength(16)]
        public string CardNumber { get; set; }
        [Required ]
        [MaxLength(50)]
        public string HolderName { get; set; }
        [Required ]
        public float Balance { get; set; }
        [Required ]
        public bool IsActive { get; set; } = true;
        [Required ]
        [MaxLength(100)]
        public string Password { get; set; }
        public int FailedAttempts { get; set; } = 0;
    }
}
