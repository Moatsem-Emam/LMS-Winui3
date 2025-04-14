using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LibraryManagement.Models
{

    public class Book
    {
        [Key]
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool IsBorrowed => BorrowedBy != null;
        public string Avilability => IsAvilable();
        public string? BorrowedBy { get; set; } // Nullable Foreign Key
        public Customer Customer { get; set; } // Navigation Property

        public DateTime? DueDate { get; set; } // When the book should be returned
        public List<Reservation> Reservations { get; set; } = new List<Reservation>(); // Reservation List
        public int LateFee { get; set; }
        public decimal LateFeePerDay => CalculateLateFee();
      
        private decimal CalculateLateFee()
        {
            if (DueDate.HasValue && DueDate.Value < DateTime.Now)
            {
                int overdueDays = (DateTime.Now - DueDate.Value).Days;
                return overdueDays * 10m; // $10 per day late fee 

            }
            return 0;
        }
        private string IsAvilable()
        {
            return (BorrowedBy == null) ? "✅ Avilable" : "❌ Not Avilable";
        }
    }

}
