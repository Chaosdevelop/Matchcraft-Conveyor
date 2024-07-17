/*using System.Collections.Generic;
using UnityEngine;

namespace BaseCore
{
	/// <summary>
	/// Storage for StorableScriptableObject instances.
	/// </summary>
	[CreateAssetMenu(fileName = "StorableStorage", menuName = "ScriptableObjects/StorableStorage", order = 1)]
	public class StorableStorage : ScriptableObject
	{
		static StorableStorage instance;

		[SerializeField]
		[ReadOnly]
		List<StorableScriptableObject> storedAssets = new List<StorableScriptableObject>();

		/// <summary>
		/// Gets the singleton instance of StorableStorage.
		/// </summary>
		public static StorableStorage Instance {
			get {
				if (instance == null)
				{
					instance = Resources.Load<StorableStorage>("StorableStorage");
					if (instance == null)
					{
						instance = CreateInstance<StorableStorage>();
						Debug.LogWarning("StorableStorage instance not found, creating a new one.");

#if UNITY_EDITOR
						string path = "Assets/Resources/StorableStorage.asset";
						UnityEditor.AssetDatabase.CreateAsset(instance, path);
						UnityEditor.AssetDatabase.SaveAssets();
						UnityEditor.AssetDatabase.Refresh();
#endif
					}
				}
				return instance;
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// Registers a StorableScriptableObject asset.
		/// </summary>
		/// <param name="asset">The asset to register.</param>
		public void RegisterAsset(StorableScriptableObject asset)
		{
			if (!storedAssets.Contains(asset))
			{
				storedAssets.Add(asset);
				UnityEditor.EditorUtility.SetDirty(this);
			}
		}

		/// <summary>
		/// Unregisters a StorableScriptableObject asset.
		/// </summary>
		/// <param name="asset">The asset to unregister.</param>
		public void UnregisterAsset(StorableScriptableObject asset)
		{
			if (storedAssets.Contains(asset))
			{
				storedAssets.Remove(asset);
				UnityEditor.EditorUtility.SetDirty(this);
			}
		}

		/// <summary>
		/// Clears null assets from the storage.
		/// </summary>
		public void ClearEmpty()
		{
			storedAssets.RemoveAll(arg => arg == null);
		}
#endif

		/// <summary>
		/// Gets all stored assets of a specific type.
		/// </summary>
		/// <typeparam name="T">The type of the assets to retrieve.</typeparam>
		/// <returns>A list of assets of the specified type.</returns>
		public static List<T> GetStorablesOfType<T>() where T : StorableScriptableObject
		{
			List<T> assets = new List<T>();
			foreach (var asset in Instance.storedAssets)
			{
				if (asset is T typedAsset)
				{
					assets.Add(typedAsset);
				}
			}
			return assets;
		}
	}
}
*/