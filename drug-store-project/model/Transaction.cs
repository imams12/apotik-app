using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_project.model
{
    public class TransactionProduct
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string TransactionDate { get; set; }
        public int IdOperator { get; set; }
    }
}
