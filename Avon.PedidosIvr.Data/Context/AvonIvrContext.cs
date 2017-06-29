using Avon.PedidosIvr.Entities;
using log4net;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Reflection;

namespace Avon.PedidosIvr.Data.Context
{
    public class AvonIvrContext : DbContext
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AvonIvrContext() : base("AvonIvrContext")
        {
            Database.Log = log => _logger.Debug(log);
            Database.SetInitializer<AvonIvrContext>(null);
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
