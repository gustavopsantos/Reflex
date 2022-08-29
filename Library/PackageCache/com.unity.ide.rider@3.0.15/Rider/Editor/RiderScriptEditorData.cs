using System;
using System.Linq;
using Packages.Rider.Editor.Util;
using Rider.Editor.Util;
using UnityEditor;
using UnityEngine;

namespace Packages.Rider.Editor
{
  internal class RiderScriptEditorData : ScriptableSingleton<RiderScriptEditorData>
  {
    [SerializeField] internal bool hasChanges = true; // activeBuildTargetChanged has changed
    [SerializeField] internal bool shouldLoadEditorPlugin;
    [SerializeField] internal bool initializedOnce;
    [SerializeField] internal SerializableVersion editorBuildNumber;
    [SerializeField] internal SerializableVersion prevEditorBuildNumber;
    [SerializeField] internal RiderPathLocator.ProductInfo productInfo;
    [SerializeField] internal string[] activeScriptCompilationDefines;

    public void Init()
    {
      if (editorBuildNumber == null)
      {
        Invalidate(RiderScriptEditor.CurrentEditor);
      }
    }

    public void InvalidateSavedCompilationDefines()
    {
      activeScriptCompilationDefines = EditorUserBuildSettings.activeScriptCompilationDefines;
    }
    
    public bool HasChangesInCompilationDefines()
    {
      if (activeScriptCompilationDefines == null)
        return false;
      
      return !EditorUserBuildSettings.activeScriptCompilationDefines.SequenceEqual(activeScriptCompilationDefines);
    }

    public void Invalidate(string editorInstallationPath, bool shouldInvalidatePrevEditorBuildNumber = false)
    {
      var riderBuildNumber = RiderPathLocator.GetBuildNumber(editorInstallationPath);
      editorBuildNumber = riderBuildNumber.ToSerializableVersion();
      if (shouldInvalidatePrevEditorBuildNumber)
        prevEditorBuildNumber = editorBuildNumber;
      productInfo = RiderPathLocator.GetBuildVersion(editorInstallationPath);
      if (riderBuildNumber == null) // if we fail to parse for some reason
        shouldLoadEditorPlugin = true;

      shouldLoadEditorPlugin = riderBuildNumber >= new Version("191.7141.156");

      if (RiderPathUtil.IsRiderDevEditor(editorInstallationPath))
      {
        shouldLoadEditorPlugin = true;
        editorBuildNumber = new SerializableVersion(new Version("999.999.999.999"));
      }
    }
  }
}