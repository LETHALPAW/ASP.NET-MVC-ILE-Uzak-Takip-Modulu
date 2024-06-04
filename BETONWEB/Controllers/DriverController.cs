using BETONWEB.Models.Classes;
using BETONWEB.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace BETONWEB.Controllers
{
    public class DriverController : Controller
    {
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
                    SELECT dbo.Tesis_Bilgileri.Tesis_Adi, 
                           Sabit_Suruculer.Surucu_AdiSoyadi,
                           Sabit_Kamyonlar.Kamyon_Plaka,
                           SUM(Uretimler.UretilenMiktar) AS Uretilen_Miktar
                    FROM dbo.G{tableSuffix}_Uretimler AS Uretimler 
                    INNER JOIN dbo.Tesis_Bilgileri ON Uretimler.Tesis_Id = dbo.Tesis_Bilgileri.Id
                    INNER JOIN Sabit_Suruculer ON Uretimler.Surucu_Id = Sabit_Suruculer.Surucu_Id
                    INNER JOIN Sabit_Kamyonlar ON Uretimler.Kamyon_Id = Sabit_Kamyonlar.Kamyon_Id
                    WHERE (Uretimler.Silindi = 0) 
                          AND (Uretimler.Uretim_Tipi IN (1, 2)) 
                          AND (Uretimler.Tesis_Id = 1)
                          AND (Uretimler.Tarih BETWEEN @ilkTarih AND @sonTarih)
                    GROUP BY dbo.Tesis_Bilgileri.Tesis_Adi, Uretimler.Kamyon_Id, Sabit_Kamyonlar.Kamyon_Plaka, Sabit_Suruculer.Surucu_AdiSoyadi";

                var ilkTarihParam = new SqlParameter("@ilkTarih", ilkTarih);
                var sonTarihParam = new SqlParameter("@sonTarih", sonTarih);

                try
                {
                    var sonuc = context.Database.SqlQuery<DriverInformation>(query, ilkTarihParam, sonTarihParam).ToList();
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
