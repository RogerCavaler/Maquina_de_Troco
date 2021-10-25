using Microsoft.EntityFrameworkCore;
using MQTroco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTroco.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<CaixaModel> Caixas { get; set; }
        public DbSet<MoedaModel> Moedas { get; set; }
        public DbSet<CaixaMoedaModel> CaixaMoedas { get; set; }
    }
}
