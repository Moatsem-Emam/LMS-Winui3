using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace LibraryManagement.Models
{

    public class Customer
    {
        [Key]
        public string CustomerID { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Book> BorrowedBooks { get; set; } = new List<Book>();
        public List<Reservation> Reservations { get; set; } = new List<Reservation>(); // Reservation List
    }
}
