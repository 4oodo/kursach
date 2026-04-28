using System;
using System.Collections.ObjectModel;
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

        public UserWindow(User user)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            _currentUser = user;

            WelcomeText.Text = $"Добро пожаловать, {user.FullName}!";

            LoadInitialData();
        }

        private void LoadInitialData()
        {
            try
            {
                RefreshBuildings();
                RefreshRoomFilters();
                RefreshEnergyFilters();
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

        private void EnergyFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Обновляем список помещений при выборе здания
                if (EnergyBuildingFilter.SelectedValue != null && sender == EnergyBuildingFilter)
                {
                    int buildingID = (int)EnergyBuildingFilter.SelectedValue;
                    EnergyRoomFilter.ItemsSource = _dbService.GetRoomsByBuilding(buildingID);
                }

                // Загружаем энергопотребление
                RefreshEnergyData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации: {ex.Message}", "Ошибка");
            }
        }

        private void RefreshEnergyData()
        {
            try
            {
                var allEnergy = _dbService.GetAllEnergyConsumption();

                // Фильтруем по выбранным критериям
                if (EnergyRoomFilter.SelectedValue != null)
                {
                    int roomID = (int)EnergyRoomFilter.SelectedValue;
                    allEnergy = new ObservableCollection<EnergyConsumption>(
                        allEnergy.Where(ec => ec.RoomID == roomID).ToList());
                }

                EnergyDataGrid.ItemsSource = allEnergy;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки фильтров поиска: {ex.Message}", "Ошибка");
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? buildingID = SearchBuildingFilter.SelectedValue as int?;
                DateTime? fromDate = SearchFromDate.SelectedDate;
                DateTime? toDate = SearchToDate.SelectedDate;

                // Выполняем поиск
                var searchResults = _dbService.GetAllEnergyConsumption();

                if (buildingID.HasValue)
                {
                    var roomsInBuilding = _dbService.GetRoomsByBuilding(buildingID.Value);
                    var roomIDs = roomsInBuilding.Select(r => r.RoomID).ToList();
                    searchResults = new ObservableCollection<EnergyConsumption>(
                        searchResults.Where(ec => roomIDs.Contains(ec.RoomID)).ToList());
                }

                if (fromDate.HasValue)
                {
                    searchResults = new ObservableCollection<EnergyConsumption>(
                        searchResults.Where(ec => ec.Date >= fromDate.Value).ToList());
                }

                if (toDate.HasValue)
                {
                    searchResults = new ObservableCollection<EnergyConsumption>(
                        searchResults.Where(ec => ec.Date <= toDate.Value).ToList());
                }

                SearchDataGrid.ItemsSource = searchResults;
                MessageBox.Show($"Найдено {searchResults.Count} записей.", "Результаты поиска");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка");
            }
        }

        #endregion

        #region Reports Tab

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int reportType = ReportTypeCombo.SelectedIndex;

                switch (reportType)
                {
                    case 0:
                        ReportDataGrid.ItemsSource = _dbService.GetEnergyReportByBuilding();
                        break;
                    case 1:
                        ReportDataGrid.ItemsSource = _dbService.GetEnergyReportByRoom();
                        break;
                    case 2:
                        ReportDataGrid.ItemsSource = _dbService.GetEnergyReportByTimePeriod();
                        break;
                    default:
                        ReportDataGrid.ItemsSource = _dbService.GetAllEnergyConsumption();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования отчета: {ex.Message}", "Ошибка");
            }
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
