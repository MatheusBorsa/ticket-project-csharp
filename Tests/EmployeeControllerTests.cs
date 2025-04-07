using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ticket_project.Controllers;
using ticket_project.Data;
using ticket_project.Models;

namespace ticket_project.Tests.Controllers
{
    public class EmployeeControllerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly EmployeeController _controller;

        public EmployeeControllerTests()
        {
            // Configurar banco de dados em memória
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nome único para cada teste
                .Options;

            _context = new ApplicationDbContext(options);
            
            // Preencher dados de teste
            _context.Employees.AddRange(
                new Employee { Id = 1, Name = "John Doe", Cpf = "12345678901", Situation = "A" },
                new Employee { Id = 2, Name = "Jane Smith", Cpf = "98765432109", Situation = "I" }
            );
            _context.SaveChanges();

            _controller = new EmployeeController(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void Index_RetornaViewComEmpregados()
        {
            var result = _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Employee>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public void Create_Get_RetornaView()
        {
            var result = _controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Create_Post_ModelValida_AdicionaEmpregado()
        {
            var newEmployee = new Employee 
            { 
                Name = "New Employee", 
                Cpf = "11122233344",
                Situation = "A"
            };

            var result = _controller.Create(newEmployee);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(3, _context.Employees.Count());
        }

        [Fact]
        public void Create_Post_ModelInvalida_RetornaView()
        {
            var invalidEmployee = new Employee { Name = "Test" }; // Faltando campos obrigatórios
            _controller.ModelState.AddModelError("Cpf", "Required");

            var result = _controller.Create(invalidEmployee);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public void Edit_Get_IdValido_RetornaView()
        {
            var result = _controller.Edit(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Employee>(viewResult.Model);
            Assert.Equal("John Doe", model.Name);
        }

        [Fact]
        public void Edit_Get_IdInvalido_RetornaNotFound()
        {
            var result = _controller.Edit(99); // ID inexistente

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Edit_Post_AtualizaStatusEmpregado()
        {
            var updatedEmployee = new Employee { Id = 1, Situation = "I" };

            var result = _controller.Edit(1, updatedEmployee);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            var employee = _context.Employees.Find(1);
            Assert.Equal("I", employee.Situation);
            Assert.NotNull(employee.UpdatedAt);
        }

        [Fact]
        public void ValidateCpf_ValidaCorretamente()
        {
            var method = typeof(EmployeeController)
                .GetMethod("ValidateCpf", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Testar com um CPF novo que não existe nos dados de teste
            var result = (ValueTuple<bool, string>)method.Invoke(_controller, new object[] { "11122233344", null });

            Assert.True(result.Item1, "O CPF válido deve retornar true");
            Assert.Null(result.Item2);
        }

        [Fact]
        public void ValidateCpf_DetectaDuplicado()
        {
            var method = typeof(EmployeeController)
                .GetMethod("ValidateCpf", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Testar com CPF que já existe nos dados de teste
            var result = (ValueTuple<bool, string>)method.Invoke(_controller, new object[] { "12345678901", null });

            Assert.False(result.Item1, "O CPF duplicado deve retornar false");
            Assert.Equal("Esse CPF já existe.", result.Item2);
        }

        [Fact]
        public void ValidateCpf_ValidaTamanho()
        {
            var method = typeof(EmployeeController)
                .GetMethod("ValidateCpf", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            // Testar com CPF de comprimento inválido
            var result = (ValueTuple<bool, string>)method.Invoke(_controller, new object[] { "123", null });

            Assert.False(result.Item1, "O CPF com comprimento inválido deve retornar false");
            Assert.Equal("CPF deve conter exatamente 11 dígitos.", result.Item2);
        }
    }
}
