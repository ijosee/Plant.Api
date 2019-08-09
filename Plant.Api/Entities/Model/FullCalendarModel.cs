using System;

namespace Plant.Api.Entities.Rq.DataTable {
    public class FullCalendarModel {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public bool AllDay { get; set; }
    }
}