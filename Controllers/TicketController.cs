using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ticket_project.Data;
using ticket_project.Models;

namespace ticket_project.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações de Tickets
    /// </summary>
    [ApiExplorerSettings(GroupName = "Tickets")]
    [Route("Tickets/[action]")]
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Inicializa uma nova instância do controlador com o contexto do banco de dados
        /// </summary>
        public TicketController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os tickets com opções de filtro
        /// </summary>
        /// <param name="filter">Filtros para pesquisa (funcionário, período, situação)</param>
        /// <returns>View com a lista de tickets filtrados</returns>
        [HttpGet]
        public IActionResult Index(ReportFilter filter)
        {
            var query = _context.Tickets
                .Include(t => t.Employee)
                .AsQueryable();

            // Aplica filtros conforme parâmetros recebidos
            if (filter.EmployeeId.HasValue)
                query = query.Where(t => t.EmployeeId == filter.EmployeeId);

            if (filter.StartDate.HasValue)
                query = query.Where(t => t.CreatedAt >= filter.StartDate);

            if (filter.EndDate.HasValue)
                query = query.Where(t => t.CreatedAt <= filter.EndDate);

            // Filtro de situação (se não informado, mostra Ativos e Inativos)
            if (!string.IsNullOrEmpty(filter.Situation))
                query = query.Where(t => t.Situation == filter.Situation);
            else
                query = query.Where(t => t.Situation == "A" || t.Situation == "I");

            var model = new TicketResponseDto
            {
                Tickets = query.ToList(),
                Employees = _context.Employees.ToList(),
                Filter = filter
            };

            return View(model);
        }

        /// <summary>
        /// Exibe o formulário de criação de novo ticket
        /// </summary>
        /// <remarks>
        /// Carrega apenas funcionários ativos ("A") para seleção
        /// </remarks>
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Employees = _context.Employees.Where(e => e.Situation == "A").ToList();
            return View();
        }

        /// <summary>
        /// Cria um novo ticket no sistema
        /// </summary>
        /// <param name="model">Dados do ticket a ser criado</param>
        /// <returns>Redireciona para a lista de tickets</returns>
        /// <remarks>
        /// Define automaticamente:
        /// - Situação como "A" (Ativo)
        /// - Data de criação como data/hora atual
        /// </remarks>
        [HttpPost]
        public IActionResult Create(Ticket model)
        {
            model.Situation = "A"; // Define como Ativo
            model.CreatedAt = DateTime.Now;

            _context.Add(model);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Atualiza a quantidade e situação de um ticket existente
        /// </summary>
        /// <param name="id">ID do ticket a ser atualizado</param>
        /// <param name="quantity">Nova quantidade</param>
        /// <param name="situation">Nova situação ("A" para Ativo, "I" para Inativo)</param>
        /// <returns>View da lista de tickets</returns>
        /// <remarks>
        /// Atualiza automaticamente:
        /// - Data de atualização para data/hora atual
        /// </remarks>
        [HttpPost]
        public IActionResult UpdateTicket(int id, int quantity, string situation)
        {
            var existingTicket = _context.Tickets.Find(id);

            existingTicket.Quantity = quantity;
            existingTicket.Situation = situation;
            existingTicket.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            return View(nameof(Index));
        }
    }
}