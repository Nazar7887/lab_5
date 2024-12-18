using System;
using System.Collections.Generic;
using System.IO;

namespace lr5
{
        enum ProductType
        {
            Electronics,
            Clothing,
            Groceries,
            Furniture
        }

        struct Product
        {
            public int WarehouseNumber;
            public ProductType Type;
            public string ProductCode;
            public string Name;
            public DateTime DateAdded;
            public int ShelfLifeDays;
            public int Quantity;
            public double UnitPrice;

            public Product(int warehouseNumber, ProductType type, string productCode, string name, DateTime dateAdded, int shelfLifeDays, int quantity, double unitPrice)
            {
                WarehouseNumber = warehouseNumber;
                Type = type;
                ProductCode = productCode;
                Name = name;
                DateAdded = dateAdded;
                ShelfLifeDays = shelfLifeDays;
                Quantity = quantity;
                UnitPrice = unitPrice;
            }

            public override string ToString()
            {
                return $"{WarehouseNumber} | {Type} | {ProductCode} | {Name} | {DateAdded.ToShortDateString()} | {ShelfLifeDays} days | {Quantity} | {UnitPrice:C}";
            }
        }

        class Program
        {
            const string FileName = "products.txt";

            // Запис у файл
            public static void WriteToFile(List<Product> products)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(FileName, true))
                    {
                        foreach (var product in products)
                        {
                            sw.WriteLine($"{product.WarehouseNumber};{product.Type};{product.ProductCode};{product.Name};{product.DateAdded.ToString("yyyy-MM-dd")};{product.ShelfLifeDays};{product.Quantity};{product.UnitPrice}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при запису в файл: {ex.Message}");
                }
            }

            public static List<Product> ReadFromFile()
            {
                List<Product> products = new List<Product>();
                try
                {
                    if (File.Exists(FileName))
                    {
                        using (StreamReader sr = new StreamReader(FileName))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                var fields = line.Split(';');
                                products.Add(new Product(
                                    int.Parse(fields[0]),
                                    Enum.Parse<ProductType>(fields[1]),
                                    fields[2],
                                    fields[3],
                                    DateTime.Parse(fields[4]),
                                    int.Parse(fields[5]),
                                    int.Parse(fields[6]),
                                    double.Parse(fields[7])
                                ));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при читанні з файлу: {ex.Message}");
                }
                return products;
            }

            public static List<Product> SearchProducts(List<Product> products, int? warehouseNumber = null, ProductType? type = null, string name = null)
            {
                List<Product> result = new List<Product>();

                foreach (var product in products)
                {
                    if ((warehouseNumber == null || product.WarehouseNumber == warehouseNumber) &&
                        (type == null || product.Type == type) &&
                        (name == null || product.Name.Contains(name, StringComparison.OrdinalIgnoreCase)))
                    {
                        result.Add(product);
                    }
                }

                return result;
            }

            public static Product GetProductFromUser()
            {
                try
                {
                    Console.WriteLine("Введіть номер складу:");
                    int warehouseNumber = int.Parse(Console.ReadLine());

                    Console.WriteLine("Виберіть тип товару (0 - Electronics, 1 - Clothing, 2 - Groceries, 3 - Furniture):");
                    ProductType type = (ProductType)Enum.Parse(typeof(ProductType), Console.ReadLine());

                    Console.WriteLine("Введіть код товару:");
                    string productCode = Console.ReadLine();

                    Console.WriteLine("Введіть найменування товару:");
                    string name = Console.ReadLine();

                    Console.WriteLine("Введіть дату появи на складі (yyyy-MM-dd):");
                    DateTime dateAdded = DateTime.Parse(Console.ReadLine());

                    Console.WriteLine("Введіть термін збереження (в днях):");
                    int shelfLifeDays = int.Parse(Console.ReadLine());

                    Console.WriteLine("Введіть кількість одиниць товару:");
                    int quantity = int.Parse(Console.ReadLine());

                    Console.WriteLine("Введіть ціну за одиницю:");
                    double unitPrice = double.Parse(Console.ReadLine());

                    return new Product(warehouseNumber, type, productCode, name, dateAdded, shelfLifeDays, quantity, unitPrice);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при введенні даних: {ex.Message}");
                    return new Product();
                }
            }

            public static void PrintProducts(List<Product> products)
            {
                foreach (var product in products)
                {
                    Console.WriteLine(product.ToString());
                }
            }

            static void Main(string[] args)
            {
                Console.WriteLine("Введіть кількість товарів для запису у файл:");
                int count = int.Parse(Console.ReadLine());

                List<Product> products = new List<Product>();

                for (int i = 0; i < count; i++)
                {
                    products.Add(GetProductFromUser());
                }

                WriteToFile(products);

                List<Product> readProducts = ReadFromFile();
                Console.WriteLine("\nПрочитані товари з файлу:");
                PrintProducts(readProducts);

                Console.WriteLine("\nПошук товарів по параметрах:");
                Console.WriteLine("Введіть номер складу для пошуку (або залиште порожнім для пропуску):");
                string warehouseInput = Console.ReadLine();
                int? warehouseNumber = string.IsNullOrEmpty(warehouseInput) ? (int?)null : int.Parse(warehouseInput);

                Console.WriteLine("Введіть тип товару для пошуку (0 - Electronics, 1 - Clothing, 2 - Groceries, 3 - Furniture, або залиште порожнім для пропуску):");
                string typeInput = Console.ReadLine();
                ProductType? type = string.IsNullOrEmpty(typeInput) ? (ProductType?)null : (ProductType)Enum.Parse(typeof(ProductType), typeInput);

                Console.WriteLine("Введіть найменування товару для пошуку (або залиште порожнім для пропуску):");
                string name = Console.ReadLine();

                var searchResult = SearchProducts(readProducts, warehouseNumber, type, name);
                PrintProducts(searchResult);
            }
        }
}
