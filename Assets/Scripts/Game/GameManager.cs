using System.Collections.Generic;
using System.Linq;
using BaseCore;
using BaseCore.Collections;
using GMF;
using GMF.Data;
using GMF.Saving;
using NetExtender.Types.Exceptions;
using R3;
using Skills;
using UnityEngine;
using Upgrades;



/// <summary>
/// Manages the overall game state and player progress.
/// </summary>
public class GameManager : SingletonMonobehavior<GameManager>
{
    [SerializeField]
    TutorialManager tutorial;


    PlayerProgress progress;

    public UpgradeManager UpgradeManager { get; private set; } = new UpgradeManager();


    ShipPartInfo[] shipPartsAll;

    public EnumDictionary<SkillSlot, SkillModel> Skills { get; private set; } = new EnumDictionary<SkillSlot, SkillModel>();

    void Awake()
    {
        var statemanager = Core.Services.GetRequiredService<IGameStateManager>();
        statemanager.OnStateChanged.Subscribe(OnLoading);
        if (statemanager.CurrentState is MainPlayState)
        {
            OnLoading(statemanager.CurrentState);
        }

    }


    void OnLoading(IGameState gameState)
    {
        if (gameState is not MainPlayState)
        {
            return;
        }

        progress = SaveLoadManager.CurrentSave;
        UnityEngine.Debug.Log($"GameManager OnLoad {progress}");
        if (!progress.ProgressInitialized)
        {
            InitializeProgress();
        }
        EventSystem.SendEventToAll(new ResourceChanged { NewValue = progress.CurrentCoins });
        UpgradeManager.SetStates(progress.UpgradeStates);
        UpgradeManager.OnUpgraded += OnUpgradeDone;

        shipPartsAll = Data<ShipPartInfo>.GetAll().ToArray();
        //shipPartsAll = StorableStorage.GetStorablesOfType<ShipPartInfo>();

        var skillsData = Data<SkillData>.GetAll();

        foreach (var item in skillsData)
        {
            Skills[item.SkillSlot] = item.CreateSkillModel();
        }
    }

    /*    void Awake()
		{

			progress = SaveLoadManager.CurrentSave;

			if (!progress.ProgressInitialized)
			{
				InitializeProgress();
			}
			EventSystem.SendEventToAll(new ResourceChanged { NewValue = progress.CurrentCoins });
			UpgradeManager.SetStates(progress.UpgradeStates);
			UpgradeManager.OnUpgraded += OnUpgradeDone;

			shipPartsAll = Data<ShipPartInfo>.GetAll().ToArray();
			//shipPartsAll = StorableStorage.GetStorablesOfType<ShipPartInfo>();

			var skillsData = Data<SkillData>.GetAll();

			foreach (var item in skillsData)
			{
				Skills[item.SkillSlot] = item.CreateSkillModel();
			}
		}*/


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
            var partType = (ShipPartType) part;
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
        SaveSystem.SaveCurrent();

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
        return (int) (scores * (1 + 0.1f * statsum));
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
        SaveSystem.SaveCurrent();
    }
}
