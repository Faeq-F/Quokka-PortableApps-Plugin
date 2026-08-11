using Quokka.ListItems;
using Quokka.PluginArch;
using System.Diagnostics;
using System.Windows;

namespace PluginPortableApps
{
  internal sealed class PortableAppsFolderItem : ListItem
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
        (string)Application.Current.Resources["FileManager"], '"' + PortableApps.PluginSettings.PortableAppsDirectory + '"'
      );
      Application.Current.MainWindow.Close();
    }
  }
}
