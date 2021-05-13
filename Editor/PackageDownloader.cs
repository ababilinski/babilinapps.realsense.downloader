using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;

namespace BabilinApps.RealSense.Downloader.Editor
{
/// <summary>
/// Unofficial Package Downloader - that downloads the official RealSense asset package from github 
/// </summary>
public class PackageDownloader : EditorWindow
{
    private static string _downloadLocation => Path.Combine(Application.temporaryCachePath, "Intel.RealSense.unitypackage");
    private const string GITHUB_DOWNLOAD_URL = "https://github.com/IntelRealSense/librealsense/releases/latest/download/Intel.RealSense.unitypackage";
    private const string PLUGIN_NOT_FOUND_KEY = "REALSENSE_PLUGIN_SEARCH_OPTION";

#region Package Not Found Text
    private const string PACKAGE_NOT_FOUND_TITLE = "RealSense Package Not Found";
    private const string PACKAGE_NOT_FOUND_MESSAGE = "The RealSense package was not not found in your project. Would you like to download it?";
    private const string PACKAGE_NOT_FOUND_OK = "Yes";
    private const string PACKAGE_NOT_FOUND_CANCEL = "No";
    private const string PACKAGE_NOT_FOUND_ALT = "No, Dont't Ask Again";
#endregion

#region Failed To Download Text
    private const string FAILED_TO_DOWNLOAD_TITLE = "Error Downloading Package";
    private const string FAILED_TO_DOWNLOAD_MESSAGE = "Error {0}.\nWould you like to try again?";
    private const string FAILED_TO_DOWNLOAD_OK = "Yes";
    private const string FAILED_TO_DOWNLOAD_CANCEL = "No";
    private const string FAILED_TO_DOWNLOAD_ALT = "Download Manually";
#endregion

#region Progress Bar
    private const string DOWNLOADING_TITLE = "RealSense Package Not Found";
    private const string DOWNLOADING_TITLE_MESSAGE = "RealSense Package Not Found";
#endregion

    [MenuItem("RealSense/Download Package From Github")]
    public static void DownloadRealSensePackageMenuItem()
    {
     
        EditorCoroutineUtility.StartCoroutineOwnerless(DownloadRealSensePackage());
    }

    /// <summary>
    /// Called when the RealSense assembly is not found by AutoRun.cs
    /// </summary>
    public static void OpenPluginNotFoundOptions()
    {
        var lookForPlugin = EditorPrefs.GetInt(PLUGIN_NOT_FOUND_KEY, 0);
        if (lookForPlugin < 1)
        {
            int PluginNotFoundOption = EditorUtility.DisplayDialogComplex(PACKAGE_NOT_FOUND_TITLE, PACKAGE_NOT_FOUND_MESSAGE,
                                                                          PACKAGE_NOT_FOUND_OK, PACKAGE_NOT_FOUND_CANCEL, PACKAGE_NOT_FOUND_ALT);
            switch (PluginNotFoundOption)
            {
                case 0: //Download
                    EditorCoroutineUtility.StartCoroutineOwnerless(DownloadRealSensePackage());
                    break;
                case 1: //Close
                    break;
                case 2: //Close, Dont't Ask Again
                    EditorPrefs.SetInt(PLUGIN_NOT_FOUND_KEY, 1);
                    break;
                
            }
        }
    }


    /// <summary>
    /// Downloads and imports the asset package 
    /// </summary>
    /// <returns></returns>
    public static IEnumerator DownloadRealSensePackage()
    {
        
        EditorUtility.DisplayProgressBar(DOWNLOADING_TITLE, DOWNLOADING_TITLE_MESSAGE, 0);
        UnityWebRequest unityWebRequest = new UnityWebRequest(GITHUB_DOWNLOAD_URL) {downloadHandler = new DownloadHandlerBuffer()};

        unityWebRequest.SendWebRequest();
        while (!unityWebRequest.isDone)
        {
            EditorUtility.DisplayProgressBar(DOWNLOADING_TITLE, DOWNLOADING_TITLE_MESSAGE, unityWebRequest.downloadProgress);
            yield return null;
        }
        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            int failedToDownloadOption = EditorUtility.DisplayDialogComplex(FAILED_TO_DOWNLOAD_TITLE,string.Format(FAILED_TO_DOWNLOAD_MESSAGE, unityWebRequest.error),
                                                                            FAILED_TO_DOWNLOAD_OK, FAILED_TO_DOWNLOAD_CANCEL,FAILED_TO_DOWNLOAD_ALT);

            switch (failedToDownloadOption)
            {
                case 0: //Try Again
                EditorCoroutineUtility.StartCoroutineOwnerless(DownloadRealSensePackage());
                break;
                case 1: //Stop
                break;
                case 2: //Download Manually
                Help.BrowseURL(GITHUB_DOWNLOAD_URL);
                break;
            }
            Debug.Log(unityWebRequest.error);
        }
        else
        {
          
            // Or retrieve results as binary data
            byte[] results = unityWebRequest.downloadHandler.data;
          

            File.WriteAllBytes(_downloadLocation, results);
            Debug.Log($"Downloaded File to {_downloadLocation}");
        

            AssetDatabase.ImportPackage(_downloadLocation, true);
            AssetDatabase.Refresh();
        }
     
        EditorUtility.ClearProgressBar();
    }

}
}