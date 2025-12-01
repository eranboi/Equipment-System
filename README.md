# Equipment System for Unity

A modular, event-driven equipment system for Unity that handles equipping, unequipping, and dropping items with visual socket-based attachment.

## Features

- **Socket-based attachment** — Define attachment points on your character (hands, back, head, etc.) and items automatically parent to the correct socket
- **Event-driven architecture** — Subscribe to `OnItemEquipped`, `OnItemUnequipped`, and `OnItemDropped` events to react to equipment changes
- **Existing instance support** — Pick up items already in the world without re-instantiating them
- **Drop with physics** — Dropped items are released to world space with optional rigidbody physics
- **Configurable offsets** — Per-slot position and rotation offsets for precise item placement
- **Interface-based** — Implement `IEquippable` on your own item definitions to integrate with your existing systems

## Installation

1. Import the package into your Unity project
2. Add `CharacterEquipmentSockets` to your character root GameObject
3. Add `EquipmentSocket` components to empty GameObjects positioned at attachment points on your character rig
4. Add `EquipmentController` and `EquipmentVisualHandler` to your character
5. Configure slot-to-socket mappings in `EquipmentVisualHandler`

## Core Components

### EquipmentController

The main controller that manages equipped items. Implements `IEquipmentSystem`.

```csharp
// Equip an item
equipmentController.Equip(equippableItem, equipperGameObject);

// Equip an existing world instance (e.g., picking up from ground)
equipmentController.Equip(equippableItem, equipperGameObject, existingWorldInstance);

// Unequip to inventory (destroys visual)
equipmentController.Unequip(EquipmentSlotType.PrimaryWeapon, equipperGameObject);

// Drop to world (releases visual with physics)
equipmentController.Drop(EquipmentSlotType.PrimaryWeapon, equipperGameObject);

// Query equipped items
IEquippable weapon = equipmentController.GetEquipped(EquipmentSlotType.PrimaryWeapon);
bool hasWeapon = equipmentController.IsSlotOccupied(EquipmentSlotType.PrimaryWeapon);
```

### EquipmentVisualHandler

Handles visual instantiation and socket parenting. Attach alongside `EquipmentController`.

Configure the **Slot to Socket Mapping** list in the Inspector to define which `EquipmentSlotType` attaches to which `EquipmentSocketType`, along with position and rotation offsets.

### CharacterEquipmentSockets

Manages all equipment sockets on a character. Attach to your character root.

```csharp
// Get a socket transform
Transform rightHand = characterSockets.GetSocket(EquipmentSocketType.RightHand);

// Check if socket exists
bool hasBackSocket = characterSockets.HasSocket(EquipmentSocketType.Back);

// Find sockets from any GameObject in the hierarchy
CharacterEquipmentSockets sockets = CharacterEquipmentSockets.GetFrom(someGameObject);
```

### EquipmentSocket

A single attachment point. Add to an empty GameObject and set the socket type in the Inspector. Gizmos help visualize socket positions in the Scene view.

## Enums

### EquipmentSlotType

Logical equipment slots:

- `None`, `PrimaryWeapon`, `SecondaryWeapon`, `Melee`, `Head`, `Body`, `Face`, `Backpack`, `Accessory`

### EquipmentSocketType

Physical attachment points on the character:

- `None`, `RightHand`, `LeftHand`, `Back`, `Hip`, `Head`, `Face`, `Chest`, `Backpack`, `LeftShoulder`, `RightShoulder`, `Belt`

## Interfaces

### IEquippable

Implement this on your item definitions:

```csharp
public interface IEquippable
{
    EquipmentSlotType SlotType { get; }
    GameObject Prefab { get; }
}
```

### IEquipmentSystem

The equipment system interface implemented by `EquipmentController`:

```csharp
public interface IEquipmentSystem
{
    bool Equip(IEquippable equippable, GameObject equipper, GameObject existingInstance = null);
    bool Unequip(EquipmentSlotType slotType, GameObject equipper);
    IEquippable GetEquipped(EquipmentSlotType slotType);
    bool IsSlotOccupied(EquipmentSlotType slotType);
    
    event Action<EquipContext> OnItemEquipped;
    event Action<EquipContext> OnItemUnequipped;
}
```

## Events

Subscribe to equipment events via `EquipmentController`:

```csharp
equipmentController.OnItemEquipped += (ctx) => {
    Debug.Log($"Equipped {ctx.equippable} to {ctx.SlotType}");
    // ctx.spawnedInstance contains the visual GameObject (newly instantiated or existing world instance)
};

equipmentController.OnItemUnequipped += (ctx) => {
    Debug.Log($"Unequipped from {ctx.SlotType}");
};

equipmentController.OnItemDropped += (ctx) => {
    Debug.Log($"Dropped at {ctx.dropPosition}");
    // ctx.spawnedInstance is now unparented with physics enabled
};

equipmentController.OnSlotCleared += (slotType) => {
    Debug.Log($"Slot {slotType} is now empty");
};
```

## EquipContext

Contains context for equip/unequip/drop operations:

| Property | Description |
|----------|-------------|
| `equippable` | The `IEquippable` item |
| `equipper` | The character GameObject |
| `existingInstance` | Pre-existing world instance (for pickups) |
| `spawnedInstance` | The visual GameObject after equipping (newly instantiated or existing world instance) |
| `dropPosition` | World position where item was dropped |
| `dropDirection` | Forward direction at drop time |
| `HasExistingInstance` | Whether picking up an existing instance |
| `HasSpawnedInstance` | Whether a visual instance exists |
| `SlotType` | The equipment slot type |

## Quick Setup

1. **Character Setup**
   - Add `CharacterEquipmentSockets` to your character root
   - Create empty child GameObjects at attachment points (e.g., on hand bones)
   - Add `EquipmentSocket` to each and set the appropriate `SocketType`
   - Add `EquipmentController` and `EquipmentVisualHandler` to the character
   - In `EquipmentVisualHandler`, assign the `CharacterEquipmentSockets` reference
   - Configure slot-to-socket mappings with offsets as needed

2. **Item Setup**
   - Create an item definiton ScriptableObject implementing `IEquippable`
   - Set the `SlotType` and assign the item `Prefab`

3. **Equipping**
   - Call `EquipmentController.Equip()` with your `IEquippable` item
   - The system handles instantiation, parenting, and positioning


## License

MIT License — see [LICENSE](LICENSE) for details.