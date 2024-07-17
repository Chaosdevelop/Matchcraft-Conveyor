using Skills;
using UnityEngine;

/*[System.Serializable]
public class SkillsData : IData
{
    [SerializeField]
    public EnumDictionary<SkillSlot, SkillData> Skills = new EnumDictionary<SkillSlot, SkillData>();

    public int Id { get; set; }

    *//*    public void Register()
        {
            TurnsForCraft.Register();
            ScoresPerMatchedChip.Register();
            Debug.Log("Register " + this);
        }

        public void Unregister()
        {
            TurnsForCraft.Unregister();
            ScoresPerMatchedChip.Unregister();
            Debug.Log("Unregister " + this);
        }*//*
}*/

[CreateAssetMenu(fileName = "SkillsDataRepository", menuName = "Repository/SkillsDataRepository", order = 1)]
public class SkillsDataRepository : DataRepositoryScriptableObject<SkillData>
{

}
