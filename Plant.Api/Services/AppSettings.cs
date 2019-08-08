using Interaces.Services.AppSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Plant.Api.Services {
    public class AppSettings : IAppSettings {
        readonly IConfiguration _configuration;
        readonly ILogger _logger;

        public AppSettings (IConfiguration configuration, ILogger<AppSettings> logger) {
            _configuration = configuration;
            _logger = logger;
        }

        public string GetDataBaseConnectionString () {
            string result = string.Empty;

            if (_configuration.GetSection ("ConnectionString").Exists ()) {
                var dataBaseSection = _configuration.GetSection ("ConnectionString");
                if (dataBaseSection.GetSection ("DatabaseConnection").Exists ()) {
                    var connectionStringSection = dataBaseSection.GetSection ("DatabaseConnection").Value;
                    result = connectionStringSection.ToString ();
                } else {
                    throw new System.Exception ("DatabaseConnection seccion do not exist, please fill up ! .");
                }
            } else {
                throw new System.Exception ("ConnectionString seccion do not exist, please fill up ! .");
            }
            return result;
        }
    }
}