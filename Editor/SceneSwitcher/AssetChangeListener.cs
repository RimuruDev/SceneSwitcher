using System;
using UnityEditor;

namespace RimuruDevUtils.SceneSwitcher
{
    public class AssetChangeListener : AssetPostprocessor
    {
        public static event Action AssetsWereChanged;

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (importedAssets.Length != 0 || deletedAssets.Length != 0)
            {
                AssetsWereChanged?.Invoke();
            }
        }
    }
}