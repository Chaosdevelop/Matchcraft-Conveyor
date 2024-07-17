using BaseCore;
using UnityEngine;
using UnityEngine.Localization;

namespace Skills
{
    /// <summary>
    /// Represents the data for a skill in the game.
    /// </summary>
    [CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObjects/SkillData", order = 1)]
    public class SkillData : DataObject
    {
        /// <summary>
        /// Gets the name of the skill.
        /// </summary>
        [field: SerializeField] public LocalizedString SkillName { get; private set; }

        /// <summary>
        /// Gets the description of the skill.
        /// </summary>
        [field: SerializeField] public LocalizedString Description { get; private set; }

        /// <summary>
        /// Gets the base energy cost of the skill.
        /// </summary>
        [field: SerializeField] public int BaseEnergyCost { get; private set; }

        /// <summary>
        /// Gets the maximum number of charges for the skill.
        /// </summary>
        [field: SerializeField] public int MaxCharges { get; private set; }

        /// <summary>
        /// Gets the icon of the skill.
        /// </summary>
        [field: SerializeField] public Sprite Icon { get; private set; }

        /// <summary>
        /// Gets the skill slot.
        /// </summary>
        [field: SerializeField] public SkillSlot SkillSlot { get; private set; }

        /// <summary>
        /// Gets the skill pattern.
        /// </summary>
        [field: SerializeReference][field: TypeSelector] public ISkillPattern SkillPattern { get; private set; }

        /// <summary>
        /// Gets the skill effect.
        /// </summary>
        [field: SerializeReference][field: TypeSelector] public ISkillEffect SkillEffect { get; private set; }


        /// <summary>
        /// Creates a new instance of <see cref="SkillModel"/> using the current skill data.
        /// </summary>
        /// <returns>A new instance of <see cref="SkillModel"/>.</returns>
        public SkillModel CreateSkillModel()
        {
            return new SkillModel(this);
        }
    }
}