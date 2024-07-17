using BaseCore.Collections;
using GMF.Data;
using UnityEngine;

public enum ItemStat
{
    Durability = 1,
    Design = 2,
    Quality = 3,
}

/// <summary>
/// Represents the result of a ship part assembly.
/// </summary>
[System.Serializable]
public class ShipPartAssemblyResult
{
    [SerializeField]
    int scores;

    [SerializeField]
    EnumDictionary<ItemStat, int> stats = new EnumDictionary<ItemStat, int>();

    [SerializeField]
    DataReference<ShipPartInfo> shipPartInfo;

    [SerializeField]
    bool done;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShipPartAssemblyResult"/> class.
    /// </summary>
    /// <param name="partInfo">The part information.</param>
    public ShipPartAssemblyResult(ShipPartInfo partInfo)
    {
        shipPartInfo = new DataReference<ShipPartInfo>(partInfo);
    }

    /// <summary>
    /// Gets or sets the scores.
    /// </summary>
    public int Scores {
        get => scores;
        set => scores = value;
    }

    /// <summary>
    /// Gets the stats dictionary.
    /// </summary>
    public EnumDictionary<ItemStat, int> Stats => stats;

    /// <summary>
    /// Gets or sets the ship part information.
    /// </summary>
    public DataReference<ShipPartInfo> ShipPartInfo {
        get => shipPartInfo;
        set => shipPartInfo = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the assembly is done.
    /// </summary>
    public bool Done {
        get => done;
        set => done = value;
    }
}
