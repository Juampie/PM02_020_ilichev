using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportRoutes.Classes
{
    /// <summary>
    /// Класс для решения задачи о транспортной проблеме
    /// </summary>
    internal class TransportProblemSolver
    {
        private int[,] costs; // Матрица стоимостей
        private int[] supply; // Массив предложений
        private int[] demand; // Массив спроса

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public TransportProblemSolver(int[,] costs, int[] supply, int[] demand)
        {
            this.costs = costs;
            this.supply = supply;
            this.demand = demand;
        }

        /// <summary>
        /// Метод для балансировки задачи
        /// </summary>
        public void BalanceProblem()
        {
            try
            {
                // Суммирование предложений и спроса
                int totalSupply = supply.Sum();
                int totalDemand = demand.Sum();

                // Если предложение равно спросу, задача сбалансирована
                if (totalSupply == totalDemand)
                {
                    Console.WriteLine("Задача сбалансирована.");
                    return;
                }

                // Если предложение больше спроса, добавляем фиктивный спрос
                if (totalSupply > totalDemand)
                {
                    int extraSupply = totalSupply - totalDemand;
                    Array.Resize(ref demand, demand.Length + 1);
                    demand[demand.Length - 1] = extraSupply;

                    int[] newColumn = new int[costs.GetLength(0)];
                    int[,] newCosts = new int[costs.GetLength(0), costs.GetLength(1) + 1];
                    Array.Copy(costs, newCosts, costs.Length);
                    for (int i = 0; i < newColumn.Length; i++)
                    {
                        newCosts[i, costs.GetLength(1)] = 0; // или небольшая стоимость
                    }
                    costs = newCosts;

                    Console.WriteLine($"Добавлен фиктивный спрос: {extraSupply}");
                }
                else // Если спрос больше предложения, добавляем фиктивное предложение
                {
                    int extraDemand = totalDemand - totalSupply;
                    Array.Resize(ref supply, supply.Length + 1);
                    supply[supply.Length - 1] = extraDemand;

                    int[,] newCosts = new int[costs.GetLength(0) + 1, costs.GetLength(1)];
                    Array.Copy(costs, newCosts, costs.Length);
                    for (int j = 0; j < costs.GetLength(1); j++)
                    {
                        newCosts[costs.GetLength(0), j] = 0; // или небольшая стоимость
                    }
                    costs = newCosts;

                    Console.WriteLine($"Добавлен фиктивное предложение: {extraDemand}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при балансировке проблемы:");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Метод для решения задачи методом минимальной стоимости
        /// </summary>
        public int[,] SolveMinimumCostMethod()
        {
            try
            {
                // Инициализация матрицы распределения
                int rows = supply.Length;
                int cols = demand.Length;
                int[,] allocation = new int[rows, cols];

                // Пока есть нераспределенные предложения или спрос
                while (true)
                {
                    // Находим позицию минимального элемента
                    int minRowIndex = -1;
                    int minColIndex = -1;
                    int minValue = int.MaxValue;

                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            if (supply[i] > 0 && demand[j] > 0 && costs[i, j] < minValue)
                            {
                                minValue = costs[i, j];
                                minRowIndex = i;
                                minColIndex = j;
                            }
                        }
                    }

                    if (minRowIndex == -1 || minColIndex == -1)
                        break;

                    // Выполняем выделение наименьшего элемента
                    int amountToAllocate = Math.Min(supply[minRowIndex], demand[minColIndex]);
                    allocation[minRowIndex, minColIndex] = amountToAllocate;
                    supply[minRowIndex] -= amountToAllocate;
                    demand[minColIndex] -= amountToAllocate;
                }

                return allocation;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка в процессе решения методом минимальной стоимости:");
                Console.WriteLine(ex.Message);
                return null; // Возвращаем null при возникновении ошибки
            }
        }

        /// <summary>
        /// Метод для решения задачи методом северо-западного угла
        /// </summary>
        public int[,] SolveNorthWestCornerMethod()
        {
            int[,] allocation = new int[supply.Length, demand.Length];
            int i = 0, j = 0;

            while (i < supply.Length && j < demand.Length)
            {
                if (supply[i] > 0 && demand[j] > 0)
                {
                    int amountToAllocate = Math.Min(supply[i], demand[j]);
                    allocation[i, j] = amountToAllocate;
                    supply[i] -= amountToAllocate;
                    demand[j] -= amountToAllocate;

                    if (supply[i] == 0) i++;
                    if (demand[j] == 0) j++;
                }
                else
                {
                    if (supply[i] == 0) i++;
                    if (demand[j] == 0) j++;
                }
            }

            return allocation;
        }

        /// <summary>
        /// Метод для вывода матрицы распределения
        /// </summary>
        public void PrintAllocation(int[,] allocation)
        {
            try
            {
                int rows = allocation.GetLength(0);
                int cols = allocation.GetLength(1);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        Console.Write($"{allocation[i, j]}\t");
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при печати выделения:");
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Метод для расчета общей стоимости
        /// </summary>
        public int CalculateTotalCost(int[,] allocation)
        {
            try
            {
                int totalCost = 0;
                int rows = allocation.GetLength(0);
                int cols = allocation.GetLength(1);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        totalCost += costs[i, j] * allocation[i, j];
                    }
                }

                Console.WriteLine(totalCost);
                return totalCost;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при вычислении общей стоимости:");
                Console.WriteLine(ex.Message);
                return -1; // Возвращаем -1 при возникновении ошибки
            }
        }
    }
}