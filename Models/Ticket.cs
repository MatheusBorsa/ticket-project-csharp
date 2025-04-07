using System.ComponentModel.DataAnnotations;

namespace ticket_project.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public int Quantity { get; set; }
        public string Situation { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}