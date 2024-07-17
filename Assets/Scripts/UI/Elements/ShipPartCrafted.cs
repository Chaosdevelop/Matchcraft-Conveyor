using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the display and state of a crafted ship part in the UI.
/// </summary>
public class ShipPartCrafted : MonoBehaviour
{
    [SerializeField]
    Image image;

    [SerializeField]
    GameObject notReadyIndicator;

    ShipPartAssemblyResult result;


    /// <summary>
    /// Sets the information for the crafted ship part.
    /// </summary>
    /// <param name="result">The assembly result containing the ship part info.</param>
    public void SetInfo(ShipPartAssemblyResult result)
    {
        this.result = result;
        if (result.Done)
        {
            image.sprite = result.ShipPartInfo.Data.Icon;
        }
        image.gameObject.SetActive(result.Done);
        notReadyIndicator.SetActive(!result.Done);
    }
}
