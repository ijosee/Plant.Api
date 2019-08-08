using System.Collections.Generic;

// namespace Plant.Api.Entities.Rq.DataTable {
namespace Plant.Api.Entities.Rq.DataTable {
    public class DataTableRq {

        /// <summary>
        /// Numero de petición que se está realizando.
        /// </summary>
        public int draw { get; set; }
        /// <summary>
        /// Diccionario con la información de por medio de campo se va a realizar el ordenamiento.
        /// </summary>
        public Dictionary<string, string>[] order { get; set; }
        /// <summary>
        /// Registro a partir de cual se va a iniciar el paginado.
        /// </summary>
        public int start { get; set; }
        /// <summary>
        /// Tamaño de la pagina
        /// </summary>
        public int length { get; set; }
        /// <summary>
        /// Valor de la búsqueda
        /// </summary>
        public Dictionary<string, string> search { get; set; }

    }
}