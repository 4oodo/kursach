using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using kursach.Models;
using kursach.Services;

namespace kursach
{
    public partial class UserWindow : Window
    {
        private DatabaseService _dbService;
        private User _currentUser;
        private bool _suppressEnergyFilterEvents;

        public UserWindow(User user)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            _currentUser = user;

            WelcomeText.Text = $"Добро пожаловать, {user.Username}!";

            Loaded += UserWindow_Loaded;
            LoadInitialData();
        }

        private void UserWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ReportTypeCombo.SelectedIndex = 0;
            var today = DateTime.Today;
            ReportFromDate.SelectedDate = new DateTime(today.Year, today.Month, 1);
            ReportToDate.SelectedDate = today;
        }

        private void LoadInitialData()
        {
            try
            {
                RefreshBuildings();
                RefreshRoomFilters();
                RefreshEnergyFilters();
                RefreshEnergyRoomItems();
                RefreshEnergyData();
                RefreshSearchFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
        }

        #region Buildings Tab

        private void RefreshBuildings()
        {
            try
            {
                BuildingDataGrid.ItemsSource = _dbService.GetAllBuildings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки зданий: {ex.Message}", "Ошибка");
            }
        }

        #endregion

        #region Rooms Tab

        private void RefreshRoomFilters()
        {
            try
            {
                RoomBuildingFilter.ItemsSource = _dbService.GetAllBuildings();
                ShowAllRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки помещений: {ex.Message}", "Ошибка");
            }
        }

        private void RoomBuildingFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoomBuildingFilter.SelectedValue != null)
            {
                try
                {
                    int buildingID = (int)RoomBuildingFilter.SelectedValue;
                    RoomDataGrid.ItemsSource = _dbService.GetRoomsByBuilding(buildingID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка фильтрации помещений: {ex.Message}", "Ошибка");
                }
            }
        }

        private void ShowAllRooms_Click(object sender, RoutedEventArgs e)
        {
            ShowAllRooms();
        }

        private void ShowAllRooms()
        {
            try
            {
                RoomBuildingFilter.SelectedIndex = -1;
                RoomDataGrid.ItemsSource = _dbService.GetAllRooms();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки помещений: {ex.Message}", "Ошибка");
            }
        }

        #endregion

        #region Energy Consumption Tab

        private void RefreshEnergyFilters()
        {
            try
            {
                EnergyBuildingFilter.ItemsSource = _dbService.GetAllBuildings();
                EnergyPeriodFilter.ItemsSource = _dbService.GetAllTimePeriods();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки фильтров: {ex.Message}", "Ошибка");
            }
        }

        private void EnergyBuildingFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressEnergyFilterEvents)
                return;
            try
            {
                RefreshEnergyRoomItems();
                RefreshEnergyData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка");
            }
        }

        private void EnergyFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressEnergyFilterEvents)
                return;
            try
            {
                RefreshEnergyData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка");
            }
        }

        private void EnergyDate_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (_suppressEnergyFilterEvents)
                return;
            try
            {
                RefreshEnergyData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации по дате: {ex.Message}", "Ошибка");
            }
        }

        private void EnergyResetFilters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _suppressEnergyFilterEvents = true;
                EnergyBuildingFilter.SelectedIndex = -1;
                EnergyRoomFilter.SelectedIndex = -1;
                EnergyPeriodFilter.SelectedIndex = -1;
                EnergyFromDate.SelectedDate = null;
                EnergyToDate.SelectedDate = null;
                _suppressEnergyFilterEvents = false;
                RefreshEnergyRoomItems();
                RefreshEnergyData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сброса: {ex.Message}", "Ошибка");
            }
        }

        private void EnergyRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RefreshEnergyData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления: {ex.Message}", "Ошибка");
            }
        }

        /// <summary>Список помещений в фильтре в зависимости от выбранного здания.</summary>
        private void RefreshEnergyRoomItems()
        {
            int? selectedRoomId = EnergyRoomFilter.SelectedValue as int?;
            IList<Room> rooms;

            if (EnergyBuildingFilter.SelectedValue is int buildingId)
                rooms = _dbService.GetRoomsByBuilding(buildingId).ToList();
            else
                rooms = _dbService.GetAllRooms().ToList();

            _suppressEnergyFilterEvents = true;
            EnergyRoomFilter.ItemsSource = rooms;
            if (selectedRoomId.HasValue && rooms.Any(r => r.RoomID == selectedRoomId.Value))
                EnergyRoomFilter.SelectedValue = selectedRoomId.Value;
            else
                EnergyRoomFilter.SelectedIndex = -1;
            _suppressEnergyFilterEvents = false;
        }

        private void RefreshEnergyData()
        {
            try
            {
                var list = _dbService.GetAllEnergyConsumptions().ToList();

                if (EnergyBuildingFilter.SelectedValue is int buildingID)
                {
                    var roomIdSet = new HashSet<int>(_dbService.GetRoomsByBuilding(buildingID).Select(r => r.RoomID));
                    list = list.Where(ec => roomIdSet.Contains(ec.RoomID)).ToList();
                }

                if (EnergyRoomFilter.SelectedValue is int roomId)
                    list = list.Where(ec => ec.RoomID == roomId).ToList();

                if (EnergyPeriodFilter.SelectedValue is int timePeriodID)
                    list = list.Where(ec => ec.TimePeriodID == timePeriodID).ToList();

                if (EnergyFromDate.SelectedDate is DateTime fromD)
                    list = list.Where(ec => ec.Date.Date >= fromD.Date).ToList();

                if (EnergyToDate.SelectedDate is DateTime toD)
                    list = list.Where(ec => ec.Date.Date <= toD.Date).ToList();

                EnergyDataGrid.ItemsSource = new ObservableCollection<EnergyConsumption>(list);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки энергопотребления: {ex.Message}", "Ошибка");
            }
        }

        #endregion

        #region Search Tab

        private void RefreshSearchFilters()
        {
            try
            {
                SearchBuildingFilter.ItemsSource = _dbService.GetAllBuildings();
                SearchPeriodFilter.ItemsSource = _dbService.GetAllTimePeriods();
                RefreshSearchRoomItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки фильтров поиска: {ex.Message}", "Ошибка");
            }
        }

        private void SearchBuildingFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                RefreshSearchRoomItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка списка помещений: {ex.Message}", "Ошибка");
            }
        }

        private void RefreshSearchRoomItems()
        {
            int? selectedRoomId = SearchRoomFilter.SelectedValue as int?;
            IList<Room> rooms;

            if (SearchBuildingFilter.SelectedValue is int buildingId)
                rooms = _dbService.GetRoomsByBuilding(buildingId).ToList();
            else
                rooms = _dbService.GetAllRooms().ToList();

            SearchRoomFilter.ItemsSource = rooms;
            if (selectedRoomId.HasValue && rooms.Any(r => r.RoomID == selectedRoomId.Value))
                SearchRoomFilter.SelectedValue = selectedRoomId.Value;
            else
                SearchRoomFilter.SelectedIndex = -1;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var searchResults = _dbService.GetAllEnergyConsumptions().ToList();

                if (SearchBuildingFilter.SelectedValue is int buildingID)
                {
                    var roomIDs = _dbService.GetRoomsByBuilding(buildingID).Select(r => r.RoomID).ToList();
                    searchResults = searchResults.Where(ec => roomIDs.Contains(ec.RoomID)).ToList();
                }

                if (SearchRoomFilter.SelectedValue is int roomId)
                    searchResults = searchResults.Where(ec => ec.RoomID == roomId).ToList();

                if (SearchPeriodFilter.SelectedValue is int periodId)
                    searchResults = searchResults.Where(ec => ec.TimePeriodID == periodId).ToList();

                if (SearchFromDate.SelectedDate is DateTime fromDate)
                    searchResults = searchResults.Where(ec => ec.Date.Date >= fromDate.Date).ToList();

                if (SearchToDate.SelectedDate is DateTime toDate)
                    searchResults = searchResults.Where(ec => ec.Date.Date <= toDate.Date).ToList();

                string q = SearchTextBox?.Text?.Trim();
                if (!string.IsNullOrEmpty(q))
                {
                    string qq = q.ToLowerInvariant();
                    searchResults = searchResults.Where(ec =>
                        (ec.RoomInfo != null && ec.RoomInfo.ToLowerInvariant().Contains(qq)) ||
                        (ec.TimePeriodName != null && ec.TimePeriodName.ToLowerInvariant().Contains(qq))).ToList();
                }

                SearchDataGrid.ItemsSource = new ObservableCollection<EnergyConsumption>(searchResults);
                if (SearchResultText != null)
                    SearchResultText.Text = $"Найдено записей: {searchResults.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка");
            }
        }

        private void SearchReset_Click(object sender, RoutedEventArgs e)
        {
            SearchBuildingFilter.SelectedIndex = -1;
            SearchRoomFilter.SelectedIndex = -1;
            SearchPeriodFilter.SelectedIndex = -1;
            SearchFromDate.SelectedDate = null;
            SearchToDate.SelectedDate = null;
            SearchTextBox.Clear();
            RefreshSearchRoomItems();
            SearchDataGrid.ItemsSource = null;
            if (SearchResultText != null)
                SearchResultText.Text = string.Empty;
        }

        #endregion

        #region Reports Tab

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int reportType = ReportTypeCombo.SelectedIndex;

                if (reportType < 0)
                {
                    MessageBox.Show("Выберите тип отчета!", "Ошибка");
                    return;
                }

                DataTable reportData;

                if (reportType == 3)
                {
                    reportData = LocalizeBuildingStatisticsTable(_dbService.GetBuildingStatistics());
                    ReportDataGrid.ItemsSource = reportData.DefaultView;
                    MessageBox.Show($"Статистика загружена. Строк: {reportData.Rows.Count}.", "Отчёт");
                    return;
                }

                var energyList = _dbService.GetAllEnergyConsumptions().ToList();
                if (ReportFromDate.SelectedDate is DateTime rFrom)
                    energyList = energyList.Where(x => x.Date.Date >= rFrom.Date).ToList();
                if (ReportToDate.SelectedDate is DateTime rTo)
                    energyList = energyList.Where(x => x.Date.Date <= rTo.Date).ToList();
                var energyData = new ObservableCollection<EnergyConsumption>(energyList);

                switch (reportType)
                {
                    case 0:
                        reportData = GenerateBuildingReport(energyData);
                        break;
                    case 1:
                        if (ReportFromDate.SelectedDate.HasValue && ReportToDate.SelectedDate.HasValue)
                        {
                            reportData = _dbService.GetEnergyConsumptionReport(
                                ReportFromDate.SelectedDate.Value.Date,
                                ReportToDate.SelectedDate.Value.Date);
                            reportData = LocalizeDetailConsumptionTable(reportData);
                        }
                        else
                            reportData = GenerateRoomReport(energyData);
                        break;
                    case 2:
                        reportData = GeneratePeriodReport(energyData);
                        break;
                    default:
                        reportData = new DataTable();
                        break;
                }

                ReportDataGrid.ItemsSource = reportData.DefaultView;
                MessageBox.Show($"Отчет сформирован. Записей: {reportData.Rows.Count}", "Готово");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования отчета: {ex.Message}", "Ошибка");
            }
        }

        private static DataTable LocalizeBuildingStatisticsTable(DataTable source)
        {
            var dt = new DataTable();
            dt.Columns.Add("Здание", typeof(string));
            dt.Columns.Add("Помещений", typeof(object));
            dt.Columns.Add("Сумма кВт·ч", typeof(object));
            dt.Columns.Add("Среднее кВт·ч", typeof(object));
            dt.Columns.Add("Макс. кВт·ч", typeof(object));
            dt.Columns.Add("Мин. кВт·ч", typeof(object));

            foreach (DataRow row in source.Rows)
            {
                dt.Rows.Add(
                    row["BuildingName"],
                    row["RoomCount"],
                    row["TotalEnergy"],
                    row["AverageEnergy"],
                    row["MaxEnergy"],
                    row["MinEnergy"]);
            }

            return dt;
        }

        private static DataTable LocalizeDetailConsumptionTable(DataTable source)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "BuildingName", "Здание" },
                { "Floor", "Этаж" },
                { "CategoryName", "Категория помещения" },
                { "PeriodName", "Период суток" },
                { "Date", "Дата" },
                { "EnergyValue", "Потребление, кВт·ч" },
                { "CumulativeEnergy", "Накопительно, кВт·ч" }
            };

            var dt = new DataTable();
            foreach (DataColumn col in source.Columns)
            {
                string header = map.TryGetValue(col.ColumnName, out string ru) ? ru : col.ColumnName;
                dt.Columns.Add(header, col.DataType);
            }

            foreach (DataRow row in source.Rows)
            {
                var newRow = dt.NewRow();
                for (int i = 0; i < source.Columns.Count; i++)
                    newRow[i] = row[i] ?? DBNull.Value;
                dt.Rows.Add(newRow);
            }

            return dt;
        }

        private DataTable GenerateBuildingReport(ObservableCollection<EnergyConsumption> energyData)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Здание", typeof(string));
            dt.Columns.Add("Общее потребление (кВт)", typeof(decimal));
            dt.Columns.Add("Среднее потребление (кВт)", typeof(decimal));
            dt.Columns.Add("Максимум (кВт)", typeof(decimal));
            dt.Columns.Add("Количество записей", typeof(int));

            var buildings = _dbService.GetAllBuildings();
            var rooms = _dbService.GetAllRooms();

            foreach (var building in buildings)
            {
                var buildingRooms = rooms.Where(r => r.BuildingID == building.BuildingID).Select(r => r.RoomID).ToList();
                var buildingEnergy = energyData.Where(ec => buildingRooms.Contains(ec.RoomID)).ToList();

                if (buildingEnergy.Count > 0)
                {
                    decimal total = buildingEnergy.Sum(ec => ec.EnergyValue);
                    decimal avg = (decimal)buildingEnergy.Average(ec => ec.EnergyValue);
                    decimal max = buildingEnergy.Max(ec => ec.EnergyValue);

                    dt.Rows.Add(building.BuildingName, total, avg, max, buildingEnergy.Count);
                }
            }

            return dt;
        }

        private DataTable GenerateRoomReport(ObservableCollection<EnergyConsumption> energyData)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Помещение", typeof(string));
            dt.Columns.Add("Здание", typeof(string));
            dt.Columns.Add("Потребление (кВт)", typeof(decimal));
            dt.Columns.Add("Дата", typeof(string));
            dt.Columns.Add("Период", typeof(string));

            var rooms = _dbService.GetAllRooms();
            var buildings = _dbService.GetAllBuildings();
            var periods = _dbService.GetAllTimePeriods();

            foreach (var energy in energyData.OrderByDescending(e => e.Date))
            {
                var room = rooms.FirstOrDefault(r => r.RoomID == energy.RoomID);
                var building = buildings.FirstOrDefault(b => b.BuildingID == room?.BuildingID);
                var period = periods.FirstOrDefault(p => p.TimePeriodID == energy.TimePeriodID);

                if (room != null && building != null && period != null)
                {
                    dt.Rows.Add(
                        $"Помещение {room.RoomID} (Этаж {room.Floor})",
                        building.BuildingName,
                        energy.EnergyValue,
                        energy.Date.ToString("dd.MM.yyyy"),
                        period.PeriodName
                    );
                }
            }

            return dt;
        }

        private DataTable GeneratePeriodReport(ObservableCollection<EnergyConsumption> energyData)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Период времени", typeof(string));
            dt.Columns.Add("Общее потребление (кВт)", typeof(decimal));
            dt.Columns.Add("Среднее потребление (кВт)", typeof(decimal));
            dt.Columns.Add("Количество записей", typeof(int));

            var periods = _dbService.GetAllTimePeriods();

            foreach (var period in periods)
            {
                var periodEnergy = energyData.Where(ec => ec.TimePeriodID == period.TimePeriodID).ToList();

                if (periodEnergy.Count > 0)
                {
                    decimal total = periodEnergy.Sum(ec => ec.EnergyValue);
                    decimal avg = (decimal)periodEnergy.Average(ec => ec.EnergyValue);

                    dt.Rows.Add(period.PeriodName, total, avg, periodEnergy.Count);
                }
            }

            return dt;
        }

        #endregion

        #region Logout

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        #endregion
    }
}
