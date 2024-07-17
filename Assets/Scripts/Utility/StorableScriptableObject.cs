/*using BaseCore;
using UnityEngine;

/// <summary>
/// Base class for scriptable objects that can be stored and referenced by resource path.
/// </summary>
public class StorableScriptableObject : ScriptableObject
{

	/// <summary>
	/// Resource path of the scriptable object.
	/// </summary>
	[field: ReadOnly]
	[field: SerializeField]
	public string ResourcePath { get; set; }


#if UNITY_EDITOR
	private void OnValidate()
	{
		string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
		if (assetPath.StartsWith("Assets/Resources/"))
		{
			assetPath = assetPath.Replace("Assets/Resources/", "").Replace(".asset", "");
			ResourcePath = assetPath;
		}
		else
		{
			Debug.LogWarning($"ScriptableObject {name} must be in a Resources folder to have a ResourcePath.");
			ResourcePath = null;
		}
	}
#endif
}
*/