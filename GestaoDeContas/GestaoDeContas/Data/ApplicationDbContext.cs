using GestaoDeContas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeContas.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ContaPagar> ContasPagar { get; set; }
    public DbSet<ContaReceber> ContasReceber { get; set; }
    public DbSet<Cartao> Cartoes { get; set; }
    public DbSet<CompraCartao> ComprasCartao { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}