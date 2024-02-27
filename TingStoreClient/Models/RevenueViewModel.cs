using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TingStoreClient.Models
{
    public class RevenueViewModel
    {
        public decimal DailyRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }
    }
}