using System;
using System.IO;

namespace Avon.PedidosIvr.Business
{
    public class BusinessUtils
    {
        public static string GetFechaFormateada()
        {
            return GetFechaFormateada(DateTime.Now);
        }

        public static string GetFechaFormateada(DateTime fecha)
        {
            return String.Format("{0:yyMMdd}", fecha);
        }

        public static string GetHoraFormateada()
        {
            return GetHoraFormateada(DateTime.Now);
        }

        public static string GetHoraFormateada(DateTime fecha)
        {
            return String.Format("{0:HHmmss}", fecha);
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

        public static DateTime GetFechaConHoraExacta(DateTime fecha, int hora)
        {
            return new DateTime(fecha.Year, fecha.Month, fecha.Day, hora, 0, 0);
        }
    }
}
