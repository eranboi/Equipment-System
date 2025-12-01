using System;
using eranboi.EquipmentSystem.Core;
using eranboi.EquipmentSystem.Interfaces;
using UnityEngine;

namespace eranboi.EquipmentSystem
{
    public interface IEquipmentSystem
    {
        bool Equip(IEquippable equippable, GameObject equipper, GameObject existingInstance = null);
        bool Unequip(EquipmentSlotType slotType, GameObject equipper);
        IEquippable GetEquipped(EquipmentSlotType slotType);
        bool IsSlotOccupied(EquipmentSlotType slotType);
        
        event Action<EquipContext> OnItemEquipped;
        event Action<EquipContext> OnItemUnequipped;
    }
}