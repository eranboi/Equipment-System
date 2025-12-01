using UnityEngine;
using eranboi.EquipmentSystem.Core;
using eranboi.EquipmentSystem.Interfaces;

namespace eranboi.EquipmentSystem.Demo
{
    /// <summary>
    ///  You should write your own implementation of this class. This is for showcasing and poorly written :)
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Equipment/Simple Item")]
    public class ItemDefinition : ScriptableObject, IEquippable
    {
        [Header("Config")]
        [SerializeField] private EquipmentSlotType slotType;
        [SerializeField] private GameObject prefab;

        // Implementation of IEquippable <- You should implement the IEquippable in your own item definition.
        public EquipmentSlotType SlotType => slotType;
        public GameObject Prefab => prefab;
    }
}