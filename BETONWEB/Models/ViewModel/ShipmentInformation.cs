using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BETONWEB.Models.ViewModel
{
    public class ShipmentInformation
    {
        public int Uretimler_Id { get; set; }
        public decimal? UretilenMiktar { get; set; }
        public decimal? NetMiktar { get; set; }
        public string Musteri_Adi { get; set; }
        public string Santiye_Adi { get; set; }
        public string Recete_Adi { get; set; }
        public string Hizmet_Adi { get; set; }
    }
}