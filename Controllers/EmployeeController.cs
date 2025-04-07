using Microsoft.AspNetCore.Mvc;
using ticket_project.Models;
using ticket_project.Data;

namespace ticket_project.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações CRUD de Funcionários
    /// </summary>
    [ApiExplorerSettings(GroupName = "Employees")]
    [Route("Employees/[action]")]
   public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor que injeta o contexto do banco de dados
        /// </summary>
        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os funcionários cadastrados
        /// </summary>
        /// <returns>View com a lista de funcionários</returns>
        [HttpGet]
        public IActionResult Index()
        {
            var employees = _context.Employees.ToList();
            return View(employees);
        }

        /// <summary>
        /// Exibe o formulário de criação de novo funcionário
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Valida o CPF conforme regras de negócio
        /// </summary>
        /// <param name="cpf">Número do CPF a ser validado</param>
        /// <param name="employeeId">ID do funcionário (opcional para edição)</param>
        /// <returns>Tuple com (valido: bool, mensagemErro: string)</returns>
        private (bool isValid, string errorMessage) ValidateCpf(string cpf, int? employeeId = null)
        {
            // Validação de tamanho (11 dígitos)
            if (cpf?.Length != 11)
            {
                return (false, "CPF deve conter exatamente 11 dígitos.");
            }

            // Validação de duplicidade
            var cpfExists = _context.Employees.Any(e => e.Cpf == cpf);
            if (cpfExists)
            {
                return (false, "Esse CPF já existe.");
            }

            return (true, null);
        }

        /// <summary>
        /// Cria um novo funcionário após validação
        /// </summary>
        /// <param name="model">Dados do funcionário a ser criado</param>
        /// <returns>Redireciona para a lista ou mostra erros de validação</returns>
        [HttpPost]
        public IActionResult Create(Employee model)
        {
            // Validação customizada do CPF
            var (isValid, errorMessage) = ValidateCpf(model.Cpf);
            if (!isValid)
            {
                ModelState.AddModelError("Cpf", errorMessage);
            }

            if (ModelState.IsValid)
            {
                // Define valores padrão
                model.CreatedAt = DateTime.Now;
                model.Situation = "A"; // 'A' para Ativo

                _context.Add(model);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// <summary>
        /// Exibe o formulário de edição para um funcionário específico
        /// </summary>
        /// <param name="id">ID do funcionário a ser editado</param>
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        /// <summary>
        /// Atualiza a situação (Ativo/Inativo) de um funcionário
        /// </summary>
        /// <param name="id">ID do funcionário</param>
        /// <param name="model">Modelo com a nova situação</param>
        /// <remarks>
        /// Apenas o campo Situation pode ser atualizado via este endpoint
        /// </remarks>
        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id,Situation")] Employee model)
        {
            var existingEmployee = _context.Employees.Find(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            // Atualiza apenas a situação e data de atualização
            existingEmployee.Situation = model.Situation;
            existingEmployee.UpdatedAt = DateTime.Now;

            _context.Update(existingEmployee);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}