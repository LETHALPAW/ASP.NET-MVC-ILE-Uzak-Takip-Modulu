using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BETONWEB.Models.ViewModel
{
    public class UserLogin
    {
        public string Kullanici { get; set; }
        public string Sifre { get; set; }
        public bool BeniHatirla { get; set; }
    }
}