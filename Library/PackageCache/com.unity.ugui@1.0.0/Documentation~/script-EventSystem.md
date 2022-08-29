# Event System Manager

This subsystem is responsible for controlling all the other elements that make up eventing. It coordinates which Input Module is currently active, which GameObject is currently considered 'selected', and a host of other high level Event System concepts.

Each 'Update' the Event System receives the call, looks through its Input Modules and figures out which is the Input Module that should be used for this tick. It then delegates the processing to the modules.


## Properties

|**_Property:_** |**_Function:_** |
|:---|:---|
|__First Selected__ | The GameObject that was selected first. |
|__Send Navigation Events__ | Should the EventSystem allow navigation events (move / submit / cancel). |
|__Drag Threshold__ | The soft area for dragging in pixels. |

Beneath the Properties table is the "Add Default Input Modules" button.
