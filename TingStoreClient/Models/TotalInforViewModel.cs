using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreClient.Models
{
    public class TotalInforViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<int> TotalUser { get; set; }
        
    }
}