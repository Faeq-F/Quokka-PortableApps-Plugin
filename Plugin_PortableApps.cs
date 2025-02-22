using Newtonsoft.Json;
using Quokka.ListItems;
using Quokka.PluginArch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WinCopies.Util;

namespace Plugin_PortableApps {

  /// <summary>
  ///  The Portable Apps Plugin
  /// </summary>
  public partial class PortableApps : Plugin {

    /// <summary>
    ///  <inheritdoc/>
    /// </summary>
    public override string PluggerName { get; set; } = "PortableApps";

    private static Settings pluginSettings = new();
    internal static Settings PluginSettings { get => pluginSettings; set => pluginSettings = value; }

    /// <summary>
    /// Loads the plugin's settings
    /// </summary>
    public PortableApps() {
      string fileName = Environment.CurrentDirectory + "\\PlugBoard\\Plugin_PortableApps\\Plugin\\settings.json";
      PluginSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(fileName))!;
      PluginSettings.PortableAppsDirectory = Path.GetFullPath(PluginSettings.PortableAppsDirectory);
    }

    private readonly List<ListItem> AllPortableApps = new();

    private static List<ListItem> RemoveBlacklistItems(List<ListItem> list) {
      foreach (string i in PluginSettings.BlackList) {
        list.RemoveAll(x => x.Name.Equals(i));
      }
      return list;
    }

    private List<ListItem> ProduceItems(string query) {
      List<ListItem> IdentifiedApps = new();
      //filtering apps
      foreach (ListItem app in AllPortableApps) {
        if (app.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                || FuzzySearch.LD(app.Name.Replace("Portable", "").Replace("portable", ""), query) < PluginSettings.FuzzySearchThreshold) {
          IdentifiedApps.Add(app);
        }
      }
      IdentifiedApps = RemoveBlacklistItems(IdentifiedApps);
      return IdentifiedApps;
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <param name="query">The app being searched for</param>
    /// <returns>List of PortableApps that possibly match what is being searched for</returns>
    public override List<ListItem> OnQueryChange(string query) {
      return ProduceItems(query);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns>The AllAppsSpecialCommand from plugin settings</returns>
    public override List<string> SpecialCommands() {
      List<string> SpecialCommand = new() {
        PluginSettings.AllAppsSpecialCommand
      };
      return SpecialCommand;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="command">The 'all apps' command (Since there is only 1 special command for this plugin)</param>
    /// <returns>All Apps sorted alphabetically + a shortcut to the portable apps folder</returns>
    public override List<ListItem> OnSpecialCommand(string command) {
      List<ListItem> AllList = new(AllPortableApps);
      AllList = AllList.OrderBy(x => x.Name).ToList();
      AllList.Insert(0, new PortableAppsFolderItem());
      AllList = RemoveBlacklistItems(AllList);
      return AllList;
    }

    /// <summary>
    /// <inheritdoc/>
    /// Creates the list of all portable apps
    /// </summary>
    public override void OnAppStartup() {
      if (Directory.Exists(PluginSettings.PortableAppsDirectory)) {
        var topLevelDirs = Directory.EnumerateDirectories(PluginSettings.PortableAppsDirectory, "*", SearchOption.TopDirectoryOnly);
        foreach (string dir in topLevelDirs) {
          foreach (string exe in Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".exe"))) {
            AllPortableApps.Add(new PortableAppsItem(exe));
          }
        }
      }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns>
    /// The PortableAppsSignifier from plugin settings
    /// </returns>
    public override List<string> CommandSignifiers() {
      return new List<string>() { pluginSettings.PortableAppsSignifier };
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="command">The PortableAppsSignifier (Since there is only 1 signifier for this plugin), followed by the app being searched for</param>
    /// <returns>List of PortableApps that possibly match what is being searched for</returns>
    public override List<ListItem> OnSignifier(string command) {
      return ProduceItems(command.Substring(pluginSettings.PortableAppsSignifier.Length));
    }

  }
}