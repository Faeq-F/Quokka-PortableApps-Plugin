namespace PluginPortableApps
{

  /// <summary>
  ///   The settings for this (Portable Apps) plugin
  /// </summary>
  public class Settings
  {

    /// <summary>
    ///   The directory in which portable apps are saved
    ///   (defaults to "\PortableApps\")
    /// </summary>
    public string PortableAppsDirectory { get; set; } = "\\PortableApps\\";

    /// <summary>
    ///   The command to show all Portable Apps found
    ///   (defaults to 'AllPortableApps')
    /// </summary>
    public string AllAppsSpecialCommand { get; set; } = "AllPortableApps";

    /// <summary>
    /// The command signifier to show only portable apps (defaults to "portapp ")<br />
    /// Using this signifier does not change the output of this plugin, it only
    /// ensures that no other plugins' results are included in the search window results list
    /// </summary>
    public string PortableAppsSignifier { get; set; } = "portapp ";

    /// <summary>
    ///   List of application names to not show (defaults to
    ///   empty - all apps can be shown)
    /// </summary>
    public List<string> BlackList { get; set; } = new List<string>(Array.Empty<string>());

    /// <summary>
    ///   The threshold for when to consider an application
    ///   name (without the text 'portable') is similar enough
    ///   to the query for it to be displayed (defaults to 60).
    ///   The larger the number, the more similar it needs to be.
    /// </summary>
    public int FuzzySearchThreshold { get; set; } = 60;
  }
}
