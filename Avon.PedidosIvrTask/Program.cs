using Avon.PedidosIvr.Business.Services;
using System;
using System.Configuration;
using System.Linq;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Avon.PedidosIvrTask
{
    class Program
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static PedidosService _pedidosService = new PedidosService();


        static void Main(string[] args)
        {
            _log.Info("Iniciando procesamiento de pedidos...");

            //Numero de pedidos en cada paquete, valor por default
            int pedidosPorPaquete = 30;
            Int32.TryParse(ConfigurationManager.AppSettings["PedidosPorPaquete"], out pedidosPorPaquete);

            var grupos = _pedidosService.GeneraGrupos(1);
            foreach(var grupo in grupos)
            {
                foreach(var encabezado in grupo.Encabezados)
                {
                    _log.InfoFormat("Procesando archivo: {0}", encabezado.Nombre);
                }
            }
            
            _log.InfoFormat("Se procesaran {0} grupos distintos", grupos.Count);

            _log.Info("Procesamiento de pedidos completo");

            Console.ReadLine();
        }
    }
}

