using System;

namespace Plant.Api.Entities.Rs.Humidity {

    public class HumidityLogRs {
        public int Id { get; set; }
        public float Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}