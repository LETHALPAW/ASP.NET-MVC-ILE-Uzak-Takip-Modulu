using BETONWEB.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BETONWEB.Models.Classes
{
    public class Context:DbContext
    {
        public DbSet<Sabit_Kullanicilar> Sabit_Kullanicilar { get; set; }
    }
}