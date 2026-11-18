using System;
using System.Data.SqlClient;

namespace ADO_NET
{
    internal class Program
    {
        SqlConnection conn = null;
        public Program()
        {
            conn = null;
            conn = new SqlConnection(
                @"Data Source=.\SQLDEVELOPER;" +
                @"Initial Catalog=Library;" +
                @"Integrated Security=True;" +
                @"TrustServerCertificate=True;");
        }

        static void Main()
        {
            Program pr = new Program();
            //pr.InsertQuery();

            pr.ReadData(); 
        }

        public void ReadData()
        {
            SqlDataReader rdr = null;
            try
            {
                conn.Open();
                SqlCommand cmd1 = new SqlCommand("select * from Authors", conn);
                rdr = cmd1.ExecuteReader();
                while (rdr.Read())
                {
                    Console.WriteLine(rdr[1] + " " + rdr[2]);
                }
            }
            finally
            {
                if (rdr != null)
                    rdr.Close();
                if (conn != null)
                    rdr.Close();
            }
        }

        public void InsertQuery()
        {
            try
            {
                conn.Open();
                string insertString = @"insert into Authors
                             (FirstName, LastName)
                             values ('Roger', 'Zelazny');
                             insert into Authors
                             (FirstName, LastName)
                             values ('Sahrutdin', 'Kurbanov')";
                SqlCommand cmd = new SqlCommand(insertString, conn);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }
    }
}