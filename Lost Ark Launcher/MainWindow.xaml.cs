using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Lost_Ark_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string BasePath;
        const string ConfigFile = @"\EFGame\Config\UserOption.xml";
        const string ExeFile = @"Binaries\Win64\Launch_Game.exe";
        const string XmlRegionPath = @"/UserOption/SaveAccountOptionData/RegionID";
        const int SteamGameId = 1599340;

        public static Dictionary<string, string> Regions = new Dictionary<string, string> {
            {"Europe", "CE"},
            {"North America East", "EA" },
            {"North America West", "WA" },
            {"South America", "SA" },
        };

        internal LostArkSettings Config { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            SetupFilePaths();
            SetRegionChoices();
        }

        private void SetupFilePaths()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App {SteamGameId}"))
            {
                if (key != null)
                {
                    BasePath = (string)key.GetValue("InstallLocation");
                    if (BasePath != null)
                    {
                        Config = new LostArkSettings(System.IO.Path.Combine(BasePath + ConfigFile));
                    }
                }
            }
        }

        private void SetRegionChoices()
        {
            if (Config == null) return;

            var activeRegion = Config.GetXmlPathValue(XmlRegionPath);
            RegionChoice.Items.Clear();
            foreach (var entry in Regions)
            {
                RegionChoice.Items.Add(entry.Key);
                if (activeRegion == entry.Value)
                {
                    RegionChoice.SelectedItem = entry.Key;
                }
            }
        }

        private void RegionChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var key = RegionChoice.SelectedItem as string;
            if (key != null && Regions.ContainsKey(key)) {
                var val = Regions[key];
                if (val != null)
                    Config.SetXmlPath(XmlRegionPath, val);
            }
        }

        private void LaunchGameButton_Click(object sender, RoutedEventArgs e)
        {
            var psinfo = new ProcessStartInfo();
            psinfo.UseShellExecute = true;
            psinfo.FileName = $"steam://rungameid/{SteamGameId}";
            var p =  Process.Start(psinfo);
            p.Exited += P_Exited;
            LaunchGameButton.IsEnabled = false;
            LaunchGameButton.Content = "Game started";
        }

        private void P_Exited(object? sender, EventArgs e)
        {
            LaunchGameButton.Content = "Launch game";
            LaunchGameButton.IsEnabled=true;
        }
    }
}
