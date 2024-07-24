using BaseCore;
using BaseCore.Collections;
using GMF.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Ship Assembly window functionality and display.
/// </summary>
public class ShipAssemblyWindow : BaseWindow
{
    [SerializeField]
    Button startCraftButton;

    [SerializeField]
    ShipPartPreview shipPartPreview;

    [SerializeField]
    EnumDictionary<ShipPartType, ShipPartCrafted> craftedViews = new EnumDictionary<ShipPartType, ShipPartCrafted>();

    [SerializeField]
    TextMeshProUGUI turns;

    [SerializeField]
    TextMeshProUGUI totalScores;

    [SerializeField]
    EnumDictionary<ItemStat, TextMeshProUGUI> totalStats = new EnumDictionary<ItemStat, TextMeshProUGUI>();

    [SerializeField]
    Button sellShipButton;

    [SerializeField]
    TextMeshProUGUI sellText;

    public override WindowType WindowType => WindowType.ShipAssembly;

    void Awake()
    {
        startCraftButton.onClick.AddListener(StartCraft);
        sellShipButton.onClick.AddListener(SellShip);
    }

    void OnEnable()
    {
        UpdateContent();
    }

    /// <summary>
    /// Updates the content of the Ship Assembly window.
    /// </summary>
    void UpdateContent()
    {
        var gm = GameManager.Instance;
        var parts = gm.GetParts();

        foreach (var item in parts)
        {
            craftedViews[item.Key].SetInfo(item.Value);
        }

        shipPartPreview.SetInfo(gm.GetCurrentCraftingPart().ShipPartInfo.Data);
        turns.text = Data<PlayerData>.Singleton.TurnsForCraft.CurrentValue.ToString();
        totalScores.text = gm.GetTotalScores().ToString();

        var stats = gm.GetTotalStats();
        foreach (var statText in totalStats)
        {
            statText.Value.text = stats[statText.Key].ToString();
        }

        var shipCompleted = gm.IsShipCompleted();
        startCraftButton.gameObject.SetActive(!shipCompleted);
        sellShipButton.gameObject.SetActive(shipCompleted);

        if (shipCompleted)
        {
            var finalScores = gm.CalculateFinalScores();
            var coins = gm.ScoresToCoins(finalScores);
            sellText.text = coins.ToString();
        }
    }

    /// <summary>
    /// Starts the crafting process.
    /// </summary>
    void StartCraft()
    {
        EventSystem.SendEventToAll(new ScreenChangeEvent { ScreenType = ScreenType.Match3Board });
    }

    /// <summary>
    /// Sells the assembled ship and starts a new assembly process.
    /// </summary>
    void SellShip()
    {
        var gm = GameManager.Instance;
        var finalScores = gm.CalculateFinalScores();
        var coins = gm.ScoresToCoins(finalScores);
        gm.ChangeCoins(coins);
        gm.StartNewShipAssembly();
        UpdateContent();
    }


}
