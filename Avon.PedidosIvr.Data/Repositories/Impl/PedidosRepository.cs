using Avon.PedidosIvr.Data.Context;
using Avon.PedidosIvr.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Avon.PedidosIvr.Data.Repositories.Impl
{
    public class PedidosRepository : GenericRepository<AvonIvrContext, Transaccion, int>
    {
        public List<Transaccion> GetTransaccionesPorEnviar()
        {
            return this.Context.Transacciones.Where(x => x.EnviarFtp == "S").ToList();
        }

        public List<Transaccion> GetTransaccionesPorEnviar(string zona, string campana)
        {
            return this.Context.Transacciones.Where(x => x.EnviarFtp == "S" && x.Pedido.Zona == zona && x.Pedido.Campana == campana).ToList();
        }
    }
}
