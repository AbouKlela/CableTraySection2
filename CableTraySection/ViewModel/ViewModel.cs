﻿using Autodesk.Revit.UI;
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


        public string oD;
        public string OD
        {

            get => oD;
            set => SetProperty(ref oD, value);


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


        private double width;
        public double Width
        {

            get => width;
            set => SetProperty(ref width, value);


        }

        private double height = 100;
        public double Height
        {

            get => height;
            set => SetProperty(ref height, value);
        }

        private double thickness = 10;
        public double Thickness
        {

            get => thickness;
            set => SetProperty(ref thickness, value);
        }

        private double fillingRatio=0;
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


        private string sectionName;

        public string SectionName { get => sectionName; set => SetProperty(ref sectionName, value); }

        private bool? table = true;

        public bool? Table { get => table; set => SetProperty(ref table, value); }


        private bool? dimention = true;

        public bool? Dimention { get => dimention; set => SetProperty(ref dimention, value); }



        //--------------------------------------------------COMMANDS --------------------------------------------------------------

        private RelayCommand addCommand;

        public RelayCommand AddCommand => addCommand ?? (addCommand = new RelayCommand(AddFunc));

        private void AddFunc(object obj)
        {

            if (OD == null || EOD == null || SelectedCableName == null || From == null || To == null)
            {
                TaskDialog.Show("Incomplete Data",
                              "All required fields for the cable data must be completed before proceeding.\nPlease review and fill in the missing information.",
                                 TaskDialogCommonButtons.Ok);

                return;
            }

            CableDatas.Add(new CableData("FROM " + From + " TO " + To, SelectedCableName, OD, EOD));
            DataHelper.CableDiameters.Clear();
            DataHelper.CableDiameters = cableDatas.ToList().Select(X => Double.Parse(X.DOD)).ToList();
            DataHelper.EarthingDiameters.Clear();
            DataHelper.EarthingDiameters = cableDatas.ToList().Select(X => Double.Parse(X.DEOD)).ToList();
            DataHelper.Data.Clear();
            DataHelper.Data = cableDatas.ToList();


        }


        private RelayCommand clearCommand;

        public RelayCommand ClearCommand => clearCommand ?? (clearCommand = new RelayCommand(ClearFunc));

        private void ClearFunc(object obj)
        {
            cableDatas.Clear();
            DataHelper.CableDiameters.Clear();
            DataHelper.EarthingDiameters.Clear();
            DataHelper.Data.Clear();

        }



        private RelayCommand calculateTray;

        public RelayCommand CalculateTray => calculateTray ?? (calculateTray = new RelayCommand(CalculateTrayFunc));

        private void CalculateTrayFunc(object obj)
        {
            if(CableDatas.Count == 0)
            {

                TaskDialog.Show("Action Required",
                 "Please add the necessary Cables before proceeding.\n", TaskDialogCommonButtons.Ok);
                return;
            }

            Width = Utils.SizeCableTray(DataHelper.CableDiameters, SpareRatio, InitialRatio, BetweenRatio);

        }


        private RelayCommand removeCalbe;

        public RelayCommand RemoveCalbe => removeCalbe ?? (removeCalbe = new RelayCommand(RemoveCableFunc));

        private void RemoveCableFunc(object obj)
        {

            CableDatas.RemoveAt(SelectedCable);
            DataHelper.CableDiameters.Clear();
            DataHelper.CableDiameters = cableDatas.ToList().Select(X => Double.Parse(X.DOD)).ToList();
            DataHelper.EarthingDiameters.Clear();
            DataHelper.EarthingDiameters = cableDatas.ToList().Select(X => Double.Parse(X.DOD)).ToList();
            DataHelper.Data.Clear();
            DataHelper.Data = cableDatas.ToList();


        }

        private RelayCommand fillingRatioCalc;

        public RelayCommand FillingRatioCalc => fillingRatioCalc ?? (fillingRatioCalc = new RelayCommand(CalculateFillingRatioFunc));

        private void CalculateFillingRatioFunc(object obj)
        {
            if (CableDatas.Count == 0) {

                TaskDialog.Show("Action Required",
                                "Please add the necessary Cables before proceeding.\n", TaskDialogCommonButtons.Ok);
                return; 
            
            }
            double sum = 0;
            foreach (double D in DataHelper.CableDiameters)
            {
                sum += Utils.CalculateAreaFromDiameter(D);
            }
            foreach (double D in DataHelper.EarthingDiameters)
            {
                sum += Utils.CalculateAreaFromDiameter(D);
            }


            FillingRatio = (sum / (Width * Height)) * 100;


        }



        private RelayCommand createViewAndTrayCommand;
        public RelayCommand CreateViewAndTrayCommand => createViewAndTrayCommand ?? (createViewAndTrayCommand = new RelayCommand(CreateViewAndTrayFunc));

        private void CreateViewAndTrayFunc(object obj)
        {

            if (string.IsNullOrEmpty(sectionName) || Width == 0 || FillingRatio == 0 || cableDatas.Count == 0)
            {
                TaskDialog.Show("Action Required",
                                "Please complete the following steps before proceeding:\n" +
                                 "1. Ensure the Section Name is filled.\n" +
                                 "2. Calculate the Width.\n" +
                                 "3. Calculate the Filling Ratio.\n" +
                                  "4. Add the necessary Cables.\n",TaskDialogCommonButtons.Ok);

                return;
            }

            EventHandeler.Initial = initialRatio;
            EventHandeler.Between = betweenRatio;
            EventHandeler.Trayheight = Height;
            EventHandeler.TrayWidht = Width;
            EventHandeler.TrayThickness = Thickness;
            EventHandeler.SectionName = SectionName;
            EventHandeler.FillingRatio = FillingRatio;
            EventHandeler.Table = (bool)Table;
            EventHandeler.Dimention = (bool)Dimention;

            EventHandeler.Event = Request.event1;
            DataHelper.ExEvent.Raise();
        }


    }
}
