using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BETONWEB.Models.ViewModel
{
    public class MusteriBazındaSiparisler
    {
        public decimal SiparisIstenen { get; set; }
        public decimal SiparisVerilen { get; set; }
        public string MusteriAdi { get; set; }
        public DateTime SiparisTarihi { get; set; }
    }
}