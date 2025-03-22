using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PotentialMethod
{
    public class Tasks
    {
        public int[,] Allocation;
        public int[,] Tariffs;
        public int[,] originaltariffs;
        public void Structure(out int[] Suppliers, out int[] StoragePoint)
        {
            // Задаём массив рандома, а также переменные отвечающие за размер массива
            Random random = new Random();
            int supplierCount = 3;
            int storageCount = 3;

            // Задаём сами массивы, а конкретно массив поставщиков, складов и тарифов
            Suppliers = new int[supplierCount];
            StoragePoint = new int[storageCount];
            Tariffs = new int[,]
        {
            { 2, 5, 2 },
            { 4, 1, 5 },
            { 3, 6, 8}
        };
            originaltariffs = (int[,])Tariffs.Clone();
            // Сделал ввод объёма поставщиков с руки
            Console.WriteLine("Введите объемы поставщиков:");
            for (int i = 0; i < Suppliers.Length; i++)
            {
                Suppliers[i] = Convert.ToInt32(Console.ReadLine());
            }
            // Сделал ввод объёма точек хранения с руки
            Console.WriteLine("Введите объемы точек хранения:");
            for (int i = 0; i < StoragePoint.Length; i++)
            {
                StoragePoint[i] = Convert.ToInt32(Console.ReadLine());
            }
            // Сделал генерацию случайных тарифов, задействуя ранее заданный массив рандома

            // Выводим матрицы
            Console.WriteLine("Точки хранения: " + string.Join(" ", StoragePoint));
            Console.WriteLine("Поставщики: " + string.Join(" ", Suppliers));

            Console.WriteLine("Тарифная матрица:");
            for (int i = 0; i < supplierCount; i++)
            {
                for (int j = 0; j < storageCount; j++)
                {
                    Console.Write(Tariffs[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
        public void DisplayPlan(int[,] Allocation, int[,] Tariffs)
        {
            int sumofobjectivefunction = 0;
            Console.WriteLine("Опорный план:");
            for (int i = 0; i < Allocation.GetLength(0); i++)
            {
                for (int j = 0; j < Allocation.GetLength(1); j++)
                {
                    Console.Write(Allocation[i, j] + " ");
                    if (Allocation[i, j] > 0)
                    {
                        sumofobjectivefunction += Allocation[i, j] * Tariffs[i, j];
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine($"Сумма целевой функции: {sumofobjectivefunction}");
        }
        public (int, int, int) FindMinimumElement(int[,] matrix)
            {
                // minValue - находит минимальное значение
                int minValue = int.MaxValue;
                // Минимальное значение строки
                int minRow = -1;
                // Минимальное значение колонки
                int minCol = -1;

                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (matrix[i, j] < minValue)
                        {
                            minValue = matrix[i, j];
                            minRow = i;
                            minCol = j;
                        }
                    }
                }
                return (minValue, minRow, minCol);
            }

            public void MinimumElemetnMethod()
            {
                Structure(out int[] Suppliers, out int[] StoragePoint);
                // Задал переменную Allocation - расположение, в которой будет записан результат
                Allocation = new int[Suppliers.Length, StoragePoint.Length];

                originaltariffs = (int[,])Tariffs.Clone();
                // Написал цикл, который проверяет задачу на открытость и закрытость, если задача открыта, то программа закрывается
                int sumofsuppliers = 0;
                int sumofstoragepoints = 0;
                for (int k = 0; k < StoragePoint.Length; k++)
                {
                    sumofstoragepoints = sumofstoragepoints + StoragePoint[k];
                }
                for (int j = 0; j < Suppliers.Length; j++)
                {
                    sumofsuppliers = sumofsuppliers + Suppliers[j];
                }
                if (sumofsuppliers != sumofstoragepoints)
                {
                    Environment.Exit(1);
                }
                // Начал цикл, который будет проходиться по строкам и столбцам и находить минимальный тариф,
                // потом в ячейку с минимальным тарифом будет подставлено минимальное значение поставщика или же склада
                while (true)
                {
                    (int minValue, int row, int col) = FindMinimumElement(Tariffs); // переменные row и 
                                                                                    // Выход из цикла, если минимальный элемент находиться за границами
                    if (row == -1 || col == -1)
                    {
                        break;
                    }
                    // Присваиваем переменной allocation минимальные элемент
                    int allocation = Math.Min(Suppliers[row], StoragePoint[col]);
                    Allocation[row, col] = allocation;
                    Suppliers[row] -= allocation;
                    StoragePoint[col] -= allocation;
                    Tariffs[row, col] = int.MaxValue;
                }

                DisplayPlan(Allocation, originaltariffs);
            }




            public void NorthWestAngle()
            {
                Structure(out int[] Suppliers, out int[] StoragePoint);
                Allocation = new int[Suppliers.Length, StoragePoint.Length];
                int row = 0;
                int cols = 0;

                int sumofsuppliers = 0;
                int sumofstoragepoints = 0;
                for (int k = 0; k < StoragePoint.Length; k++)
                {
                    sumofstoragepoints = sumofstoragepoints + StoragePoint[k];
                }
                for (int j = 0; j < Suppliers.Length; j++)
                {
                    sumofsuppliers = sumofsuppliers + Suppliers[j];
                }
                if (sumofsuppliers != sumofstoragepoints)
                {
                    Environment.Exit(1);
                }

                while (row < Suppliers.Length && cols < StoragePoint.Length)
                {
                    int allocation = Math.Min(Suppliers[row], StoragePoint[cols]);
                    Allocation[row, cols] = allocation;
                    Suppliers[row] -= allocation;
                    StoragePoint[cols] -= allocation;
                    if (Suppliers[row] == 0)
                    {
                        row++;
                    }
                    else if (StoragePoint[cols] == 0)
                    {
                        cols++;
                    }
                }
                DisplayPlan(Allocation, Tariffs);
            }

            
            public bool RunPotentialMethod()
            {
                if (Allocation == null)
                {
                    Console.WriteLine("Ошибка: переменная Allocation не найдена");
                    return false;
                }
                Console.WriteLine("Метод потенциалов: ");
                int supplierCount = Allocation.GetLength(0);
                int storagePointCount = Allocation.GetLength(1);
                int[] potentialsRow = Enumerable.Repeat(int.MaxValue, supplierCount).ToArray();
                int[] potentialsCol = Enumerable.Repeat(int.MaxValue, storagePointCount).ToArray();


                potentialsRow[0] = 0;

            bool changesMade;
            do
            {
                changesMade = false;

                for (int i = 0; i < supplierCount; i++)
                {
                    for (int j = 0; j < storagePointCount; j++)
                    {
                        Console.WriteLine($"  Клетка [{i}, {j}]: Allocation = {Allocation[i, j]}, Tariffs = {originaltariffs[i, j]}"); // Отладочный вывод
                        if (Allocation[i, j] > 0 && originaltariffs[i, j] != int.MaxValue) // Проверяем, есть ли поставка
                        {
                            if (potentialsRow[i] != int.MaxValue && potentialsCol[j] == int.MaxValue)
                            {
                                potentialsCol[j] = originaltariffs[i, j] - potentialsRow[i];
                                changesMade = true;
                            }
                            else if (potentialsRow[i] == int.MaxValue && potentialsCol[j] != int.MaxValue)
                            {
                                potentialsRow[i] = originaltariffs[i, j] - potentialsCol[j];
                                changesMade = true;
                            }
                        }
                    }
                }
            } while (changesMade);
            Console.WriteLine("Потенциалы строк:");
            Console.WriteLine(string.Join(" ", potentialsRow));

            Console.WriteLine("Потенциалы столбцов:");
            Console.WriteLine(string.Join(" ", potentialsCol));


            Console.WriteLine("\nОценка свободных ячеек:");
            bool isOptimal = true;
            for (int i = 0; i < supplierCount; i++)
            {
                for (int j = 0; j < storagePointCount; j++)
                {
                    if (Allocation[i, j] == 0) // Свободная ячейка
                    {
                        double delta = originaltariffs[i, j] - potentialsRow[i] - potentialsCol[j];
                        Console.WriteLine($"  Ячейка [{i}, {j}]: Оценка = {delta}");
                        if (delta < 0)
                        {
                            isOptimal = false;
                        }
                    }
                }
            }

            if (isOptimal)
            {
                Console.WriteLine("\nОпорный план оптимален.");
                return true;
            }
            else
            {
                Console.WriteLine("\nОпорный план неоптимален.");
                return false;
            }
        }
            
    }
        internal class Program
        {
            static void Main(string[] args)
            {
                Tasks tasks = new Tasks();
                tasks.MinimumElemetnMethod();
                tasks.RunPotentialMethod();
                //tasks.NorthWestAngle();
                Console.ReadKey();
            }
        }

}
