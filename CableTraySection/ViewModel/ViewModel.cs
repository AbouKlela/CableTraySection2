using CableTraySection.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace CableTraySection.ViewModel
{
    public class ViewModel : ViewModelBase
    {
        //--------------------------------------------------PROPERTIES --------------------------------------------------------------

        public string from;
        public string From
        {

            get => from;
            set => SetProperty(ref from, value);
        }

        public string to;
        public string To
        {

            get => to;
            set => SetProperty(ref to, value);
        }


        public string CoreNumber { get; set; }

        private readonly List<string> coreNumbers = new List<string>() { "1C", "2C", "3C", "3.5C", "4C" };

        public List<string> CoreNumbers
        {
            get { return coreNumbers; }

        }

        public string ConductorType { get; set; }

        private readonly List<string> conductortypes = new List<string>() { "CU", "AL" };

        public List<string> Conductortypes
        {
            get { return conductortypes; }

        }

        public string cableType { get; set; }

        private readonly List<string> cabletypes = new List<string>() { "PVC/PVC", "XLPE/PVC", "MICA/XLPE/LS0H", "XLPE/LSF0H" };

        public List<string> CableTypes
        {
            get { return cabletypes; }

        }

        public string oD;
        public string OD
        {

            get => oD;
            set => SetProperty(ref oD, value);


        }



        private static ObservableCollection<string> cablesizes = new ObservableCollection<string>();
        public static ObservableCollection<string> CableSizes
        {

            get { return cablesizes; }
            set { cablesizes = value; }

        }

        private static ObservableCollection<string> earthingCS = new ObservableCollection<string>();
        public static ObservableCollection<string> EarthingCS
        {
            get { return earthingCS; }
            set { earthingCS = value; }


        }



        public string eOD;
        public string EOD
        {

            get => eOD;
            set => SetProperty(ref eOD, value);


        }

        private int selectedCable;
        public int SelectedCable
        {

            get => selectedCable;
            set => SetProperty(ref selectedCable, value);


        }

        private string selectedCableName;
        public string SelectedCableName
        {

            get => selectedCableName;
            set => SetProperty(ref selectedCableName, value);


        }


        private static ObservableCollection<CableData> cableDatas = new ObservableCollection<CableData>();
        public static ObservableCollection<CableData> CableDatas
        {
            get
            {


                return cableDatas;


            }
            set
            {
                cableDatas = value;




            }

        }

        private double width;
        public double Width
        {

            get => width;
            set => SetProperty(ref width, value);


        }

        private int height = 100;
        public int Height
        {

            get => height;
            set => SetProperty(ref height, value);
        }

        private double fillingRatio;
        public double FillingRatio
        {

            get => fillingRatio;
            set => SetProperty(ref fillingRatio, value);
        }


        private double initialRatio = 0.5;
        public double InitialRatio
        {

            get => initialRatio;
            set => SetProperty(ref initialRatio, value);
        }
        private double betweenRatio = 0.5;
        public double BetweenRatio
        {

            get => betweenRatio;
            set => SetProperty(ref betweenRatio, value);
        }

        private double spareRatio = 1.2;

        public double SpareRatio
        {

            get => spareRatio;
            set => SetProperty(ref spareRatio, value);
        }




        //--------------------------------------------------COMMANDS --------------------------------------------------------------
        
        private RelayCommand addCommand;

        public RelayCommand AddCommand => addCommand ?? (addCommand = new RelayCommand(AddFunc));

        private void AddFunc(object obj)
        {
            CableDatas.Add(new CableData(from + " To " + to, SelectedCableName, OD, EOD));

            DataHelper.CableDiameters.Clear();
            DataHelper.CableDiameters = cableDatas.ToList().Select(X => Double.Parse(X.DOD)).ToList();
            DataHelper.Data.Clear();
            DataHelper.Data = cableDatas.ToList();


        }


        private RelayCommand clearCommand;

        public RelayCommand ClearCommand => clearCommand ?? (clearCommand = new RelayCommand(ClearFunc));

        private void ClearFunc(object obj)
        {
            cableDatas.Clear();
            DataHelper.CableDiameters.Clear();
            DataHelper.Data.Clear();

        }



        private RelayCommand calculateTray;

        public RelayCommand CalculateTray => calculateTray ?? (calculateTray = new RelayCommand(CalculateTrayFunc));

        private void CalculateTrayFunc(object obj)
        {

            Width = Utils.SizeCableTray(DataHelper.CableDiameters, SpareRatio, InitialRatio, BetweenRatio);

        }


        private RelayCommand removeCalbe;

        public RelayCommand RemoveCalbe => removeCalbe ?? (removeCalbe = new RelayCommand(RemoveCableFunc));

        private void RemoveCableFunc(object obj)
        {
           
                CableDatas.RemoveAt(SelectedCable);
                DataHelper.CableDiameters.Clear();
                DataHelper.CableDiameters = cableDatas.ToList().Select(X => Double.Parse(X.DOD)).ToList();
                DataHelper.Data.Clear();
                DataHelper.Data = cableDatas.ToList();
            

        }

        private RelayCommand fillingRatioCalc;

        public RelayCommand FillingRatioCalc => fillingRatioCalc ?? (fillingRatioCalc = new RelayCommand(CalculateFillingRatioFunc));

        private void CalculateFillingRatioFunc(object obj)
        {

            double sum = 0;
            foreach (double D in DataHelper.CableDiameters)
            {
                sum += D;
            }
            FillingRatio = (sum /Width) * 100;


        }



        private RelayCommand createViewAndTrayCommand;
        public RelayCommand CreateViewAndTrayCommand => createViewAndTrayCommand ?? (createViewAndTrayCommand = new RelayCommand(CreateViewAndTrayFunc));

        private void CreateViewAndTrayFunc(object obj)
        {
            EventHandeler.Initial = initialRatio;
            EventHandeler.Between = betweenRatio;
            EventHandeler.Trayheight = Height;
            EventHandeler.TrayWidht = Width;
            EventHandeler.Event = Request.event1;
            DataHelper.ExEvent.Raise();
        }
    }
}
