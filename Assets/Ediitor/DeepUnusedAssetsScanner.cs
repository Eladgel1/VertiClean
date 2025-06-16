using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class DeepUnusedAssetsScanner : EditorWindow {
    private Vector2 scroll;
    private List<(string path, long size)> unusedAssets = new List<(string, long)>();

    [MenuItem("Tools/Deep Scan for Unused Assets")]
    public static void ShowWindow() {
        GetWindow<DeepUnusedAssetsScanner>("Deep Unused Assets Scanner");
    }

    private void OnGUI() {
        if (GUILayout.Button("Scan by dependency (Prefabs & Scenes)")) {
            ScanProjectAssets();
        }

        GUILayout.Space(10);
        GUILayout.Label("Assets not referenced (sorted by size):", EditorStyles.boldLabel);

        scroll = GUILayout.BeginScrollView(scroll);
        foreach (var asset in unusedAssets) {
            GUILayout.Label($"{(asset.size / 1024f):F1} KB - {asset.path}");
        }
        GUILayout.EndScrollView();
    }

    private void ScanProjectAssets() {
        unusedAssets.Clear();

        string[] allAssets = AssetDatabase.GetAllAssetPaths()
            .Where(path =>
                path.StartsWith("Assets/") &&
                !AssetDatabase.IsValidFolder(path) &&
                !path.Contains("/Editor/") &&
                !path.Contains("/StreamingAssets/"))
            .ToArray();

        string[] sceneAndPrefabGuids = AssetDatabase.FindAssets("t:Scene t:Prefab");
        HashSet<string> usedAssets = new HashSet<string>();

        foreach (string guid in sceneAndPrefabGuids) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string[] dependencies = AssetDatabase.GetDependencies(path, true);
            foreach (var dep in dependencies) {
                usedAssets.Add(dep);
            }
        }

        foreach (string asset in allAssets) {
            if (!usedAssets.Contains(asset)) {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), asset);
                long size = File.Exists(fullPath) ? new FileInfo(fullPath).Length : 0;
                unusedAssets.Add((asset, size));
            }
        }

        // Sort by size (largest first)
        unusedAssets = unusedAssets.OrderByDescending(a => a.size).ToList();

        Debug.Log($"Scan complete. {unusedAssets.Count} unused assets found.");
    }
}
