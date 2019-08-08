using System;

namespace Plant.Api.Entities.Rq.DataTable {
    public class FullCalendarModel {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}