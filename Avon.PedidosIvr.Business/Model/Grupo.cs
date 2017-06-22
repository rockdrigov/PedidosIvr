using System.Collections.Generic;

namespace Avon.PedidosIvr.Business.Model
{
    public class Grupo
    {
        public string Zona { get; set; }

        public string Campana { get; set; }

        public List<Encabezado> Encabezados { get; set; }
    }
}
