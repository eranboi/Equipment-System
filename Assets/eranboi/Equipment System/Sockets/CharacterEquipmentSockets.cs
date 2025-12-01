using System.Collections.Generic;
using UnityEngine;

namespace eranboi.EquipmentSystem.Sockets
{
   /// <summary>
    /// Manages all equipment sockets on a character.
    /// Attach this to the character root (capsule/player).
    /// </summary>
    public class CharacterEquipmentSockets : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool findSocketsOnAwake = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;
        
        private readonly Dictionary<EquipmentSocketType, EquipmentSocket> sockets = new();
        
        // Per-character instance cache for quick lookup
        private static readonly Dictionary<GameObject, CharacterEquipmentSockets> İnstances = new();
        
        private void Awake()
        {
            İnstances[gameObject] = this;
            
            if (findSocketsOnAwake)
            {
                FindAllSockets();
            }
        }
        
        private void OnDestroy()
        {
            İnstances.Remove(gameObject);
        }
        
        /// <summary>
        /// Finds and registers all EquipmentSocket components in children.
        /// </summary>
        [ContextMenu("Find All Sockets")]
        public void FindAllSockets()
        {
            sockets.Clear();
            
            var _foundSockets = GetComponentsInChildren<EquipmentSocket>(true);
            
            foreach (var _socket in _foundSockets)
            {
                if (_socket.SocketType == EquipmentSocketType.None) continue;
                
                if (!sockets.TryAdd(_socket.SocketType, _socket))
                {
                    Debug.LogWarning($"[CharacterEquipmentSockets] Duplicate socket type: {_socket.SocketType} on {gameObject.name}");
                    continue;
                }

                if (showDebugInfo)
                {
                    Debug.Log($"[CharacterEquipmentSockets] Found socket: {_socket.SocketType} at {_socket.name}");
                }
            }
            
            Debug.Log($"[CharacterEquipmentSockets] Total sockets found: {sockets.Count} on {gameObject.name}");
        }
        
        /// <summary>
        /// Gets the transform for a specific socket type.
        /// </summary>
        /// <param name="socketType">The socket type to find.</param>
        /// <returns>Socket transform or null if not found.</returns>
        public Transform GetSocket(EquipmentSocketType socketType)
        {
            if (sockets.TryGetValue(socketType, out EquipmentSocket _socket))
            {
                return _socket.transform;
            }
            
            Debug.LogWarning($"[CharacterEquipmentSockets] Socket not found: {socketType} on {gameObject.name}");
            return null;
        }
        
        /// <summary>
        /// Checks if a socket type exists on this character.
        /// </summary>
        public bool HasSocket(EquipmentSocketType socketType)
        {
            return sockets.ContainsKey(socketType);
        }
        
        /// <summary>
        /// Returns all registered sockets.
        /// </summary>
        public IReadOnlyDictionary<EquipmentSocketType, EquipmentSocket> GetAllSockets()
        {
            return sockets;
        }
        
        /// <summary>
        /// Finds CharacterEquipmentSockets from any GameObject.
        /// Searches the object itself, then parents, then children.
        /// </summary>
        /// <param name="target">The GameObject to search from.</param>
        /// <returns>CharacterEquipmentSockets or null if not found.</returns>
        public static CharacterEquipmentSockets GetFrom(GameObject target)
        {
            if (target == null) return null;
            
            // Check cache first
            if (İnstances.TryGetValue(target, out var _cached))
            {
                return _cached;
            }
            
            // Search as component
            var _socketSystem = target.GetComponent<CharacterEquipmentSockets>();
            if (_socketSystem != null) return _socketSystem;
            
            // Search in parents
            _socketSystem = target.GetComponentInParent<CharacterEquipmentSockets>();
            if (_socketSystem != null) return _socketSystem;
            
            // Search in children (rare case)
            _socketSystem = target.GetComponentInChildren<CharacterEquipmentSockets>();
            return _socketSystem;
        }
    }
}