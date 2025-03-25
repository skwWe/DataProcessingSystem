using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;



namespace DataProcessingSystem
{
    
    [XmlRoot("MedicalRecords")]
    public class MedicalRecord
    {
        [XmlElement("id")]
        public int id { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Age")]
        public int Age { get; set; }
        [XmlElement("Diagnosis")]
        public string Diagnosis { get; set; }

    }


    //Сделал интерфейс в который закинул
    public interface IDataProcessor<T>
    {
        List<T> ReadData(string FilePath);
        void WriteData(string FilePath, List<T> data);

        List<T> SortData(List<T> data, Func<T, object> keySelector);
        List<T> SearchData(List<T> data, Func<T, bool> predicate);


    }

    public abstract class BaseDataProcessor<T> : IDataProcessor<T>
    {
        public abstract List<T> ReadData(string filePath);
        public abstract void WriteData(string filePath, List<T> data);

        public List<T> SortData(List<T> data, Func<T, object> keySelector)
        {
            return data.OrderBy(keySelector).ToList();
        }

        public List<T> SearchData(List<T> data, Func<T, bool> predicate)
        {
            return data.Where(predicate).ToList();
        }
    }

    //Сделал JSON-обработчик
    public class JsonDataProcessor<T> : BaseDataProcessor<T>
    {
        public override List<T> ReadData(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }

        public override void WriteData(string filePath, List<T> data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }

    public class CsvDataProcessor<T> : BaseDataProcessor<T>
    {
        public override List<T> ReadData(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            var reader = new StreamReader(filePath);
            var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));
            return csv.GetRecords<T>().ToList();
        }

        public override void WriteData(string filePath, List<T> data)
        {
            var writer = new StreamWriter(filePath);
            var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
            csv.WriteRecords(data);
        }
    }

    public class XmlDataProcessor<T> : BaseDataProcessor<T>
    {
        public override List<T> ReadData(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            var serializer = new XmlSerializer(typeof(List<T>));
            var reader = new StreamReader(filePath);
            return (List<T>)serializer.Deserialize(reader);
        }

        public override void WriteData(string filePath, List<T> data)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            var writer = new StreamWriter(filePath);
            serializer.Serialize(writer, data);
        }
    }

    public class YamlDataProcessor<T> : BaseDataProcessor<T>
    {
        public override List<T> ReadData(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string yamlContent = File.ReadAllText(filePath);
            return deserializer.Deserialize<List<T>>(yamlContent);
        }

        public override void WriteData(string filePath, List<T> data)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string yamlContent = serializer.Serialize(data);
            File.WriteAllText(filePath, yamlContent);
        }

    }


    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine($"Текущая директория: {Directory.GetCurrentDirectory()}");
            Console.WriteLine("Выберите формат файла (1 - JSON, 2 - CSV, 3 - XML, 4 - YAML");

            string choice = Console.ReadLine();
            string FilePath = "";
            BaseDataProcessor<MedicalRecord> processor;

            switch (choice)
            {
                case "1":
                    processor = new JsonDataProcessor<MedicalRecord>();
                    FilePath = "MedicalRecord.json";
                    break;
                case "2":
                    processor = new CsvDataProcessor<MedicalRecord>();
                    FilePath = "MedicalRecord.csv";
                    break;
                case "3":
                    processor = new XmlDataProcessor<MedicalRecord>();
                    FilePath = "MedicalRecord.xml";
                    break;
                case "4":
                    processor = new YamlDataProcessor<MedicalRecord>();
                    FilePath = "MedicalRecord.yaml";
                    break;
                default:
                    Console.WriteLine("Неверный ввод, выход из программы.");
                    return;
            }

            List<MedicalRecord> medicalRecords = processor.ReadData(FilePath);

            while (true)
            {
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1. Вывести данные");
                Console.WriteLine("2. Добавить запись");
                Console.WriteLine("3. Удалить запись");
                Console.WriteLine("4. Изменить запись");
                Console.WriteLine("5. Поиск");
                Console.WriteLine("6. Сортировка");
                Console.WriteLine("7. Записать данные в файл");
                Console.WriteLine("8. Выйти");

                Console.WriteLine("Выберите действие: ");

                string action = Console.ReadLine();

                switch (action)
                {
                    case "1":
                        PrintRecords(medicalRecords);
                        break;
                    case "2":
                        AddRecord(medicalRecords);
                        break;
                    case "3":
                        DeleteRecord(medicalRecords);
                        break;
                    case "4":
                        EditRecord(medicalRecords);
                        break;
                    case "5":
                        SearchRecords(medicalRecords);
                        break;
                    case "6":
                        SortRecords(medicalRecords);
                        break;
                    case "7":
                        processor.WriteData(FilePath, medicalRecords);
                        Console.WriteLine("Данные сохранены!");
                        break;
                }
                }
                //    Console.WriteLine("Введите поисковый запрос:");
                //string searchQuery = Console.ReadLine();

                //List<MedicalRecord> searchResults = processor.SearchData(medicalRecords,
                //    record => record.Name.Contains(searchQuery) ||
                //              record.Diagnosis.Contains(searchQuery));

                //if (searchResults.Count > 0)
                //{
                //    Console.WriteLine("Результаты поиска:");
                //    foreach (var record in searchResults)
                //    {
                //        Console.WriteLine($"ID: {record.id}, Имя: {record.Name}, Возраст: {record.Age}, Диагноз: {record.Diagnosis}");
                //    }
                //}
                //else
                //{
                //    Console.WriteLine("Совпадений не найдено.");
                //}


                //Console.WriteLine("Сортировка по (1 - id, 2 - Имени, 3 - Возврасту)");
                //string sortChoice = Console.ReadLine();

                //switch (sortChoice)
                //{
                //    case "1":
                //        medicalRecords = processor.SortData(medicalRecords, record => record.id);
                //        break;
                //    case "2":
                //        medicalRecords = processor.SortData(medicalRecords, record => record.Name);
                //        break;
                //    case "3":
                //        medicalRecords = processor.SortData(medicalRecords, record => record.Age);
                //        break;
                //    default:
                //        Console.WriteLine("Неверный выбор, данные не были отсортированы.");
                //        break;
                //}




                //try
                //{

                //    Console.WriteLine("Данные успешно загружены!");



                //    foreach (var medicalRecord in medicalRecords)
                //    {
                //        Console.WriteLine($"ID: {medicalRecord.id}, Имя: {medicalRecord.Name}, Возраст: {medicalRecord.Age}, Медицинский диагноз: {medicalRecord.Diagnosis}");
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine($"Ошибка: {ex.Message}");
                //}
            }


            static void PrintRecords(List<MedicalRecord> records)
            {
                if (records.Count == 0)
                {
                    Console.WriteLine("Нет записей");
                    return;
                }
                foreach (var record in records)
                {
                    Console.WriteLine($"ID: {record.id}, Имя: {record.Name}, Возраст: {record.Age}, Медицинский диагноз: {record.Diagnosis}");
                }
            }

            static void AddRecord(List<MedicalRecord> records)
            {
                Console.WriteLine("Введите id");
                int id = int.Parse(Console.ReadLine());

                Console.Write("Введите имя: ");
                string name = Console.ReadLine();

                Console.Write("Введите возраст: ");
                int age = int.Parse(Console.ReadLine());

                Console.Write("Введите диагноз: ");
                string diagnosis = Console.ReadLine();

                records.Add(new MedicalRecord { id = id, Name = name, Age = age, Diagnosis = diagnosis });
                Console.WriteLine("Запись добавлена");
            }

            static void DeleteRecord(List<MedicalRecord> records)
            {
                Console.WriteLine("Введите ID записи, которой хотите удалить: ");
                int id = int.Parse(Console.ReadLine());

                var record = records.FirstOrDefault(r => r.id == id);
                if (record != null)
                {
                    records.Remove(record);
                    Console.WriteLine("Запись удалена");

                }
                else
                {
                    Console.WriteLine("Запись не найдена!");
                }
            }

            static void EditRecord(List<MedicalRecord> records)
            {
                Console.Write("Введите ID записи для редактирования: ");
                int id = int.Parse(Console.ReadLine());

                var record = records.FirstOrDefault(r => r.id == id);
                if (record == null)
                {
                    Console.WriteLine("Запись не найдена!");
                    return;
                }

                Console.Write("Введите новое имя (оставьте пустым для пропуска): ");
                string name = Console.ReadLine();
                if (!string.IsNullOrEmpty(name)) record.Name = name;

                Console.Write("Введите новый возраст (оставьте пустым для пропуска): ");
                string ageInput = Console.ReadLine();
                if (int.TryParse(ageInput, out int age)) record.Age = age;

                Console.Write("Введите новый диагноз (оставьте пустым для пропуска): ");
                string diagnosis = Console.ReadLine();
                if (!string.IsNullOrEmpty(diagnosis)) record.Diagnosis = diagnosis;

                Console.WriteLine("Запись обновлена!");
            }

            static void SearchRecords(List<MedicalRecord> records)
            {
                Console.Write("Введите подстроку для поиска: ");
                string query = Console.ReadLine();

                var results = records.Where(r => r.Name.Contains(query) || r.Diagnosis.Contains(query)).ToList();

                if (results.Count > 0)
                {
                    Console.WriteLine("Найденные записи:");
                    PrintRecords(results);
                }
                else
                {
                    Console.WriteLine("Совпадений не найдено!");
                }
            }

            static void SortRecords(List<MedicalRecord> records)
            {
                Console.WriteLine("Сортировка по (1 - id, 2 - Имени, 3 - Возврасту)");
                string sortChoice = Console.ReadLine();

                switch (sortChoice)
                {
                    case "1":
                        records.Sort((a,b) => a.id.CompareTo(b.id));
                        break;
                    case "2":
                        records.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "3":
                        records.Sort((a, b) => a.Age.CompareTo(b.Age));
                        break;
                    default:
                        Console.WriteLine("Неверный выбор, данные не были отсортированы.");
                        break;
                }

                Console.WriteLine("Данные после сортировки:");
                PrintRecords(records);
        }
    }
}
