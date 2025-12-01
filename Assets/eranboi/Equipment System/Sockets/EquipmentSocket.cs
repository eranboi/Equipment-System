// EquipmentSocket.cs

using UnityEngine;

namespace eranboi.EquipmentSystem.Sockets
{
    /// <summary>
    /// Represents a single equipment attachment point.
    /// Add this to an empty GameObject and position it where items should attach.
    /// </summary>
    public class EquipmentSocket : MonoBehaviour
    {
        [SerializeField] private EquipmentSocketType socketType;
        
        public EquipmentSocketType SocketType => socketType;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.05f);
            
            // Show forward direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward * 0.1f);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.08f);
            
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.1f, socketType.ToString());
#endif
        }
    }
}