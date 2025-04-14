using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.DTOS
{
    public class LateFeeReportItem
    {
        public string BookTitle { get; set; }
        public string CustomerName { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Fee { get; set; }

        public string DueDateFormatted => $"Due: {DueDate:yyyy-MM-dd}";
        public string FeeFormatted => $"Late Fee: ${Fee}";
    }
}
