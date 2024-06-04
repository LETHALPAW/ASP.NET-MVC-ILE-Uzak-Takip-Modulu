using BETONWEB.Models.Classes;
using BETONWEB.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace BETONWEB.Controllers
{
    public class CustomerAmountController : Controller
    {
        // GET: CustomerAmount
        Context C = new Context();

        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(DateTime ilkTarih, DateTime sonTarih)
        {
            int year = DateTime.Now.Year;  // Geçerli yıl bilgisi
            string tableSuffix = year.ToString();  // Yıl bilgisini string olarak al

            using (var context = new Context())
            {
                var query = $@"
                    SELECT Ana_Tablo.UretilenMiktar, 
                           Ana_Tablo.NetMiktar, 
                           dbo.Sabit_Musteriler.Musteri_Adi
                    FROM (
                        SELECT SUM(UretilenMiktar) AS UretilenMiktar, 
                               SUM(NetMiktar) AS NetMiktar, 
                               Recete_Id
                        FROM dbo.G{tableSuffix}_Uretimler
                        WHERE Silindi = 0 
                              AND (Uretim_Tipi = 1 OR Uretim_Tipi = 2) 
                              AND (UretimBitisTarihi BETWEEN @ilkTarih AND @sonTarih)
                        GROUP BY Recete_Id
                    ) AS Ana_Tablo 
                    INNER JOIN dbo.Sabit_Receteler ON Ana_Tablo.Recete_Id = dbo.Sabit_Receteler.Recete_Id 
                    LEFT OUTER JOIN (
                        SELECT Recete_Id, 
                               SUM(dbo.G{tableSuffix}_Malzeme_Detay.Toplam) AS Miktar, 
                               dbo.G{tableSuffix}_Malzeme_Detay.MalzemeAdi
                        FROM dbo.G{tableSuffix}_Uretimler AS G{tableSuffix}_Uretimler_1 
                        INNER JOIN dbo.G{tableSuffix}_Malzeme_Detay ON G{tableSuffix}_Uretimler_1.Uretimler_Id = dbo.G{tableSuffix}_Malzeme_Detay.Uretimler_Id
                        WHERE G{tableSuffix}_Uretimler_1.Silindi = 0 
                              AND (G{tableSuffix}_Uretimler_1.Uretim_Tipi = 1 OR G{tableSuffix}_Uretimler_1.Uretim_Tipi = 2) 
                              AND (G{tableSuffix}_Uretimler_1.UretimBitisTarihi BETWEEN @ilkTarih AND @sonTarih) 
                              AND dbo.G{tableSuffix}_Malzeme_Detay.Silindi = 0
                        GROUP BY dbo.G{tableSuffix}_Malzeme_Detay.MalzemeAdi, G{tableSuffix}_Uretimler_1.Recete_Id
                    ) AS D ON Ana_Tablo.Recete_Id = D.Recete_Id
                    LEFT OUTER JOIN dbo.Sabit_Musteriler ON Ana_Tablo.Recete_Id = dbo.Sabit_Musteriler.Musteri_Id
                    ORDER BY Ana_Tablo.Recete_Id";

                // Log the query for debugging purposes
                System.Diagnostics.Debug.WriteLine(query);

                var ilkTarihParam = new SqlParameter("@ilkTarih", ilkTarih);
                var sonTarihParam = new SqlParameter("@sonTarih", sonTarih);

                try
                {
                    var sonuc = context.Database.SqlQuery<CustomerAmountInformation>(query, ilkTarihParam, sonTarihParam).ToList();
                    ViewData["Veriler"] = sonuc;

                    if (!sonuc.Any())
                    {
                        ViewData["Message"] = "Seçilen tarih arası gösterilecek bir data bulunamadı. !";
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
