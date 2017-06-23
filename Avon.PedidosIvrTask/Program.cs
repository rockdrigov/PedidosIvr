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
        private static PedidosService _pedidosService = new PedidosService(ConfigurationManager.AppSettings["RutaArchivosTemporales"], ConfigurationManager.AppSettings["RutaArchivosFinales"]);

        static void Main(string[] args)
        {
            _log.Info("Iniciando procesamiento de pedidos...");

            //Numero de pedidos en cada paquete, valor por default
            int pedidosPorPaquete = 30;
            Int32.TryParse(ConfigurationManager.AppSettings["PedidosPorPaquete"], out pedidosPorPaquete);

             int numeroPaquetesGenerados = _pedidosService.GenerarPaquetes(DateTime.Now, DateTime.Now.AddDays(-1), pedidosPorPaquete);
            _log.InfoFormat("Se generaron {0} paquetes", numeroPaquetesGenerados);

            _log.Info("Procesamiento de pedidos completo");

            Console.ReadLine();
        }
    }
}

