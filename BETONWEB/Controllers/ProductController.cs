using BETONWEB.Models.Classes;
using BETONWEB.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace BETONWEB.Controllers
{
    public class ProductController : Controller
    {
        Context C = new Context();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //[Authorize]
        public ActionResult Index(DateTime ilkTarih, DateTime sonTarih)
        {
            int year = DateTime.Now.Year;  // Geçerli yıl bilgisi
            string tableSuffix = year.ToString();  // Yıl bilgisini string olarak al

            using (var context = new Context())
            {
                var query = $@"
                    SELECT ISNULL(Uretimler.Iade_Kullanildi, 0) as Iade_Kullanildi, 
                           dbo.Sabit_Musteriler.Musteri_Kodu, 
                           Uretimler.Toplam_Periyot, 
                           Uretimler.Toplam_Sure, 
                           Uretimler.Tarih, 
                           Uretimler.Kullanici, 
                           Uretimler.UretilenMiktar, 
                           Uretimler.HazirMiktar, 
                           Uretimler.IadeMiktar, 
                           Uretimler.NetMiktar, 
                           Uretimler.FreeMiktari, 
                           Uretimler.UretimFazlasi, 
                           Uretimler.Uretim_No, 
                           Uretimler.Tesis_Id, 
                           dbo.Sabit_Musteriler.Musteri_Adi, 
                           dbo.Sabit_Kamyonlar.Kamyon_Plaka, 
                           Uretimler.Irsaliye_No, 
                           dbo.Sabit_Receteler.Recete_Adi, 
                           dbo.Sabit_Santiyeler.Santiye_Adi, 
                           dbo.Sabit_Hizmetler.Hizmet_Adi, 
                           dbo.Sabit_Kamyonlar.Kamyon_Sicil, 
                           dbo.Sabit_Pompalar.Pompa, 
                           dbo.Sabit_Pompalar.Pompa_Plaka, 
                           Sabit_Suruculer_1.Surucu_AdiSoyadi, 
                           Sabit_Suruculer_1.Surucu_Ad, 
                           Sabit_Suruculer_1.Surucu_Soyad, 
                           dbo.Sabit_Suruculer.Surucu_Ad+' '+Sabit_Suruculer.Surucu_Soyad AS Pompaci, 
                           Uretimler.UretimBitisTarihi, 
                           dbo.G{tableSuffix}_Siparisler.Siparis_Kodu, 
                           dbo.G{tableSuffix}_Siparisler.Siparis_sira, 
                           dbo.Sabit_Receteler.Recete_Kodu, 
                           dbo.Sabit_Receteler.Recete_Sinifi, 
                           dbo.Sabit_Musteriler.Musteri_Yetkili, 
                           Uretimler.Aciklama,
                           Uretimler.UretimBaslamaTarihi, 
                           Uretimler.Kamyon_Net, 
                           Uretimler.Kamyon_Brut, 
                           Uretimler.Kamyon_Dara, 
                           Uretimler.Toplam_Malzeme, 
                           Uretimler.Toplam_Maliyet, 
                           Uretimler.Birim_Maliyet, 
                           Uretimler.Tolerans_Hatasi, 
                           Uretimler.Alinan_Iade_Miktari,
                           Uretimler.EIrsaliyeNo,
                           Uretimler.EIrsaliyeBase64,
                           Uretimler.EIrsaliyeString,
                           Uretimler.EIrsaliyePdf
                    FROM dbo.Sabit_Pompalar 
                    RIGHT OUTER JOIN dbo.G{tableSuffix}_Siparisler 
                    RIGHT OUTER JOIN dbo.G{tableSuffix}_Uretimler AS Uretimler 
                    LEFT OUTER JOIN dbo.Sabit_Suruculer 
                    ON Uretimler.Pompa_SurucuId = dbo.Sabit_Suruculer.Surucu_Id 
                    ON dbo.G{tableSuffix}_Siparisler.Siparis_Id = Uretimler.Siparis_Id 
                    LEFT OUTER JOIN dbo.Sabit_Suruculer AS Sabit_Suruculer_1 
                    ON Uretimler.Surucu_Id = Sabit_Suruculer_1.Surucu_Id 
                    ON dbo.Sabit_Pompalar.Pompa_Id = Uretimler.Pompa_Id 
                    LEFT OUTER JOIN dbo.Sabit_Hizmetler 
                    ON Uretimler.Hizmet_Id = dbo.Sabit_Hizmetler.Hizmet_Id 
                    LEFT OUTER JOIN dbo.Sabit_Santiyeler 
                    ON Uretimler.Santiye_Id = dbo.Sabit_Santiyeler.Santiye_Id 
                    LEFT OUTER JOIN dbo.Sabit_Receteler 
                    ON Uretimler.Tesis_Id = dbo.Sabit_Receteler.Recete_Tesis 
                    AND Uretimler.Recete_Id = dbo.Sabit_Receteler.Recete_Id 
                    LEFT OUTER JOIN dbo.Sabit_Kamyonlar 
                    ON Uretimler.Kamyon_Id = dbo.Sabit_Kamyonlar.Kamyon_Id 
                    LEFT OUTER JOIN dbo.Sabit_Musteriler 
                    ON Uretimler.Musteri_Id = dbo.Sabit_Musteriler.Musteri_Id
                    WHERE (Uretimler.Silindi = 0) 
                    AND (Uretimler.Uretim_Tipi = 1 OR Uretimler.Uretim_Tipi = 2 OR Uretimler.Uretim_Tipi = 5) 
                    AND (ISNULL(Uretimler.Iade_Kullanildi, 0) = 0) 
                    AND (Uretimler.UretimBitisTarihi BETWEEN @ilkTarih AND @sonTarih)";

                // Log the query for debugging purposes
                System.Diagnostics.Debug.WriteLine(query);

                var ilkTarihParam = new SqlParameter("@ilkTarih", ilkTarih);
                var sonTarihParam = new SqlParameter("@sonTarih", sonTarih);

                try
                {
                    var sonuc = context.Database.SqlQuery<ProductInformation>(query, ilkTarihParam, sonTarihParam).ToList();
                    ViewData["Veriler"] = sonuc;

                    if (!sonuc.Any())
                    {
                        ViewData["Message"] = "Seçili tarih arası gösterilecek herhangi bir data bulunamadı !.";
                    }

                    return View();
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = $"Hata !: {ex.Message}";
                    return View();
                }
            }
        }
    }
}
