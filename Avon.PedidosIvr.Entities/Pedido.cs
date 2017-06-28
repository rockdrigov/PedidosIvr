using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avon.PedidosIvr.Entities
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        [Column("Id_Pedido")]
        public string Id { get; set; }

        [Column("Zona")]
        public string Zona { get; set; }

        [Column("Registro")]
        public string Registro { get; set; }

        [Column("Camp")]
        public string Campana { get; set; }

        [Column("FechaF")]
        public DateTime FechaF { get; set; }

        [Column("FechaP")]
        public DateTime FechaP { get; set; }

        [Column("ProductoClave")]
        public string ProductoClave { get; set; }

        [Column("ProductoClaveAnt")]
        public string ProductoClaveAnt { get; set; }

        [NotMapped]
        public int Consecutivo { get; set; }

    }
}
