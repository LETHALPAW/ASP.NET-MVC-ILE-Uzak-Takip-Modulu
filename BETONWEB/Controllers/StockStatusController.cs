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
    public class StockStatusController : Controller
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
                // Query'deki tablo adlarını ve yıl bilgisini değişkene atayarak kodun daha okunaklı olmasını sağlayalım
                string tableSuffix = ilkTarih.Year.ToString();

                var query = $@"SELECT 
                                    Stok_Id, 
                                    Malzeme, 
                                    Stok_TesisId, 
                                    Stok_Silono, 
                                    ISNULL(Devir_Miktari, 0) AS Devir, 
                                    ISNULL(Giren_Miktar, 0) AS Giren, 
                                    ISNULL(Indicator_Okunan, 0) AS Cikan, 
                                    ISNULL(Sayim_Miktar, 0) AS Sayim, 
                                    ISNULL(Devir_Miktari, 0) + ISNULL(Giren_Miktar, 0) - ISNULL(Indicator_Okunan, 0) + ISNULL(Sayim_Miktar, 0) AS Kalan
                                FROM 
                                (
                                    SELECT 
                                        Stok_Id, 
                                        Malzeme, 
                                        Stok_TesisId, 
                                        Stok_Silono,
                                        (
                                            SELECT 
                                                SUM(ISNULL(Giren_Miktar, 0)) - SUM(ISNULL(Indicator_Okunan, 0)) + SUM(ISNULL(Sayim_Miktar, 0)) AS Devir
                                            FROM 
                                                dbo.G{tableSuffix}_Malzeme_Detay
                                            WHERE 
                                                IslemTarihi < @ilkTarih
                                                AND Tesis_Id = Tablo.Stok_TesisId 
                                                AND Silono = Tablo.Stok_Silono 
                                                AND Stok_Id = Tablo.Stok_Id
                                        ) AS Devir_Miktari,
                                        (
                                            SELECT 
                                                SUM(ISNULL(Giren_Miktar, 0)) AS Giren
                                            FROM 
                                                dbo.G{tableSuffix}_Malzeme_Detay
                                            WHERE 
                                                IslemTarihi BETWEEN @ilkTarih AND @sonTarih 
                                                AND Tesis_Id = Tablo.Stok_TesisId 
                                                AND Silono = Tablo.Stok_Silono 
                                                AND Stok_Id = Tablo.Stok_Id
                                        ) AS Giren_Miktar,
                                        (
                                            SELECT 
                                                SUM(ISNULL(Indicator_Okunan, 0)) AS Cikan
                                            FROM 
                                                dbo.G{tableSuffix}_Malzeme_Detay
                                            WHERE 
                                                IslemTarihi BETWEEN @ilkTarih AND @sonTarih 
                                                AND Tesis_Id = Tablo.Stok_TesisId 
                                                AND Silono = Tablo.Stok_Silono 
                                                AND Stok_Id = Tablo.Stok_Id
                                        ) AS Indicator_Okunan,
                                        (
                                            SELECT 
                                                SUM(ISNULL(Sayim_Miktar, 0)) AS Sayim
                                            FROM 
                                                dbo.G{tableSuffix}_Malzeme_Detay
                                            WHERE 
                                                IslemTarihi BETWEEN @ilkTarih AND @sonTarih 
                                                AND Tesis_Id = Tablo.Stok_TesisId 
                                                AND Silono = Tablo.Stok_Silono 
                                                AND Stok_Id = Tablo.Stok_Id
                                        ) AS Sayim_Miktar
                                    FROM 
                                    (
                                        SELECT 
                                            dbo.Malzemeler.Id AS Stok_Id, 
                                            dbo.Malzemeler.Malzeme, 
                                            dbo.Sabit_SiloListesi.Stok_TesisId, 
                                            dbo.Sabit_SiloListesi.Stok_Silono
                                        FROM 
                                            dbo.Malzemeler 
                                        INNER JOIN 
                                            dbo.Sabit_SiloListesi ON dbo.Malzemeler.Id = dbo.Sabit_SiloListesi.Malzeme_Id
                                        WHERE 
                                            dbo.Malzemeler.StokHareketi = 1
                                        GROUP BY 
                                            dbo.Malzemeler.Id, 
                                            dbo.Malzemeler.Malzeme, 
                                            dbo.Sabit_SiloListesi.Stok_TesisId, 
                                            dbo.Sabit_SiloListesi.Stok_Silono
                                    ) AS Tablo
                                ) AS Tbl";

                var ilkTarihParam = new SqlParameter("@ilkTarih", ilkTarih);
                var sonTarihParam = new SqlParameter("@sonTarih", sonTarih);
                var sonuc = context.Database.SqlQuery<StockInformation>(query, ilkTarihParam, sonTarihParam).ToList();
                ViewData["Veriler"] = sonuc;

                return View();
            }
        }
    }
}
