using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apotik_project.model
{
    public class TransactionDetail
    {
        public int Id { get; set; }
        public string Id_Product { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int Sub_total { get; set; }
    }

}
