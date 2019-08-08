namespace Plant.Api.Entities.Rq.WatterPump {
    public class WatterPumpLogRq {
        public int Value { get; set; }
        public int DegreesInitial { get; set; }
        public int DegreesFinal { get; set; }
        public int OpenedTimeInSeconds { get; set; }
    }
}