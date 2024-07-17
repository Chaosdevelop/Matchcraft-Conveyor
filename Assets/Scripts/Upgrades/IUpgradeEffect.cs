using BaseCore;
using GMF.Tags;
using Skills;
using UnityEngine;

namespace Upgrades
{
    /// <summary>
    /// Defines a base interface for upgrade effects.
    /// </summary>
    public interface IUpgradeEffect
    {
        /// <summary>
        /// Applies the upgrade effect.
        /// </summary>
        void Apply();
    }

    /// <summary>
    /// Defines an interface for skill upgrade effects.
    /// </summary>
    public interface ISkillUpgradeEffect : IUpgradeEffect { }

    /// <summary>
    /// Defines an interface for macro upgrade effects.
    /// </summary>
    public interface IMacroUpgradeEffect : IUpgradeEffect { }

    /// <summary>
    /// Represents a skill upgrade effect.
    /// </summary>
    [System.Serializable]
    public class SkillUpgradeEffect : ISkillUpgradeEffect
    {
        [SerializeField]
        SkillSlot targetSkillSlot;

        [SerializeReference]
        [TypeSelector]
        ISkillEnhancement[] skillEnhancements;

        /// <summary>
        /// Applies the skill upgrade effect.
        /// </summary>
        public void Apply()
        {
            var gm = GameManager.Instance;
            var skill = gm.Skills[targetSkillSlot];
            foreach (var item in skillEnhancements)
            {
                item.Apply(skill);
            }
        }
    }

    /// <summary>
    /// Represents an effect that grants extra turns.
    /// </summary>
    public class ExtraTurnsEffect : IMacroUpgradeEffect
    {
        /*		[SerializeField]
                int turns;*/
        [SerializeField]
        TaggedModifier modifier;
        /// <summary>
        /// Applies the extra turns effect.
        /// </summary>
        public void Apply()
        {
            TaggedValueModificationManager.AddModifier(modifier);
            /*		var gm = GameManager.Instance;
                    gm.AdditionalTurnsPerCraft += turns;*/
        }
    }

    /// <summary>
    /// Represents an effect that grants extra scores.
    /// </summary>
    public class ExtraScoresEffect : IMacroUpgradeEffect
    {
        /*		[SerializeField]
                int scores;*/
        [SerializeField]
        TaggedModifier modifier;
        /// <summary>
        /// Applies the extra scores effect.
        /// </summary>
        public void Apply()
        {
            TaggedValueModificationManager.AddModifier(modifier);
            /*			var gm = GameManager.Instance;
                        gm.AdditionalScoresPerMatch += scores;*/
        }
    }
}
