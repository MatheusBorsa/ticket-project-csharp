using System.ComponentModel.DataAnnotations;

namespace ticket_project.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "O campo CPF é obrigatório.")]
        public string Cpf { get; set; }
        public string Situation { get; set; } = "A";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}