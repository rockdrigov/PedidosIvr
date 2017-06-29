using Avon.PedidosIvr.Business.Model;
using Avon.PedidosIvr.Data.Repositories.Impl;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Avon.PedidosIvr.Entities;
using Avon.PedidosIvr.Business.Compresion;

namespace Avon.PedidosIvr.Business.Services
{
    public class PedidosService
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Ruta donde se crearan los archivos temporales
        /// </summary>
        private string _rutaArchivosTemporales;

        /// <summary>
        /// Ruta donde se colocará el archivo final
        /// </summary>
        private string _rutaArchivosFinales;

        /// <summary>
        /// Repositorio para el manejo de pedidos y transacciones
        /// </summary>
        private PedidosRepository _pedidosRepository = new PedidosRepository();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rutaArchivosTemporales"></param>
        public PedidosService(string rutaArchivosTemporales, string rutaArchivosFinales)
        {
            _rutaArchivosTemporales = rutaArchivosTemporales;
            _rutaArchivosFinales = rutaArchivosFinales;
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

            if(grupos != null)
            {
                foreach (var grupo in grupos)
                {
                    foreach (var encabeado in grupo.Encabezados)
                    {
                        GeneraArchivoEncabezado(encabeado);

                        GeneraArchivosDetalle(encabeado);

                        paquetesGenerados++;
                    }
                }

                var rutaArchivoZip = _rutaArchivosFinales + @"\" + String.Format("orderivr.{0:yyyy-MM-dd}.zip", DateTime.Now);

                Zip.ComprimirPaquete(rutaArchivoZip, _rutaArchivosTemporales);

                return paquetesGenerados;
            }

            return 0;
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
            string rutaTemporalGrupo = _rutaArchivosTemporales + @"\";
            int consecutivoPedido = 1;

            using (FileStream archivoEncabezado = File.Create(rutaTemporalGrupo + encabezado.Nombre))
            {
                using (StreamWriter streamWriter = new StreamWriter(archivoEncabezado))
                {
                    var fechaInicio = BusinessUtils.GetFechaFormateada();
                    var horaInicio = BusinessUtils.GetHoraFormateada();
                    var contenidoEncabezado = String.Format("{0}019000{1}4000 101{1}4{2}Y{3}{4}", encabezado.Grupo.Campana, encabezado.Consecutivo.ToString("D4"), encabezado.Pedidos.Count, fechaInicio, horaInicio);
                    streamWriter.Write(contenidoEncabezado);

                    foreach (var pedido in encabezado.Pedidos)
                    {
                        pedido.Consecutivo = consecutivoPedido;

                        streamWriter.Write(String.Format("{0}01{1}400{2}", encabezado.Grupo.Campana, encabezado.Consecutivo.ToString("D4"), consecutivoPedido.ToString("D2")));
                        consecutivoPedido++;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Genera todos los archivos detalle pertenecientes a un encabezado
        /// </summary>
        /// <param name="encabezado"></param>
        /// <returns></returns>
        private bool GeneraArchivosDetalle(Encabezado encabezado)
        {
            foreach (var pedido in encabezado.Pedidos)
            {
                GeneraArchivosDetalle(pedido, encabezado.Consecutivo);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pedido"></param>
        private void GeneraArchivosDetalle(Pedido pedido, int consecutivoEncabezado)
        {
            var rutaTemporalGrupo = _rutaArchivosTemporales + @"\";
            var clavePedido = String.Format("{0}01{1}4{2}", pedido.Campana, consecutivoEncabezado.ToString("D4"), pedido.Consecutivo.ToString("D4"));
            var nombreArchivoDetalle = "ORDER." + clavePedido;

            using (FileStream archivoDetalle = File.Create(rutaTemporalGrupo + nombreArchivoDetalle))
            {
                using (StreamWriter streamWriter = new StreamWriter(archivoDetalle))
                {

                    //Encabezado
                    var identificadorEncabezado = String.IsNullOrEmpty(pedido.ProductoClave) ? "H" : "A";
                    var detalleOrden = String.Format("{6}  101{0}{1} {2}{3}{4}{5}00000000010000000000000000000000000000000", consecutivoEncabezado.ToString("D4"), pedido.Consecutivo.ToString("D2"), BusinessUtils.GetFechaFormateada(pedido.FechaP), BusinessUtils.GetHoraFormateada(pedido.FechaP), pedido.Zona, pedido.Registro, identificadorEncabezado);

                    streamWriter.WriteLine(clavePedido + "0");
                    streamWriter.WriteLine(detalleOrden);

                    //Pedido Actual
                    if (!String.IsNullOrEmpty(pedido.ProductoClave))
                    {
                        streamWriter.WriteLine(clavePedido + "2");
                        foreach(var detalleLinea in pedido.ProductoClave.Split('^'))
                        {
                            var linea = detalleLinea.Split(',')[0].Substring(0,5);
                            var cantidad = detalleLinea.Split(',')[1];
                            var lineaFormateada = String.Format("0{0}00{1}", linea, Convert.ToInt16(cantidad).ToString("D3"));
                            streamWriter.Write(lineaFormateada);
                        }
                    }

                    //Pedido Anterios
                    if (!String.IsNullOrEmpty(pedido.ProductoClaveAnt.Trim()))
                    {
                        streamWriter.WriteLine(Environment.NewLine + clavePedido + "3");
                        foreach (var detalleLinea in pedido.ProductoClaveAnt.Split('^'))
                        {
                            var linea = detalleLinea.Split(',')[0].Substring(0, 5);
                            var cantidad = detalleLinea.Split(',')[1];
                            var lineaFormateada = String.Format("B 000{0}00{1}0000000000000{2}01", linea, Convert.ToInt16(cantidad).ToString("D3"), Convert.ToInt16(pedido.Campana) - 1);
                            streamWriter.WriteLine(lineaFormateada);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene los distintos grupos a procesar segun la zona y camapaña
        /// </summary>
        /// <returns></returns>
        private List<Grupo> GetGruposPorEnviar()
        {
            _log.Debug("Obteniendo Grupor para enviar");

            List<Grupo> grupos;
            var transaccionesPorEnviar = _pedidosRepository.GetTransaccionesPorEnviar();

            if(transaccionesPorEnviar != null)
            {
                _log.DebugFormat("Se obtuvieron {0} pedidos", transaccionesPorEnviar.Count);

                grupos = transaccionesPorEnviar
                        .Select(x => new { x.Pedido.Campana, x.Pedido.Zona })
                        .Distinct()
                        .Select(x => new Grupo() { Campana = x.Campana, Zona = x.Zona })
                        .ToList();

                _log.DebugFormat("Se obtuvieron {0} grupos", grupos.Count);

                return grupos;
            }
            else
            {
                _log.Debug("No se obtuvo ninguna transacción");
            }

            return null;
        }
    }
}
