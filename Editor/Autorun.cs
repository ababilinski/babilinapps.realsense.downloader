using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;


namespace BabilinApps.RealSense.Downloader.Editor
{

    /// <summary>
    /// Runs when the asset database reloads to check if the Intel RealSense package has been imported
    /// </summary>
    [InitializeOnLoad]
    internal static class Autorun
    {
        private const string REALSENSE_ASSEMBLY_PATH_KEY = "REALSENSE_ASSEMBLY_PATH";
        private const string REALSENSE_ASSEMBLY_NAME = "RealSense";
        private static bool cachedAssemblyPathLoaded = false;
        private static bool cachedAssemblyPathEmpty = true;
        private static string _assemblyPath = "";
    
        static Autorun()
        {
            if (!cachedAssemblyPathLoaded)
            {
                var projectPrefix = PlayerSettings.companyName + "." + PlayerSettings.productName;
                _assemblyPath = EditorPrefs.GetString(projectPrefix+":"+REALSENSE_ASSEMBLY_PATH_KEY, "");
                if (!string.IsNullOrWhiteSpace(_assemblyPath))
                {
                    cachedAssemblyPathEmpty = false;
                }
                cachedAssemblyPathLoaded = true;
            }

            if (!cachedAssemblyPathEmpty && File.Exists(_assemblyPath))
            { 
                return;
            }

            var assembly = GetAssemblyByName(REALSENSE_ASSEMBLY_NAME);
            if (assembly != null)
            { 
                var projectPrefix = PlayerSettings.companyName + "." + PlayerSettings.productName;
                _assemblyPath = assembly.Location;
                EditorPrefs.SetString(projectPrefix+":"+REALSENSE_ASSEMBLY_PATH_KEY, assembly.Location);
                cachedAssemblyPathEmpty = false;

            }
            else
            {   
                if (!AssetDatabase.IsAssetImportWorkerProcess())
                {
                    PackageDownloader.OpenPluginNotFoundOptions();
                }
            }

       
        }

        static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == name);
        }
    }

}