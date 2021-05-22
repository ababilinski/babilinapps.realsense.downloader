using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace BabilinApps.RealSense.Downloader.Editor
{

    /// <summary>
    /// Runs when the asset database reloads to check if the Intel RealSense package has been imported
    /// </summary>
    [InitializeOnLoad]
    internal static class Autorun
    {
        private const string REALSENSE_DLL_PATH_KEY = "REALSENSE_DLL_PATH";
    
        private const string REALSENSE_DEFINES = "REALSENSE";



        private static bool cachedDllPathLoaded = false;
        private static bool cachedDllPathEmpty = true;

        private static string _dllPath = "";
        public static string AbsolutePath
        {
            get
            {
                return "../" + Application.dataPath;
            }
        }

        /// <summary>
        /// Determines whether the specified file exists relative to the root project
        /// </summary>
        /// <returns></returns>
        public static bool FileExistsInAbsolutePath(string path)
        {
            return File.Exists(AbsolutePath + path);
        }

        /// <summary>
        /// Check if the current define symbols contain a definition
        /// </summary>
        public static bool ContainsDefineSymbol(string symbol)
        {
            string definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            return allDefines.Contains(symbol);
        }

         /// <summary>
    /// Add define symbols as soon as Unity gets done compiling.
    /// </summary>
    public static void AddDefineSymbols(string[] symbols)
    {
      string definesString =
          PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
      List<string> allDefines = definesString.Split(';').ToList();
      allDefines.AddRange(symbols.Except(allDefines));
      PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                                                       string.Join(";", allDefines.ToArray()));
    }

    /// <summary>
    /// Remove define symbols as soon as Unity gets done compiling.
    /// </summary>
    public static void RemoveDefineSymbols(string[] symbols)
    {
      string definesString =
          PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
      List<string> allDefines = definesString.Split(';').ToList();

      for (int i = 0; i < symbols.Length; i++)
      {
        if (!allDefines.Contains(symbols[i]))
        {
          Debug.LogWarning($"Remove Defines Ignored. Symbol [{symbols[i]}] does not exists.");

        }
        else
        {
          allDefines.Remove(symbols[i]);
        }

      }
      PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                                                       string.Join(";", allDefines.ToArray()));
    }

    /// <summary>
    /// Add define symbol as soon as Unity gets done compiling.
    /// </summary>
    public static void AddDefineSymbol(string symbol)
    {
      string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
      List<string> allDefines = definesString.Split(';').ToList();
      if (allDefines.Contains(symbol))
      {
        Debug.LogWarning($"Add Defines Ignored. Symbol [{symbol}] already exists.");
        return;
      }

      allDefines.Add(symbol);
      PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                                                       string.Join(";", allDefines.ToArray()));
    }

    /// <summary>
    /// Remove define symbol as soon as Unity gets done compiling.
    /// </summary>
    public static void RemoveDefineSymbol(string symbol)
    {
      string definesString =
          PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
      List<string> allDefines = definesString.Split(';').ToList();
      if (!allDefines.Contains(symbol))
      {
        Debug.LogWarning($"Remove Defines Ignored. Symbol [{symbol}] does not exists.");

      }
      else
      {
        allDefines.Remove(symbol);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
                                                         string.Join(";", allDefines.ToArray()));
      }

    }


        static Autorun()
        {
           

            if (!cachedDllPathLoaded)
            {
                var projectPrefix = PlayerSettings.companyName + "." + PlayerSettings.productName;
                
                _dllPath = EditorPrefs.GetString(projectPrefix + ":" + REALSENSE_DLL_PATH_KEY, "");


                if (!string.IsNullOrWhiteSpace(_dllPath))
                {
                    cachedDllPathEmpty = false;
                }

                cachedDllPathLoaded = true;
            }

            bool hasFileAndSymbol = false;
            if (!cachedDllPathEmpty && File.Exists(_dllPath))
            {
                if (!ContainsDefineSymbol(REALSENSE_DEFINES))
                {
                    string[] files = Directory.GetFiles(Application.dataPath, "*.dll", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        if (file.Contains("RealSense.dll"))
                        {
                            _dllPath = file;
                            hasFileAndSymbol = true;
                            AddDefineSymbol(REALSENSE_DEFINES);
                            break;
                        }
                    }
                }
                else
                {
                    hasFileAndSymbol = true;
                }
            }
            else
            {
                if (ContainsDefineSymbol(REALSENSE_DEFINES))
                {
                    RemoveDefineSymbol(REALSENSE_DEFINES);
                }

                string[] files = Directory.GetFiles(Application.dataPath, "*.dll", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    if (file.Contains("RealSense.dll"))
                    {
                        _dllPath = file;
                        hasFileAndSymbol = true;
                        AddDefineSymbol(REALSENSE_DEFINES);
                        break;
                    }
                }
            }


            if (hasFileAndSymbol)
            { 
                var projectPrefix = PlayerSettings.companyName + "." + PlayerSettings.productName;
                EditorPrefs.SetString(projectPrefix+":"+ REALSENSE_DLL_PATH_KEY, _dllPath);
                cachedDllPathEmpty = false;

            }
            else
            {   
                if (!AssetDatabase.IsAssetImportWorkerProcess())
                {
                    PackageDownloader.OpenPluginNotFoundOptions();
                }
            }

       
        }

       
    }

}