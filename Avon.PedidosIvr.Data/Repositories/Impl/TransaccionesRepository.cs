using Avon.PedidosIvr.Data.Context;
using Avon.PedidosIvr.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avon.PedidosIvr.Data.Repositories.Impl
{
    public class TransaccionesRepository : GenericRepository<AvonIvrContext, Transaccion, int>
    {
        public List<Transaccion> GetTransaccionesPorEnviar()
        {
            return this.Context.Transacciones.Where(x => x.EnviarFtp == "S").ToList();
        }
    }
}
