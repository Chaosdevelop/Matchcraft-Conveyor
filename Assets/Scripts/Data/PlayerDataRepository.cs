using GMF.Data;
using GMF.Tags;
using UnityEngine;

[System.Serializable]
public class PlayerData : IData, ITaggedContainerRegistrator
{
    [SerializeField]
    public TaggedIntValue TurnsForCraft;
    [SerializeField]
    public TaggedIntValue ScoresPerMatchedChip;

    public int Id { get; set; }

    public void Register()
    {
        TurnsForCraft.Register();
        ScoresPerMatchedChip.Register();

    }

    public void Unregister()
    {
        TurnsForCraft.Unregister();
        ScoresPerMatchedChip.Unregister();

    }
}

[CreateAssetMenu(fileName = "PlayerDataRepository", menuName = "Repository/PlayerDataRepository", order = 1)]
public class PlayerDataRepository : DataRepositoryScriptableObject<PlayerData>
{

}
