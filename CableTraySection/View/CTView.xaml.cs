using CableTraySection.Model;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
namespace CableTraySection.View
{
    /// <summary>
    /// Interaction logic for CTView.xaml
    /// </summary>
    public partial class CTView : Window
    {
        ViewModel.ViewModel viewModel = new ViewModel.ViewModel();
        public CTView()
        {

            InitializeComponent();


            DataContext = viewModel;
        }

        // UI Control
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Close();

        }



        // UPDATE OD OF CABLES 
        private void CrossSectionComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string x = null;
            foreach (var pair in DataHelper.CSOD)
            {
                // Try to get the value based on the selected CrossSectionComboBox text
                if (pair.TryGetValue(this.CrossSectionComboBox.Text, out x))
                {
                    // If a match is found, update the TextBlock and break the loop
                    viewModel.OD = x;


                    break; // Exit the loop after finding the match
                }
            }

            ChangeSelectedCable();
        }


        // UPDATE OD OF EARTHING
        private void EarthingCSComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string x = null;
            foreach (var pair in DataHelper.EOD)
            {
                // Try to get the value based on the selected CrossSectionComboBox text
                if (pair.TryGetValue(this.EarthingCSComboBox.Text, out x))
                {
                    // If a match is found, update the TextBlock and break the loop
                    viewModel.EOD = x;
                    break; // Exit the loop after finding the match
                }
            }
            ChangeSelectedCable();
        }

        // LOAD DATA FROM EXCEL
        private void CoresComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.CoresComboBox.SelectedItem != null && this.ConductorTypeComboBox.SelectedItem != null && this.CableTypeComboBox.SelectedItem != null)
            {
                Utils.Excel(this.CoresComboBox.SelectedItem.ToString(), this.ConductorTypeComboBox.SelectedItem.ToString(), this.CableTypeComboBox.SelectedItem.ToString());

            }
            ChangeSelectedCable();
        }


        //Change SelectedCable
        public void ChangeSelectedCable()
        {
            if (EarthingCSComboBox.SelectedItem == null || EarthingCSComboBox.Text == "0")
            {
                //was textbox
                viewModel.SelectedCableName = $"({this.CoresComboBox.SelectedItem}x{this.CrossSectionComboBox.SelectedItem}) mm² {this.ConductorTypeComboBox.SelectedItem}/{this.CableTypeComboBox.SelectedItem}";

            }
            else
            {
                viewModel.SelectedCableName = $"({this.CoresComboBox.SelectedItem}x{this.CrossSectionComboBox.SelectedItem}) mm² {this.ConductorTypeComboBox.SelectedItem}/{this.CableTypeComboBox.SelectedItem}" +
                    $" + (1x{this.EarthingCSComboBox.SelectedItem}) mm² CU/PVC";

            }
        }


       

        private void PackIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.linkedin.com/in/klela/",
                UseShellExecute = true
            });
        }


    }
}
