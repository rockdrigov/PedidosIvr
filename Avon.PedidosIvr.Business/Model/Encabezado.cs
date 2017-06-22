using Avon.PedidosIvr.Entities;
using System.Collections.Generic;

namespace Avon.PedidosIvr.Business.Model
{
    public class Encabezado
    {
        public Encabezado(int consecutivo)
        {
            Pedidos = new List<Pedido>();
            Consecutivo = consecutivo;
        }

        public int Consecutivo { get; set; }

        public string Nombre { get; set; }

        public Grupo Grupo { get; set; }

        public List<Pedido> Pedidos { get; set; }
    }
}
