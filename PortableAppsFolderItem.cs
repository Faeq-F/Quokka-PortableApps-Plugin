using Quokka;
using Quokka.ListItems;
using Quokka.PluginArch;
using System.Diagnostics;

namespace PluginPortableApps
{
  internal class PortableAppsFolderItem : ListItem
  {

    public PortableAppsFolderItem()
    {
      Name = "Portable Apps Folder";
      Description = "Shortcut to the folder containing your portable apps";
      Icon = IconCache.GetOrAdd(
        Environment.CurrentDirectory + "\\PlugBoard\\PluginPortableApps\\Plugin\\portableAppsFolder.png"
      );
    }

    public override void Execute()
    {
      Process.Start(
        (string)App.Current.Resources["FileManager"], '"' + PortableApps.PluginSettings.PortableAppsDirectory + '"'
      );
      App.Current.MainWindow.Close();
    }
  }
}
