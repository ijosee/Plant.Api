using System.Collections.Generic;
using Newtonsoft.Json;

namespace Plant.Api.Entities.Rs.DataTable {
    public class DataTableRs<T> {
        /// <summary>
        /// Representa el número de veces que se ha realizado una petición.
        /// </summary>
        [JsonProperty ("draw")]
        public int Draw { get; set; }
        /// <summary>
        /// Total de registros antes de filtrar.
        /// </summary>
        [JsonProperty ("recordsTotal")]
        public int RecordsTotal { get; set; }
        /// <summary>
        /// Total de registros ya filtrados.
        /// </summary>
        [JsonProperty ("recordsFiltered")]
        public int RecordsFiltered { get; set; }
        /// <summary>
        /// Arreglo de datos que se va a mostrar en la tabla.
        /// </summary>
        [JsonProperty ("data")]
        public List<T> Data { get; set; }
        /// <summary>
        /// Parámetro opcional y nos permite mandar mensajes de error que hayan pasado del lado del servidor.
        /// </summary>
        [JsonProperty ("error")]
        public string Error { get; set; }

    }
}