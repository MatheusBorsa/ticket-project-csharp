using Microsoft.EntityFrameworkCore;
using ticket_project.Models;


namespace ticket_project.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Definindo que o cpf deve ser unico
            modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Cpf)
            .IsUnique();
        }
    }
}