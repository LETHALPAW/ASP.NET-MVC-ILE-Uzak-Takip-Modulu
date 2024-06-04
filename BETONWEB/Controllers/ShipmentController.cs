using BETONWEB.Models.Classes;
using BETONWEB.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BETONWEB.Controllers
{
    public class ShipmentController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(DateTime ilkTarih, DateTime sonTarih)
        {
            int year = DateTime.Now.Year; // Geçerli yılı al

            using (var context = new Context())
            {
                string tableSuffix = year.ToString(); // Yıl bilgisini string olarak al

                var query = $@"SELECT
                                    Uretimler.Uretimler_Id,
                                    Uretimler.UretilenMiktar, 
                                    Uretimler.NetMiktar,
                                    dbo.Sabit_Musteriler.Musteri_Adi, 
                                    dbo.Sabit_Santiyeler.Santiye_Adi,                
                                    dbo.Sabit_Receteler.Recete_Adi,  
                                    dbo.Sabit_Hizmetler.Hizmet_Adi                    
                            FROM dbo.Sabit_Pompalar 
                            RIGHT OUTER JOIN dbo.Sabit_Suruculer 
                            RIGHT OUTER JOIN G{tableSuffix}_Uretimler AS Uretimler 
                            INNER JOIN dbo.Tesis_Bilgileri ON Uretimler.Tesis_Id = dbo.Tesis_Bilgileri.Id 
                            ON dbo.Sabit_Suruculer.Surucu_Id = Uretimler.Pompa_SurucuId 
                            LEFT OUTER JOIN dbo.G{tableSuffix}_Siparisler 
                            ON Uretimler.Siparis_Id = dbo.G{tableSuffix}_Siparisler.Siparis_Id 
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
                                AND (Uretimler.Uretim_Tipi = 1 OR Uretimler.Uretim_Tipi = 2) 
                                AND (Uretimler.UretimBitisTarihi BETWEEN @ilkTarih AND @sonTarih)";

                var ilkTarihParam = new SqlParameter("@ilkTarih", ilkTarih);
                var sonTarihParam = new SqlParameter("@sonTarih", sonTarih);
                var sonuc = context.Database.SqlQuery<ShipmentInformation>(query, ilkTarihParam, sonTarihParam).ToList();
                ViewData["Veriler"] = sonuc;

                return View();
            }
        }
    }
}
