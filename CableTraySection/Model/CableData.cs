using System.Collections.Generic;

namespace CableTraySection.Model
{
    public class CableData
    {
       


        public string DfromTo { get; set; }
        public string DSelectedCable { get; set; }
        public string DOD { get; set; }

        public string DEOD { get; set; }





        public CableData(string fromto, string selectedCalbe, string CableOD, string EarthingOD)
        {
            this.DfromTo = fromto;
            this.DSelectedCable = selectedCalbe;
            this.DOD = CableOD;
            this.DEOD = EarthingOD;
        }
    }
}
