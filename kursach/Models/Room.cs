namespace kursach.Models
{
    public class Room
    {
        public int RoomID { get; set; }
        public int BuildingID { get; set; }
        public int Floor { get; set; }
        public int RoomCategoryID { get; set; }
        public string BuildingName { get; set; }
        public string CategoryName { get; set; }

        /// <summary>Подпись для списков и фильтров.</summary>
        public string RoomDisplay =>
            string.IsNullOrEmpty(BuildingName)
                ? $"Помещение №{RoomID}, эт. {Floor}"
                : $"{BuildingName}, эт. {Floor}, №{RoomID} ({CategoryName})";
    }
}
