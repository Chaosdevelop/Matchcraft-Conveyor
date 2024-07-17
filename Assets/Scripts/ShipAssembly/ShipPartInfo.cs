using UnityEngine;
using UnityEngine.Localization;

public enum ShipPartType
{
    Hull = 1,
    Engine = 2,
    Weapon = 4,
    Utility = 8,

}/// <summary>
 /// Represents information about a ship part.
 /// </summary>

[CreateAssetMenu(fileName = "ShipPartInfo", menuName = "ScriptableObjects/ShipPartInfo", order = 1)]
public class ShipPartInfo : DataObject
{
    /// <summary>
    /// Gets the type of the ship part.
    /// </summary>
    [field: SerializeField]
    public ShipPartType PartType { get; private set; }

    /// <summary>
    /// Gets the name of the ship part.
    /// </summary>
    [field: SerializeField]
    public LocalizedString PartName { get; private set; }

    /// <summary>
    /// Gets the description of the ship part.
    /// </summary>
    [field: SerializeField]
    public LocalizedString Description { get; private set; }

    /// <summary>
    /// Gets the icon of the ship part.
    /// </summary>
    [field: SerializeField]
    public Sprite Icon { get; private set; }

    /// <summary>
    /// Gets a value indicating whether to use common generation.
    /// </summary>
    [field: SerializeField]
    public bool UseCommonGeneration { get; private set; }
}

