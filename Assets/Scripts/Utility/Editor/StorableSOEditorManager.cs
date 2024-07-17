/*using UnityEditor;
using UnityEngine;

namespace BaseCore
{
	/// <summary>
	/// Postprocessor for managing StorableScriptableObject assets in the editor.
	/// </summary>
	class StorableAssetPostprocessor : AssetPostprocessor
	{
		static StorableAssetPostprocessor()
		{
			EditorApplication.projectChanged += UpdateAllAssets;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		/// <summary>
		/// Handles play mode state changes to save assets when exiting play or edit mode.
		/// </summary>
		private static void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingEditMode || state == PlayModeStateChange.ExitingPlayMode)
			{
				AssetDatabase.SaveAssets();
			}
		}

		/// <summary>
		/// Updates all assets of type StorableScriptableObject in the project.
		/// </summary>
		private static void UpdateAllAssets()
		{
			string[] guids = AssetDatabase.FindAssets("t:StorableScriptableObject");
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				StorableScriptableObject asset = AssetDatabase.LoadAssetAtPath<StorableScriptableObject>(path);
				UpdateAsset(asset, path);
			}
		}

		/// <summary>
		/// Processes assets after they are imported, deleted, or moved.
		/// </summary>
		static void OnPostprocessAllAssets(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths)
		{
			foreach (string path in importedAssets)
			{
				if (path.EndsWith(".asset"))
				{
					StorableScriptableObject asset = AssetDatabase.LoadAssetAtPath<StorableScriptableObject>(path);
					if (asset != null)
					{
						StorableStorage.Instance.RegisterAsset(asset);
						UpdateAsset(asset, path);
					}
				}
			}

			foreach (string path in deletedAssets)
			{
				if (path.EndsWith(".asset"))
				{
					StorableScriptableObject asset = AssetDatabase.LoadAssetAtPath<StorableScriptableObject>(path);
					if (asset != null)
					{
						StorableStorage.Instance.UnregisterAsset(asset);
					}
				}
			}

			for (int i = 0; i < movedAssets.Length; i++)
			{
				string newPath = movedAssets[i];
				string oldPath = movedFromAssetPaths[i];

				if (newPath.EndsWith(".asset") && oldPath.EndsWith(".asset"))
				{
					StorableScriptableObject newAsset = AssetDatabase.LoadAssetAtPath<StorableScriptableObject>(newPath);
					StorableScriptableObject oldAsset = AssetDatabase.LoadAssetAtPath<StorableScriptableObject>(oldPath);

					if (newAsset != null && oldAsset != null)
					{
						StorableStorage.Instance.UnregisterAsset(oldAsset);
						StorableStorage.Instance.RegisterAsset(newAsset);
						UpdateAsset(newAsset, newPath);
					}
				}
			}

			StorableStorage.Instance.ClearEmpty();
		}

		/// <summary>
		/// Updates the asset's resource path.
		/// </summary>
		private static void UpdateAsset(StorableScriptableObject asset, string path)
		{
			if (path.StartsWith("Assets/Resources/"))
			{
				path = path.Replace("Assets/Resources/", "").Replace(".asset", "");
				if (asset.ResourcePath != path)
				{
					asset.ResourcePath = path;
					EditorUtility.SetDirty(asset);
				}
			}
			else
			{
				Debug.LogWarning($"ScriptableObject {asset.name} must be in a Resources folder to have a ResourcePath.");
				asset.ResourcePath = null;
				EditorUtility.SetDirty(asset);
			}
		}
	}
}
*/