using eranboi.EquipmentSystem.Core;
using UnityEngine;

namespace eranboi.EquipmentSystem.Interfaces
{
    /// <summary>
    /// Item definition in game should implement this interface to work with the equipment system.
    /// </summary>
    public interface IEquippable
    {
        EquipmentSlotType SlotType { get; }
        GameObject Prefab { get; }
    }
}

