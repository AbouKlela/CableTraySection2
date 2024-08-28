using CableTraySection.ViewModel;
using FireSharp.Config;
using FireSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using System.Windows.Markup;
using CableTraySection.View;

namespace CableTraySection.ViewModel
{
    public class ViewModelLogIn : ViewModelBase
    {
        public static event EventHandler RequestClose;
        public void OnRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
        FirebaseConfig config { get; set; }
        FirebaseClient client { get; set; }
        Dictionary<string, string> retrievedMacAddresses = new Dictionary<string, string>();

        bool login = false;
        public ViewModelLogIn()
        {

            config = new FirebaseConfig()
            {
                AuthSecret = "RsDTG2wsWcCs4ATd4mwUf0ZyNbhYy7wpnBzLzdSS",
                BasePath = "https://login-a3b34-default-rtdb.firebaseio.com/"
            };
            client = new FirebaseClient(config);
        }



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


        private RelayCommand loginCommand;
        public ICommand Logincommand
        {
            get
            {
                if (loginCommand == null)
                {
                    loginCommand = new RelayCommand(Login);
                }

                return loginCommand;
            }
        }

        private async void Login(object obj)
        {


            if (MacAdress != null)
            {
                if (client != null)
                {
                    var response = await client.GetTaskAsync("Users");
                    retrievedMacAddresses = response.ResultAs<Dictionary<string, string>>();
                    login = false;
                    foreach (var mac in retrievedMacAddresses)
                    {
                        if (mac.Value == MacAdress)
                        {
                            login = true;
                            OnRequestClose();
                            CTView view = new CTView();
                            view.Show();

                            break;
                        }
                    }

                    if (!login)
                    {
                        TaskDialog.Show("Access Denied", "You are not authorized to use this application");
                    }
                }
                else
                {
                    TaskDialog.Show("Connection Error", "Please, check your internet connection");
                } 
            }
            else
            {
                TaskDialog.Show("Error", "Please, get your MAC address first");
            }
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

        private async void RequestAccess(object commandParameter)
        {
            if (MacAdress != null)
            {

                await client.PushTaskAsync("Requests", MacAdress);
                TaskDialog.Show("Request Sent", "Your request has been sent to the administrator");
                OnRequestClose();
            }
            else
            {
                TaskDialog.Show("Error", "Please, get your MAC address first");
            }
        }

    }
}
