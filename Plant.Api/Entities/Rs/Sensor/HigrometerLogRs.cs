using System;

namespace Plant.Api.Entities.Rs.Sensor {

    public class HigrometerLogRs {
        public int Id { get; set; }
        public int Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}