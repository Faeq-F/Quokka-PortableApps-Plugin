using Quokka.ListItems;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Plugin_PortableApps {
  /// <summary>
  /// The Item class for the plugin - represents a portable app
  /// </summary>
  internal class PortableAppsItem : ListItem {
    public string ExePath { get; set; }
    public string ExtraDetails { get; set; }

    public PortableAppsItem(string exePath) {
      ExePath = exePath;
      Name = Path.GetFileNameWithoutExtension(exePath);
      Description = exePath;
      ExtraDetails = FileVersionInfo.GetVersionInfo(exePath).LegalCopyright + "\n" + FileVersionInfo.GetVersionInfo(exePath).CompanyName + "\n" + FileVersionInfo.GetVersionInfo(exePath).FileVersion;
      Icon = Imaging.CreateBitmapSourceFromHIcon(System.Drawing.Icon.ExtractAssociatedIcon(exePath)!.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }

    public override void Execute() {
      Process.Start(Description);
      Application.Current.MainWindow.Close();
    }
  }
}
