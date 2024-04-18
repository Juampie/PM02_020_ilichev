using System.Text;
using System.Windows;
using System.Windows.Controls;
using TransportRoutes.Classes;

namespace TransportRoutes.Pages
{
    /// <summary>
    /// Главная страница приложения
    /// </summary>
    public partial class MainPage : Page
    {
        // Массивы для хранения данных о затратах, предложении и спросе
        private int[,] _costs;
        private int[] _supply;
        private int[] _demand;
        private int[,] _allocation;

        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик нажатия кнопки создания массива затрат
        /// </summary>
        private void CreateCostsButton_Click(object sender, RoutedEventArgs e)
        {
            // Попытка преобразовать введенные значения в числа
            if (int.TryParse(WidthInput.Text, out int width) && int.TryParse(HeightCostsInput.Text, out int height))
            {
                // Создание массивов с заданными размерами
                _costs = new int[height, width];
                _supply = new int[height];
                _demand = new int[width];
                CountSupply.Text = height.ToString();
                CountDemand.Text = width.ToString();
                MessageBox.Show($"Создан массив Costs размером {height}x{width}.");
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные значения ширины и высоты для Costs.");
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки ввода затрат
        /// </summary>
        private void EnterCostsButton_Click(object sender, RoutedEventArgs e)
        {
            // Попытка преобразовать введенные значения в числа
            if (int.TryParse(WidthCostsTextBox.Text, out int width) && int.TryParse(HeightCostsTextBox.Text, out int height) && int.TryParse(ValueCostsTextBox.Text, out int value))
            {
                // Проверка корректности введенных индексов
                if (width < _costs.GetLength(1) && height < _costs.GetLength(0))
                {
                    // Запись значения в массив
                    _costs[height, width] = value;
                    MessageBox.Show($"Значение Costs в {height}x{width} установлено на {value}.");
                }
                else
                {
                    MessageBox.Show("Пожалуйста, введите корректные значения ширины и высоты в пределах размера массива Costs.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректные значения ширины, высоты и значения для Costs.");
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки ввода предложения
        /// </summary>
        private void EnterSupplyButton_Click(object sender, RoutedEventArgs e)
        {
            // Попытка преобразовать введенные значения в числа
            if (int.TryParse(CountSupplyTextBox.Text, out int index) && int.TryParse(ValueSupplyTextBox.Text, out int value))
            {
                // Проверка корректности введенного индекса
                if (index < _supply.Length)
                {
                    // Запись значения в массив
                    _supply[index] = value;
                    MessageBox.Show($"Значение Supply на {index} установлено на {value}.");
                }
                else
                {
                    MessageBox.Show("Пожалуйста, введите корректный индекс в пределах размера массива Supply.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректный индекс и значение для Supply.");
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки ввода спроса
        /// </summary>
        private void EnterDemandButton_Click(object sender, RoutedEventArgs e)
        {
            // Попытка преобразовать введенные значения в числа
            if (int.TryParse(CountDemandTextBox.Text, out int index) && int.TryParse(ValueDemandTextBox.Text, out int value))
            {
                // Проверка корректности введенного индекса
                if (index < _demand.Length)
                {
                    // Запись значения в массив
                    _demand[index] = value;
                    MessageBox.Show($"Значение Demand на {index} установлено на {value}.");
                }
                else
                {
                    MessageBox.Show("Пожалуйста, введите корректный индекс в пределах размера массива Demand.");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректный индекс и значение для Demand.");
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки получения данных
        /// </summary>
        private void GetDataButton_Click(object sender, RoutedEventArgs e)
        {
            JsonFileExplorer explorer = new JsonFileExplorer();
            var (costs, supply, demand) = explorer.GetArraysFromJsonFile();
            _costs = costs;
            _supply = supply;
            _demand = demand;
        }

        /// <summary>
        /// Обработчик нажатия кнопки сохранения данных
        /// </summary>
        private void SaveDataButton_Click(object sender, RoutedEventArgs e)
        {
            JsonFileExplorer explorer = new JsonFileExplorer();
            explorer.SaveArraysToJsonFile(_allocation);
        }

        /// <summary>
        /// Обработчик нажатия кнопки расчета методом северо-западного угла
        /// </summary>
        private void WestCalculate_Click(object sender, RoutedEventArgs e)
        {
            var transport = new TransportProblemSolver(_costs, _supply, _demand);
            transport.BalanceProblem();
            _allocation = transport.SolveNorthWestCornerMethod();
            DisplayAllocationAsTable();
            transport.PrintAllocation(_allocation);
            TotalCostTextBlock.Text = transport.CalculateTotalCost(_allocation).ToString();
        }

        /// <summary>
        /// Обработчик нажатия кнопки расчета методом минимальной стоимости
        /// </summary>
        private void MinCalculate_Click(object sender, RoutedEventArgs e)
        {
            var transport = new TransportProblemSolver(_costs, _supply, _demand);
            transport.BalanceProblem();
            _allocation = transport.SolveMinimumCostMethod();
            DisplayAllocationAsTable();
            transport.PrintAllocation(_allocation);
            TotalCostTextBlock.Text = transport.CalculateTotalCost(_allocation).ToString();
        }

        private void DisplayAllocationAsTable()
        {
            if (_allocation != null)
            {
                StringBuilder sb = new StringBuilder();

                // Создаем таблицу из данных массива _allocation
                for (int i = 0; i < _allocation.GetLength(0); i++)
                {
                    for (int j = 0; j < _allocation.GetLength(1); j++)
                    {
                        sb.AppendFormat("{0}\t", _allocation[i, j]);
                    }
                    sb.AppendLine();
                }

                DataOutput.Text = sb.ToString();
            }
            else
            {
                DataOutput.Text = "Массив пуст.";
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearData();
        }
        private void ClearData()
        {
            _costs = null;
            _supply = null;
            _demand = null;
            _allocation = null;

            CountSupply.Text = string.Empty;
            CountDemand.Text = string.Empty;
            DataOutput.Text = string.Empty;

            // Очистка всех TextBox, чтобы пользователь мог ввести новые значения
            WidthInput.Text = string.Empty;
            HeightCostsInput.Text = string.Empty;
            WidthCostsTextBox.Text = string.Empty;
            HeightCostsTextBox.Text = string.Empty;
            ValueCostsTextBox.Text = string.Empty;
            CountSupplyTextBox.Text = string.Empty;
            ValueSupplyTextBox.Text = string.Empty;
            CountDemandTextBox.Text = string.Empty;
            ValueDemandTextBox.Text = string.Empty;
            TotalCostTextBlock.Text = string.Empty;
        }
    }
}