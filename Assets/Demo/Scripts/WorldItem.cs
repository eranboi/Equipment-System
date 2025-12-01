using UnityEngine;
namespace eranboi.EquipmentSystem.Demo
{
    /// <summary>
    ///  You should write your own implementation of this class. This is for showcasing and poorly written :)
    /// </summary>
    public class WorldItem : MonoBehaviour
    {
        [Header("Data")]
        [Tooltip("The ScriptableObject that defines this item")]
        [SerializeField] private ItemDefinition itemDefinition;

        private Rigidbody rb;
        private Collider col;

        private EquipmentController _equipmentController;
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponentInChildren<Collider>();
        }
        
        public void Interact(GameObject interactor)
        {
            _equipmentController = interactor.GetComponent<EquipmentController>();
            
            if (_equipmentController == null) return;
            var _success = _equipmentController.Equip(itemDefinition, interactor, existingInstance: gameObject);
            
            _equipmentController.OnItemDropped += EquipmentControllerOnOnItemDropped;

            if (!_success) return;
            
            if (rb) rb.isKinematic = true;
            if (col) col.enabled = false;
        }

        private void EquipmentControllerOnOnItemDropped(EquipContext obj)
        {
            if (col) col.enabled = true;
            _equipmentController.OnItemDropped += EquipmentControllerOnOnItemDropped;

        }
    }
}