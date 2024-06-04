using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BETONWEB.Models.ViewModel
{
    public class StockInformation
    {
        [Key]
        public int Stock_Id { get; set; }
        public string Malzeme { get; set; }
        public int Stok_TesisId { get; set; }
        public int Stok_Grup { get; set; }
        public int Stok_Silono { get; set; }
        public decimal Devir { get; set; }
        public decimal Giren { get; set; }
        public decimal Cikan { get; set; }
        public decimal Sayim { get; set; }
        public decimal Kalan { get; set; }
    }
}