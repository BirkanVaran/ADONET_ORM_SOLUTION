using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADONET_ORM_Common;
using ADONET_ORM_Entites.Entites;


namespace ADONET_ORM_Entites.Entites
{
    [Table(TableName = "Islem", PrimaryColumn = "IslemId", IdentityColumn = "IslemId")]
   public class OduncIslem
    {
        public int IslemId { get; set; }
        public int OgrId { get; set; }
        public int KitapId { get; set; }
        public DateTime OduncAldigiTarih { get; set; }
        public DateTime OduncBitisTarihi { get; set; }
        public bool TeslimEdildiMİ { get; set; }
        
    }
}
