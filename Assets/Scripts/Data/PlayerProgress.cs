using System.Collections.Generic;
using BaseCore.Collections;
using GMF.Saving;
using UnityEngine;
using Upgrades;

/// <summary>
/// Represents the player's progress for JSON serialization.
/// </summary>
[System.Serializable]
public class PlayerProgress : ISaveData
{
    [SerializeField]
    int currentCoins = 100;

    [SerializeField]
    EnumDictionary<ShipPartType, ShipPartAssemblyResult> shipParts = new EnumDictionary<ShipPartType, ShipPartAssemblyResult>();

    [SerializeField]
    bool progressInitialized;

    [SerializeField]
    List<UpgradeState> upgradeStates = new List<UpgradeState>();

    public ShipPartType CurrentShipPartType = ShipPartType.Hull;

    /// <summary>
    /// Gets the current crafting part based on the current ship part type.
    /// </summary>
    public ShipPartAssemblyResult CurrentCraftingPart => shipParts[CurrentShipPartType];

    /// <summary>
    /// Gets or sets the current number of coins.
    /// </summary>
    public int CurrentCoins {
        get => currentCoins;
        set => currentCoins = value;
    }

    /// <summary>
    /// Gets or sets the ship parts dictionary.
    /// </summary>
    public EnumDictionary<ShipPartType, ShipPartAssemblyResult> ShipParts {
        get => shipParts;
        set => shipParts = value;
    }

    /// <summary>
    /// Gets or sets the progress initialization status.
    /// </summary>
    public bool ProgressInitialized {
        get => progressInitialized;
        set => progressInitialized = value;
    }

    /// <summary>
    /// Gets or sets the list of upgrade states.
    /// </summary>
    public List<UpgradeState> UpgradeStates {
        get => upgradeStates;
        set => upgradeStates = value;
    }

    /// <summary>
    /// Gets the total stats from all ship parts.
    /// </summary>
    /// <returns>An EnumDictionary containing the total stats.</returns>
    public EnumDictionary<ItemStat, int> GetTotalStats()
    {
        var statsDictionary = new EnumDictionary<ItemStat, int>();
        foreach (var part in shipParts)
        {
            foreach (var stat in part.Value.Stats)
            {
                statsDictionary[stat.Key] += stat.Value;
            }
        }
        return statsDictionary;
    }

    /// <summary>
    /// Gets the total scores from all ship parts.
    /// </summary>
    /// <returns>The total scores.</returns>
    public int GetTotalScores()
    {
        var totalScores = 0;
        foreach (var part in shipParts)
        {
            totalScores += part.Value.Scores;
        }
        return totalScores;
    }

    /// <summary>
    /// Checks if the ship is completed.
    /// </summary>
    /// <returns>True if the ship is completed, otherwise false.</returns>
    public bool IsShipCompleted()
    {
        foreach (var part in shipParts)
        {
            if (!part.Value.Done)
            {
                return false;
            }
        }
        return true;
    }

    //public Dictionary<string, object> SubData => new Dictionary<string, object>() { { "Coins", currentCoins } };
    public Dictionary<string, object> SubData => new Dictionary<string, object>();
    public void OnBeforeSave()
    {
        Debug.Log($"OnAfterLoad UpgradeStates {UpgradeStates?.Count}");
    }

    public void OnAfterLoad()
    {
        // UpgradeStates ??= new List<UpgradeState>();
        Debug.Log($"OnAfterLoad UpgradeStates {UpgradeStates?.Count}");
        //upgradeStates = upgradeStates != null ? upgradeStates : new List<UpgradeState>() ;
    }
}
