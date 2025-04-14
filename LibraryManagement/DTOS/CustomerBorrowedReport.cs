using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.DTOS
{
    public class CustomerBorrowedReport
    {
        public string DisplayName { get; set; }
        public List<string> BorrowedBooks { get; set; } = new();
    }
}
