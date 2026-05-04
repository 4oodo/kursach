using System;

namespace kursach.Models
{
    public class EnergyConsumption
    {
        public int ConsumptionID { get; set; }
        public int RoomID { get; set; }
        public DateTime Date { get; set; }
        public int TimePeriodID { get; set; }
        public decimal EnergyValue { get; set; }
        public string RoomInfo { get; set; }
        public string TimePeriodName { get; set; }
    }
}
