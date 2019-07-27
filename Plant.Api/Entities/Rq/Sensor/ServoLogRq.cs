namespace Plant.Api.Entities.Rq.Sensor {
    public class ServoLogRq {
        public int Value { get; set; }
        public int DegreesInitial { get; set; }
        public int DegreesFinal { get; set; }
        public int OpenedTimeInSeconds { get; set; }
    }
}