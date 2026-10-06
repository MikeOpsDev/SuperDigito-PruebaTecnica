namespace SuperDigitoApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public ICollection<HistorialCalculo> Historiales { get; set; } = new List<HistorialCalculo>();
    }
}
