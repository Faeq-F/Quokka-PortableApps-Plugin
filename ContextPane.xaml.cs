using Quokka;
using Quokka.ListItems;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Plugin_PortableApps {

  /// <summary>
  /// The context pane for PortableAppItems.
  /// </summary>
  public partial class ContextPane : ItemContextPane {

    private readonly PortableAppsItem? Item;

    /// <summary>
    /// Grabs details about the selected app and does nothing if the selected item was the PortableAppsFolderItem
    /// </summary>
    public ContextPane() {
      InitializeComponent();
      try {
        this.Item = (PortableAppsItem) ( (SearchWindow) Application.Current.MainWindow ).SelectedItem!;
      } catch (InvalidCastException) {//Used to handle the PortableAppsFolderItem
        base.ReturnToSearch();
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
    protected override void Page_KeyDown(object sender, KeyEventArgs e) {
      ButtonsListView.Focus();
      switch (e.Key) {
        case Key.Enter:
          if (( ButtonsListView.SelectedIndex == -1 )) ButtonsListView.SelectedIndex = 0;
          Grid CurrentItem = (Grid) ButtonsListView.SelectedItem;
          Button CurrentButton = (Button) ( (Grid) CurrentItem!.Children[1] ).Children[0];
          CurrentButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
          break;
        case Key.Down:
          if (( ButtonsListView.SelectedIndex == -1 )) {
            ButtonsListView.SelectedIndex = 1;
          } else if (ButtonsListView.SelectedIndex == ButtonsListView.Items.Count - 1) {
            ButtonsListView.SelectedIndex = 0;
          } else {
            ButtonsListView.SelectedIndex++;
          }
          ButtonsListView.ScrollIntoView(ButtonsListView.SelectedItem);
          break;
        case Key.Up:
          if (( ButtonsListView.SelectedIndex == -1 ) || ( ButtonsListView.SelectedIndex == 0 )) {
            ButtonsListView.SelectedIndex = ButtonsListView.Items.Count - 1;
          } else {
            ButtonsListView.SelectedIndex--;
          }
          ButtonsListView.ScrollIntoView(ButtonsListView.SelectedItem);
          break;
        case var value when value == (System.Windows.Input.Key) App.Current.Resources["ContextPaneKey"]:
          base.ReturnToSearch();
          break;
        default:
          return;
      }
      e.Handled = true;
    }

    private void OpenApp(object sender, RoutedEventArgs e) {
      Item!.Execute();
    }

    private void RunAsAdmin(object sender, RoutedEventArgs e) {
      Process proc = new Process();
      proc.StartInfo.FileName = Item!.Description;
      proc.StartInfo.UseShellExecute = true;
      proc.StartInfo.Verb = "runas";
      proc.Start();
      App.Current.MainWindow.Close();
    }

    private void OpenContainingFolder(object sender, RoutedEventArgs e) {
      using Process folderopener = new();
      folderopener.StartInfo.FileName = (string) App.Current.Resources["FileManager"];
      folderopener.StartInfo.Arguments = '"' + Item!.Description.Remove(Item.Description.LastIndexOf('\\')) + '"';
      folderopener.Start();
      App.Current.MainWindow.Close();
    }
  }
}
