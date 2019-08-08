using System;
using System.Collections.Generic;
using System.Linq;
using Interaces.Services.AppSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Plant.Api.Entities.Rq.DataTable;
using Plant.Api.Entities.Rq.Light;
using Plant.Api.Entities.Rs.Light;

namespace Plant.Api.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class LightController : ControllerBase {
        public ILogger _logger;
        readonly IAppSettings _appSettings;

        public LightController (ILogger<LightController> logger, IAppSettings appSettings) {
            _logger = logger;
            _appSettings = appSettings;
        }

        [HttpGet]
        public ActionResult<IEnumerable<int>> Get () {

            List<int> result = new List<int> ();
            try {
                using (MySqlConnection connection = new MySqlConnection (_appSettings.GetDataBaseConnectionString ())) {

                    connection.Open ();
                    var query = $"SELECT * FROM LIGHT_LOGS ";

                    var command = new MySqlCommand (query, connection);

                    var sqlReader = command.ExecuteReader ();
                    if (sqlReader.HasRows) {
                        while (sqlReader.Read ()) {
                            result.Add (sqlReader.GetInt32 (sqlReader.GetOrdinal ("id")));
                        }
                    } else {
                        _logger.LogInformation ($" No results .");
                        return StatusCode (204);
                    }
                }
            } catch (System.Exception ex) {
                _logger.LogError ($"{ex.Message}");
                throw;
            }

            _logger.LogInformation ($" Total results : {result.Count()}");
            return result;
        }

        [HttpGet ("{id}")]
        public ActionResult<LightLogRs> Get (int id) {

            var result = new LightLogRs ();
            _logger.LogInformation ($" [*********************** >][Request - id] : {id}");
            try {

                using (MySqlConnection connection = new MySqlConnection (_appSettings.GetDataBaseConnectionString ())) {

                    var value = id;

                    connection.Open ();
                    var query = $"SELECT * FROM LIGHT_LOGS " +
                        $"WHERE id = @id ";

                    var command = new MySqlCommand (query, connection);
                    command.Parameters.Add ("@id", MySqlDbType.Int32);
                    command.Parameters["@id"].Value = value;

                    var sqlReader = command.ExecuteReader ();
                    if (sqlReader.HasRows) {
                        result = new LightLogRs ();
                        while (sqlReader.Read ()) {
                            result.Id = sqlReader.GetInt32 (sqlReader.GetOrdinal ("id"));
                            result.Value = sqlReader.GetInt32 (sqlReader.GetOrdinal ("value"));
                            result.Timestamp = sqlReader.GetDateTime (sqlReader.GetOrdinal ("timestamp"));
                        }
                    } else {
                        _logger.LogInformation ($" No results .");
                        return StatusCode (204);
                    }
                }

            } catch (System.Exception ex) {
                _logger.LogError ($"{ex.Message}");
                return StatusCode (500);
            }
            _logger.LogInformation ($" [*********************** >][Result - id] : {JsonConvert.SerializeObject(result)}");
            return result;
        }

        [HttpPost]
        public IActionResult Post ([FromBody] LightLogRq request) {

            if (request == null) {
                return StatusCode (400, "Please fill correctly request.");
            } else {
                _logger.LogInformation ($" [Request] ... {JsonConvert.SerializeObject(request)}");
            }

            try {

                _logger.LogInformation ($" [Request] ... {JsonConvert.SerializeObject(request)}");

                using (MySqlConnection connection = new MySqlConnection (_appSettings.GetDataBaseConnectionString ())) {

                    connection.Open ();
                    var query = $"INSERT INTO " +
                        $"LIGHT_LOGS(`value`,`timestamp`) " +
                        $"VALUES (@value,@date)";

                    var command = new MySqlCommand (query, connection);
                    command.Parameters.Add ("@value", MySqlDbType.Int32);
                    command.Parameters["@value"].Value = request.Value;

                    command.Parameters.Add ("@date", MySqlDbType.DateTime);
                    command.Parameters["@date"].Value = DateTime.Now;

                    Int32 rowsAffected = command.ExecuteNonQuery ();
                    _logger.LogInformation ($" [RowsAffected] ... {rowsAffected}");
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

                _logger.LogInformation ($" [Request] ... {id}");

                using (MySqlConnection connection = new MySqlConnection (_appSettings.GetDataBaseConnectionString ())) {

                    connection.Open ();
                    var query = $"DELETE FROM LIGHT_LOGS WHERE id = @id ";

                    var command = new MySqlCommand (query, connection);
                    command.Parameters.Add ("@id", MySqlDbType.Int32);
                    command.Parameters["@id"].Value = id;

                    var sqlResult = command.ExecuteNonQuery ();
                    if (sqlResult != 1) {
                        _logger.LogInformation ($" [Deleted] ... {id}");
                    }
                }

            } catch (System.Exception ex) {
                _logger.LogError ($"{ex.Message}");
                return StatusCode (500);
            }

            return StatusCode (200);
        }

        /// <summary>
        /// Adapter from chart.js
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>a formated value for draw chart.js bootstrap format</returns>
        [HttpGet]
        [Route ("GetChart")]
        public ActionResult<IEnumerable<ChartModel>> GetChartLightData (DateTime from, DateTime to) {

            var addDateFilter = false;
            if (!from.Equals (DateTime.MinValue) && !from.ToString ("u").Equals ("0000-00-00 00:00:00Z") ||
                !to.Equals (DateTime.MinValue) && !to.ToString ("u").Equals ("0000-00-00 00:00:00Z")
            ) {
                addDateFilter = true;
            }
            _logger.LogInformation ($" [*********************** >][Request - from] : {from.ToString ("u")}");
            _logger.LogInformation ($" [*********************** >][Request - to] : {to.ToString ("u")}");

            var result = new List<ChartModel> ();
            try {

                using (MySqlConnection connection = new MySqlConnection (_appSettings.GetDataBaseConnectionString ())) {

                    var query = $"SELECT * FROM LIGHT_LOGS WHERE 1=1 ";
                    var command = new MySqlCommand ();

                    if (addDateFilter) {
                        query += $"AND timestamp BETWEEN " +
                            $"CONVERT('{from.ToString ("u")}',datetime) " +
                            $"AND CONVERT('{to.ToString ("u")}',datetime) ";

                        // command.Parameters.Add ("@from", MySqlDbType.DateTime);
                        // command.Parameters["@from"].Value = from;

                        // command.Parameters.Add ("@to", MySqlDbType.DateTime);
                        // command.Parameters["@to"].Value = to;
                    }

                    command.CommandText = query;
                    _logger.LogInformation ($" Query : {query}");
                    command.Connection = connection;

                    connection.Open ();
                    var sqlReader = command.ExecuteReader ();
                    if (sqlReader.HasRows) {
                        while (sqlReader.Read ()) {

                            var item = new ChartModel ();

                            var valueInt = sqlReader.GetInt32 (sqlReader.GetOrdinal ("value"));
                            item.y = $"{valueInt}";
                            var date = sqlReader.GetDateTime (sqlReader.GetOrdinal ("timestamp"));
                            item.x = date.ToString ("dd/MM/yyyy HH:mm:ss");

                            result.Add (item);
                        }

                    } else {
                        _logger.LogInformation ($" No results .");
                        return StatusCode (204);
                    }
                }
            } catch (System.Exception ex) {
                _logger.LogError ($"{ex.Message}");
                return StatusCode (500);
            }

            _logger.LogInformation ($" [*********************** >][Result - count] : {result.Count}");
            return result;
        }

        /// <summary>
        /// Adapter from dataTable.js
        /// </summary>
        /// <returns>a formated value for draw dataTable.js bootstrap format</returns>
        [HttpPost]
        [Route ("GetDataTable")]
        public ActionResult<LightDataTableRs> GetDataTable (LightDataTableRq request) {

            if (request == null) {
                return StatusCode (400);
            } else {
                _logger.LogInformation ($" [*********************** >][Request] : {JsonConvert.SerializeObject(request)}");
            }
            var result = new LightDataTableRs ();
            List<LightLogRs> DataBaseResult = new List<LightLogRs> ();
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
                    var query = $"SELECT * FROM LIGHT_LOGS " +
                        $"WHERE 1=1 " +
                        $"{queryConditions}" +
                        $"";

                    var command = new MySqlCommand (query, connection);
                    var sqlReader = command.ExecuteReader ();
                    if (sqlReader.HasRows) {
                        while (sqlReader.Read ()) {
                            var log = new LightLogRs ();

                            log.Id = sqlReader.GetInt32 (sqlReader.GetOrdinal ("id"));
                            log.Value = sqlReader.GetInt32 (sqlReader.GetOrdinal ("value"));
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
                        _logger.LogInformation ($" No results .");
                        return StatusCode (204);
                    }
                }
            } catch (System.Exception ex) {
                _logger.LogError ($"{ex.Message}");
                return StatusCode (500);
            }

            _logger.LogInformation ($" [*********************** >][Result - count] : {result.RecordsTotal}");
            return result;
        }

    }
}