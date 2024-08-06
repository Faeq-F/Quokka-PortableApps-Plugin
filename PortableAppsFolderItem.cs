using Quokka;
using Quokka.ListItems;
using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace Plugin_PortableApps {
  internal class PortableAppsFolderItem : ListItem {

    public PortableAppsFolderItem() {
      Name = "Portable Apps Folder";
      Description = "Shortcut to the folder containing your portable apps";
      Icon = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\PlugBoard\\Plugin_PortableApps\\Plugin\\portableAppsFolder.png"));
    }

    public override void Execute() {
      Process.Start((string) App.Current.Resources["FileManager"], '"' + PortableApps.PluginSettings.PortableAppsDirectory + '"');
      App.Current.MainWindow.Close();
    }
  }
}
