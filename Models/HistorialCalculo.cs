using System;

namespace SuperDigitoApp.Models
{
    public class HistorialCalculo
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int Numero { get; set; }
        public int Resultado { get; set; }
        public DateTime FechaHora { get; set; }

        public Usuario Usuario { get; set; }
    }
}
