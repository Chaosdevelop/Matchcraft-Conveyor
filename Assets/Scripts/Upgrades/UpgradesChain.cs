using GMF.Data;
using UnityEngine;

namespace Upgrades
{
    /// <summary>
    /// Types of upgrade chains.
    /// </summary>
    public enum UpgradeChainType
    {
        Skill1,
        Skill2,
        Skill3,
        Turns,
        Scores,
    }

    /// <summary>
    /// Represents a chain of upgrades.
    /// </summary>
    [System.Serializable]
    public class UpgradesChain : IData
    {
        public int Id { get; set; }

        /// <summary>
        /// Array of upgrades in the chain.
        /// </summary>
        [field: SerializeField]
        public UpgradeData[] Upgrades { get; private set; }

        /// <summary>
        /// Gets the chain type.
        /// </summary>
        [field: SerializeField]
        public UpgradeChainType ChainType { get; private set; }
    }
}