using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BETONWEB.Models.Entity
{
    public class Sabit_Kullanicilar
    {
        [Key]
        public int Id { get; set; }
        public string Kullanici { get; set; }
        public string Sifre { get; set; }
        public bool YetkiliKullanici { get; set; }
    }
}