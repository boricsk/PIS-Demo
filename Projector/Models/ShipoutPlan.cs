using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projector.Models
{
    public class ShipoutPlan
    {
        /// <summary>
        /// Gets or sets the document number associated with the record.
        /// </summary>
        public string Bizonylatszam { get; set; }
        public string Szam { get; set; }
        public decimal NyitottMennyiseg { get; set; }
        public DateTime EredetiKertDatum { get; set; }
        public DateTime KiszallitasiDatum { get; set; }
        public string CustomerName { get; set; }
        public string SapSoNum { get; set; }
        public string SapPoNum { get; set; }
        public double EgysegarAfaNelkul { get; set; }
        public decimal NyitottOsszeg { get; set; }
        public DateTime OrderDate { get; set; }
        public string SzamlazasiVevoSzam { get; set; }
        public string CustomerNo { get; set; }
        public string SeiCustRefNo { get; set; }
        public string ETD { get; set; }
        public decimal ShipOutStd {get; set; } = 0;
    }
}
