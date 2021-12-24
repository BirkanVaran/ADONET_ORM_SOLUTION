using ADONET_ORM_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADONET_ORM_Entites.Entites
{
    [Table(TableName ="Yazarlar",PrimaryColumn ="YazarId",IdentityColumn ="YazarId")]
    public class Yazar
    {
        public int YazarId { get; set; }
        public string YazarAdSoyad { get; set; }
        public DateTime KayitTarihi { get; set; }
        public bool SilindiMi { get; set; }
    }
}
