using Avon.PedidosIvr.Business.Services;
using System;
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
            var grupos = _pedidosService.GeneraGrupos(1);

            _log.Info("Iniciando procesamiento de pedidos...");

            _log.InfoFormat("Se procesaran {0} grupos distintos", grupos.Count);

            _log.Info("Procesamiento de pedidos completo");

            Console.ReadLine();
        }
    }
}

