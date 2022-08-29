# Standalone Input Module

The module is designed to work as you would expect a controller / mouse input to work. Events for button presses, dragging, and similar are sent in response to input.

The module sends pointer events to components as a mouse / input device is moved around, and uses the [Graphics Raycaster](script-GraphicRaycaster.md) and [Physics Raycaster](script-PhysicsRaycaster.md) to calculate which element is currently pointed at by a given pointer device. You can configure these raycasters to detect or ignore parts of your Scene, to suit your requirements.

The module sends move events and submit / cancel events in response to Input tracked via the [Input](https://docs.unity3d.com/Manual/class-InputManager.html) window. This works for both keyboard and controller input. The tracked axis and keys can be configured in the module's inspector.


## Properties

|**_Property:_** |**_Function:_** |
|:---|:---|
|__Horizontal Axis__ | Type the desired manager name for the horizontal axis button. |
|__Vertical Axis__ | Type the desired manager name for the vertical axis. |
|__Submit Button__ | Type the desired manager name for the Submit button. |
|__Cancel Button__ | Type the desired manager name for the Cancel button. |
|__Input Actions Per Second__ | Number of keyboard/controller inputs allowed per second. |
|__Repeat Delay__ | Delay in seconds before the input actions per second repeat rate takes effect. |
|__Force Module Active__ | Enable this property to force this __Standalone Input Module__ to be active. |

## Details
The module uses:

- Vertical / Horizontal axis for keyboard and controller navigation
- Submit / Cancel button for sending submit and cancel events
- Has a timeout between events to only allow a maximum number of events a second.

The flow for the module is as follows

- Send a Move event to the selected object if a valid axis from the Input window is entered
- Send a submit or cancel event to the selected object if a submit or cancel button is pressed
- Process Mouse input
    - If it is a new press
        - Send PointerEnter event (sent to every object up the hierarchy that can handle it)
        - Send PointerPress event
        - Cache the drag handler (first element in the hierarchy that can handle it)
        - Send BeginDrag event to the drag handler
        - Set the 'Pressed' object as Selected in the event system
    - If this is a continuing press
        - Process movment
        - Send DragEvent to the cached drag handler
        - Handle PointerEnter and PointerExit events if touch moves between objects
    - If this is a release
        - Send PointerUp event to the object that received the PointerPress
        - If the current hover object is the same as the PointerPress object send a PointerClick event
        - Send a Drop event if there was a drag handler cached
        - Send a EndDrag event to the cached drag handler
    - Process scroll wheel events
