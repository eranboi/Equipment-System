// EquipmentSocketType.cs
namespace eranboi.EquipmentSystem.Sockets
{
    /// <summary>
    /// Socket types for character equipment attachment points.
    /// </summary>
    public enum EquipmentSocketType
    {
        None,
        
        // Weapons
        RightHand,
        LeftHand,
        Back,
        Hip,
        
        // Armor/Clothing
        Head,
        Face,
        Chest,
        Backpack,
        
        // Extra
        LeftShoulder,
        RightShoulder,
        Belt
    }
}