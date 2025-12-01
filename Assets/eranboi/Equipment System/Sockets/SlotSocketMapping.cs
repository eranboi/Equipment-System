using System;
using eranboi.EquipmentSystem.Core;
using eranboi.EquipmentSystem.Sockets;
using UnityEngine;

namespace eranboi.EquipmentSystem
{
    [Serializable]
    public struct SlotSocketMapping
    {
        public EquipmentSlotType slotType;
        public EquipmentSocketType socketType;
        
        [Tooltip("Local position offset after parenting")]
        public Vector3 positionOffset;
        
        [Tooltip("Local rotation offset after parenting")]
        public Vector3 rotationOffset;
    }
}