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
    public class TruckController : Controller
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
            using (var context = new Context())
            {
                // Yıl bilgisini otomatik olarak alacak şekilde kodu güncelledim
                string tableSuffix = ilkTarih.Year.ToString();

                var query = $@"SELECT 
                                    dbo.Tesis_Bilgileri.Tesis_Adi, 
                                    Sabit_Kamyonlar.Kamyon_Plaka,
                                    SUM(Uretimler.UretilenMiktar) AS Uretilen_Miktar
                                FROM 
                                    G{tableSuffix}_Uretimler AS Uretimler 
                                INNER JOIN 
                                    dbo.Tesis_Bilgileri ON Uretimler.Tesis_Id = dbo.Tesis_Bilgileri.Id
                                INNER JOIN 
                                    Sabit_Kamyonlar ON Uretimler.Kamyon_Id = Sabit_Kamyonlar.Kamyon_Id 
                                WHERE 
                                    (Uretimler.Silindi = 0) 
                                    AND (Uretimler.Uretim_Tipi IN (1, 2)) 
                                    AND (Uretimler.Tesis_Id = 1)
                                    AND (Uretimler.Tarih BETWEEN @ilkTarih AND @sonTarih) 
                                GROUP BY 
                                    dbo.Tesis_Bilgileri.Tesis_Adi,  Sabit_Kamyonlar.Kamyon_Plaka";

                var ilkTarihParam = new SqlParameter("@ilkTarih", ilkTarih);
                var sonTarihParam = new SqlParameter("@sonTarih", sonTarih);
                var sonuc = context.Database.SqlQuery<TruckInformation>(query, ilkTarihParam, sonTarihParam).ToList();
                ViewData["Veriler"] = sonuc;

                return View();
            }
        }
    }
}
