using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Q2_Bank.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        [MaxLength(16), MinLength(16)]
        public string SourceCardNumber { get; set; }

        [Required]
        [MaxLength(16), MinLength(16)]
        public string DestinationCardNumber { get; set; }

        [Required]
        public float Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public bool IsSuccessful { get; set; }
    }
}
