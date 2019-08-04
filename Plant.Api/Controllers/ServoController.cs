using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Plant.Api.Entities.Model;
using Plant.Api.Entities.Rq.Sensor;
using Plant.Api.Entities.Rs;

namespace Plant.Api.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class ServoController : ControllerBase {

        public string ConnectionString = "";

        IConfiguration _configuration;
        public ServoController (IConfiguration configuration) {
            _configuration = configuration;

            ConnectionString = _configuration.GetSection ("ConnectionString").GetSection ("DatabaseConnection").Value.ToString ();
        }

        [HttpGet]
        public ActionResult<IEnumerable<int>> Get () {

            List<int> result = new List<int> ();
            try {
                using (MySqlConnection connection = new MySqlConnection (ConnectionString)) {

                    connection.Open ();
                    var query = $"SELECT * FROM SERVO_LOGS ";

                    var command = new MySqlCommand (query, connection);

                    var sqlReader = command.ExecuteReader ();
                    if (sqlReader.HasRows) {
                        while (sqlReader.Read ()) {
                            result.Add (sqlReader.GetInt32 (sqlReader.GetOrdinal ("id")));
                        }
                    }
                }
            } catch (System.Exception ex) {
                Console.WriteLine ($"ERROR ... {ex.Message}");
                throw;
            }

            return result;
        }

        [HttpGet ("{id}")]
        public ActionResult<ServoLogRs> Get (int id) {

            ServoLogRs result = null;
            try {

                Console.WriteLine ($"[Request] ... {id}");

                using (MySqlConnection connection = new MySqlConnection (ConnectionString)) {

                    var value = id;

                    Console.WriteLine ($"[value] ... {value}");

                    connection.Open ();
                    var query = $"SELECT * FROM SERVO_LOGS " +
                        $"WHERE id = @id ";

                    var command = new MySqlCommand (query, connection);
                    command.Parameters.Add ("@id", MySqlDbType.String);
                    command.Parameters["@id"].Value = value;

                    var sqlReader = command.ExecuteReader ();
                    if (sqlReader.HasRows) {
                        result = new ServoLogRs ();
                        while (sqlReader.Read ()) {
                            result.Id = sqlReader.GetInt32 (sqlReader.GetOrdinal ("id"));
                            result.Value = sqlReader.GetInt32 (sqlReader.GetOrdinal ("value"));
                            result.DegreesInitial = sqlReader.GetInt32 (sqlReader.GetOrdinal ("degreesInitial"));
                            result.DegreesFinal = sqlReader.GetInt32 (sqlReader.GetOrdinal ("degreesFinal"));
                            result.OpenedTimeInSeconds = sqlReader.GetInt32 (sqlReader.GetOrdinal ("openedTimeInSeconds"));
                            result.Timestamp = sqlReader.GetDateTime (sqlReader.GetOrdinal ("timestamp"));
                        }
                    }
                }

            } catch (System.Exception ex) {
                Console.WriteLine ($"ERROR ... {ex.Message}");
                throw;
            }

            return result;
        }

        [HttpPost]
        public void Post ([FromBody] ServoLogRq request) {

            try {

                Console.WriteLine ($"[Request] ... {JsonConvert.SerializeObject(request)}");

                using (MySqlConnection connection = new MySqlConnection (ConnectionString)) {

                    var value = request.Value;
                    var degreesInitial = request.DegreesInitial;
                    var degreesFinal = request.DegreesFinal;
                    var openedTimeInSeconds = request.OpenedTimeInSeconds;
                    var date = DateTime.Now;

                    Console.WriteLine ($"[value] ... {value}");
                    Console.WriteLine ($"[degreesInitial] ... {degreesInitial}");
                    Console.WriteLine ($"[degreesFinal] ... {degreesFinal}");
                    Console.WriteLine ($"[openedTimeInSeconds] ... {openedTimeInSeconds}");
                    Console.WriteLine ($"[date] ... {date}");

                    connection.Open ();
                    var query = $"INSERT INTO " +
                        $"SERVO_LOGS(`value`,`degreesInitial`,`degreesFinal`,`openedTimeInSeconds`,`timestamp`) " +
                        $"VALUES (@value,@degreesInitial,@degreesFinal,@openedTimeInSeconds,@date)";

                    var command = new MySqlCommand (query, connection);
                    command.Parameters.Add ("@value", MySqlDbType.Int32);
                    command.Parameters["@value"].Value = value;

                    command.Parameters.Add ("@degreesInitial", MySqlDbType.Int32);
                    command.Parameters["@degreesInitial"].Value = degreesInitial;

                    command.Parameters.Add ("@degreesFinal", MySqlDbType.Int32);
                    command.Parameters["@degreesFinal"].Value = degreesFinal;

                    command.Parameters.Add ("@openedTimeInSeconds", MySqlDbType.Int32);
                    command.Parameters["@openedTimeInSeconds"].Value = openedTimeInSeconds;

                    command.Parameters.Add ("@date", MySqlDbType.DateTime);
                    command.Parameters["@date"].Value = date;

                    Int32 rowsAffected = command.ExecuteNonQuery ();
                    Console.WriteLine ("RowsAffected: {0}", rowsAffected);
                }

            } catch (System.Exception ex) {
                Console.WriteLine ($"ERROR ... {ex.Message}");
                throw;
            }

        }

        // DELETE api/values/5
        [HttpDelete ("{id}")]
        public void Delete (int id) {

            try {

                Console.WriteLine ($"[Request] ... {id}");

                using (MySqlConnection connection = new MySqlConnection (ConnectionString)) {

                    var value = id;

                    Console.WriteLine ($"[value] ... {value}");

                    connection.Open ();
                    var query = $"DELETE FROM SERVO_LOGS WHERE id = @id ";

                    var command = new MySqlCommand (query, connection);
                    command.Parameters.Add ("@id", MySqlDbType.Int32);
                    command.Parameters["@id"].Value = value;

                    var sqlResult = command.ExecuteNonQuery ();
                    if (sqlResult != 1) {
                        Console.WriteLine ($"[Deleted] ... {id}");
                    }
                }

            } catch (System.Exception ex) {
                Console.WriteLine ($"ERROR ... {ex.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route ("GetChart")]
        public ActionResult<IEnumerable<ChartModel>> GetChartServoData (DateTime from, DateTime to) {

            List<ChartModel> result = new List<ChartModel> ();
            try {

                if (from.Equals (DateTime.MinValue) || from.ToString ("u").Equals ("0000-00-00 00:00:00Z") ||
                    to.Equals (DateTime.MinValue) || to.ToString ("u").Equals ("0000-00-00 00:00:00Z")
                ) {
                    using (MySqlConnection connection = new MySqlConnection (ConnectionString)) {

                        connection.Open ();
                        var query = $"SELECT * FROM SERVO_LOGS ";

                        var command = new MySqlCommand (query, connection);

                        var sqlReader = command.ExecuteReader ();
                        if (sqlReader.HasRows) {
                            while (sqlReader.Read ()) {

                                var item = new ChartModel ();

                                var valueInt = sqlReader.GetInt32 (sqlReader.GetOrdinal ("value"));
                                item.y = $"{valueInt}";
                                var date = sqlReader.GetDateTime (sqlReader.GetOrdinal ("timestamp"));
                                item.x = date.ToString ("MM/dd/yyyy HH:mm:ss");
                                // will use this value to know the time opened will call mode
                                item.mode = sqlReader.GetInt32 (sqlReader.GetOrdinal ("openedTimeInSeconds"));

                                result.Add (item);
                            }
                        }
                    }
                } else {

                    using (MySqlConnection connection = new MySqlConnection (ConnectionString)) {

                        connection.Open ();
                        var query = $"SELECT * FROM SERVO_LOGS " +
                            $"WHERE timestamp >= @from " +
                            $"AND timestamp <= @to";

                        var command = new MySqlCommand (query, connection);
                        command.Parameters.Add ("@from", MySqlDbType.Datetime);
                        command.Parameters["@from"].Value = from;

                        command.Parameters.Add ("@to", MySqlDbType.Datetime);
                        command.Parameters["@to"].Value = to;

                        var sqlReader = command.ExecuteReader ();
                        if (sqlReader.HasRows) {
                            while (sqlReader.Read ()) {

                                var item = new ChartModel ();

                                var valueInt = sqlReader.GetInt32 (sqlReader.GetOrdinal ("value"));
                                item.y = $"{valueInt}";
                                var date = sqlReader.GetDateTime (sqlReader.GetOrdinal ("timestamp"));
                                item.x = date.ToString ("MM/dd/yyyy HH:mm:ss");
                                // will use this value to know the time opened will call mode
                                item.mode = sqlReader.GetInt32 (sqlReader.GetOrdinal ("openedTimeInSeconds"));

                                result.Add (item);
                            }
                        }
                    }

                }

            } catch (System.Exception ex) {
                Console.WriteLine ($"ERROR ... {ex.Message}");
                throw;
            }

            return result;
        }

        /// <summary>
        /// Datatable interface
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route ("GetAll")]
        public ActionResult<DataTableAdapterRs<ServoLogRs>> GetAll (DataTableRq request) {

            var result = new DataTableAdapterRs<ServoLogRs> ();
            List<ServoLogRs> DataBaseResult = new List<ServoLogRs> ();
            try {
                using (MySqlConnection connection = new MySqlConnection (ConnectionString)) {

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
                    var query = $"SELECT * FROM SERVO_LOGS " +
                        $"WHERE 1=1 " +
                        $"{queryConditions}" +
                        $"";

                    var command = new MySqlCommand (query, connection);
                    var sqlReader = command.ExecuteReader ();
                    if (sqlReader.HasRows) {
                        while (sqlReader.Read ()) {
                            var log = new ServoLogRs ();

                            log.Id = sqlReader.GetInt32 (sqlReader.GetOrdinal ("id"));
                            log.Value = sqlReader.GetInt32 (sqlReader.GetOrdinal ("value"));
                            log.DegreesInitial = sqlReader.GetInt32 (sqlReader.GetOrdinal ("degreesInitial"));
                            log.DegreesFinal = sqlReader.GetInt32 (sqlReader.GetOrdinal ("degreesFinal"));
                            log.OpenedTimeInSeconds = sqlReader.GetInt32 (sqlReader.GetOrdinal ("openedTimeInSeconds"));
                            log.Timestamp = sqlReader.GetDateTime (sqlReader.GetOrdinal ("timestamp"));

                            DataBaseResult.Add (log);
                        }

                        // fill other properties
                        result.Data = DataBaseResult;
                        result.Draw = request.draw;
                        if (DataBaseResult != null && DataBaseResult.Any ()) {
                            result.RecordsTotal = this.Get ().Value.ToList ().Count;
                            result.RecordsFiltered = this.Get ().Value.ToList ().Count;
                        }
                    }
                }
            } catch (System.Exception ex) {
                Console.WriteLine ($"ERROR ... {ex.Message}");
                throw;
            }

            return result;
        }

    }
}