using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Models
{
    public class Reservation

    {

        [Key]
        public int ReservationId { get; set; }

        [Required]

        public string BookISBN { get; set; }

        [ForeignKey("BookISBN")]

        public Book Book { get; set; }

        [Required]
        public string CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public DateTime ReservationDate { get; set; }

    }
}
