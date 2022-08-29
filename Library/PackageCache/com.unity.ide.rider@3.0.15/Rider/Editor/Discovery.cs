using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Win32;
using Packages.Rider.Editor.Util;
using Unity.CodeEditor;
using UnityEngine;

namespace Packages.Rider.Editor
{
  internal interface IDiscovery
  {
    CodeEditor.Installation[] PathCallback();
  }

  internal class Discovery : IDiscovery
  {
    public CodeEditor.Installation[] PathCallback()
    {
      var res = RiderPathLocator.GetAllRiderPaths()
        .Select(riderInfo => new CodeEditor.Installation
        {
          Path = riderInfo.Path,
          Name = riderInfo.Presentation
        })
        .ToList();

      var editorPath = RiderScriptEditor.CurrentEditor;
      if (RiderScriptEditor.IsRiderInstallation(editorPath) &&
          !res.Any(a => a.Path == editorPath) &&
          FileSystemUtil.EditorPathExists(editorPath))
      {
        // External editor manually set from custom location
        var info = new RiderPathLocator.RiderInfo(editorPath, false);
        var installation = new CodeEditor.Installation
        {
          Path = info.Path,
          Name = info.Presentation
        };
        res.Add(installation);
      }

      return res.ToArray();
    }
  }

  /// <summary>
  /// This code is a modified version of the JetBrains resharper-unity plugin listed here:
  /// https://github.com/JetBrains/resharper-unity/blob/master/unity/JetBrains.Rider.Unity.Editor/EditorPlugin/RiderPathLocator.cs
  /// </summary>
  internal static class RiderPathLocator
  {
#if !(UNITY_4_7 || UNITY_5_5)
    public static RiderInfo[] GetAllRiderPaths()
    {
      try
      {
        switch (SystemInfo.operatingSystemFamily)
        {
          case OperatingSystemFamily.Windows:
          {
            return CollectRiderInfosWindows();
          }

          case OperatingSystemFamily.MacOSX:
          {
            return CollectRiderInfosMac();
          }

          case OperatingSystemFamily.Linux:
          {
            return CollectAllRiderPathsLinux();
          }
        }
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }

      return new RiderInfo[0];
    }
#endif

#if RIDER_EDITOR_PLUGIN // can't be used in com.unity.ide.rider
    internal static RiderInfo[] GetAllFoundInfos(OperatingSystemFamilyRider operatingSystemFamily)
    {
      try
      {
        switch (operatingSystemFamily)
        {
          case OperatingSystemFamilyRider.Windows:
          {
            return CollectRiderInfosWindows();
          }
          case OperatingSystemFamilyRider.MacOSX:
          {
            return CollectRiderInfosMac();
          }
          case OperatingSystemFamilyRider.Linux:
          {
            return CollectAllRiderPathsLinux();
          }
        }
      }
      catch (Exception e)
      {
        Debug.LogException(e);
      }

      return new RiderInfo[0];
    }

    internal static string[] GetAllFoundPaths(OperatingSystemFamilyRider operatingSystemFamily)
    {
      return GetAllFoundInfos(operatingSystemFamily).Select(a=>a.Path).ToArray();
    }
#endif

    private static RiderInfo[] CollectAllRiderPathsLinux()
    {
      var installInfos = new List<RiderInfo>();
      var home = Environment.GetEnvironmentVariable("HOME");
      if (!string.IsNullOrEmpty(home))
      {
        var toolboxRiderRootPath = GetToolboxBaseDir();
        installInfos.AddRange(CollectPathsFromToolbox(toolboxRiderRootPath, "bin", "rider.sh", false)
          .Select(a => new RiderInfo(a, true)).ToList());

        //$Home/.local/share/applications/jetbrains-rider.desktop
        var shortcut = new FileInfo(Path.Combine(home, @".local/share/applications/jetbrains-rider.desktop"));

        if (shortcut.Exists)
        {
          var lines = File.ReadAllLines(shortcut.FullName);
          foreach (var line in lines)
          {
            if (!line.StartsWith("Exec=\""))
              continue;
            var path = line.Split('"').Where((item, index) => index == 1).SingleOrDefault();
            if (string.IsNullOrEmpty(path))
              continue;

            if (installInfos.Any(a => a.Path == path)) // avoid adding similar build as from toolbox
              continue;
            installInfos.Add(new RiderInfo(path, false));
          }
        }
      }

      // snap install
      var snapInstallPath = "/snap/rider/current/bin/rider.sh";
      if (new FileInfo(snapInstallPath).Exists)
        installInfos.Add(new RiderInfo(snapInstallPath, false));
      
      return installInfos.ToArray();
    }

    private static RiderInfo[] CollectRiderInfosMac()
    {
      var installInfos = new List<RiderInfo>();
      // "/Applications/*Rider*.app"
      var folder = new DirectoryInfo("/Applications");
      if (folder.Exists)
      {
        installInfos.AddRange(folder.GetDirectories("*Rider*.app")
          .Select(a => new RiderInfo(a.FullName, false))
          .ToList());
      }

      // /Users/user/Library/Application Support/JetBrains/Toolbox/apps/Rider/ch-1/181.3870.267/Rider EAP.app
      var toolboxRiderRootPath = GetToolboxBaseDir();
      var paths = CollectPathsFromToolbox(toolboxRiderRootPath, "", "Rider*.app", true)
        .Select(a => new RiderInfo(a, true));
      installInfos.AddRange(paths);

      return installInfos.ToArray();
    }

    private static RiderInfo[] CollectRiderInfosWindows()
    {
      var installInfos = new List<RiderInfo>();
      var toolboxRiderRootPath = GetToolboxBaseDir();
      var installPathsToolbox = CollectPathsFromToolbox(toolboxRiderRootPath, "bin", "rider64.exe", false).ToList();
      installInfos.AddRange(installPathsToolbox.Select(a => new RiderInfo(a, true)).ToList());
      
      var installPaths = new List<string>();
      const string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
      CollectPathsFromRegistry(registryKey, installPaths);
      const string wowRegistryKey = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
      CollectPathsFromRegistry(wowRegistryKey, installPaths);
      
      installInfos.AddRange(installPaths.Select(a => new RiderInfo(a, false)).ToList());

      return installInfos.ToArray();
    }

    private static string GetToolboxBaseDir()
    {
      switch (SystemInfo.operatingSystemFamily)
      {
        case OperatingSystemFamily.Windows:
        {
          var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
          return GetToolboxRiderRootPath(localAppData);
        }

        case OperatingSystemFamily.MacOSX:
        {
          var home = Environment.GetEnvironmentVariable("HOME");
          if (!string.IsNullOrEmpty(home))
          {
            var localAppData = Path.Combine(home, @"Library/Application Support");
            return  GetToolboxRiderRootPath(localAppData);
          }
          break;
        }

        case OperatingSystemFamily.Linux:
        {
          var home = Environment.GetEnvironmentVariable("HOME");
          if (!string.IsNullOrEmpty(home))
          {
            var localAppData = Path.Combine(home, @".local/share");
            return GetToolboxRiderRootPath(localAppData);
          }
          break;
        }
      }
      return string.Empty;
    }
    
    
    private static string GetToolboxRiderRootPath(string localAppData)
    {
      var toolboxPath = Path.Combine(localAppData, @"JetBrains/Toolbox");
      var settingsJson = Path.Combine(toolboxPath, ".settings.json");

      if (File.Exists(settingsJson))
      {
        var path = SettingsJson.GetInstallLocationFromJson(File.ReadAllText(settingsJson));
        if (!string.IsNullOrEmpty(path))
          toolboxPath = path;
      }

      var toolboxRiderRootPath = Path.Combine(toolboxPath, @"apps/Rider");
      return toolboxRiderRootPath;
    }
    
    internal static ProductInfo GetBuildVersion(string path)
    {
      var buildTxtFileInfo = new FileInfo(Path.Combine(path, GetRelativePathToBuildTxt()));
      var dir = buildTxtFileInfo.DirectoryName;
      if (!Directory.Exists(dir))
        return null;
      var buildVersionFile = new FileInfo(Path.Combine(dir, "product-info.json"));
      if (!buildVersionFile.Exists) 
        return null;
      var json = File.ReadAllText(buildVersionFile.FullName);
      return ProductInfo.GetProductInfo(json);
    }
    
    internal static Version GetBuildNumber(string path)
    {
      var file = new FileInfo(Path.Combine(path, GetRelativePathToBuildTxt()));
      if (!file.Exists) 
        return null;
      var text = File.ReadAllText(file.FullName);
      var index = text.IndexOf("-", StringComparison.Ordinal) + 1; // RD-191.7141.355
      if (index <= 0) 
        return null;
      
      var versionText = text.Substring(index);
      return Version.TryParse(versionText, out var v) ? v : null;
    }

    internal static bool GetIsToolbox(string path)
    {
      return Path.GetFullPath(path).StartsWith(Path.GetFullPath(GetToolboxBaseDir()));
    }

    private static string GetRelativePathToBuildTxt()
    {
      switch (SystemInfo.operatingSystemFamily)
      {
        case OperatingSystemFamily.Windows: 
        case OperatingSystemFamily.Linux:
          return "../../build.txt";
        case OperatingSystemFamily.MacOSX:
          return "Contents/Resources/build.txt";
      }
      throw new Exception("Unknown OS");
    }
    private static void CollectPathsFromRegistry(string registryKey, List<string> installPaths)
    {
      using (var key = Registry.CurrentUser.OpenSubKey(registryKey))
      {
        CollectPathsFromRegistry(installPaths, key);
      }
      using (var key = Registry.LocalMachine.OpenSubKey(registryKey))
      {
        CollectPathsFromRegistry(installPaths, key);
      }
    }

    private static void CollectPathsFromRegistry(List<string> installPaths, RegistryKey key)
    {
      if (key == null) return;
      foreach (var subkeyName in key.GetSubKeyNames())
      {
        using (var subkey = key.OpenSubKey(subkeyName))
        {
          var folderObject = subkey?.GetValue("InstallLocation");
          if (folderObject == null) continue;
          var folder = folderObject.ToString();
          if (folder.Length == 0) continue;
          var displayName = subkey.GetValue("DisplayName");
          if (displayName == null) continue;
          if (!displayName.ToString().Contains("Rider")) continue;
          try // possible "illegal characters in path"
          {
            var possiblePath = Path.Combine(folder, @"bin\rider64.exe"); 
            if (File.Exists(possiblePath))
              installPaths.Add(possiblePath);
          }
          catch (ArgumentException) { }
        }
      }
    }

    private static string[] CollectPathsFromToolbox(string toolboxRiderRootPath, string dirName, string searchPattern,
      bool isMac)
    {
      if (!Directory.Exists(toolboxRiderRootPath))
        return new string[0];

      var channelDirs = Directory.GetDirectories(toolboxRiderRootPath);
      var paths = channelDirs.SelectMany(channelDir =>
        {
          try
          {
            // use history.json - last entry stands for the active build https://jetbrains.slack.com/archives/C07KNP99D/p1547807024066500?thread_ts=1547731708.057700&cid=C07KNP99D
            var historyFile = Path.Combine(channelDir, ".history.json");
            if (File.Exists(historyFile))
            {
              var json = File.ReadAllText(historyFile);
              var build = ToolboxHistory.GetLatestBuildFromJson(json);
              if (build != null)
              {
                var buildDir = Path.Combine(channelDir, build);
                var executablePaths = GetExecutablePaths(dirName, searchPattern, isMac, buildDir);
                if (executablePaths.Any())
                  return executablePaths;
              }
            }

            var channelFile = Path.Combine(channelDir, ".channel.settings.json");
            if (File.Exists(channelFile))
            {
              var json = File.ReadAllText(channelFile).Replace("active-application", "active_application");
              var build = ToolboxInstallData.GetLatestBuildFromJson(json);
              if (build != null)
              {
                var buildDir = Path.Combine(channelDir, build);
                var executablePaths = GetExecutablePaths(dirName, searchPattern, isMac, buildDir);
                if (executablePaths.Any())
                  return executablePaths;
              }
            }

            // changes in toolbox json files format may brake the logic above, so return all found Rider installations
            return Directory.GetDirectories(channelDir)
              .SelectMany(buildDir => GetExecutablePaths(dirName, searchPattern, isMac, buildDir));
          }
          catch (Exception e)
          {
            // do not write to Debug.Log, just log it.
            Logger.Warn($"Failed to get RiderPath from {channelDir}", e);
          }

          return new string[0];
        })
        .Where(c => !string.IsNullOrEmpty(c))
        .ToArray();
      return paths;
    }

    private static string[] GetExecutablePaths(string dirName, string searchPattern, bool isMac, string buildDir)
    {
      var folder = new DirectoryInfo(Path.Combine(buildDir, dirName));
      if (!folder.Exists)
        return new string[0];

      if (!isMac)
        return new[] {Path.Combine(folder.FullName, searchPattern)}.Where(File.Exists).ToArray();
      return folder.GetDirectories(searchPattern).Select(f => f.FullName)
        .Where(Directory.Exists).ToArray();
    }

    // Disable the "field is never assigned" compiler warning. We never assign it, but Unity does.
    // Note that Unity disable this warning in the generated C# projects
#pragma warning disable 0649
    
    [Serializable]
    class SettingsJson
    {
      // ReSharper disable once InconsistentNaming
      public string install_location;
      
      [CanBeNull]
      public static string GetInstallLocationFromJson(string json)
      {
        try
        {
#if UNITY_4_7 || UNITY_5_5
          return JsonConvert.DeserializeObject<SettingsJson>(json).install_location;
#else
          return JsonUtility.FromJson<SettingsJson>(json).install_location;
#endif
        }
        catch (Exception)
        {
          Logger.Warn($"Failed to get install_location from json {json}");
        }

        return null;
      }
    }

    [Serializable]
    class ToolboxHistory
    {
      public List<ItemNode> history;

      [CanBeNull]
      public static string GetLatestBuildFromJson(string json)
      {
        try
        {
#if UNITY_4_7 || UNITY_5_5
          return JsonConvert.DeserializeObject<ToolboxHistory>(json).history.LastOrDefault()?.item.build;
#else
          return JsonUtility.FromJson<ToolboxHistory>(json).history.LastOrDefault()?.item.build;
#endif
        }
        catch (Exception)
        {
          Logger.Warn($"Failed to get latest build from json {json}");
        }

        return null;
      }
    }

    [Serializable]
    class ItemNode
    {
      public BuildNode item;
    }

    [Serializable]
    class BuildNode
    {
      public string build;
    }

    [Serializable]
    internal class ProductInfo
    {
      public string version;
      public string versionSuffix;

      [CanBeNull]
      internal static ProductInfo GetProductInfo(string json)
      {
        try
        {
          var productInfo = JsonUtility.FromJson<ProductInfo>(json);
          return productInfo;
        }
        catch (Exception)
        {
          Logger.Warn($"Failed to get version from json {json}");
        }

        return null;
      }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    [Serializable]
    class ToolboxInstallData
    {
      // ReSharper disable once InconsistentNaming
      public ActiveApplication active_application;

      [CanBeNull]
      public static string GetLatestBuildFromJson(string json)
      {
        try
        {
#if UNITY_4_7 || UNITY_5_5
          var toolbox = JsonConvert.DeserializeObject<ToolboxInstallData>(json);
#else
          var toolbox = JsonUtility.FromJson<ToolboxInstallData>(json);
#endif
          var builds = toolbox.active_application.builds;
          if (builds != null && builds.Any())
            return builds.First();
        }
        catch (Exception)
        {
          Logger.Warn($"Failed to get latest build from json {json}");
        }

        return null;
      }
    }

    [Serializable]
    class ActiveApplication
    {
      // ReSharper disable once InconsistentNaming
      public List<string> builds;
    }

#pragma warning restore 0649

    internal struct RiderInfo
    {
      public bool IsToolbox;
      public string Presentation;
      public Version BuildNumber;
      public ProductInfo ProductInfo;
      public string Path;

      public RiderInfo(string path, bool isToolbox)
      {
        if (path == RiderScriptEditor.CurrentEditor)
        {
          RiderScriptEditorData.instance.Init();
          BuildNumber = RiderScriptEditorData.instance.editorBuildNumber.ToVersion();
          ProductInfo = RiderScriptEditorData.instance.productInfo;
        }
        else
        {
          BuildNumber = GetBuildNumber(path);
          ProductInfo = GetBuildVersion(path);
        }
        Path = new FileInfo(path).FullName; // normalize separators
        var presentation = $"Rider {BuildNumber}";

        if (ProductInfo != null && !string.IsNullOrEmpty(ProductInfo.version))
        {
          var suffix = string.IsNullOrEmpty(ProductInfo.versionSuffix) ? "" : $" {ProductInfo.versionSuffix}";
          presentation = $"Rider {ProductInfo.version}{suffix}";
        }

        if (isToolbox)
          presentation += " (JetBrains Toolbox)";

        Presentation = presentation;
        IsToolbox = isToolbox;
      }
    }

    private static class Logger
    {
      internal static void Warn(string message, Exception e = null)
      {
#if RIDER_EDITOR_PLUGIN // can't be used in com.unity.ide.rider
        Log.GetLog(typeof(RiderPathLocator).Name).Warn(message);
        if (e != null) 
          Log.GetLog(typeof(RiderPathLocator).Name).Warn(e);
#else
        Debug.LogError(message);
        if (e != null)
          Debug.LogException(e);
#endif
      }
    }
  }
}