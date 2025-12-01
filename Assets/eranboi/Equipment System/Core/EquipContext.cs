using eranboi.EquipmentSystem.Core;
using eranboi.EquipmentSystem.Interfaces;
using UnityEngine;

namespace eranboi.EquipmentSystem
{
    /// <summary>
    /// Contains all context for an equip/unequip/drop operation.
    /// </summary>
    public struct EquipContext
    {
        /// <summary>
        /// The item being equipped/unequipped/dropped.
        /// </summary>
        public IEquippable equippable;
        
        /// <summary>
        /// The character equipping/unequipping the item.
        /// </summary>
        public GameObject equipper;
        
        /// <summary>
        /// Existing world instance (for ground pickups).
        /// Null = instantiate from prefab.
        /// </summary>
        public GameObject existingInstance;
        
        /// <summary>
        /// The spawned/placed GameObject after equip.
        /// Set by EquipmentVisualHandler.
        /// </summary>
        public GameObject spawnedInstance;
        
        /// <summary>
        /// World position where item was dropped. Only set for Drop operations.
        /// </summary>
        public Vector3 dropPosition;
        
        /// <summary>
        /// Forward direction of equipper at drop time.
        /// </summary>
        public Vector3 dropDirection;
        
        public bool HasExistingInstance => existingInstance != null;
        public bool HasSpawnedInstance => spawnedInstance != null;
        public EquipmentSlotType SlotType => equippable?.SlotType ?? EquipmentSlotType.None;
    }
}