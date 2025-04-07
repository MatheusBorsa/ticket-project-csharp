namespace ticket_project.Models
{
    public class TicketResponseDto
{
    public List<Ticket> Tickets { get; set; }
    public List<Employee> Employees { get; set; }
    public ReportFilter Filter { get; set; }
}
}