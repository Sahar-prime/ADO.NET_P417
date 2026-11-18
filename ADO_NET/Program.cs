using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ADO_NET
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SqlConnection conn = null;
            conn = new SqlConnection();
            conn.ConnectionString = $"Data Source=./SQLDEVELOPER;" +
                $"Initial Catalog=Library;" +
                $"Integrated Security=True;TrustServerCertificate=True;";
            string insertString = @"insert into Author
                                    (FirstName, LastName)
                                    values ('Roger', 'Zelezny')";
            SqlCommand cmd = new SqlCommand(insertString, conn);
            //или
            string insertString1 = @"insert into Author
                                    (FirstName, LastName)
                                    values ('Roger', 'Zelezny')";
            SqlCommand cmd1 = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = insertString;
        }
    }
}
