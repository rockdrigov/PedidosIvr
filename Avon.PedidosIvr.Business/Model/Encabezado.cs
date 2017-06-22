using Avon.PedidosIvr.Entities;
using System;
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

        public string Nombre
        {
            get
            {
                return string.Format("ORDER.{0}019000{1}4", Convert.ToInt32(Grupo.Campana).ToString("D2"),Consecutivo.ToString("D4"));
            }
        }
        public Grupo Grupo { get; set; }

        public List<Pedido> Pedidos { get; set; }
    }
}
