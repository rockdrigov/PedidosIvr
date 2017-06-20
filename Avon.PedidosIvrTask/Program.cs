using Avon.PedidosIvr.Data.Repositories.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Avon.PedidosIvrTask
{
    class Program
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static TransaccionesRepository _transaccionesRepo = new TransaccionesRepository();

        static void Main(string[] args)
        {
            var transaccionesPendientes = _transaccionesRepo.GetTransaccionesPorEnviar();

            _log.Info("Iniciando procesamiento de pedidos...");
            _log.InfoFormat("Se procesaran {0} pedidos", transaccionesPendientes.Count);

            foreach(var transaccion in transaccionesPendientes)
            {
                _log.InfoFormat("Procesando pedido: {0}", transaccion.Pedido.Registro);
            }

            _log.Info("Procesamiento de pedidos completo");
            Console.ReadLine();
        }
    }
}

