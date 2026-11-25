using System;
using System.Configuration;
using System.Data.SqlClient;

class Program
{
    static string connectionString =
        ConfigurationManager.
        ConnectionStrings["WarehouseConnectionString"].
        ConnectionString;

    static void Main(string[] args)
    {
        try
        {
            ClearDatabase();
            Console.ReadKey();

            Console.WriteLine("--- Задание 1: Вставка данных ---");
            int typeId = InsertProductType("Электроника");
            int supplierId = InsertSupplier("ТехноПром");
            InsertProduct("Ноутбук", typeId, supplierId, 15, 45000.00m, DateTime.Now.AddDays(-10));

            int typeId2 = InsertProductType("Канцелярия");
            int supplierId2 = InsertSupplier("ОфисМир");
            InsertProduct("Ручка", typeId2, supplierId2, 100, 15.50m, DateTime.Now.AddDays(-40));
            InsertProduct("Бумага А4", typeId2, supplierId2, 50, 320.00m, DateTime.Now.AddDays(-5));

            Console.WriteLine("\n--- Задание 2: Обновление данных ---");
            UpdateProductType(typeId, "Бытовая электроника");
            UpdateSupplier(supplierId, "ТехноПром Групп");
            UpdateProduct(1, "Игровой Ноутбук", typeId, supplierId, 12, 50000.00m, DateTime.Now.AddDays(-10));

            Console.WriteLine("\n--- Задание 4: Выборка и агрегация ---");
            ShowSupplierWithMaxProducts();
            ShowSupplierWithMinProducts();
            ShowTypeWithMaxProducts();
            ShowTypeWithMinProducts();
            ShowProductsByDaysPassed(30);

            Console.WriteLine("\n--- Задание 3: Удаление данных ---");
            DeleteProduct(1);
            DeleteSupplier(supplierId);
            DeleteProductType(typeId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    // Задание 1
    static int InsertProductType(string name)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO ProductTypes (TypeName) OUTPUT INSERTED.Id VALUES (@name)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", name);
            conn.Open();
            int id = (int)cmd.ExecuteScalar();
            Console.WriteLine($"Тип товара '{name}' добавлен с ID: {id}");
            return id;
        }
    }

    static int InsertSupplier(string name)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO Suppliers (SupplierName) OUTPUT INSERTED.Id VALUES (@name)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", name);
            conn.Open();
            int id = (int)cmd.ExecuteScalar();
            Console.WriteLine($"Поставщик '{name}' добавлен с ID: {id}");
            return id;
        }
    }

    static void InsertProduct(string name, int typeId, int supplierId, 
        int qty, decimal price, DateTime date)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query =
                "INSERT INTO Products (ProductName, ProductTypeId, SupplierId, Quantity, CostPrice, DeliveryDate) " +
                           "VALUES (@name, @typeId, @supplierId, @qty, @price, @date)";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@typeId", typeId);
            cmd.Parameters.AddWithValue("@supplierId", supplierId);
            cmd.Parameters.AddWithValue("@qty", qty);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@date", date);
            conn.Open();
            cmd.ExecuteNonQuery();
            Console.WriteLine($"Товар '{name}' успешно добавлен.");
        }
    }

    //Задание 2
    static void UpdateProductType(int id, string newName)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "UPDATE ProductTypes SET TypeName = @name WHERE Id = @id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", newName);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? $"Тип с ID {id} обновлен." : "Тип не найден.");
        }
    }

    static void UpdateSupplier(int id, string newName)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "UPDATE Suppliers SET SupplierName = @name WHERE Id = @id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", newName);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? $"Поставщик с ID {id} обновлен." : 
                "Поставщик не найден.");
        }
    }

    static void UpdateProduct(int id, string name, int typeId, int supplierId,
        int qty, decimal price, DateTime date)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "UPDATE Products SET ProductName = @name, ProductTypeId = @typeId, SupplierId = @supplierId, " +
                           "Quantity = @qty, CostPrice = @price, DeliveryDate = @date WHERE Id = @id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@typeId", typeId);
            cmd.Parameters.AddWithValue("@supplierId", supplierId);
            cmd.Parameters.AddWithValue("@qty", qty);
            cmd.Parameters.AddWithValue("@price", price);
            cmd.Parameters.AddWithValue("@date", date);
            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? $"Товар с ID {id} обновлен." :
                "Товар не найден.");
        }
    }

    // Задание 3
    static void DeleteProduct(int id)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM Products WHERE Id = @id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? $"Товар с ID {id} удален." : "Товар не найден.");
        }
    }

    static void DeleteSupplier(int id)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM Suppliers WHERE Id = @id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? $"Поставщик с ID {id} удален." : "Поставщик не найден.");
        }
    }

    static void DeleteProductType(int id)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM ProductTypes WHERE Id = @id";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? $"Тип с ID {id} удален." : "Тип не найден.");
        }
    }

    // Задание 4
    static void ShowSupplierWithMaxProducts()
    {
        ExecuteAndPrintScalar(
            "SELECT TOP 1 s.SupplierName, SUM(p.Quantity) as Total FROM Suppliers s " +
            "JOIN Products p ON s.Id = p.SupplierId GROUP BY s.SupplierName ORDER BY Total DESC",
            "Поставщик с наибольшим кол-вом товаров: ");
    }

    static void ShowSupplierWithMinProducts()
    {
        ExecuteAndPrintScalar(
            "SELECT TOP 1 s.SupplierName, SUM(p.Quantity) as Total FROM Suppliers s " +
            "JOIN Products p ON s.Id = p.SupplierId GROUP BY s.SupplierName ORDER BY Total ASC",
            "Поставщик с наименьшим кол-вом товаров: ");
    }

    static void ShowTypeWithMaxProducts()
    {
        ExecuteAndPrintScalar(
            "SELECT TOP 1 t.TypeName, SUM(p.Quantity) as Total FROM ProductTypes t " +
            "JOIN Products p ON t.Id = p.ProductTypeId GROUP BY t.TypeName ORDER BY Total DESC",
            "Тип товаров с наибольшим кол-вом: ");
    }

    static void ShowTypeWithMinProducts()
    {
        ExecuteAndPrintScalar(
            "SELECT TOP 1 t.TypeName, SUM(p.Quantity) as Total FROM ProductTypes t " +
            "JOIN Products p ON t.Id = p.ProductTypeId GROUP BY t.TypeName ORDER BY Total ASC",
            "Тип товаров с наименьшим кол-вом: ");
    }

    static void ShowProductsByDaysPassed(int days)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "SELECT ProductName, DeliveryDate, Quantity FROM Products WHERE DATEDIFF(day, DeliveryDate, GETDATE()) >= @days";
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@days", days);

            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine($"\nТовары, с поставки которых прошло более {days} дней:");
                if (!reader.HasRows) Console.WriteLine("Таких товаров нет.");
                while (reader.Read())
                {
                    Console.WriteLine($"- {reader["ProductName"]}" +
                        $" (Дата поставки: {((DateTime)reader["DeliveryDate"]).ToShortDateString()}" +
                        $", Кол-во: {reader["Quantity"]})");
                }
            }
        }
    }

    // Вспомогательный метод для выполнения запросов и вывода информации
    static void ExecuteAndPrintScalar(string query, string messagePrefix)
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    Console.WriteLine($"{messagePrefix}{reader[0]} (Количество: {reader[1]})");
                }
                else
                {
                    Console.WriteLine($"{messagePrefix}Данные отсутствуют.");
                }
            }
        }
    }

    static void ClearDatabase()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = @"
                DELETE FROM Products;
                DELETE FROM Suppliers;
                DELETE FROM ProductTypes;
                DBCC CHECKIDENT ('Products', RESEED, 0);
                DBCC CHECKIDENT ('Suppliers', RESEED, 0);
                DBCC CHECKIDENT ('ProductTypes', RESEED, 0);";
            

            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();
            cmd.ExecuteNonQuery();
            Console.WriteLine("База данных успешно очищена. Начинаем чистый запуск...\n");
        }
    }

}