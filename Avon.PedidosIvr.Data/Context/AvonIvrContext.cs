using Avon.PedidosIvr.Data.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Avon.PedidosIvr.Data.Context
{
    public class AvonIvrContext : DbContext
    {
        public AvonIvrContext() : base("AvonIvrContext")
        {

        }

        public virtual DbSet<Transaccion> Transacciones { get; set; }

        public virtual DbSet<Pedido> Pedidos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
