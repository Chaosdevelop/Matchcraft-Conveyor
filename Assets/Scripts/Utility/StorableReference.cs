/*using UnityEngine;

namespace BaseCore
{
	/// <summary>
	/// A generic reference class that stores a path to a StorableScriptableObject and loads it on demand.
	/// </summary>
	/// <typeparam name="T">Type of the StorableScriptableObject.</typeparam>
	[System.Serializable]
	public class StorableReference<T> where T : StorableScriptableObject
	{
		T cached;

		[SerializeField]
		[ReadOnly]
		string storablePath;

		/// <summary>
		/// Gets the referenced StorableScriptableObject. Loads it if not already loaded.
		/// </summary>
		public T Storable {
			get {
				if (cached == null)
				{
					cached = Resources.Load<T>(storablePath);
				}
				return cached;
			}
			private set => cached = value;
		}

		/// <summary>
		/// Initializes a new instance of the StorableReference class with a given StorableScriptableObject.
		/// </summary>
		/// <param name="refobj">The StorableScriptableObject to reference.</param>
		public StorableReference(T refobj)
		{
			Storable = refobj;
			storablePath = refobj.ResourcePath;
		}
	}
}
*/