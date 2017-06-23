using System;
using System.IO;

namespace Avon.PedidosIvr.Business
{
    public class BusinessUtils
    {
        public static string GetFechaFormateada()
        {
            return String.Format("{0:yyMMdd}", DateTime.Now);
        }

        public static string GetHoraFormateada()
        {
            return String.Format("{0:HHmmss}", DateTime.Now);
        }

        public static void PreparaRutaTemporal(string rutaArchivosTemporales)
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(rutaArchivosTemporales);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch
            {
                throw new Exception("No se ha podido limpiar el directorio temporal");
            }
        }
    }
}
