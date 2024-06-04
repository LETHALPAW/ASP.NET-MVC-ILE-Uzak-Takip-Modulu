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
    public class SpendController : Controller
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
                                    dbo.G{tableSuffix}_Malzeme_Detay.MalzemeAdi, 
                                    SUM(Uretimler.UretilenMiktar) AS ToplamUretilenMiktar,
                                    SUM(dbo.G{tableSuffix}_Malzeme_Detay.Toplam) AS ToplamHarcanan
                                FROM
                                    dbo.G{tableSuffix}_Malzeme_Detay 
                                INNER JOIN
                                    dbo.G{tableSuffix}_Uretimler AS Uretimler ON dbo.G{tableSuffix}_Malzeme_Detay.Uretimler_Id = Uretimler.Uretimler_Id
                                    AND dbo.G{tableSuffix}_Malzeme_Detay.Tesis_Id = Uretimler.Tesis_Id
                                LEFT OUTER JOIN
                                    dbo.Sabit_Receteler ON Uretimler.Tesis_Id = dbo.Sabit_Receteler.Recete_Tesis 
                                    AND Uretimler.Recete_Id = dbo.Sabit_Receteler.Recete_Id
                                WHERE
                                    (Uretimler.Uretim_Tipi IN (1, 2)) 
                                    AND (Uretimler.UretimBitisTarihi BETWEEN @ilkTarih AND @sonTarih) 
                                    AND (Uretimler.Silindi = 0)
                                GROUP BY
                                    dbo.G{tableSuffix}_Malzeme_Detay.MalzemeAdi";


                var ilkTarihParam = new SqlParameter("@ilkTarih", ilkTarih);
                var sonTarihParam = new SqlParameter("@sonTarih", sonTarih);
                var sonuc = context.Database.SqlQuery<SpendInformation>(query, ilkTarihParam, sonTarihParam).ToList();
                ViewData["Veriler"] = sonuc;

                return View();
            }
        }
    }
}
