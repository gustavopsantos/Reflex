# Touch Input Module

**Note: TouchInputModule is obsolete. Touch input is now handled in [StandaloneInputModule](script-StandaloneInputModule.md).**

This module is designed to work with touch devices. It sends pointer events for touching and dragging in response to user input. The module supports multitouch.

The module uses the scene configured Raycasters to calculate what element is currently being touched over. A raycast is issued for each current touch.


## Properties

|**_Property:_** |**_Function:_** |
|:---|:---|
|__Force Module Active__ | Forces this module to be active. |

## Details

The flow for the module is as follows:

- For each touch event
    - If it is a new press
        - Send PointerEnter event (sent to every object up the hierarchy that can handle it)
        - Send PointerPress event
        - Cache the drag handler (first element in the hierarchy that can handle it)
        - Send BeginDrag event to the drag handler
        - Set the 'Pressed' object as Selected in the event system
    - If this is a continuing press
        - Process movement
        - Send DragEvent to the cached drag handler
        - Handle PointerEnter and PointerExit events if touch moves between objects
    - If this is a release
        - Send PointerUp event to the object that received the PointerPress
        - If the current hover object is the same as the PointerPress object send a PointerClick event
        - Send a Drop event if there was a drag handler cached
        - Send a EndDrag event to the cached drag handler
