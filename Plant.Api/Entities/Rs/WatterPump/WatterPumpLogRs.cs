using System;
namespace Plant.Api.Entities.Rs.WatterPump {
    public class WatterPumpLogRs {
        public int Id { get; set; }
        public int Value { get; set; }
        public int Flow { get; set; }
        public int OpenedTimeInSeconds { get; set; }
        public DateTime Timestamp { get; set; }
    }
}