# Event System

The Event System is a way of sending events to objects in the application based on input, be it keyboard, mouse, touch, or custom input. The Event System consists of a few components that work together to send events.

When you add an Event System component to a GameObject you will notice that it does not have much functionality exposed, this is because the Event System itself is designed as a manager and facilitator of communication between Event System modules.

The primary roles of the Event System are as follows:

- Manage which GameObject is considered selected
- Manage which Input Module is in use
- Manage Raycasting (if required)
- Updating all Input Modules as required

## Input Modules

An Input Module is where the main logic of how you want the Event System to behave lives, they are used for:

- Handling Input
- Managing event state
- Sending events to scene objects.

Only one Input Module can be active in the Event System at a time, and they must be components on the same GameObject as the Event System component.

If you want to write a custom Input Module, send events supported by existing UI components in Unity. To extend and write your own events, see the [Messaging System](MessagingSystem.md) documentation.

## Raycasters

Raycasters are used for figuring out what the pointer is over. It is common for Input Modules to use the Raycasters configured in the Scene to calculate what the pointing device is over.

There are 3 provided Raycasters that exist by default:


- [Graphic Raycaster](script-GraphicRaycaster.md) - Used for UI elements
- [Physics 2D Raycaster](script-Physics2DRaycaster.md) - Used for 2D physics elements
- [Physics Raycaster](script-PhysicsRaycaster.md) - Used for 3D physics elements

If you have a 2d / 3d Raycaster configured in your Scene, it is easy to make non-UI elements receive messages from the Input Module. Simply attach a script that implements one of the event interfaces. For examples of this, see the [IPointerEnterHandler](xref:UnityEngine.EventSystems.IPointerEnterHandler) and [IPointerClickHandler](xref:UnityEngine.EventSystems.IPointerClickHandler) Scripting Reference pages.
