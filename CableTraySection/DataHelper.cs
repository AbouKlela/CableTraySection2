using CableTraySection.Model;
using System;
using System.Collections.Generic;

namespace CableTraySection
{
    internal class DataHelper
    {
        public static string FilePath { get; set; } = null;
        public static List<Dictionary<string, string>> CSOD;
        public static List<Dictionary<string, string>> EOD;
        public static List<CableData> Data { get; set; } = new List<CableData>();

        public static List<Double> CableDiameters = new List<double>();


    }
}
