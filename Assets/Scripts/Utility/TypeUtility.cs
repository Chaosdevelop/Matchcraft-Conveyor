using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BaseCore
{
	/// <summary>
	/// Provides helper methods related to types.
	/// </summary>
	public static class TypeUtility
	{
		static Dictionary<Assembly, Type[]> _cachedAssemblies = new Dictionary<Assembly, Type[]>();

		/// <summary>
		/// Gets cached assemblies and their types.
		/// </summary>
		static Dictionary<Assembly, Type[]> CachedAssemblies {
			get {
				if (_cachedAssemblies == null || _cachedAssemblies.Count == 0)
				{
					_cachedAssemblies = new Dictionary<Assembly, Type[]>();
					foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						if (IsAssemblyIgnored(assembly.FullName))
						{
							continue;
						}

						try
						{
							_cachedAssemblies.Add(assembly, assembly.GetTypes());
						}
						catch (ReflectionTypeLoadException ex)
						{
							_cachedAssemblies.Add(assembly, ex.Types);
						}
					}
				}

				return _cachedAssemblies;
			}
		}

		/// <summary>
		/// Retrieves all types derived from the specified target type.
		/// </summary>
		/// <param name="targetType">The base type to find derived types for.</param>
		/// <param name="mustBeInstantiable">If true, only instantiable types will be returned.</param>
		/// <param name="allowMonoBehaviours">If true, allows MonoBehaviour derived types.</param>
		/// <param name="includeTargetType">If true, includes the target type in the results.</param>
		/// <returns>A list of types derived from the target type.</returns>
		public static List<Type> GetAllTypesDerivedFrom(Type targetType, bool mustBeInstantiable = false,
			bool allowMonoBehaviours = true, bool includeTargetType = false)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException(nameof(targetType), "targetType cannot be null.");
			}

			List<Type> derivedTypes = new List<Type>();
			foreach (KeyValuePair<Assembly, Type[]> assemblyKVP in CachedAssemblies)
			{
				foreach (Type type in assemblyKVP.Value)
				{
					if (type != targetType && targetType.IsAssignableFrom(type) &&
						(!mustBeInstantiable || CheckInstantiable(type) ||
						(allowMonoBehaviours && typeof(MonoBehaviour).IsAssignableFrom(type))))
					{
						derivedTypes.Add(type);
					}
				}
			}

			if (includeTargetType && (!mustBeInstantiable || CheckInstantiable(targetType)))
			{
				derivedTypes.Add(targetType);
			}

			return derivedTypes;
		}

		/// <summary>
		/// Returns a sanitized type name string for the specified type.
		/// </summary>
		/// <param name="type">The type to sanitize the name for.</param>
		/// <returns>A sanitized type name string.</returns>
		public static string GetSanitizedTypeNameString(Type type)
		{
			if (type == null)
			{
				return "null";
			}
			else if (type.IsArray)
			{
				return GetSanitizedTypeNameString(type.GetElementType()) + "[]";
			}
			else if (type.IsGenericType)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(type.Name.Substring(0, type.Name.Length - 2));
				for (int index = 0; index < type.GetGenericArguments().Length; index++)
				{
					Type argument = type.GetGenericArguments()[index];
					sb.Append(GetSanitizedTypeNameString(argument));
					if (index < type.GetGenericArguments().Length - 1)
					{
						sb.Append(", ");
					}
				}
				return sb.ToString();
			}

			return type.Name;
		}

		/// <summary>
		/// Checks if the specified type is instantiable.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns>True if the type is instantiable, otherwise false.</returns>
		public static bool CheckInstantiable(Type type)
		{
			if (typeof(Component).IsAssignableFrom(type))
			{
				return false;
			}

			if (type == typeof(string) || type.IsValueType)
			{
				return true;
			}

			return !type.IsInterface && !type.IsGenericTypeDefinition &&
				   !type.IsAbstract && type.IsVisible;
		}

		/// <summary>
		/// Determines if the specified assembly should be ignored.
		/// </summary>
		/// <param name="assemblyFullName">The full name of the assembly.</param>
		/// <returns>True if the assembly should be ignored, otherwise false.</returns>
		static bool IsAssemblyIgnored(string assemblyFullName)
		{
			return assemblyFullName.StartsWith("UnityEditor") ||
				   assemblyFullName.StartsWith("UnityTest") ||
				   assemblyFullName.StartsWith("Assembly-CSharp-Editor") ||
				   assemblyFullName.StartsWith("Castle.Core") ||
				   assemblyFullName.StartsWith("nunit.framework");
		}
	}
}
