using Quokka;
using Quokka.ListItems;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Keys = System.Windows.Forms.Keys;

namespace PluginPortableApps
{

  /// <summary>
  /// The context pane for PortableAppItems.
  /// </summary>
  public partial class ContextPane : ItemContextPane
  {

    private readonly PortableAppsItem? Item;

    /// <summary>
    /// Grabs details about the selected app and does nothing if the selected item was the PortableAppsFolderItem
    /// </summary>
    public ContextPane()
    {
      InitializeComponent();
      try
      {
        Item = (PortableAppsItem)((SearchWindow)Application.Current.MainWindow).SelectedItem!;
      }
      catch (InvalidCastException)
      {//Used to handle the PortableAppsFolderItem
        ReturnToSearch();
        return;
      }
      DetailsImage.Source = Item.Icon;
      NameText.Text = Item.Name;
      DescText.Text = Item.Description;
      ExtraDetails.Text = Item.ExtraDetails;
    }

    /// <summary>
    /// <inheritdoc/><br />
    /// Up and down keys select list items and the enter key executes the item's action
    /// </summary>
    /// <param name="sender"><inheritdoc/></param>
    /// <param name="e"><inheritdoc/></param>
    protected override void PageKeyDown(object sender, KeyEventArgs e)
    {
      if (e != null)
      {
        ButtonsListView.Focus();
        switch (e.Key)
        {
          case Key.Enter:
            if (ButtonsListView.SelectedIndex == -1)
            {
              ButtonsListView.SelectedIndex = 0;
            }
            Grid CurrentItem = (Grid)ButtonsListView.SelectedItem;
            Button CurrentButton = (Button)((Grid)CurrentItem.Children[1]).Children[0];
            CurrentButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            break;
          case Key.Down:
            if (ButtonsListView.SelectedIndex == -1)
            {
              ButtonsListView.SelectedIndex = 1;
            }
            else if (ButtonsListView.SelectedIndex == ButtonsListView.Items.Count - 1)
            {
              ButtonsListView.SelectedIndex = 0;
            }
            else
            {
              ButtonsListView.SelectedIndex++;
            }
            ButtonsListView.ScrollIntoView(ButtonsListView.SelectedItem);
            break;
          case Key.Up:
            if (ButtonsListView.SelectedIndex is -1 or 0)
            {
              ButtonsListView.SelectedIndex = ButtonsListView.Items.Count - 1;
            }
            else
            {
              ButtonsListView.SelectedIndex--;
            }
            ButtonsListView.ScrollIntoView(ButtonsListView.SelectedItem);
            break;
          case var value when value == KeyInterop.KeyFromVirtualKey((int)(Keys)Application.Current.Resources["ContextPaneKey"]):
            ReturnToSearch();
            break;
          default:
            return;
        }
        e.Handled = true;
      }
    }

    private void OpenApp(object sender, RoutedEventArgs e)
    {
      Item!.Execute();
    }

    private void RunAsAdmin(object sender, RoutedEventArgs e)
    {
      using (Process proc = new())
      {
        proc.StartInfo.FileName = Item!.Description;
        proc.StartInfo.UseShellExecute = true;
        proc.StartInfo.Verb = "runas";
        proc.Start();
      }

      Application.Current.MainWindow.Close();
    }

    private void OpenContainingFolder(object sender, RoutedEventArgs e)
    {
      using Process folderopener = new();
      folderopener.StartInfo.FileName = (string)Application.Current.Resources["FileManager"];
      folderopener.StartInfo.Arguments = '"' + Item!.Description.Remove(Item.Description.LastIndexOf('\\')) + '"';
      folderopener.Start();
      Application.Current.MainWindow.Close();
    }
  }
}
