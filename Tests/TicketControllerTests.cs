using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ticket_project.Controllers;
using ticket_project.Data;
using ticket_project.Models;

namespace ticket_project.Tests.Controllers
{
    public class TicketControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly TicketController _controller;

        public TicketControllerTests()
        {
            // Configurar banco de dados em memória
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            
            // Preencher dados de teste
            SeedTestData();
            
            _controller = new TicketController(_context);
        }

        private void SeedTestData()
        {
            // Adicionar empregados
            var activeEmployee = new Employee { Id = 1, Name = "Active Employee", Cpf = "11111111111", Situation = "A" };
            var inactiveEmployee = new Employee { Id = 2, Name = "Inactive Employee", Cpf = "22222222222", Situation = "I" };
            _context.Employees.AddRange(activeEmployee, inactiveEmployee);

            // Adicionar tickets
            _context.Tickets.AddRange(
                new Ticket { Id = 1, Quantity = 5, Situation = "A", EmployeeId = 1, CreatedAt = DateTime.Now.AddDays(-2) },
                new Ticket { Id = 2, Quantity = 3, Situation = "I", EmployeeId = 1, CreatedAt = DateTime.Now.AddDays(-1) },
                new Ticket { Id = 3, Quantity = 10, Situation = "A", EmployeeId = 2, CreatedAt = DateTime.Now }
            );
            
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Index_RetornaViewComTodosOsTickets_QuandoSemFiltro()
        {
            var result = _controller.Index(new ReportFilter());

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TicketResponseDto>(viewResult.Model);
            Assert.Equal(3, model.Tickets.Count);
            Assert.Equal(2, model.Employees.Count);
        }

        [Fact]
        public void Index_FiltraPorEmployeeId()
        {
            var filter = new ReportFilter { EmployeeId = 1 };

            var result = _controller.Index(filter);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TicketResponseDto>(viewResult.Model);
            Assert.Equal(2, model.Tickets.Count);
            Assert.All(model.Tickets, t => Assert.Equal(1, t.EmployeeId));
        }

        [Fact]
        public void Index_FiltraPorIntervaloDeDatas()
        {
            var filter = new ReportFilter 
            { 
                StartDate = DateTime.Now.AddDays(-1.5),
                EndDate = DateTime.Now.AddDays(-0.5) 
            };

            var result = _controller.Index(filter);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TicketResponseDto>(viewResult.Model);
            Assert.Single(model.Tickets);
            Assert.Equal(2, model.Tickets[0].Id);
        }

        [Fact]
        public void Index_FiltraPorSituacao()
        {
            var filter = new ReportFilter { Situation = "A" };

            var result = _controller.Index(filter);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<TicketResponseDto>(viewResult.Model);
            Assert.Equal(2, model.Tickets.Count);
            Assert.All(model.Tickets, t => Assert.Equal("A", t.Situation));
        }

        [Fact]
        public void Create_Get_RetornaViewComEmpregadosAtivos()
        {
            var result = _controller.Create();

            var viewResult = Assert.IsType<ViewResult>(result);
            var employees = viewResult.ViewData["Employees"] as System.Collections.Generic.List<Employee>;
            Assert.Single(employees); // Apenas o empregado ativo
            Assert.Equal("Active Employee", employees[0].Name);
        }

        [Fact]
        public void Create_Post_ModelValida_AdicionaTicket()
        {
            var newTicket = new Ticket 
            { 
                Quantity = 7,
                EmployeeId = 1
            };

            var result = _controller.Create(newTicket);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(4, _context.Tickets.Count());
            
            var addedTicket = _context.Tickets.Last();
            Assert.Equal("A", addedTicket.Situation);
            Assert.NotNull(addedTicket.CreatedAt);
        }

        [Fact]
        public void UpdateTicket_AtualizaQuantidadeESituacao()
        {
            var ticketId = 1;
            var newQuantity = 8;
            var newSituation = "I";

            var result = _controller.UpdateTicket(ticketId, newQuantity, newSituation);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            
            var updatedTicket = _context.Tickets.Find(ticketId);
            Assert.Equal(newQuantity, updatedTicket.Quantity);
            Assert.Equal(newSituation, updatedTicket.Situation);
            Assert.NotNull(updatedTicket.UpdatedAt);
        }

        [Fact]
        public void UpdateTicket_IdInvalido_LancaExcecao()
        {
            var invalidId = 99;

            Assert.Throws<NullReferenceException>(() => 
                _controller.UpdateTicket(invalidId, 1, "A"));
        }
    }
}
