using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using BETONWEB.Models.ViewModel;
using BETONWEB.Models.Classes;

namespace BETONWEB.Controllers
{
    public class InvoiceProductController : Controller
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
                var query = $@"SELECT     
                                   dbo.Tesis_Bilgileri.Tesis_Adi, 
                                   Sabit_Receteler.Recete_Adi,
                                   Sabit_Musteriler.Musteri_Adi,
                                   SUM(Uretimler.UretilenMiktar) AS Uretilen_Miktar
                               FROM 
                                   dbo.G{tableSuffix}_Uretimler AS Uretimler 
                               INNER JOIN
                                   dbo.Tesis_Bilgileri ON Uretimler.Tesis_Id = dbo.Tesis_Bilgileri.Id 
                               INNER JOIN 
                                   sabit_receteler ON Uretimler.Recete_Id = sabit_receteler.Recete_Id
                               INNER JOIN 
                                   Sabit_Musteriler ON Uretimler.Musteri_Id = Sabit_Musteriler.Musteri_Id
                               WHERE (Uretimler.Silindi = 0) 
                                     AND (Uretimler.Uretim_Tipi IN (1, 2)) 
                                     AND (Uretimler.Tesis_Id = 1) 
                                     AND (Uretimler.Tarih BETWEEN @ilkTarih AND @sonTarih) 
                               GROUP BY dbo.Tesis_Bilgileri.Tesis_Adi, sabit_receteler.Recete_Adi, Sabit_Musteriler.Musteri_Adi;";

                var ilkTarihParam = new SqlParameter("@ilkTarih", ilkTarih);
                var sonTarihParam = new SqlParameter("@sonTarih", sonTarih);

                try
                {
                    var sonuc = context.Database.SqlQuery<ProductInvoiceInformation>(query, ilkTarihParam, sonTarihParam).ToList();
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
