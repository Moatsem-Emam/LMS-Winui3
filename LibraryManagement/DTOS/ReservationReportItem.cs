using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.DTOS
{
    public class ReservationReportItem
    {
        public string BookTitle { get; set; }
        public string Customer { get; set; }
        public DateTime ReservationDate { get; set; }
    }
}
