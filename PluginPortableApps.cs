using Newtonsoft.Json;
using Quokka.ListItems;
using Quokka.PluginArch;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace PluginPortableApps
{

  /// <summary>
  ///  The Portable Apps Plugin
  /// </summary>
  public partial class PortableApps : Plugin
  {

    /// <summary>
    ///  <inheritdoc/>
    /// </summary>
    public override string PluginName { get; set; } = "PortableApps";

    private static readonly Lazy<Settings> _lazyPluginSettings = new(() =>
    {
      string fileName = Path.Combine(Environment.CurrentDirectory, "PlugBoard", "PluginPortableApps", "Plugin", "settings.json");
      if (File.Exists(fileName))
      {
        Settings settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(fileName)) ?? new Settings();
        settings.PortableAppsDirectory = Path.GetFullPath(settings.PortableAppsDirectory);
        return settings;
      }
      return new Settings();
    });

    internal static Settings PluginSettings => _lazyPluginSettings.Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="PortableApps"/> class.
    /// </summary>
    public PortableApps() { }

    internal static List<ListItem> AllPortableApps = new();

    private static Collection<ListItem> RemoveBlacklistItems(List<ListItem> list)
    {
      foreach (string i in PluginSettings.BlackList)
      {
        list.RemoveAll(x => x.Name.Equals(i, StringComparison.Ordinal));
      }
      return new Collection<ListItem>(list);
    }

    private static Collection<ListItem> ProduceItems(string query)
    {
      List<ListItem> IdentifiedApps = FuzzySearch.SearchAll(query, new Collection<string>(
        AllPortableApps.ConvertAll(static x => x.Name.ToLower(new CultureInfo("en-US"))
        .Replace("portable", "", StringComparison.OrdinalIgnoreCase))), PluginSettings.FuzzySearchThreshold)
        .Select(x => AllPortableApps[x.Index])
        .ToList();

      return RemoveBlacklistItems(IdentifiedApps);
    }

    /// <summary>
    /// <inheritdoc />
    /// </summary>
    /// <param name="query">The app being searched for</param>
    /// <returns>Collection of PortableApps that possibly match what is being searched for</returns>
    public override Collection<ListItem> OnQueryChange(string query)
    {
      return ProduceItems(query);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns>The AllAppsSpecialCommand from plugin settings</returns>
    public override Collection<string> SpecialCommands()
    {
      Collection<string> SpecialCommand = new() {
        PluginSettings.AllAppsSpecialCommand
      };
      return SpecialCommand;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="command">The 'all apps' command (Since there is only 1 special command for this plugin)</param>
    /// <returns>All Apps sorted alphabetically + a shortcut to the portable apps folder</returns>
    public override Collection<ListItem> OnSpecialCommand(string command)
    {
      List<ListItem> AllList = new(AllPortableApps);
      AllList = AllList.OrderBy(x => x.Name).ToList();
      AllList.Insert(0, new PortableAppsFolderItem());
      return new Collection<ListItem>(RemoveBlacklistItems(AllList));
    }

    /// <summary>
    /// <inheritdoc/>
    /// Creates the list of all portable apps
    /// </summary>
    public override void OnAppStartup()
    {
      if (Directory.Exists(PluginSettings.PortableAppsDirectory))
      {
        IEnumerable<string> topLevelDirs = Directory.EnumerateDirectories(PluginSettings.PortableAppsDirectory, "*", SearchOption.TopDirectoryOnly);
        foreach (string dir in topLevelDirs)
        {
          foreach (string app in Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly).Where(s => PluginSettings.Extensions.Any(ext => s.EndsWith('.' + ext, StringComparison.OrdinalIgnoreCase))))
          {
            AllPortableApps.Add(new PortableAppsItem(app));
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
    public override Collection<string> CommandSignifiers()
    {
      return new Collection<string>() { PluginSettings.PortableAppsSignifier };
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="command">The PortableAppsSignifier (Since there is only 1 signifier for this plugin), followed by the app being searched for</param>
    /// <returns>Collection of PortableApps that possibly match what is being searched for</returns>
    public override Collection<ListItem> OnSignifier(string command)
    {
      command ??= "";
      command = command.Substring(PluginSettings.PortableAppsSignifier.Length);
      return FuzzySearch.Sort(command, ProduceItems(command));
    }

  }
}
