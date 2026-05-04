using System;
using System.Windows;
using System.Windows.Controls;
using kursach.Models;
using kursach.Services;

namespace kursach
{
    public partial class MainWindow : Window
    {
        private DatabaseService _dbService;
        private User _currentUser;
        private Building _selectedBuilding;
        private RoomCategory _selectedCategory;
        private TimePeriod _selectedTimePeriod;
        private Room _selectedRoom;
        private EnergyConsumption _selectedEnergy;

        public MainWindow()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            _currentUser = null;
            LoadInitialData();
        }

        public MainWindow(User user)
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            _currentUser = user;
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            try
            {
                RefreshBuildingBtn_Click(null, null);
                RefreshCategoryBtn_Click(null, null);
                RefreshTimePeriodBtn_Click(null, null);
                RefreshRoomBtn_Click(null, null);
                RefreshEnergyBtn_Click(null, null);
                RefreshAdminKeys();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка");
            }
        }

        #region Building CRUD

        private void RefreshBuildingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BuildingDataGrid.ItemsSource = _dbService.GetAllBuildings();
                RoomBuildingCombo.ItemsSource = _dbService.GetAllBuildings();
                RoomBuildingCombo.DisplayMemberPath = "BuildingName";
                RoomBuildingCombo.SelectedValuePath = "BuildingID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки зданий: {ex.Message}");
            }
        }

        private void AddBuildingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BuildingNameTextBox.Text) || string.IsNullOrWhiteSpace(BuildingAddressTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля");
                    return;
                }

                var building = new Building
                {
                    BuildingName = BuildingNameTextBox.Text,
                    Address = BuildingAddressTextBox.Text
                };

                _dbService.AddBuilding(building);
                MessageBox.Show("Здание добавлено успешно");
                BuildingNameTextBox.Clear();
                BuildingAddressTextBox.Clear();
                RefreshBuildingBtn_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления здания: {ex.Message}");
            }
        }

        private void UpdateBuildingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedBuilding == null)
                {
                    MessageBox.Show("Выберите здание для обновления");
                    return;
                }

                _selectedBuilding.BuildingName = BuildingNameTextBox.Text;
                _selectedBuilding.Address = BuildingAddressTextBox.Text;
                _dbService.UpdateBuilding(_selectedBuilding);
                MessageBox.Show("Здание обновлено успешно");
                BuildingNameTextBox.Clear();
                BuildingAddressTextBox.Clear();
                _selectedBuilding = null;
                RefreshBuildingBtn_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления здания: {ex.Message}");
            }
        }

        private void DeleteBuildingBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedBuilding == null)
                {
                    MessageBox.Show("Выберите здание для удаления");
                    return;
                }

                if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _dbService.DeleteBuilding(_selectedBuilding.BuildingID);
                    MessageBox.Show("Здание удалено успешно");
                    BuildingNameTextBox.Clear();
                    BuildingAddressTextBox.Clear();
                    _selectedBuilding = null;
                    RefreshBuildingBtn_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления здания: {ex.Message}");
            }
        }

        private void BuildingDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (BuildingDataGrid.SelectedItem is Building building)
            {
                _selectedBuilding = building;
                BuildingNameTextBox.Text = building.BuildingName;
                BuildingAddressTextBox.Text = building.Address;
            }
        }

        #endregion

        #region RoomCategory CRUD

        private void RefreshCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CategoryDataGrid.ItemsSource = _dbService.GetAllRoomCategories();
                RoomCategoryCombo.ItemsSource = _dbService.GetAllRoomCategories();
                RoomCategoryCombo.DisplayMemberPath = "CategoryName";
                RoomCategoryCombo.SelectedValuePath = "RoomCategoryID";
                EnergyTimePeriodCombo.ItemsSource = _dbService.GetAllTimePeriods();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки категорий: {ex.Message}");
            }
        }

        private void AddCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
                {
                    MessageBox.Show("Введите название категории");
                    return;
                }

                var category = new RoomCategory { CategoryName = CategoryNameTextBox.Text };
                _dbService.AddRoomCategory(category);
                MessageBox.Show("Категория добавлена успешно");
                CategoryNameTextBox.Clear();
                RefreshCategoryBtn_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления категории: {ex.Message}");
            }
        }

        private void UpdateCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedCategory == null)
                {
                    MessageBox.Show("Выберите категорию для обновления");
                    return;
                }

                _selectedCategory.CategoryName = CategoryNameTextBox.Text;
                _dbService.UpdateRoomCategory(_selectedCategory);
                MessageBox.Show("Категория обновлена успешно");
                CategoryNameTextBox.Clear();
                _selectedCategory = null;
                RefreshCategoryBtn_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления категории: {ex.Message}");
            }
        }

        private void DeleteCategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedCategory == null)
                {
                    MessageBox.Show("Выберите категорию для удаления");
                    return;
                }

                if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _dbService.DeleteRoomCategory(_selectedCategory.RoomCategoryID);
                    MessageBox.Show("Категория удалена успешно");
                    CategoryNameTextBox.Clear();
                    _selectedCategory = null;
                    RefreshCategoryBtn_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления категории: {ex.Message}");
            }
        }

        #endregion

        #region TimePeriod CRUD

        private void RefreshTimePeriodBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TimePeriodDataGrid.ItemsSource = _dbService.GetAllTimePeriods();
                EnergyTimePeriodCombo.ItemsSource = _dbService.GetAllTimePeriods();
                EnergyTimePeriodCombo.DisplayMemberPath = "PeriodName";
                EnergyTimePeriodCombo.SelectedValuePath = "TimePeriodID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки периодов: {ex.Message}");
            }
        }

        private void AddTimePeriodBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TimePeriodNameTextBox.Text))
                {
                    MessageBox.Show("Введите название периода");
                    return;
                }

                var period = new TimePeriod { PeriodName = TimePeriodNameTextBox.Text };
                _dbService.AddTimePeriod(period);
                MessageBox.Show("Период добавлен успешно");
                TimePeriodNameTextBox.Clear();
                RefreshTimePeriodBtn_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления периода: {ex.Message}");
            }
        }

        private void UpdateTimePeriodBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedTimePeriod == null)
                {
                    MessageBox.Show("Выберите период для обновления");
                    return;
                }

                _selectedTimePeriod.PeriodName = TimePeriodNameTextBox.Text;
                _dbService.UpdateTimePeriod(_selectedTimePeriod);
                MessageBox.Show("Период обновлен успешно");
                TimePeriodNameTextBox.Clear();
                _selectedTimePeriod = null;
                RefreshTimePeriodBtn_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления периода: {ex.Message}");
            }
        }

        private void DeleteTimePeriodBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedTimePeriod == null)
                {
                    MessageBox.Show("Выберите период для удаления");
                    return;
                }

                if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _dbService.DeleteTimePeriod(_selectedTimePeriod.TimePeriodID);
                    MessageBox.Show("Период удален успешно");
                    TimePeriodNameTextBox.Clear();
                    _selectedTimePeriod = null;
                    RefreshTimePeriodBtn_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления периода: {ex.Message}");
            }
        }

        #endregion

        #region Room CRUD

        private void RefreshRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RoomDataGrid.ItemsSource = _dbService.GetAllRooms();
                EnergyRoomCombo.ItemsSource = _dbService.GetAllRooms();
                EnergyRoomCombo.DisplayMemberPath = "BuildingName";
                EnergyRoomCombo.SelectedValuePath = "RoomID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки помещений: {ex.Message}");
            }
        }

        private void AddRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RoomBuildingCombo.SelectedValue == null || string.IsNullOrWhiteSpace(RoomFloorTextBox.Text) || RoomCategoryCombo.SelectedValue == null)
                {
                    MessageBox.Show("Заполните все поля");
                    return;
                }

                var room = new Room
                {
                    BuildingID = (int)RoomBuildingCombo.SelectedValue,
                    Floor = int.Parse(RoomFloorTextBox.Text),
                    RoomCategoryID = (int)RoomCategoryCombo.SelectedValue
                };

                _dbService.AddRoom(room);
                MessageBox.Show("Помещение добавлено успешно");
                RoomFloorTextBox.Clear();
                RefreshRoomBtn_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления помещения: {ex.Message}");
            }
        }

        private void UpdateRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedRoom == null)
                {
                    MessageBox.Show("Выберите помещение для обновления");
                    return;
                }

                _selectedRoom.BuildingID = (int)RoomBuildingCombo.SelectedValue;
                _selectedRoom.Floor = int.Parse(RoomFloorTextBox.Text);
                _selectedRoom.RoomCategoryID = (int)RoomCategoryCombo.SelectedValue;
                _dbService.UpdateRoom(_selectedRoom);
                MessageBox.Show("Помещение обновлено успешно");
                RoomFloorTextBox.Clear();
                _selectedRoom = null;
                RefreshRoomBtn_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления помещения: {ex.Message}");
            }
        }

        private void DeleteRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedRoom == null)
                {
                    MessageBox.Show("Выберите помещение для удаления");
                    return;
                }

                if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _dbService.DeleteRoom(_selectedRoom.RoomID);
                    MessageBox.Show("Помещение удалено успешно");
                    RoomFloorTextBox.Clear();
                    _selectedRoom = null;
                    RefreshRoomBtn_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления помещения: {ex.Message}");
            }
        }

        private void RoomDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (RoomDataGrid.SelectedItem is Room room)
            {
                _selectedRoom = room;
                RoomBuildingCombo.SelectedValue = room.BuildingID;
                RoomFloorTextBox.Text = room.Floor.ToString();
                RoomCategoryCombo.SelectedValue = room.RoomCategoryID;
            }
        }

        #endregion

        #region Energy Consumption CRUD

        private void RefreshEnergyBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnergyDataGrid.ItemsSource = _dbService.GetAllEnergyConsumptions();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void AddEnergyBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (EnergyRoomCombo.SelectedValue == null || EnergyDatePicker.SelectedDate == null || 
                    EnergyTimePeriodCombo.SelectedValue == null || string.IsNullOrWhiteSpace(EnergyValueTextBox.Text))
                {
                    MessageBox.Show("Заполните все поля");
                    return;
                }

                var consumption = new EnergyConsumption
                {
                    RoomID = (int)EnergyRoomCombo.SelectedValue,
                    Date = EnergyDatePicker.SelectedDate.Value,
                    TimePeriodID = (int)EnergyTimePeriodCombo.SelectedValue,
                    EnergyValue = decimal.Parse(EnergyValueTextBox.Text)
                };

                _dbService.AddEnergyConsumption(consumption);
                MessageBox.Show("Данные энергопотребления добавлены успешно");
                EnergyValueTextBox.Clear();
                RefreshEnergyBtn_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления данных: {ex.Message}");
            }
        }

        private void UpdateEnergyBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedEnergy == null)
                {
                    MessageBox.Show("Выберите запись для обновления");
                    return;
                }

                _selectedEnergy.RoomID = (int)EnergyRoomCombo.SelectedValue;
                _selectedEnergy.Date = EnergyDatePicker.SelectedDate.Value;
                _selectedEnergy.TimePeriodID = (int)EnergyTimePeriodCombo.SelectedValue;
                _selectedEnergy.EnergyValue = decimal.Parse(EnergyValueTextBox.Text);
                _dbService.UpdateEnergyConsumption(_selectedEnergy);
                MessageBox.Show("Данные обновлены успешно");
                EnergyValueTextBox.Clear();
                _selectedEnergy = null;
                RefreshEnergyBtn_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления данных: {ex.Message}");
            }
        }

        private void DeleteEnergyBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedEnergy == null)
                {
                    MessageBox.Show("Выберите запись для удаления");
                    return;
                }

                if (MessageBox.Show("Вы уверены?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _dbService.DeleteEnergyConsumption(_selectedEnergy.ConsumptionID);
                    MessageBox.Show("Запись удалена успешно");
                    EnergyValueTextBox.Clear();
                    _selectedEnergy = null;
                    RefreshEnergyBtn_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления записи: {ex.Message}");
            }
        }

        private void EnergyDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (EnergyDataGrid.SelectedItem is EnergyConsumption energy)
            {
                _selectedEnergy = energy;
                EnergyRoomCombo.SelectedValue = energy.RoomID;
                EnergyDatePicker.SelectedDate = energy.Date;
                EnergyTimePeriodCombo.SelectedValue = energy.TimePeriodID;
                EnergyValueTextBox.Text = energy.EnergyValue.ToString();
            }
        }

        #endregion

        #region SQL Console

        private void ExecuteQueryBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SqlQueryTextBox.Text))
                {
                    MessageBox.Show("Введите SQL запрос");
                    return;
                }

                var result = _dbService.ExecuteCustomQuery(SqlQueryTextBox.Text);
                SqlResultGrid.ItemsSource = result.DefaultView;
                SqlResultStatus.Text = $"Результаты: {result.Rows.Count} строк";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}");
                SqlResultStatus.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void ClearQueryBtn_Click(object sender, RoutedEventArgs e)
        {
            SqlQueryTextBox.Clear();
            SqlResultGrid.ItemsSource = null;
            SqlResultStatus.Text = "Результаты:";
        }

        #endregion

        #region Reports

        private void GenerateEnergyReportBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ReportStartDatePicker.SelectedDate == null || ReportEndDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите диапазон дат");
                    return;
                }

                var report = _dbService.GetEnergyConsumptionReport(
                    ReportStartDatePicker.SelectedDate.Value,
                    ReportEndDatePicker.SelectedDate.Value);

                EnergyReportGrid.ItemsSource = report.DefaultView;
                MessageBox.Show($"Отчет сформирован. Строк: {report.Rows.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования отчета: {ex.Message}");
            }
        }

        private void GenerateStatisticsBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var stats = _dbService.GetBuildingStatistics();
                StatisticsGrid.ItemsSource = stats.DefaultView;
                MessageBox.Show($"Статистика загружена. Строк: {stats.Rows.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка формирования статистики: {ex.Message}");
            }
        }

        #endregion

        #region AdminKey Management

        private void AddAdminKeyBtn_Click(object sender, RoutedEventArgs e)
        {
            string newKeyValue = NewAdminKeyTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(newKeyValue))
            {
                MessageBox.Show("Введите значение ключа!", "Ошибка");
                return;
            }

            if (newKeyValue.Length < 8)
            {
                MessageBox.Show("Ключ должен быть не менее 8 символов!", "Ошибка");
                return;
            }

            try
            {
                if (_dbService.AddAdminKey(newKeyValue))
                {
                    MessageBox.Show("✅ Ключ администратора успешно добавлен!", "Успех");
                    NewAdminKeyTextBox.Clear();
                    RefreshAdminKeys();
                }
                else
                {
                    MessageBox.Show("❌ Ошибка при добавлении ключа. Возможно, такой ключ уже существует.", "Ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void DeactivateKeyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AdminKeysDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите ключ из списка!", "Ошибка");
                return;
            }

            AdminKey selectedKey = AdminKeysDataGrid.SelectedItem as AdminKey;
            if (selectedKey == null)
                return;

            MessageBoxResult result = MessageBox.Show(
                $"Вы уверены, что хотите деактивировать ключ '{selectedKey.AdminKeyValue}'?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (_dbService.DeactivateAdminKey(selectedKey.KeyID))
                    {
                        MessageBox.Show("✅ Ключ успешно деактивирован!", "Успех");
                        RefreshAdminKeys();
                    }
                    else
                    {
                        MessageBox.Show("❌ Ошибка при деактивации ключа.", "Ошибка");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                }
            }
        }

        private void DeleteKeyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AdminKeysDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите ключ из списка!", "Ошибка");
                return;
            }

            AdminKey selectedKey = AdminKeysDataGrid.SelectedItem as AdminKey;
            if (selectedKey == null)
                return;

            MessageBoxResult result = MessageBox.Show(
                $"Вы уверены, что хотите удалить ключ '{selectedKey.AdminKeyValue}'?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (_dbService.DeleteAdminKey(selectedKey.KeyID))
                    {
                        MessageBox.Show("✅ Ключ успешно удален!", "Успех");
                        RefreshAdminKeys();
                    }
                    else
                    {
                        MessageBox.Show("❌ Ошибка при удалении ключа.", "Ошибка");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
                }
            }
        }

        private void RefreshKeysBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshAdminKeys();
        }

        private void AdminKeysDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // При двойном клике можно добавить дополнительные действия
            if (AdminKeysDataGrid.SelectedItem != null)
            {
                AdminKey selectedKey = AdminKeysDataGrid.SelectedItem as AdminKey;
                MessageBox.Show($"Ключ: {selectedKey.AdminKeyValue}\nАктивен: {(selectedKey.IsActive ? "Да" : "Нет")}", "Информация о ключе");
            }
        }

        private void RefreshAdminKeys()
        {
            try
            {
                AdminKeysDataGrid.ItemsSource = _dbService.GetAllAdminKeys();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ключей: {ex.Message}", "Ошибка");
            }
        }

        #endregion
    }
}
