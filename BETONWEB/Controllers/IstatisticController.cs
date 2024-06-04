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
    public class IstatisticController : Controller
    {
        Context C = new Context();

        // GET: Istatistic
        [Authorize]
        public ActionResult Index()
        {
            using (var context = new Context())
            {
                int year = DateTime.Now.Year; // Geçerli yıl bilgisini al

                // İstatistik sorguları
                var query1 = $@"SELECT  SabitReceteler.Recete_Adi, SUM(SabitUretimler.NetMiktar) AS NETMİKTAR
                                FROM G{year}_Uretimler AS SabitUretimler
                                INNER JOIN sabit_receteler AS SabitReceteler ON SabitUretimler.Recete_Id = SabitReceteler.Recete_Id
                                GROUP BY SabitUretimler.Recete_Id, SabitReceteler.Recete_Adi
                                ORDER BY SUM(SabitUretimler.NetMiktar) DESC;";

                var query2 = $@"SELECT  SabitSuruculer.Surucu_AdiSoyadi AS SURUCUADISOYADI, COUNT(*) AS SEFER
                                FROM G{year}_Uretimler AS SabitUretimler
                                INNER JOIN Sabit_Suruculer AS SabitSuruculer ON SabitUretimler.Surucu_Id = SabitSuruculer.Surucu_Id
                                GROUP BY SabitSuruculer.Surucu_AdiSoyadi 
                                ORDER BY COUNT(*) DESC;";

                var query3 = $@"SELECT  SabitSuruculer.Surucu_AdiSoyadi AS POMPALISURUCUADISOYADI, COUNT(*) AS SEFER
                                FROM G{year}_Uretimler AS SabitUretimler
                                INNER JOIN Sabit_Suruculer AS SabitSuruculer ON SabitUretimler.Surucu_Id = SabitSuruculer.Surucu_Id 
                                INNER JOIN Sabit_Pompalar AS Pompalar ON SabitUretimler.Pompa_Id = Pompalar.Pompa_Id 
                                GROUP BY SabitSuruculer.Surucu_AdiSoyadi
                                ORDER BY COUNT(*) DESC;";

                var query4 = $@"SELECT RIGHT(CONVERT(varchar(7), URETİMLER.tarih, 120), 2) AS Ay, SUM(URETİMLER.NetMiktar) AS ToplamMiktar
                                FROM g{year}_uretimler AS URETİMLER
                                WHERE URETİMLER.tarih IS NOT NULL
                                GROUP BY RIGHT(CONVERT(varchar(7), URETİMLER.tarih, 120), 2)
                                ORDER BY RIGHT(CONVERT(varchar(7), URETİMLER.tarih, 120), 2);";

                // İstatistik verilerini sorgula
                var receteBazindaUretimler = context.Database.SqlQuery<ReceteBazindaUretim>(query1).ToList();
                var mixerBazliSefer = context.Database.SqlQuery<MixerBazliSefer>(query2).ToList();
                var pompaBazliSefer = context.Database.SqlQuery<PompaBazliSefer>(query3).ToList();
                var AyBazındaUretimler = context.Database.SqlQuery<AyBazındaUretimler>(query4).ToList();

                // ViewData kullanarak istatistik verilerini görünüme aktar
                ViewData["UretimBazindaRecete"] = receteBazindaUretimler;
                ViewData["mixerBazliSefer"] = mixerBazliSefer;
                ViewData["pompaBazliSefer"] = pompaBazliSefer;
                ViewData["ayBazindaUretimler"] = AyBazındaUretimler;

                return View();
            }
        }

        // Ajax isteğiyle müşteri bazında siparişleri getir
        [HttpGet]
        public ActionResult MusteriBazindaSiparislerGetir(DateTime ilkTarih, DateTime sonTarih)
        {
            using (var context = new Context())
            {
                int year = DateTime.Now.Year; // Geçerli yıl bilgisini al

                // Siparişler için sorgu
                var query5 = $@"SELECT 
                                    Siparis_Istenen AS SiparisIstenen, 
                                    Siparis_Verilen AS SiparisVerilen, 
                                    Musteri_Adi AS MusteriAdi,
                                    Siparis_Tarih AS SiparisTarihi
                                FROM G{year}_Siparisler AS Siparisler
                                INNER JOIN Sabit_Musteriler AS SabitMusteriler 
                                ON SabitMusteriler.Musteri_Id = Siparisler.Siparis_Musteri
                                WHERE Siparis_Tarih BETWEEN @ilkTarih AND @sonTarih
                                GROUP BY Musteri_Adi, Siparis_Istenen, Siparis_Verilen, Siparis_Tarih;";

                // Müşteri bazında siparişleri sorgula
                var musteriBazindaSiparisler = context.Database.SqlQuery<MusteriBazındaSiparisler>(query5,
                    new SqlParameter("@ilkTarih", ilkTarih),
                    new SqlParameter("@sonTarih", sonTarih))
                    .ToList();

                // JSON olarak müşteri bazında siparişleri döndür
                return Json(new { data = musteriBazindaSiparisler }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
