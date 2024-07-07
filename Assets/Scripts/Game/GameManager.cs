using System;
using BaseCore;
using BaseCore.Collections;
using Skills;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NetExtender.Initializer.Types.Handlers.Chain;
using NetExtender.Types.Collections;
using NetExtender.Types.Collections.Interfaces;
using NetExtender.Types.Exceptions;
using UnityEngine;
using Upgrades;

/// <summary>
/// Manages the overall game state and player progress.
/// </summary>
public class GameManager : SingletonMonobehavior<GameManager>
{
	[SerializeField]
	TutorialManager tutorial;

	[SerializeField]
	PlayerProgress progress;

	[SerializeField]
	EnumDictionary<SkillSlot, SkillData> skillsData = new EnumDictionary<SkillSlot, SkillData>();

	[field: SerializeField]
	public UpgradeManager UpgradeManager { get; private set; }

	[field: SerializeField]
	public int InitialTurns { get; private set; }

	[field: SerializeField]
	public int InitialScoresPerMatch { get; private set; }

	public int AdditionalTurnsPerCraft { get; set; }

	public int AdditionalScoresPerMatch { get; set; }

	List<ShipPartInfo> shipPartsAll;

	public int TurnsPerCraft => InitialTurns + AdditionalTurnsPerCraft;

	public int ScoresPerMatch => InitialScoresPerMatch + AdditionalScoresPerMatch;

	public EnumDictionary<SkillSlot, SkillModel> Skills = new EnumDictionary<SkillSlot, SkillModel>();
    
    IChainHandlerValue<Int32> Health = new SuppressObservableChainHandlerValue<Int32>(50);
    
	void Awake()
    {
        Health.PropertyChanged += HealthOnPropertyChanged;
        Health.Add(new DynamicChainHandler<Int32>(value => value *= 2));
        
		SaveManager.Load(progress);
		if (!progress.ProgressInitialized)
		{
			InitializeProgress();
		}
		EventSystem.SendEventToAll(new ResourceChanged { NewValue = progress.CurrentCoins });
		UpgradeManager.SetStates(progress.UpgradeStates);
		UpgradeManager.OnUpgraded += OnUpgradeDone;

		shipPartsAll = StorableStorage.GetStorablesOfType<ShipPartInfo>();

		foreach (var item in skillsData)
		{
			Skills[item.Key] = item.Value.CreateSkillModel();
		}
	}
    
    private void HealthOnPropertyChanged(System.Object sender, PropertyChangedEventArgs args)
    {
        if (sender is IChainHandlerValue<Int32> handler)
        {
            Debug.Log($"Health: {handler.Value}");
        }
    }
    
    /// <summary>
	/// Initializes player progress for the first time.
	/// </summary>
	public void InitializeProgress()
	{
		progress.ProgressInitialized = true;
		progress.CurrentShipPartType = ShipPartType.Hull;
		StartTutorialShipAssembly();
	}

	/// <summary>
	/// Reset all player progress.
	/// </summary>
	public void ResetProgress()
	{
		progress = new PlayerProgress();
		InitializeProgress();
	}

	/// <summary>
	/// Starts the tutorial ship assembly.
	/// </summary>
	public void StartTutorialShipAssembly()
	{
		var parts = tutorial.StartingParts;
		foreach (var part in parts)
		{
			progress.ShipParts[part.Key] = CreateUndonePart(part.Value);
		}
	}

	/// <summary>
	/// Starts a new ship assembly.
	/// </summary>
	public void StartNewShipAssembly()
	{
		var partTypes = System.Enum.GetValues(typeof(ShipPartType));
		foreach (var part in partTypes)
		{
			var partType = (ShipPartType)part;
			var randomPart = GenerateItemForCraft(partType);
			progress.ShipParts[partType] = CreateUndonePart(randomPart);
		}
	}

	/// <summary>
	/// Changes the player's coin count and sends an event.
	/// </summary>
	/// <param name="amount">The amount to change the coin count by.</param>
	public void ChangeCoins(int amount)
	{
		progress.CurrentCoins += amount;
		EventSystem.SendEventToAll(new ResourceChanged { ResourceType = ResourceType.Manacoins, NewValue = progress.CurrentCoins, Delta = amount });
	}

	/// <summary>
	/// Checks if the player has enough coins to spend.
	/// </summary>
	/// <param name="amount">The amount of coins to check.</param>
	/// <returns>True if the player has enough coins, otherwise false.</returns>
	public bool CanSpendCoins(int amount)
	{
		return progress.CurrentCoins >= amount;
	}

	/// <summary>
	/// Advances to the next ship part type.
	/// </summary>
	public void NextShipType()
	{
		progress.CurrentShipPartType = progress.CurrentShipPartType switch
		{
			ShipPartType.Hull => ShipPartType.Engine,
			ShipPartType.Engine => ShipPartType.Weapon,
			ShipPartType.Weapon => ShipPartType.Utility,
			ShipPartType.Utility => ShipPartType.Hull,
			_ => throw new EnumUndefinedOrNotSupportedException<ShipPartType>(progress.CurrentShipPartType, nameof(progress) + "." + nameof(progress.CurrentShipPartType), null)
		};
	}

	/// <summary>
	/// Checks if the ship is fully assembled.
	/// </summary>
	/// <returns>True if the ship is completed, otherwise false.</returns>
	public bool IsShipCompleted() => progress.IsShipCompleted();

	ShipPartAssemblyResult CreateUndonePart(ShipPartInfo partInfo)
	{
		return new ShipPartAssemblyResult(partInfo);
	}

	/// <summary>
	/// Generates a random item for crafting based on the ship part type.
	/// </summary>
	/// <param name="shipPartType">The type of ship part to generate.</param>
	/// <returns>The generated ship part info.</returns>
	public ShipPartInfo GenerateItemForCraft(ShipPartType shipPartType)
	{
		return shipPartsAll
			.Where(arg => arg.PartType == shipPartType && arg.UseCommonGeneration)
			.ToList()
			.PickRandom();
	}

	/// <summary>
	/// Gets the current crafting part.
	/// </summary>
	/// <returns>The current crafting part.</returns>
	public ShipPartAssemblyResult GetCurrentCraftingPart() => progress.CurrentCraftingPart;

	/// <summary>
	/// Gets all ship parts.
	/// </summary>
	/// <returns>An enum dictionary of all ship parts.</returns>
	public EnumDictionary<ShipPartType, ShipPartAssemblyResult> GetParts() => progress.ShipParts;

	/// <summary>
	/// Gets the total stats from all ship parts.
	/// </summary>
	/// <returns>An enum dictionary of total stats.</returns>
	public EnumDictionary<ItemStat, int> GetTotalStats() => progress.GetTotalStats();

	/// <summary>
	/// Gets the total scores from all ship parts.
	/// </summary>
	/// <returns>The total scores.</returns>
	public int GetTotalScores() => progress.GetTotalScores();

	/// <summary>
	/// Calculates the final scores based on the total stats and scores.
	/// </summary>
	/// <returns>The calculated final scores.</returns>
	public int CalculateFinalScores()
	{
		var statsum = GetTotalStats().Sum(arg => arg.Value);
		var scores = GetTotalScores();
		return (int)(scores * (1 + 0.1f * statsum));
	}

	/// <summary>
	/// Converts scores to coins.
	/// </summary>
	/// <param name="scores">The scores to convert.</param>
	/// <returns>The converted coin value.</returns>
	public int ScoresToCoins(int scores)
	{
		var coins = scores / 100;
		return Mathf.Max(coins, 1);
	}

	void OnUpgradeDone(List<UpgradeState> upgradeStates)
	{
		progress.UpgradeStates = upgradeStates;
	}
}
