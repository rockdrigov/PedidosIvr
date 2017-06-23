using Avon.PedidosIvr.Business.Model;
using Avon.PedidosIvr.Data.Repositories.Impl;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace Avon.PedidosIvr.Business.Services
{
    public class PedidosService
    {
        /// <summary>
        /// Ruta donde se crearan los archivos temporales
        /// </summary>
        private string _rutaArchivosTemporales;

        /// <summary>
        /// Repositorio para el manejo de pedidos y transacciones
        /// </summary>
        private PedidosRepository _pedidosRepository = new PedidosRepository();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rutaArchivosTemporales"></param>
        public PedidosService(string rutaArchivosTemporales)
        {
            _rutaArchivosTemporales = rutaArchivosTemporales;
        }

        /// <summary>
        /// Punto de entrada para la generacion de paquetes
        /// </summary>
        /// <param name="fecheaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="pedidosPorEncabezado"></param>
        /// <returns></returns>
        public int GenerarPaquetes(DateTime fecheaInicio, DateTime fechaFin, int pedidosPorEncabezado)
        {
            var grupos = GeneraGrupos(fecheaInicio, fechaFin, pedidosPorEncabezado);
            int paquetesGenerados = 0;

            BusinessUtils.PreparaRutaTemporal(_rutaArchivosTemporales);

            foreach (var grupo in grupos)
            {
                foreach (var encabeado in grupo.Encabezados)
                {
                    GeneraArchivoEncabezado(encabeado);
                    paquetesGenerados++;
                }
            }

            return paquetesGenerados;
        }

        /// <summary>
        /// Agrupa los pedidos por encabezados, segun el paramentro @pedidosPorEncabezado
        /// </summary>
        /// <param name="fecheaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="pedidosPorEncabezado">Indica el numero de pedidos que habrá por encabezado</param>
        /// <returns></returns>
        private List<Grupo> GeneraGrupos(DateTime fecheaInicio, DateTime fechaFin, int pedidosPorEncabezado)
        {
            var grupos = GetGruposPorEnviar();
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

                    if (encabezado.Pedidos.Count == pedidosPorEncabezado)
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

        /// <summary>
        /// Genera archivo encabezado con la lista de pedidos que contiene
        /// </summary>
        /// <param name="encabezado"></param>
        /// <returns></returns>
        private bool GeneraArchivoEncabezado(Encabezado encabezado)
        {
            string rutaTemporalGrupo = _rutaArchivosTemporales + @"\" + encabezado.Consecutivo.ToString("D4");
            int consecutivoPedido = 1;

            if (Directory.Exists(rutaTemporalGrupo))
            {
                Directory.Delete(rutaTemporalGrupo, true);
            }

            Directory.CreateDirectory(rutaTemporalGrupo);

            using (FileStream archivoEncabezado = File.Create(rutaTemporalGrupo + @"\" + encabezado.Nombre))
            {
                using (StreamWriter streamWriter = new StreamWriter(archivoEncabezado))
                {
                    var fechaInicio = BusinessUtils.GetFechaFormateada();
                    var horaInicio = BusinessUtils.GetHoraFormateada();
                    var contenidoEncabezado = String.Format("{0}019000{1}4000 101{1}4{2}Y{3}{4}", encabezado.Grupo.Campana, encabezado.Consecutivo.ToString("D4"), encabezado.Pedidos.Count, fechaInicio, horaInicio);
                    streamWriter.Write(contenidoEncabezado);

                    foreach (var pedido in encabezado.Pedidos)
                    {
                        streamWriter.Write(String.Format("{0}01{1}400{2}", encabezado.Grupo.Campana, encabezado.Consecutivo.ToString("D4"), consecutivoPedido.ToString("D2")));
                        consecutivoPedido++;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Obtiene los distintos grupos a procesar segun la zona y camapaña
        /// </summary>
        /// <returns></returns>
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
