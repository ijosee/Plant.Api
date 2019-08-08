using System;
namespace Plant.Api.Entities.Rs.Calendar {
    public class FullCalendarRs {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}