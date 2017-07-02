using Avon.PedidosIvr.Data.Context;
using Avon.PedidosIvr.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Avon.PedidosIvr.Data.Repositories.Impl
{
    public class PedidosRepository : GenericRepository<AvonIvrContext, Transaccion, int>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <returns></returns>
        public List<Transaccion> GetTransaccionesPorEnviar(DateTime inicio, DateTime fin)
        {
            return this.Context.Transacciones.Where(x => x.EnviarFtp == "S"
                                                         && x.Pedido != null
                                                         && x.Pedido.FechaP >= inicio
                                                         && x.Pedido.FechaP <= fin)
                    .Include(x => x.Pedido)
                    .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zona"></param>
        /// <param name="campana"></param>
        /// <param name="inicio"></param>
        /// <param name="fin"></param>
        /// <returns></returns>
        public List<Transaccion> GetTransaccionesPorEnviar(string zona, string campana, DateTime inicio, DateTime fin)
        {
            return this.Context.Transacciones.Where(x => x.EnviarFtp == "S"
                                                        && x.Pedido != null
                                                        && x.Pedido.Zona == zona
                                                        && x.Pedido.Campana == campana
                                                        && x.Pedido.FechaP >= inicio
                                                        && x.Pedido.FechaP <= fin)
                    .Include(x => x.Pedido).ToList()
                    .ToList();
        }
    }
}
