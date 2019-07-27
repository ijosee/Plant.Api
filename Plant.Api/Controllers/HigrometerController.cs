﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Plant.Api.Entities.Model;
using Plant.Api.Entities.Rq.Sensor;
using Plant.Api.Entities.Rs.Sensor;

namespace Plant.Api.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class HigrometerController : ControllerBase {

        public string ConnectionString = "";

        IConfiguration _configuration;
        public HigrometerController (IConfiguration configuration) {
            _configuration = configuration;

            ConnectionString = _configuration.GetSection ("ConnectionString").GetSection ("DatabaseConnection").Value.ToString ();
        }

        [HttpGet]
        public ActionResult<IEnumerable<int>> Get () {

            List<int> result = new List<int> ();
            try {
                using (MySqlConnection connection = new MySqlConnection (ConnectionString)) {

                    connection.Open ();
                    var query = $"SELECT * FROM HIGROMETER_LOGS ";

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
        public ActionResult<HigrometerLogRs> Get (int id) {

            HigrometerLogRs result = null;
            try {

                Console.WriteLine ($"[Request] ... {id}");

                using (MySqlConnection connection = new MySqlConnection (ConnectionString)) {

                    var value = id;

                    Console.WriteLine ($"[value] ... {value}");

                    connection.Open ();
                    var query = $"SELECT * FROM HIGROMETER_LOGS " +
                        $"WHERE id = @id ";

                    var command = new MySqlCommand (query, connection);
                    command.Parameters.Add ("@id", MySqlDbType.Int32);
                    command.Parameters["@id"].Value = value;

                    var sqlReader = command.ExecuteReader ();
                    if (sqlReader.HasRows) {
                        result = new HigrometerLogRs ();
                        while (sqlReader.Read ()) {
                            result.Id = sqlReader.GetInt32 (sqlReader.GetOrdinal ("id"));
                            result.Value = sqlReader.GetInt32 (sqlReader.GetOrdinal ("value"));
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
        public void Post ([FromBody] HigrometerLogRq request) {

            try {

                Console.WriteLine ($"[Request] ... {JsonConvert.SerializeObject(request)}");

                using (MySqlConnection connection = new MySqlConnection (ConnectionString)) {

                    var value = request.Value;
                    var date = DateTime.Now;

                    Console.WriteLine ($"[value] ... {value}");
                    Console.WriteLine ($"[date] ... {date}");

                    connection.Open ();
                    var query = $"INSERT INTO " +
                        $"HIGROMETER_LOGS(`value`,`timestamp`) " +
                        $"VALUES (@value,@date)";

                    var command = new MySqlCommand (query, connection);
                    command.Parameters.Add ("@value", MySqlDbType.Int32);
                    command.Parameters["@value"].Value = value;

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
                    var query = $"DELETE FROM HIGROMETER_LOGS WHERE id = @id ";

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
        public ActionResult<IEnumerable<ChartModel>> GetChartHigrometerData (DateTime from,DateTime to) {

            List<ChartModel> result = new List<ChartModel> ();
            try {

                if (from.Equals(DateTime.MinValue) || from.ToString("u").Equals("0000-00-00 00:00:00Z")
                    || to.Equals(DateTime.MinValue) || to.ToString("u").Equals("0000-00-00 00:00:00Z")
                    )
                {
                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    {

                        connection.Open();
                        var query = $"SELECT * FROM HIGROMETER_LOGS ";

                        var command = new MySqlCommand(query, connection);

                        var sqlReader = command.ExecuteReader();
                        if (sqlReader.HasRows)
                        {
                            while (sqlReader.Read())
                            {

                                var item = new ChartModel();

                                var valueInt = sqlReader.GetInt32(sqlReader.GetOrdinal("value"));
                                item.y = $"{valueInt}";
                                var date = sqlReader.GetDateTime(sqlReader.GetOrdinal("timestamp"));
                                item.x = date.ToString("HH:mm");

                                result.Add(item);
                            }
                        }
                    }
                }
                else {

                    using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                    {

                        connection.Open();
                        var query = $"SELECT * FROM HIGROMETER_LOGS " +
                            $"WHERE timestamp >= @from " +
                            $"AND timestamp <= @to";

                        var command = new MySqlCommand(query, connection);
                        command.Parameters.Add("@from", MySqlDbType.Datetime);
                        command.Parameters["@from"].Value = from;

                        command.Parameters.Add("@to", MySqlDbType.Datetime);
                        command.Parameters["@to"].Value = to;

                        var sqlReader = command.ExecuteReader();
                        if (sqlReader.HasRows)
                        {
                            while (sqlReader.Read())
                            {

                                var item = new ChartModel();

                                var valueInt = sqlReader.GetInt32(sqlReader.GetOrdinal("value"));
                                item.y = $"{valueInt}";
                                var date = sqlReader.GetDateTime(sqlReader.GetOrdinal("timestamp"));
                                item.x = date.ToString("HH:mm");

                                result.Add(item);
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
    }
}