using Avon.PedidosIvr.Business.Model;
using Avon.PedidosIvr.Data.Repositories.Impl;
using System.Collections.Generic;
using System.Linq;

namespace Avon.PedidosIvr.Business.Services
{
    public class PedidosService
    {
        private PedidosRepository _pedidosRepository = new PedidosRepository();

        public List<Grupo> GeneraGrupos(int pedidosPorEncabezado)
        {
            var grupos = GetGruposPorEnviar();
            //var gruposProcesados = new List<Grupo>();
            int consecutivo = 1;

            foreach (var grupo in grupos)
            {
                var transacciones = _pedidosRepository.GetTransaccionesPorEnviar(grupo.Zona, grupo.Campana);
                bool nuevoEncabezado = false;

                grupo.Encabezados = new List<Encabezado>();
                var encabezado = new Encabezado(consecutivo);
                encabezado.Grupo = grupo;

                foreach (var transaccion in transacciones)
                {
                    encabezado.Pedidos.Add(transaccion.Pedido);

                    if(encabezado.Pedidos.Count == pedidosPorEncabezado)
                    {
                        consecutivo++;
                        grupo.Encabezados.Add(encabezado);
                        encabezado = new Encabezado(consecutivo);
                        encabezado.Grupo = grupo;

                        nuevoEncabezado = true;
                    }
                }

                if (!nuevoEncabezado)
                {
                    grupo.Encabezados.Add(encabezado);
                }
                
            }

            return grupos;
        }

        private List<Grupo> GetGruposPorEnviar()
        {
            return _pedidosRepository.GetTransaccionesPorEnviar()
                    .Select(x => new { x.Pedido.Campana, x.Pedido.Zona })
                    .Distinct()
                    .Select(x => new Grupo() { Campana = x.Campana, Zona = x.Zona })
                    .ToList();
        }


    }
}
