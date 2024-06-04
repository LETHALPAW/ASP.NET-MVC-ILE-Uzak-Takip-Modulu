using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BETONWEB.Models.ViewModel
{
    public class ProductInformation
    {
        public bool? Iade_Kullanildi { get; set; }
        public string Musteri_Kodu { get; set; }
        public int? Toplam_Periyot { get; set; }
        public decimal? Toplam_Sure { get; set; }
        public DateTime Tarih { get; set; }
        public string Kullanici { get; set; }
        public decimal? UretilenMiktar { get; set; }
        public decimal? HazirMiktar { get; set; }
        public decimal? IadeMiktar { get; set; }
        public decimal? NetMiktar { get; set; }
        public decimal? FreeMiktar { get; set; }
        public decimal? UretimFazlasi { get; set; }
        public int? Uretim_No { get; set; }
        public int? Tesis_Id { get; set; }
        public string Musteri_Adi { get; set; }
        public int? Irsaliye_No { get; set; }
        public string Recete_Adi { get; set; }
        public string Hizmet_Adi { get; set; }
        public string Kamyon_Sicil { get; set; }
        public string Pompa { get; set; }
        public string Pompa_Plaka { get; set; }
        public string Surucu_AdiSoyadi { get; set; }
        public string Pompaci { get; set; }
        public DateTime UretimBitisTarihi { get; set; }
        public string Siparis_Kodu { get; set; }
        public string Recete_Kodu { get; set; }
        public string Recete_Sinifi { get; set; }
        public string Aciklama { get; set; }
        public DateTime UretimBaslamaTarihi { get; set; }
        public decimal? Alinan_Iade_Miktari { get; set; }
        public string EIrsaliyeNo { get; set; }
    }
}