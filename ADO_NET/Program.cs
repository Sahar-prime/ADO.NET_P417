using System;
using System.Data.SqlClient;
using System.Configuration;

namespace ADO_NET
{
    internal class Program
    {
        SqlConnection conn = null;
        public Program()
        {
            conn = 
                new SqlConnection
                (ConfigurationManager.ConnectionStrings["MyconnString"]
                .ConnectionString);
            //conn.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
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
                SqlCommand cmd1 =
                    new SqlCommand("select * from Authors; select * from Books", 
                    conn);
                rdr = cmd1.ExecuteReader();
                int line = 0;
                do
                {
                    while (rdr.Read())
                    {
                        if (line == 0)
                        {
                            for (int i = 0; i < rdr.FieldCount; i++)
                            {
                                Console.WriteLine(rdr.GetName(i).ToString() + "\t");
                            }
                        }
                        Console.WriteLine();
                        line++;
                        Console.WriteLine(rdr[0] + "\t" + rdr[1] + "\t" + rdr[2]);
                    }
                } while (rdr.NextResult());
                Console.WriteLine();
                Console.WriteLine("Обработано записей: " + line.ToString());
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

                cmd.Parameters.AddWithValue("@p1", "firstname");

                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void ExecStoredProcedure() 
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("GetBooksNumber1", conn);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add("@AuthorsId", System.Data.SqlDbType.Int).Value = 1;
            SqlParameter outputParam = new SqlParameter(
                "@BookCount", System.Data.SqlDbType.Int);
            outputParam.Direction = System.Data.ParameterDirection.Output;
            //outputParam.Value = 0; //Так делать не надо!!!
            cmd.Parameters.Add(outputParam);
            cmd.ExecuteNonQuery();
            Console.WriteLine(cmd.Parameters["@BookCount"].Value.ToString());
        }
    }
}

/*
 * В App.config
 * <connectionStrings>
		<add name="MyconnString" connectionString="Data Source=localhost;
			 Initial Catalog=Library;
			 Integrated Security=True;TrustServerCertificate=True;"/>
	</connectionStrings>
 */