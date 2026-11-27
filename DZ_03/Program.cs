using System;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ItemsApp
{
    public class ItemsRepository
    {
        private DbProviderFactory _factory;
        private string _connectionString;

        // Переключение СУБД по имени из App.config (Задание 2 и Задание 6)
        public void SetProviderFromConfig(string connectionName)
        {
            ConnectionStringSettings settings
                = ConfigurationManager.ConnectionStrings[connectionName];
            if (settings == null)
            {
                throw new 
                    InvalidOperationException("Строка подключения '"
                    + connectionName + "' не найдена в App.config.");
            }

            _factory = DbProviderFactories.
                GetFactory(settings.ProviderName);
            _connectionString = settings.ConnectionString;

            Console.WriteLine("\n[Система] Активная БД: "
                + connectionName + " (Провайдер: " 
                + settings.ProviderName + ")");
        }

        private DbConnection CreateConnection()
        {
            if (_factory == null)
            {
                throw new
                    InvalidOperationException("Провайдер БД не инициализирован.");
            }

            DbConnection connection = _factory.CreateConnection();
            if (connection == null)
            {
                throw new 
                    InvalidOperationException("Не удалось создать подключение.");
            }

            connection.ConnectionString = _connectionString;
            return connection;
        }

        // Задание 3 + Задание 7
        public async Task ReadAllAsync()
        {
            Stopwatch sw = Stopwatch.StartNew();

            using (DbConnection connection = CreateConnection())
            {
                await connection.OpenAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Id, Name, Type, Color, Calories FROM Items";

                    using (DbDataReader reader = await command.ExecuteReaderAsync())
                    {
                        Console.WriteLine("\n--- Содержимое таблицы Items ---");
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine(string.
                                Format("ID: {0} | {1} ({2}) | Цвет: {3} | Калории: {4}",
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetString(3),
                                reader.GetInt32(4)));
                        }
                    }
                }
            }

            sw.Stop();
            Console.WriteLine(
                string.Format("[Таймер] Запрос выполнен за: {0:F4} сек.", 
                sw.Elapsed.TotalSeconds));
        }

        // Задание 4 + Задание 7
        public async Task UpdateCaloriesAsync(int id, int newCalories)
        {
            Stopwatch sw = Stopwatch.StartNew();

            using (DbConnection connection = CreateConnection())
            {
                await connection.OpenAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Items SET Calories = @calories WHERE Id = @id";

                    DbParameter pCalories = command.CreateParameter();
                    pCalories.ParameterName = "@calories";
                    pCalories.Value = newCalories;
                    command.Parameters.Add(pCalories);

                    DbParameter pId = command.CreateParameter();
                    pId.ParameterName = "@id";
                    pId.Value = id;
                    command.Parameters.Add(pId);

                    int rows = await command.ExecuteNonQueryAsync();
                    sw.Stop();
                    Console.
                        WriteLine(
                        string.Format("\nОбновлено записей: {0}. Время: {1:F4} сек.",
                        rows, sw.Elapsed.TotalSeconds));
                }
            }
        }

        // Задание 5 + Задание 7
        public async Task DeleteAsync(int id)
        {
            Stopwatch sw = Stopwatch.StartNew();

            using (DbConnection connection = CreateConnection())
            {
                await connection.OpenAsync();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Items WHERE Id = @id";

                    DbParameter pId = command.CreateParameter();
                    pId.ParameterName = "@id";
                    pId.Value = id;
                    command.Parameters.Add(pId);

                    int rows = await command.ExecuteNonQueryAsync();
                    sw.Stop();
                    Console.WriteLine(
                        string.Format("\nУдалено записей: {0}. Время: {1:F4} сек.",
                        rows, sw.Elapsed.TotalSeconds));
                }
            }
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ItemsRepository repo = new ItemsRepository();
            repo.SetProviderFromConfig("SqlServer");

            while (true)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Показать все записи из Items (Асинхронное чтение)");
                Console.WriteLine("2. Обновить калорийность по ID (Асинхронное обновление)");
                Console.WriteLine("3. Удалить запись по ID (Асинхронное удаление)");
                Console.WriteLine("4. Переключить СУБД на MS SQL Server");
                Console.WriteLine("5. Переключить СУБД на MS Access (OleDb)");
                Console.WriteLine("0. Выход");
                Console.Write("> ");

                string choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            await repo.ReadAllAsync();
                            break;
                        case "2":
                            Console.Write("Введите ID: ");
                            int updId = int.Parse(Console.ReadLine());
                            Console.Write("Новая калорийность: ");
                            int cal = int.Parse(Console.ReadLine());
                            await repo.UpdateCaloriesAsync(updId, cal);
                            break;
                        case "3":
                            Console.Write("Введите ID для удаления: ");
                            int delId = int.Parse(Console.ReadLine());
                            await repo.DeleteAsync(delId);
                            break;
                        case "4":
                            repo.SetProviderFromConfig("SqlServer");
                            break;
                        case "5":
                            repo.SetProviderFromConfig("AccessDb");
                            break;
                        case "0":
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }
        }
    }
}

/*
 * В App.config
 * <connectionStrings>
		<add name="SqlServer"
             connectionString="Server=localhost;
			 Database=VegetablesAndFruits;
			 Trusted_Connection=True;"
             providerName="System.Data.SqlClient" />

		<add name="AccessDb"
             connectionString="Provider=Microsoft.ACE.OLEDB.12.0;Data Source=fruits.accdb;"
             providerName="System.Data.OleDb" />
	</connectionStrings>
 */