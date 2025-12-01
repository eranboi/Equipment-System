using UnityEngine;
using UnityEngine.InputSystem;

namespace eranboi.EquipmentSystem.Demo
{
    public class SimpleInteractor : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float interactionRadius = 2.0f;
        [SerializeField] private LayerMask itemLayer;

        private void Update()
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                TryPickup();
            }
        }

        private void TryPickup()
        {
            // Find all colliders nearby
            var _hits = Physics.OverlapSphere(transform.position, interactionRadius, itemLayer);

            // Just pick up the first valid one found
            foreach (var _hit in _hits)
            {
                // Look for the WorldItem component in parent (in case we hit a child collider)
                var _item = _hit.GetComponentInParent<WorldItem>();
                
                if (_item != null)
                {
                    _item.Interact(gameObject);
                    Debug.Log($"Picked up {_hit.name}");
                    return; // Stop after picking up one item
                }
            }
        }

        // Draw debug sphere in editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}