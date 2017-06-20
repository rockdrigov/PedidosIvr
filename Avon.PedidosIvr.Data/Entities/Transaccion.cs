using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avon.PedidosIvr.Data.Entities
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
