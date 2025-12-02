using System;
using System.Collections.Generic;
using eranboi.EquipmentSystem.Core;
using eranboi.EquipmentSystem.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace eranboi.EquipmentSystem
{
    public class EquipmentController : MonoBehaviour, IEquipmentSystem
    {
        private readonly Dictionary<EquipmentSlotType, IEquippable> equippedItems = new();
        private readonly Dictionary<EquipmentSlotType, EquipContext> activeContexts = new();
        
        // Events
        public event Action<EquipContext> OnItemEquipped;
        public event Action<EquipContext> OnItemUnequipped;
        public event Action<EquipContext> OnItemDropped;
        public event Action<EquipmentSlotType> OnSlotCleared;

        private void Awake()
        {
            InitializeSlots();
        }

        private void InitializeSlots()
        {
            foreach (EquipmentSlotType _slotType in Enum.GetValues(typeof(EquipmentSlotType)))
            {
                if (_slotType != EquipmentSlotType.None)
                    equippedItems[_slotType] = null;
            }
        }
        
        public bool Equip(IEquippable equippable, GameObject equipper, GameObject existingInstance = null)
        {
            if (equippable == null)
            {
                Debug.LogWarning("[EquipmentController] Cannot equip null item");
                return false;
            }
            
            var _slotType = equippable.SlotType;
            
            if (_slotType == EquipmentSlotType.None)
            {
                Debug.LogWarning("[EquipmentController] Item has None slot type");
                return false;
            }
            
            // Unequip existing item first
            if (IsSlotOccupied(_slotType))
            {

                Drop(_slotType, equipper);
            }
            
            equippedItems[_slotType] = equippable;
            
            // Create context
            var _context = new EquipContext
            {
                equippable = equippable,
                equipper = equipper,
                existingInstance = existingInstance,
                spawnedInstance = null
            };
            
            activeContexts[_slotType] = _context;
            
            // Fire event
            OnItemEquipped?.Invoke(_context);
            
            Debug.Log($"[EquipmentController] Equipped to {_slotType}");
            return true;
        }
        
        /// <summary>
        /// Unequip item to inventory. Destroys the visual instance.
        /// </summary>
        public bool Unequip(EquipmentSlotType slotType, GameObject equipper)
        {
            if (!TryGetEquippedItem(slotType, out var _item))
                return false;
            
            var _context = RemoveFromSlot(slotType, equipper);
            
            // Fire unequip event - VisualHandler will destroy instance
            OnItemUnequipped?.Invoke(_context);
            OnSlotCleared?.Invoke(slotType);
            
            Debug.Log($"[EquipmentController] Unequipped from {slotType}");
            return true;
        }
        
        /// <summary>
        /// Drop item to world. Releases the visual instance to world space.
        /// </summary>
        public bool Drop(EquipmentSlotType slotType, GameObject equipper)
        {
            if (!TryGetEquippedItem(slotType, out var _item))
                return false;
            
            var _context = RemoveFromSlot(slotType, equipper);
            
            // Add drop info
            _context.dropPosition = equipper.transform.position;
            _context.dropDirection = equipper.transform.forward;
            
            // Fire drop event
            // VisualHandler will release instance
            OnItemDropped?.Invoke(_context);
            OnSlotCleared?.Invoke(slotType);
            
            Debug.Log($"[EquipmentController] Dropped from {slotType}");
            return true;
        }
        
        private bool TryGetEquippedItem(EquipmentSlotType slotType, out IEquippable item)
        {
            item = null;
            
            if (!equippedItems.ContainsKey(slotType) || equippedItems[slotType] == null)
            {
                Debug.LogWarning($"[EquipmentController] No item in {slotType}");
                return false;
            }
            
            item = equippedItems[slotType];
            return true;
        }
        
        private EquipContext RemoveFromSlot(EquipmentSlotType slotType, GameObject equipper)
        {
            var _item = equippedItems[slotType];
            equippedItems[slotType] = null;
            
            // Get stored context or create minimal one
            var _context = activeContexts.TryGetValue(slotType, out var _ctx) 
                ? _ctx 
                : new EquipContext { equippable = _item, equipper = equipper };
            
            activeContexts.Remove(slotType);
            
            return _context;
        }
        
        public IEquippable GetEquipped(EquipmentSlotType slotType)
        {
            return equippedItems.GetValueOrDefault(slotType);
        }
        
        public bool IsSlotOccupied(EquipmentSlotType slotType)
        {
            return equippedItems.ContainsKey(slotType) && equippedItems[slotType] != null;
        }
        
        public EquipContext? GetContext(EquipmentSlotType slotType)
        {
            return activeContexts.TryGetValue(slotType, out var _ctx) ? _ctx : null;
        }
        
        /// <summary>
        /// Called by EquipmentVisualHandler to update spawned instance.
        /// </summary>
        public void SetSpawnedInstance(EquipmentSlotType slotType, GameObject instance)
        {
            if (activeContexts.TryGetValue(slotType, out var _ctx))
            {
                _ctx.spawnedInstance = instance;
                activeContexts[slotType] = _ctx;
            }
        }
        
        public GameObject GetSpawnedInstance(EquipmentSlotType slotType)
        {
            return activeContexts.TryGetValue(slotType, out var _ctx) ? _ctx.spawnedInstance : null;
        }
        
        public Dictionary<EquipmentSlotType, IEquippable> GetAllEquipped()
        {
            return new Dictionary<EquipmentSlotType, IEquippable>(equippedItems);
        }
    }
}