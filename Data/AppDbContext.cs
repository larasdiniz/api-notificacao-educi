using Microsoft.EntityFrameworkCore;
using ApiNotificacoesPush.Entities;
using System.Collections.Generic;

namespace ApiNotificacoesPush.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<DispositivoUsuario> Dispositivos { get; set; }
    }
}
