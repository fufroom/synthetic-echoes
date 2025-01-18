using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

class CustomBuildPipeline : MonoBehaviour
{
    [MenuItem("Build/Build Linux", false, 10)]
    public static bool Linux()
    {
        Debug.Log("======== Starting Linux Build ========");
        var result = Build(BuildTarget.StandaloneLinux64);
        Debug.Log("======== Linux Build Completed ========");
        return result;
    }

    [MenuItem("Build/Send to Cabinet", false, 11)]
    public static void SendToCabinet()
    {
        Debug.Log("======== Sending to Cabinet ========");
        var scriptPath = Path.Combine(Application.dataPath, "Editor/send-to-cabinet.sh");

        if (!File.Exists(scriptPath))
        {
            Debug.LogError($"Shell script not found at: {scriptPath}");
            return;
        }

        ExecuteShellScript(scriptPath);
        Debug.Log("======== Send to Cabinet Completed ========");
    }

    private static bool Build(BuildTarget platform)
    {
        EditorUtility.DisplayProgressBar("Building Game", $"Starting build for {platform}", 0.1f);
        Debug.Log($"======== Starting Build for {platform} ========");

        var path = BuildPathForPlatform(platform);
        var scenes = GetScenePaths().ToArray();
        EditorUtility.DisplayProgressBar("Building Game", $"Building {platform}", 0.5f);

        var buildReport = BuildPipeline.BuildPlayer(scenes, path, platform, BuildOptions.None);

        EditorUtility.DisplayProgressBar("Building Game", $"Finishing build for {platform}", 0.9f);
        EditorUtility.ClearProgressBar();

        if (buildReport.summary.result == BuildResult.Succeeded)
        {
            CleanupAfterBuild(path);
            Debug.Log($"======== Build for {platform} Completed Successfully ========");
            return true;
        }
        else
        {
            Debug.LogError($"======== Build for {platform} Failed ========");
            return false;
        }
    }

    private static string BuildPathForPlatform(BuildTarget platform)
    {
        Debug.Log($"======== Determining Build Path for {platform} ========");
        var version = PlayerSettings.bundleVersion;
        var path = $"linux-builds/{PlayerSettings.productName}_{version}.x86_64";
        Debug.Log($"Build path: {path}");
        return path;
    }

    private static IEnumerable<string> GetScenePaths()
    {
        Debug.Log("======== Fetching Scene Paths ========");
        return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path);
    }

    private static void CleanupAfterBuild(string buildPath)
    {
        Debug.Log($"======== Cleaning up after build: {buildPath} ========");
        var gameName = PlayerSettings.productName;
        var debugFolder = Path.Combine(buildPath, $"{gameName}_BurstDebugInformation_DoNotShip");
        if (Directory.Exists(debugFolder))
        {
            try
            {
                Directory.Delete(debugFolder, true);
                Debug.Log("Deleted debug folder: " + debugFolder);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to delete debug folder: {ex.Message}");
            }
        }
    }

    private static void ExecuteShellScript(string scriptPath)
    {
        var process = new System.Diagnostics.Process()
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"\"{scriptPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            }
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (!string.IsNullOrEmpty(output))
        {
            Debug.Log("Shell command output: " + output);
        }

        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError("Shell command error: " + error);
        }
    }
}
