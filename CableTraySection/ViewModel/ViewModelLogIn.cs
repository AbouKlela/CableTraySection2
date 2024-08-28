using CableTraySection.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CableTraySection.ViewModel
{
    internal class ViewModelLogIn :ViewModelBase
    {

        private string macAdress;

        public string MacAdress { get => macAdress; set => SetProperty(ref macAdress, value); }

        private RelayCommand getMacAddressCommand;

        public ICommand GetMacAddressCommand
        {
            get
            {
                if (getMacAddressCommand == null)
                {
                    getMacAddressCommand = new RelayCommand(GetMacAddress);
                }

                return getMacAddressCommand;
            }
        }

        private void GetMacAddress(object commandParameter)
        {

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Get only the network interfaces that are up and have a valid MAC address
                if (nic.NetworkInterfaceType != NetworkInterfaceType.Loopback && nic.OperationalStatus == OperationalStatus.Up)
                {
                    Console.WriteLine($"MAC Address of {nic.Name}: {nic.GetPhysicalAddress()}");
                    Console.WriteLine(nic.GetPhysicalAddress().ToString());
                    MacAdress = nic.GetPhysicalAddress().ToString();
                }
            }
        }

        private RelayCommand logInCommand;

        public ICommand LogInCommand
        {
            get
            {
                if (logInCommand == null)
                {
                    logInCommand = new RelayCommand(LogIn);
                }

                return logInCommand;
            }
        }

        private void LogIn(object commandParameter)
        {

        }

        private RelayCommand requestAccessCommand;

        public ICommand RequestAccessCommand
        {
            get
            {
                if (requestAccessCommand == null)
                {
                    requestAccessCommand = new RelayCommand(RequestAccess);
                }

                return requestAccessCommand;
            }
        }

        private void RequestAccess(object commandParameter)
        {
        }

    }
}
