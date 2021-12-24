using ADONET_ORM_Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADONET_ORM_Entites.Entites
{
    [Table(TableName = "Turler", PrimaryColumn = "TurId", IdentityColumn = "TurId")]
   public class Tur
    {
        public int TurId { get; set; }
        public string TurAdi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
    }
}
