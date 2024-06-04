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
    public class ServiceController : Controller
    {
        Context C = new Context();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Index(DateTime ilkTarih, DateTime sonTarih)
        {
            int year = DateTime.Now.Year; // Geçerli yılı al

            using (var context = new Context())
            {
                string tableSuffix = year.ToString(); // Yıl bilgisini string olarak al

                var query = $@"SELECT dbo.Tesis_Bilgileri.Tesis_Adi, 
                                       SUM(Uretimler.UretilenMiktar) AS Uretilen_Miktar,
                                       Sabit_Hizmetler.Hizmet_Adi 
                                FROM G{tableSuffix}_Uretimler AS Uretimler 
                                INNER JOIN dbo.Tesis_Bilgileri ON Uretimler.Tesis_Id = dbo.Tesis_Bilgileri.Id
                                INNER JOIN Sabit_Hizmetler ON Uretimler.Hizmet_Id = Sabit_Hizmetler.Hizmet_Id 
                                WHERE (Uretimler.Silindi = 0) 
                                      AND (Uretimler.Uretim_Tipi IN (1, 2)) 
                                      AND (Uretimler.UretimBitisTarihi < @sonTarih) 
                                      AND (Uretimler.Tesis_Id = 1)
                                GROUP BY dbo.Tesis_Bilgileri.Tesis_Adi, Sabit_Hizmetler.Hizmet_Adi";

                var sonTarihParam = new SqlParameter("@sonTarih", sonTarih);
                var sonuc = context.Database.SqlQuery<ServicesInformation>(query, sonTarihParam).ToList();
                ViewData["Veriler"] = sonuc;

                return View();
            }
        }
    }
}
