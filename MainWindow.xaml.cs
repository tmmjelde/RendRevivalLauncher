using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.IO.Pipes;

namespace RendRevivalLauncher
{
    /// <summary>
    /// Todo
    /// Add a check for the hacked dll file
    /// Add server browser capability
    /// Add UDP serverlist testing
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static RegistryKey BaseKey = Registry.CurrentUser;
        static RegistryKey HKLM = Registry.LocalMachine;
        static string subFolderPath = "SOFTWARE\\RendRevivalLauncher";

        //Now write Read and Write method
        public static void Registry_Write(string ValueName, string ValueData)
        {
            RegistryKey RegKey = BaseKey;
            RegistryKey subKey = RegKey.CreateSubKey(subFolderPath);
            subKey.SetValue(ValueName, ValueData);
        }
        public static string Registry_Read(string ValueName)
        {
            RegistryKey RegKey = BaseKey;
            RegistryKey subKey = RegKey.OpenSubKey(subFolderPath);
            if (subKey != null)
            {
                if (subKey.GetValue(ValueName) != null)
                {
                    return subKey.GetValue(ValueName).ToString();
                }
                else
                {
                    return "";
                }
            } else
            {
                return "";
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            ComboboxFaction.Items.Add("Revenant");
            ComboboxFaction.Items.Add("Covenant");
            ComboboxFaction.Items.Add("Order");
            ComboboxFaction.SelectedItem = "Covenant";
            
            ComboBoxMultihomeIP.ItemsSource = DisplayIPAddresses();
            GetSettings();
        }

        private void GetLaunchParameters()
        {
            string launchparameters = "";
            if ((bool)CheckBoxNoEAC.IsChecked) { launchparameters = launchparameters + " -NoEAC"; }
            if ((bool)CheckBoxlangame.IsChecked) { launchparameters = launchparameters + " -langame"; }
            if ((bool)CheckBoxplayername.IsChecked) { launchparameters = launchparameters + " -playername=" + TextBoxPlayername.Text; }
            if ((bool)CheckBoxfaction.IsChecked) {
                if (ComboboxFaction.SelectedItem.ToString()== "Revenant"){launchparameters = launchparameters + " -faction=1";}
                else if (ComboboxFaction.SelectedItem.ToString() == "Covenant") { launchparameters = launchparameters + " -faction=2"; }
                else if (ComboboxFaction.SelectedItem.ToString() == "Order") { launchparameters = launchparameters + " -faction=3"; }
            }
            if ((bool)CheckBoxserver.IsChecked){
                //if (TextBoxServerIP.Text != null) { launchparameters = launchparameters + " -server=" + TextBoxServerIP.Text; }
            }
            if ((bool)CheckBoxmultihome.IsChecked)
            {
                if (ComboBoxMultihomeIP.SelectedItem != null) { launchparameters = launchparameters + " -multihome=" + ComboBoxMultihomeIP.SelectedItem; }
            }
            

            

            TextBoxLaunchParameters.Text = launchparameters;
        }
        public static void PipeSend(string msg)
        {
            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream("PipeOwO"))
            {
                pipeStream.Connect();
                using (StreamWriter sw = new StreamWriter(pipeStream))
                {
                    sw.AutoFlush = true;
                    sw.WriteLine(msg);
                }
            }
        }
        public static List<System.Net.IPAddress> DisplayIPAddresses()
        {
            List<System.Net.IPAddress> returnAddress = new List<System.Net.IPAddress>();
            // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection)
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface network in networkInterfaces)
            {
                // Read the IP configuration for each network
                IPInterfaceProperties properties = network.GetIPProperties();

                if ( network.OperationalStatus == OperationalStatus.Up &&
                       !network.Description.ToLower().Contains("pseudo"))
                {
                    // Each network interface may have multiple IP addresses
                    foreach (IPAddressInformation address in properties.UnicastAddresses)
                    {
                        // We're only interested in IPv4 addresses for now
                        if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;

                        // Ignore loopback addresses (e.g., 127.0.0.1)
                        if (IPAddress.IsLoopback(address.Address))
                            continue;

                        returnAddress.Add(address.Address);
                        
                    }
                }
            }

            return returnAddress;
        }
        private void GetSettings()
        {
            var playername = Registry_Read("Playername");
            if (playername != "")
            {
                TextBoxPlayername.Text = playername;
            }
            var ClientExeFilePath = Registry_Read("ClientExeFilePath");
            if (ClientExeFilePath != "")
            {
                TextBoxClientExeFile.Text = ClientExeFilePath;
            }
            var ServerIP = Registry_Read("ServerIP");
            if (ServerIP != "")
            {
                TextBoxServerIP.Text = ServerIP;
            }
            var ServerPort = Registry_Read("ServerPort");
            if (ServerPort != "")
            {
                TextBoxServerPort.Text = ServerPort;
            }
            var FactionSelection = Registry_Read("FactionSelection");
            if (FactionSelection != "")
            {
                ComboboxFaction.SelectedItem = FactionSelection;
            }
            var NoEAC = Registry_Read("NoEAC");
            if (NoEAC != "")
            {
                CheckBoxNoEAC.IsChecked = Convert.ToBoolean(NoEAC);
            }
            var langame = Registry_Read("langame");
            if (langame != "")
            {
                CheckBoxlangame.IsChecked = Convert.ToBoolean(langame);
            }
            var faction = Registry_Read("faction");
            if (faction != "")
            {
                CheckBoxfaction.IsChecked = Convert.ToBoolean(faction);
            }
            var server = Registry_Read("server");
            if (server != "")
            {
                CheckBoxserver.IsChecked = Convert.ToBoolean(server);
            }
            var port = Registry_Read("port");
           
            var multihome = Registry_Read("multihome");
            if (multihome != "")
            {
                CheckBoxmultihome.IsChecked = Convert.ToBoolean(multihome);
            }
            var multihomeip = Registry_Read("multihomeip");
            if (multihomeip != "")
            {
                if (ComboBoxMultihomeIP.Items.ToString().Contains(multihomeip.ToString())){
                    ComboBoxMultihomeIP.SelectedItem = multihomeip; //This won't work because I can't select based on string? Dont know..
                }
            }
            var Playernamechecked = Registry_Read("Playernamechecked");
            if (Playernamechecked != "")
            {
                CheckBoxplayername.IsChecked = Convert.ToBoolean(Playernamechecked);
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Registry_Write("Playername", TextBoxPlayername.Text.ToString());
            Registry_Write("Playernamechecked", CheckBoxplayername.IsChecked.ToString());
            Registry_Write("ClientExeFilePath", TextBoxClientExeFile.Text.ToString());
            Registry_Write("ServerIP", TextBoxServerIP.Text.ToString());
            Registry_Write("ServerPort", TextBoxServerPort.Text.ToString());
            Registry_Write("Faction", ComboboxFaction.SelectedItem.ToString());
            Registry_Write("NoEAC", CheckBoxNoEAC.IsChecked.ToString());
            Registry_Write("langame", CheckBoxlangame.IsChecked.ToString());
            Registry_Write("faction", CheckBoxfaction.IsChecked.ToString());
            Registry_Write("server", CheckBoxserver.IsChecked.ToString());
            Registry_Write("multihome", CheckBoxmultihome.IsChecked.ToString());
            //Registry_Write("multihomeip", ComboBoxMultihomeIP.SelectedItem.ToString()); This won't work if nothing is selected in the dropdown.
        }

        private void ButtonAutodetect_Click(object sender, RoutedEventArgs e)
        {

            var libraries = GetSteamLibs();
            foreach (var library in libraries)
            {
                var exefile = System.IO.Path.Combine(library, @"steamapps\common\Rend\RendClient.exe");
                if (File.Exists(exefile))
                {
                    TextBoxClientExeFile.Text = exefile;
                }
            }

            //RendServer appmanifest_550790.acf
            //RendClient appmanifest_547860.acf


        }

        static string GetSteamPath()
        {
            RegistryKey RegKey = HKLM;
            string subFolderPathSteam32 = "SOFTWARE\\Valve\\Steam";
            RegistryKey subKey32 = RegKey.OpenSubKey(subFolderPathSteam32);
            string subFolderPathSteam64 = "SOFTWARE\\WOW6432Node\\Valve\\Steam";
            RegistryKey subKey64 = RegKey.OpenSubKey(subFolderPathSteam64);
            if (subKey32 != null)
            {
                var SteamPath = subKey32.GetValue("InstallPath").ToString();
                return SteamPath;
            }
            else if (subKey64 != null)
            {
                var SteamPath = subKey64.GetValue("InstallPath").ToString();
                return SteamPath;
            } else
            {
                return null;
            }
        }

        static List<string> GetSteamLibs()
        {

            var steamPath = GetSteamPath();
            var libraries = new List<string>() { steamPath };

            var listFile = System.IO.Path.Combine(steamPath, @"steamapps\libraryfolders.vdf");
            var lines = File.ReadAllLines(listFile);
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"""(?<path>\w:\\\\.*)""");
                if (match.Success)
                {
                    var path = match.Groups["path"].Value.Replace(@"\\", @"\");
                    if (Directory.Exists(path))
                    {
                        libraries.Add(path);
                    }
                }
            }
            return libraries;
        }

        private void ButtonLaunch_Click(object sender, RoutedEventArgs e)
        {
            string filename = TextBoxClientExeFile.Text;
            string Parameters = TextBoxLaunchParameters.Text;
            var proc = System.Diagnostics.Process.Start(filename, Parameters);
        }

        private void CheckBoxNoEAC_Checked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void CheckBoxNoEAC_Unchecked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void CheckBoxlangame_Checked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void CheckBoxlangame_Unchecked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void CheckBoxmultihome_Checked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
            ComboBoxMultihomeIP.ItemsSource = DisplayIPAddresses();
        }

        private void CheckBoxplayername_Checked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void CheckBoxplayername_Unchecked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void TextBoxPlayername_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void CheckBoxfaction_Unchecked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void CheckBoxserver_Unchecked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void CheckBoxport_Unchecked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void CheckBoxfaction_Checked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void CheckBoxserver_Checked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void CheckBoxport_Checked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void CheckBoxmultihome_Unchecked(object sender, RoutedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void ComboboxFaction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void TextBoxServerIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void TextBoxServerPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void ComboBoxMultihomeIP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetLaunchParameters();
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            //string<TChar> converted = "connect " + TextBoxServerIP.Text + ":" + TextBoxServerPort.Text;

            PipeSend("connect " + TextBoxServerIP.Text + ":" + TextBoxServerPort.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PipeSend(tbcustompipe.Text); 
        }
        // <summary>
        // </summary>
        // <param name="sender"></param>
        // <param name="e"></param>
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "RendClient.exe|RendClient.exe";

            if (openFileDialog.ShowDialog() == true)
                TextBoxClientExeFile.Text = openFileDialog.FileName;
        }
    }
}
