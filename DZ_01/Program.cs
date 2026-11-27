using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace VegetablesAndFruitsApp
{
    internal class Program
    {
        private static SqlConnection _connection;

        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.WriteLine("\n================ МЕНЮ ================");
                Console.WriteLine("1. Подключиться к БД");
                Console.WriteLine("2. Отключиться от БД");
                Console.WriteLine("--- Задание 3 ---");
                Console.WriteLine("3. Показать всю информацию из таблицы");
                Console.WriteLine("4. Показать все названия овощей и фруктов");
                Console.WriteLine("5. Показать все цвета");
                Console.WriteLine("6. Показать максимальную калорийность");
                Console.WriteLine("7. Показать минимальную калорийность");
                Console.WriteLine("8. Показать среднюю калорийность");
                Console.WriteLine("--- Задание 4 ---");
                Console.WriteLine("9. Показать количество овощей");
                Console.WriteLine("10. Показать количество фруктов");
                Console.WriteLine("11. Показать количество по заданному цвету");
                Console.WriteLine("12. Показать количество по каждому цвету");
                Console.WriteLine("13. Показать с калорийностью НИЖЕ указанной");
                Console.WriteLine("14. Показать с калорийностью ВЫШЕ указанной");
                Console.WriteLine("15. Показать с калорийностью В ДИАПАЗОНЕ");
                Console.WriteLine("16. Показать все желтого или красного цвета");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "1": Connect(); break;
                        case "2": Disconnect(); break;
                        case "3": ShowAllItems(); break;
                        case "4": ShowAllNames(); break;
                        case "5": ShowAllColors(); break;
                        case "6": ShowScalar("SELECT MAX(Calories) FROM Items",
                            "Максимальная калорийность"); break;
                        case "7": ShowScalar("SELECT MIN(Calories) FROM Items",
                            "Минимальная калорийность"); break;
                        case "8": ShowScalar("SELECT AVG(CAST(Calories AS FLOAT)) FROM Items",
                            "Средняя калорийность"); break;
                        case "9": ShowScalar("SELECT COUNT(*) FROM Items WHERE Type = N'Овощ'",
                            "Количество овощей"); break;
                        case "10": ShowScalar("SELECT COUNT(*) FROM Items WHERE Type = N'Фрукт'",
                            "Количество фруктов"); break;
                        case "11": ShowCountByColor(); break;
                        case "12": ShowCountByEachColor(); break;
                        case "13": ShowByCaloriesComparison("<"); break;
                        case "14": ShowByCaloriesComparison(">"); break;
                        case "15": ShowByCaloriesRange(); break;
                        case "16": ShowYellowOrRed(); break;
                        case "0":
                            Disconnect();
                            return;
                        default:
                            Console.WriteLine("Неверный ввод. Попробуйте снова.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Ошибка]: " + ex.Message);
                }
            }
        }

        // Задание 2
        private static void Connect()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                Console.WriteLine("Подключение уже установлено.");
                return;
            }

            ConnectionStringSettings settings = 
                ConfigurationManager.ConnectionStrings["VegFruitsDb"];
            if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
            {
                Console.WriteLine
                    ("Ошибка: строка подключения 'VegFruitsDb' не найдена в App.config.");
                return;
            }

            _connection = new SqlConnection(settings.ConnectionString);
            _connection.Open();
            Console.WriteLine("Успешное подключение к базе данных «Овощи и фрукты»!");
        }

        private static void Disconnect()
        {
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                _connection.Close();
                Console.WriteLine("Подключение к базе данных закрыто.");
            }
            else
            {
                Console.WriteLine("Активное подключение отсутствует.");
            }
        }

        private static bool EnsureConnected()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                Console.WriteLine
                    ("Сначала необходимо подключиться к базе данных (пункт 1).");
                return false;
            }
            return true;
        }

        // Вспомогательный метод для вывода таблицы
        private static void PrintItems(SqlCommand cmd)
        {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("Записи не найдены.");
                    return;
                }

                Console.WriteLine(string.
                    Format("{0,-4} | {1,-15} | {2,-10} | {3,-12} | {4,-6}",
                    "ID", "Название", "Тип", "Цвет", "Ккал"));
                Console.WriteLine(new string('-', 57));

                while (reader.Read())
                {
                    Console.WriteLine(string.
                        Format("{0,-4} | {1,-15} | {2,-10} | {3,-12} | {4,-6}",
                        reader["Id"],
                        reader["Name"], 
                        reader["Type"], 
                        reader["Color"],
                        reader["Calories"]));
                }
            }
        }

        // Задание 3
        private static void ShowAllItems()
        {
            if (!EnsureConnected()) return;
            using (SqlCommand cmd = 
                new SqlCommand(
                    "SELECT Id, Name, Type, Color, Calories FROM Items",
                _connection))
            {
                PrintItems(cmd);
            }
        }

        private static void ShowAllNames()
        {
            if (!EnsureConnected()) return;
            using (SqlCommand cmd = new SqlCommand("SELECT Name FROM Items", 
                _connection))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Названия в базе:");
                while (reader.Read())
                {
                    Console.WriteLine("- " + reader["Name"]);
                }
            }
        }

        private static void ShowAllColors()
        {
            if (!EnsureConnected()) return;
            using (SqlCommand cmd = new SqlCommand(
                "SELECT DISTINCT Color FROM Items", _connection))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Уникальные цвета:");
                while (reader.Read())
                {
                    Console.WriteLine("- " + reader["Color"]);
                }
            }
        }

        private static void ShowScalar(string query, string label)
        {
            if (!EnsureConnected()) return;
            using (SqlCommand cmd = new SqlCommand(query, _connection))
            {
                object result = cmd.ExecuteScalar();
                Console.WriteLine(label + ": " + (result ?? "Нет данных"));
            }
        }

        // Задание 4
        private static void ShowCountByColor()
        {
            if (!EnsureConnected()) return;
            Console.Write("Введите цвет (например, Красный): ");
            string color = (Console.ReadLine() ?? "").Trim();

            using (SqlCommand cmd = 
                new SqlCommand("SELECT COUNT(*) FROM Items WHERE Color = @Color",
                _connection))
            {
                cmd.Parameters.Add("@Color", SqlDbType.NVarChar, 50).Value = color;
                int count = (int)cmd.ExecuteScalar();
                Console.WriteLine("Количество объектов цвета «"
                    + color + "»: " + count);
            }
        }

        private static void ShowCountByEachColor()
        {
            if (!EnsureConnected()) return;
            using (SqlCommand cmd = 
                new SqlCommand
                ("SELECT Color, COUNT(*) AS Cnt FROM Items GROUP BY Color",
                _connection))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Количество по каждому цвету:");
                while (reader.Read())
                {
                    Console.WriteLine("- " + reader["Color"] + ": " + reader["Cnt"]);
                }
            }
        }

        private static void ShowByCaloriesComparison(string op)
        {
            if (!EnsureConnected()) return;
            Console.Write("Введите значение калорийности: ");
            int cal;
            if (!int.TryParse(Console.ReadLine(), out cal))
            {
                Console.WriteLine("Некорректное число.");
                return;
            }

            using (SqlCommand cmd =
                new SqlCommand
                ("SELECT Id, Name, Type, Color, Calories FROM Items WHERE Calories "
                + op + " @Cal", _connection))
            {
                cmd.Parameters.AddWithValue("@Cal", cal);
                PrintItems(cmd);
            }
        }

        private static void ShowByCaloriesRange()
        {
            if (!EnsureConnected()) return;
            Console.Write("Введите минимальную калорийность: ");
            int minCal;
            if (!int.TryParse(Console.ReadLine(), out minCal)) return;
            Console.Write("Введите максимальную калорийность: ");
            int maxCal;
            if (!int.TryParse(Console.ReadLine(), out maxCal)) return;
            using (SqlCommand cmd = 
                new SqlCommand
                ("SELECT Id, Name, Type, Color, Calories FROM Items WHERE Calories BETWEEN @Min AND @Max",
                _connection))
            {
                cmd.Parameters.AddWithValue("@Min", minCal);
                cmd.Parameters.AddWithValue("@Max", maxCal);
                PrintItems(cmd);
            }
        }
        private static void ShowYellowOrRed()
        {
            if (!EnsureConnected()) return;
            using (SqlCommand cmd = 
                new SqlCommand
                ("SELECT Id, Name, Type, Color, Calories FROM Items WHERE Color IN (N'Желтый', N'Красный')", 
                _connection))
            {
                PrintItems(cmd);
            }
        }
    }
}

/*
 * В App.config
 * <connectionStrings>
		<add name="VegFruitsDb"
             connectionString="Data Source=localhost;
			 Initial Catalog=VegetablesAndFruits;Integrated Security=True;"
             providerName="System.Data.SqlClient" />
	</connectionStrings>
 */