namespace ticket_project.Models
{
    public class ReportFilter
    {
        public int? EmployeeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Situation { get; set; }
    }   
}