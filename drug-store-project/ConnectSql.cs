using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace apotik_project
{
    class ConnectSql
    {
        public SqlConnection GetConnect()
        {
            SqlConnection Conn = new SqlConnection();
            Conn.ConnectionString = "Data source=DESKTOP-L6V31S2; initial catalog=DB_APP; integrated security=true; TrustServerCertificate=True";
            return Conn;
        }
    }
}
