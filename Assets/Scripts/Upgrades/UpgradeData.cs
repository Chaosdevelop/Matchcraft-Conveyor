using BaseCore;
using UnityEngine;
using UnityEngine.Localization;

namespace Upgrades
{
    /// <summary>
    /// Abstract base class for upgrade data. Stores common properties for all upgrades.
    /// </summary>
    public abstract class UpgradeData : DataObject
    {
        /// <summary>
        /// Gets the localized name of the upgrade.
        /// </summary>
        [field: SerializeField]
        public LocalizedString UpgradeName { get; private set; }

        /// <summary>
        /// Gets the localized description of the upgrade.
        /// </summary>
        [field: SerializeField]
        public LocalizedString Description { get; private set; }

        /// <summary>
        /// Gets the icon representing the upgrade.
        /// </summary>
        [field: SerializeField]
        public Sprite Icon { get; private set; }

        /// <summary>
        /// Gets the cost of the upgrade.
        /// </summary>
        [field: SerializeField]
        public int Cost { get; private set; }



        /// <summary>
        /// Abstract method to retrieve the effect of the upgrade.
        /// </summary>
        /// <returns>An instance of <see cref="IUpgradeEffect"/> representing the upgrade's effect.</returns>
        public abstract IUpgradeEffect GetEffect();
    }

    /// <summary>
    /// Generic upgrade data class. Stores the effect of the upgrade.
    /// </summary>
    /// <typeparam name="T">Type of the upgrade effect that implements <see cref="IUpgradeEffect"/>.</typeparam>
    public class GenericUpgradeData<T> : UpgradeData where T : IUpgradeEffect
    {
        [SerializeReference]
        [TypeSelector]
        T effect;

        /// <summary>
        /// Retrieves the effect of the upgrade.
        /// </summary>
        /// <returns>An instance of <see cref="IUpgradeEffect"/> representing the upgrade's effect.</returns>
        public override IUpgradeEffect GetEffect()
        {
            return effect;
        }
    }
}
