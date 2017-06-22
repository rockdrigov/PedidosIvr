using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Avon.PedidosIvr.Entities
{
    [Table("tbl_registro_transacciones")]
    public class Transaccion
    {
        [Key]
        [Column("id_pedido")]
        public string Id { get; set; }

        [Column("enviar_ftp")]
        public string EnviarFtp { get; set; }

        [ForeignKey("Id")]
        public virtual Pedido Pedido {get; set;}
    }
}
