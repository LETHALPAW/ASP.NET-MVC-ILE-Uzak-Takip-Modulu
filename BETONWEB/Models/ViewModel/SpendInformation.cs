using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BETONWEB.Models.ViewModel
{
    public class SpendInformation
    {
        public int Uretimler_Id { get; set; }
        public string MalzemeAdi  { get; set; }
        public decimal ToplamUretilenMiktar  { get; set; }
        public decimal ToplamHarcanan  { get; set; }
    }
}