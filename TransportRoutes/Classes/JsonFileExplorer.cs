using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TransportRoutes.Classes
{
    /// <summary>
    /// Класс для работы с JSON файлами
    /// </summary>
    internal class JsonFileExplorer
    {
        /// <summary>
        /// Метод для получения массивов из JSON файла
        /// </summary>
        public (int[,], int[], int[]) GetArraysFromJsonFile()
        {
            try
            {
                // Создаем диалог для выбора файла
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "json files (*.json)|*.json|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = true
                };

                // Если файл выбран
                if (openFileDialog.ShowDialog() == true)
                {
                    // Читаем содержимое файла
                    string filePath = openFileDialog.FileName;
                    string fileContent = File.ReadAllText(filePath);

                    // Десериализуем JSON в объект Data
                    var data = JsonSerializer.Deserialize<Data>(fileContent);

                    // Если данные пусты, возвращаем пустые массивы
                    if (data?.Costs == null)
                    {
                        Console.WriteLine("Данные Costs пусты");
                        return (null, null, null);
                    }

                    // Преобразуем ступенчатый массив в 2D массив
                    int[,] costs = new int[data.Costs.Length, data.Costs.Max(subArray => subArray.Length)];
                    for (int i = 0; i < data.Costs.Length; i++)
                    {
                        for (int j = 0; j < data.Costs[i].Length; j++)
                        {
                            costs[i, j] = data.Costs[i][j];
                        }
                    }

                    // Возвращаем массивы
                    return (costs, data.Supply, data.Demand);
                }
            }
            catch (Exception ex)
            {
                // Логируем или обрабатываем исключение по необходимости
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }

            // Возвращаем пустые массивы в случае ошибки
            return (null, null, null);
        }

        /// <summary>
        /// Метод для сохранения массивов в JSON файл
        /// </summary>
        public void SaveArraysToJsonFile(int[,] allocation)
        {
            try
            {
                // Если данные пусты, возвращаемся
                if (allocation == null)
                {
                    Console.WriteLine("Данные Allocation пусты");
                    return;
                }

                // Создаем диалог для сохранения файла
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "json files (*.json)|*.json|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = true
                };

                // Если файл выбран
                if (saveFileDialog.ShowDialog() == true)
                {
                    // Получаем путь к файлу
                    string filePath = saveFileDialog.FileName;

                    // Преобразуем 2D массив в ступенчатый массив
                    int[][] allocationJagged = new int[allocation.GetLength(0)][];
                    for (int i = 0; i < allocation.GetLength(0); i++)
                    {
                        allocationJagged[i] = new int[allocation.GetLength(1)];
                        for (int j = 0; j < allocation.GetLength(1); j++)
                        {
                            allocationJagged[i][j] = allocation[i, j];
                        }
                    }

                    // Создаем объект данных
                    AllocationData data = new AllocationData
                    {
                        allocation = allocationJagged
                    };

                    // Сериализуем данные в JSON
                    string json = JsonSerializer.Serialize(data);

                    // Записываем JSON в файл
                    File.WriteAllText(filePath, json);
                }
            }
            catch (Exception ex)
            {
                // Логируем или обрабатываем исключение по необходимости
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        /// <summary>
        /// Класс для десериализации данных из JSON
        /// </summary>
        private class Data
        {
            public int[][] Costs { get; set; }
            public int[] Supply { get; set; }
            public int[] Demand { get; set; }
        }

        /// <summary>
        /// Класс для сериализации данных в JSON
        /// </summary>
        private class AllocationData
        {
            public int[][] allocation { get; set; }
        }
    }
}