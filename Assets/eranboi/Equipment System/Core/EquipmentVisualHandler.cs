using System.Collections.Generic;
using eranboi.EquipmentSystem.Core;
using eranboi.EquipmentSystem.Sockets;
using UnityEngine;

namespace eranboi.EquipmentSystem
{
    /// <summary>
    /// Handles visual instantiation and socket parenting for equipped items.
    /// </summary>
    [RequireComponent(typeof(EquipmentController))]
    public class EquipmentVisualHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterEquipmentSockets sockets;
        
        [Header("Slot to Socket Mapping")]
        [SerializeField] private List<SlotSocketMapping> mappings = new();
        
        [Header("Drop Settings")]
        [SerializeField] private bool addRigidbodyOnDrop = true;
        [SerializeField] private float dropForce = 2f;
        
        private EquipmentController equipment;
        private readonly Dictionary<EquipmentSlotType, EquipmentSocketType> slotToSocket = new();
        private readonly Dictionary<EquipmentSlotType, SlotSocketMapping> slotToMapping = new();
        
        private void Awake()
        {
            equipment = GetComponent<EquipmentController>();
            
            if (sockets == null)
                sockets = GetComponentInChildren<CharacterEquipmentSockets>();
            
            // Build lookup
            foreach (var _mapping in mappings)
            {
                slotToSocket[_mapping.slotType] = _mapping.socketType;
                slotToMapping[_mapping.slotType] = _mapping;
            }
        }
        
        private void OnEnable()
        {
            equipment.OnItemEquipped += HandleEquipped;
            equipment.OnItemUnequipped += HandleUnequipped;
            equipment.OnItemDropped += HandleDropped;
        }
        
        private void OnDisable()
        {
            equipment.OnItemEquipped -= HandleEquipped;
            equipment.OnItemUnequipped -= HandleUnequipped;
            equipment.OnItemDropped -= HandleDropped;
        }
        
        private void HandleEquipped(EquipContext ctx)
        {
            if (ctx.equippable?.Prefab == null && !ctx.HasExistingInstance)
                return;
            
            var _slotType = ctx.SlotType;
            
            // Get or create instance
            var _instance = ctx.HasExistingInstance ? ctx.existingInstance : Instantiate(ctx.equippable.Prefab);

            // Remove rigidbody if it has one (was dropped before)
            var _rb = _instance.GetComponent<Rigidbody>();

            if (_rb != null)
            {
                _rb.isKinematic = true;
            }
            
            // Parent to socket
            var _socket = GetSocketForSlot(_slotType);
            if (_socket != null)
            {
                _instance.transform.SetParent(_socket);
                
                // Apply offset if mapping exists
                if (slotToMapping.TryGetValue(_slotType, out var _mapping))
                {
                    _instance.transform.localPosition = _mapping.positionOffset;
                    _instance.transform.localRotation = Quaternion.Euler(_mapping.rotationOffset);
                }
                else
                {
                    _instance.transform.localPosition = Vector3.zero;
                    _instance.transform.localRotation = Quaternion.identity;
                }
            }
            
            // Update context with spawned instance
            equipment.SetSpawnedInstance(_slotType, _instance);
            
            // Debug.Log($"[EquipmentVisualHandler] Equipped {_instance.name} at {_socket?.name ?? "no socket"}");
        }
        
        private void HandleUnequipped(EquipContext ctx)
        {
            // Destroy instance
            
            // Prefarably you should send this item to the inventory using your own scripts since this package is equipment only
            // If you don't have an inventory system and want to just drop the item to the world, try Drop method in EquipmentController.
            
            if (ctx.HasSpawnedInstance)
            {
                Destroy(ctx.spawnedInstance);
                // Debug.Log($"[EquipmentVisualHandler] Destroyed {ctx.spawnedInstance.name} (to inventory)");
            }
        }
        
        private void HandleDropped(EquipContext ctx)
        {
            if (!ctx.HasSpawnedInstance)
                return;
            
            var _instance = ctx.spawnedInstance;
            
            // Unparent from socket
            _instance.transform.SetParent(null);
            
            // Enable rigidbody for physics
            if (addRigidbodyOnDrop)
            {
                var _rb = _instance.GetComponent<Rigidbody>();
                if (_rb == null)
                {
                    _rb = _instance.AddComponent<Rigidbody>();
                }
                
                _rb.isKinematic = false;
                _rb.AddForce(ctx.dropDirection * dropForce, ForceMode.Impulse);
            }
            
            // Debug.Log($"[EquipmentVisualHandler] Dropped {_instance.name}");
        }
        
        private Transform GetSocketForSlot(EquipmentSlotType slotType)
        {
            if (sockets == null)
            {
                // Debug.LogWarning("[EquipmentVisualHandler] No sockets assigned");
                return null;
            }
            
            if (slotToSocket.TryGetValue(slotType, out var _socketType))
            {
                return sockets.GetSocket(_socketType);
            }
            
            // Debug.LogWarning($"[EquipmentVisualHandler] No socket mapping for {slotType}");
            return null;
        }
        
        /// <summary>
        /// Get the spawned instance for a slot.
        /// </summary>
        public GameObject GetSpawnedInstance(EquipmentSlotType slotType)
        {
            return equipment.GetSpawnedInstance(slotType);
        }
    }
}