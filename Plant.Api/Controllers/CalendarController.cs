using System;
using System.Collections.Generic;
using System.Linq;
using Interaces.Services.AppSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Plant.Api.Entities.Rq.Calendar;
using Plant.Api.Entities.Rq.DataTable;
using Plant.Api.Entities.Rs.Calendar;

namespace Plant.Api.Controllers {

    [Route ("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase {

        public ILogger _logger;
        readonly IAppSettings _appSettings;

        public CalendarController (ILogger logger, IAppSettings appSettings) {
            _logger = logger;
            _appSettings = appSettings;
        }

        [HttpGet]
        public ActionResult<IEnumerable<int>> Get () {

            List<int> result = new List<int> ();
            try {
                using (MySqlConnection connection = new MySqlConnection (_appSettings.GetDataBaseConnectionString ())) {

                    connection.Open ();
                    var query = $"SELECT * FROM CALENDAR_EVENT ";

                    var command = new MySqlCommand (query, connection);

                    var sqlReader = command.ExecuteReader ();
                    if (sqlReader.HasRows) {
                        while (sqlReader.Read ()) {
                            result.Add (sqlReader.GetInt32 (sqlReader.GetOrdinal ("id")));
                        }
                    } else {
                        return StatusCode (204);
                    }
                }
            } catch (System.Exception ex) {
                _logger.LogError ($"{ex.Message}");
                return StatusCode (500);
            }

            _logger.LogInformation ($"Total results : {result.Count()}");
            return result;
        }

        [HttpGet ("{id}")]
        public ActionResult<CalendarLogRs> Get (int id) {

            var result = new CalendarLogRs ();
            _logger.LogInformation ($"[*********************** >][Request - id] : {id}");
            try {

                using (MySqlConnection connection = new MySqlConnection (_appSettings.GetDataBaseConnectionString ())) {

                    var value = id;

                    connection.Open ();
                    var query = $"SELECT * FROM CALENDAR_EVENT " +
                        $"WHERE id = @id ";

                    var command = new MySqlCommand (query, connection);
                    command.Parameters.Add ("@id", MySqlDbType.Int32);
                    command.Parameters["@id"].Value = value;

                    var sqlReader = command.ExecuteReader ();
                    if (sqlReader.HasRows) {
                        result = new CalendarLogRs ();
                        while (sqlReader.Read ()) {
                            result.Id = sqlReader.GetInt32 (sqlReader.GetOrdinal ("id"));
                            result.Title = sqlReader.GetString (sqlReader.GetOrdinal ("title"));
                            result.Description = sqlReader.GetString (sqlReader.GetOrdinal ("description"));
                            result.Start = sqlReader.GetDateTime (sqlReader.GetOrdinal ("start"));
                            result.End = sqlReader.GetDateTime (sqlReader.GetOrdinal ("end"));
                        }
                    } else {
                        return StatusCode (204);
                    }
                }

            } catch (System.Exception ex) {
                _logger.LogError ($"{ex.Message}");
                return StatusCode (500);
            }
            _logger.LogInformation ($"[*********************** >][Result - id] : {JsonConvert.SerializeObject(result)}");
            return result;
        }

        [HttpPost]
        public IActionResult Post ([FromBody] CalendarLogRs request) {

            if (request == null || string.IsNullOrEmpty (request.Title) || string.IsNullOrEmpty (request.Description)) {
                return StatusCode (400, "Please fill correctly request.");
            } else {
                _logger.LogInformation ($"[Request] ... {JsonConvert.SerializeObject(request)}");
            }

            try {

                using (MySqlConnection connection = new MySqlConnection (_appSettings.GetDataBaseConnectionString ())) {

                    connection.Open ();
                    var query = $"INSERT INTO " +
                        $"CALENDAR_EVENT(`title`,`description`,`start`,`end`,`timestamp`) " +
                        $"VALUES (@title,@description,@start,@end,@timestamp)";

                    var command = new MySqlCommand (query, connection);
                    command.Parameters.Add ("@title", MySqlDbType.String);
                    command.Parameters["@title"].Value = request.Title;

                    command.Parameters.Add ("@description", MySqlDbType.String);
                    command.Parameters["@description"].Value = request.Description;

                    command.Parameters.Add ("@start", MySqlDbType.DateTime);
                    command.Parameters["@start"].Value = request.Start;

                    command.Parameters.Add ("@end", MySqlDbType.DateTime);
                    command.Parameters["@end"].Value = request.End;

                    command.Parameters.Add ("@timestamp", MySqlDbType.DateTime);
                    command.Parameters["@timestamp"].Value = DateTime.Now;

                    Int32 rowsAffected = command.ExecuteNonQuery ();
                    _logger.LogInformation ($"[RowsAffected] ... {rowsAffected}");
                }

            } catch (System.Exception ex) {
                _logger.LogError ($"{ex.Message}");
                return StatusCode (500);
            }

            return StatusCode (200);
        }

        [HttpDelete ("{id}")]
        public IActionResult Delete (int id) {

            try {

                _logger.LogInformation ($"[Request] ... {id}");

                using (MySqlConnection connection = new MySqlConnection (_appSettings.GetDataBaseConnectionString ())) {

                    connection.Open ();
                    var query = $"DELETE FROM CALENDAR_EVENT WHERE id = @id ";

                    var command = new MySqlCommand (query, connection);
                    command.Parameters.Add ("@id", MySqlDbType.Int32);
                    command.Parameters["@id"].Value = id;

                    var sqlResult = command.ExecuteNonQuery ();
                    if (sqlResult != 1) {
                        _logger.LogInformation ($"[Deleted] ... {id}");
                    }
                }

            } catch (System.Exception ex) {
                _logger.LogError ($"{ex.Message}");
                return StatusCode (500);
            }

            return StatusCode (200);
        }

        /// <summary>
        /// Adapter for fullcalendar
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>a formated value for draw fullcalendar.js  format</returns>
        [HttpGet]
        [Route ("GetFullCalendar")]
        public ActionResult<List<FullCalendarModel>> GetFullCalendar (DateTime from, DateTime to) {

            var addDateFilter = false;
            if (!from.Equals (DateTime.MinValue) && !from.ToString ("u").Equals ("0000-00-00 00:00:00Z") ||
                !to.Equals (DateTime.MinValue) && !to.ToString ("u").Equals ("0000-00-00 00:00:00Z")
            ) {
                addDateFilter = true;
            }
            _logger.LogInformation ($"[*********************** >][Request - from] : {from.ToString ("u")}");
            _logger.LogInformation ($"[*********************** >][Request - to] : {to.ToString ("u")}");

            var result = new List<FullCalendarModel> ();
            try {

                using (MySqlConnection connection = new MySqlConnection (_appSettings.GetDataBaseConnectionString ())) {

                    var query = $"SELECT * FROM CALENDAR_EVENT WHERE 1=1 ";
                    var command = new MySqlCommand ();

                    if (addDateFilter) {
                        query += $"AND start BETWEEN @to AND @from ";

                        command.Parameters.Add ("@from", MySqlDbType.DateTime);
                        command.Parameters["@from"].Value = from;

                        command.Parameters.Add ("@to", MySqlDbType.DateTime);
                        command.Parameters["@to"].Value = to;
                    }

                    command.CommandText = query;
                    _logger.LogInformation ($"Query : {query}");
                    command.Connection = connection;

                    connection.Open ();
                    var sqlReader = command.ExecuteReader ();
                    if (sqlReader.HasRows) {
                        while (sqlReader.Read ()) {

                            var item = new FullCalendarModel ();

                            item.Id = sqlReader.GetInt32 (sqlReader.GetOrdinal ("id"));
                            item.Title = sqlReader.GetString (sqlReader.GetOrdinal ("title"));
                            item.Description = sqlReader.GetString (sqlReader.GetOrdinal ("description"));
                            item.Start = sqlReader.GetDateTime (sqlReader.GetOrdinal ("start"));
                            item.End = sqlReader.GetDateTime (sqlReader.GetOrdinal ("end"));

                            result.Add (item);
                        }

                    } else {
                        return StatusCode (204);
                    }
                }
            } catch (System.Exception ex) {
                _logger.LogError ($"{ex.Message}");
                return StatusCode (500);
            }

            _logger.LogInformation ($"[*********************** >][Result - count] : {result.Count}");
            return result;
        }

        /// <summary>
        /// Adapter from dataTable.js
        /// </summary>
        /// <returns>a formated value for draw dataTable.js bootstrap format</returns>
        [HttpPost]
        [Route ("GetDataTable")]
        public ActionResult<CalendarDataTableRs> GetDataTable (CalendarDataTableRq request) {

            if (request == null) {
                return StatusCode (400);
            } else {
                _logger.LogInformation ($"[*********************** >][Request] : {JsonConvert.SerializeObject(request)}");
            }
            var result = new CalendarDataTableRs ();
            List<CalendarLogRs> DataBaseResult = new List<CalendarLogRs> ();
            try {
                using (MySqlConnection connection = new MySqlConnection (_appSettings.GetDataBaseConnectionString ())) {

                    var queryConditions = "";

                    if (request.order != null && request.order.Any ()) {
                        foreach (var item in request.order) {
                            foreach (var itemOfDicctionary in item) {

                                if (itemOfDicctionary.Key.Equals ("column")) {
                                    queryConditions += $" ORDER BY id ";
                                }
                                if (itemOfDicctionary.Key.Equals ("dir")) {
                                    queryConditions += $" {itemOfDicctionary.Value.ToUpper()} ";
                                }
                            }
                        }
                        queryConditions += $"";
                    }

                    if (request.length > 0) {
                        queryConditions += $" LIMIT {request.start},{request.length} ";
                    }

                    connection.Open ();
                    var query = $"SELECT * FROM CALENDAR_EVENT " +
                        $"WHERE 1=1 " +
                        $"{queryConditions}" +
                        $"";

                    var command = new MySqlCommand (query, connection);
                    var sqlReader = command.ExecuteReader ();
                    if (sqlReader.HasRows) {
                        while (sqlReader.Read ()) {
                            var log = new CalendarLogRs ();

                            log.Id = sqlReader.GetInt32 (sqlReader.GetOrdinal ("id"));
                            log.Title = sqlReader.GetString (sqlReader.GetOrdinal ("title"));
                            log.Description = sqlReader.GetString (sqlReader.GetOrdinal ("description"));
                            log.Start = sqlReader.GetDateTime (sqlReader.GetOrdinal ("start"));
                            log.End = sqlReader.GetDateTime (sqlReader.GetOrdinal ("end"));
                            log.Timestamp = sqlReader.GetDateTime (sqlReader.GetOrdinal ("timestamp"));

                            DataBaseResult.Add (log);
                        }

                        // satisfy datatable model
                        result.Data = DataBaseResult;
                        result.Draw = request.draw;
                        if (DataBaseResult != null && DataBaseResult.Any ()) {
                            result.RecordsTotal = this.Get ().Value.ToList ().Count;
                            result.RecordsFiltered = this.Get ().Value.ToList ().Count;
                        }
                    } else {
                        return StatusCode (204);
                    }
                }
            } catch (System.Exception ex) {
                _logger.LogError ($"{ex.Message}");
                return StatusCode (500);
            }

            _logger.LogInformation ($"[*********************** >][Result - count] : {result.RecordsTotal}");
            return result;
        }
    }
}