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

            ComprimirPaquete(rutaArchivoZip, rutaTemporalGrupo);

            if (ValidarConexionSftp())
            {
                SubirArchivoSftp(rutaArchivoZip, "/");
            }
            

            Directory.Delete(rutaTemporalGrupo, true);
        }

        public static void ComprimirPaquete(string outPathname, string folderName)
        {

            FileStream fsOut = File.Create(outPathname);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);

            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression


            // This setting will strip the leading part of the folder path in the entries, to
            // make the entries relative to the starting folder.
            // To include the full path for each entry up to the drive root, assign folderOffset = 0.
            int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

            ComprimirDirectorio(folderName, zipStream, folderOffset);

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        private static void ComprimirDirectorio(string ruta, ZipOutputStream zipStream, int folderOffset)
        {

            string[] files = Directory.GetFiles(ruta);

            foreach (string filename in files)
            {

                FileInfo fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(ruta);
            foreach (string folder in folders)
            {
                ComprimirDirectorio(folder, zipStream, folderOffset);
            }
        }
    }
}
