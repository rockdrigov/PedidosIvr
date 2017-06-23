using System.Configuration;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using Tamir.SharpSsh;
using System;


namespace Avon.PedidosIvrTask
{
    class ProgramOld
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string rutaArchivosTemporales = ConfigurationManager.AppSettings["RutaArchivosTemporales"];

        //Configuracion SFTP
        private static readonly string sftpServidor = ConfigurationManager.AppSettings["sftpServidor"];
        private static readonly string sftpUsuario = ConfigurationManager.AppSettings["sftpUsuario"];
        private static readonly string sftpPassword = ConfigurationManager.AppSettings["sftpPassword"];
        private static readonly string sftpRuta = ConfigurationManager.AppSettings["sftpPassword"];

        static void MainOld(string[] args)
        {
            _log.Info("Iniciando procesamiento de pedidos...");

            //Se obtiene la lista de grupos
            for (int i = 1001; i <= 1005; i++)
            {
                _log.InfoFormat("Procesando grupo: {0}", i);
                GenerarPaquete(i);
            }

            _log.Info("Procesamiento de pedidos completo");

            Console.ReadLine();
        }

        private static bool ValidarConexionSftp()
        {
            Sftp client = new Sftp(sftpServidor, sftpUsuario, sftpPassword);

            _log.DebugFormat("Validando conexion Sftp. Servidor:{0}, Usuario:{1}, Password: {2}", sftpServidor, sftpUsuario, sftpPassword);

            try
            {
                client.Connect();

                if (client.Connected)
                {
                    _log.DebugFormat("Conexion exitosa");
                    return true;
                }
                else
                {
                    _log.DebugFormat("No se pudo establecer la conexion");
                }
            }catch(Exception ex)
            {
                _log.Error(ex.Message);
            }

            return false;
        }

        private static void SubirArchivoSftp(string archivo, string rutaDestino)
        {
            Sftp client = new Sftp(sftpServidor, sftpUsuario, sftpPassword);
            client.Connect();
            if (client.Connected)
            {
                client.Put(archivo, rutaDestino);
            }
        }

        private static void GenerarPaquete(int grupo)
        {
            string rutaTemporalGrupo = rutaArchivosTemporales + @"\" + grupo.ToString();
            string nombreArchivoEncabezado = "ORDER.10019000" + grupo.ToString() + "4";
            string nombreArchivoDetalle = "ORDER.1001" + grupo.ToString() + "4" + "0001";
            string rutaArchivoZip = rutaArchivosTemporales + @"\" + nombreArchivoEncabezado + ".zip";

            if (Directory.Exists(rutaTemporalGrupo))
            {
                Directory.Delete(rutaTemporalGrupo, true);
            }

            Directory.CreateDirectory(rutaTemporalGrupo);


            var archivoEncabezado = File.Create(rutaTemporalGrupo + @"\" + nombreArchivoEncabezado);
            archivoEncabezado.Close();

            var archivoDetalle = File.Create(rutaTemporalGrupo + @"\" + nombreArchivoDetalle);
            archivoDetalle.Close();

            //ComprimirPaquete(rutaArchivoZip, rutaTemporalGrupo);

            if (ValidarConexionSftp())
            {
                SubirArchivoSftp(rutaArchivoZip, "/");
            }
            

            Directory.Delete(rutaTemporalGrupo, true);
        }

    }
}
