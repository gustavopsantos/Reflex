# Canvas Group

The Canvas Group can be used to control certain aspects of a whole group of UI elements from one place without needing to handle them each individually. The properties of the Canvas Group affect the GameObject it is on as well as all children.

![](images/UI_CanvasGroupInspector.png)

## Properties

|**Property:** |**Function:** |
|:---|:---|
|**Alpha** | The opacity of the UI elements in this group. The value is between 0 and 1 where 0 is fully transparent and 1 is fully opaque. Note that elements retain their own transparency as well, so the Canvas Group alpha and the alpha values of the individual UI elements are multiplied with each other. |
|**Interactable** | Determines if this component will accept input.  When it is set to false interaction is disabled. |
|**Block Raycasts** | Will this component act as a collider for Raycasts? You will need to call the RayCast function on the graphic raycaster attached to the Canvas.  This does _not_ apply to **Physics.Raycast**. |
|**Ignore Parent Groups** | Will this group also be affected by the settings in Canvas Group components further up in the Game Object hierarchy, or will it ignore those and hence override them? |

## Details

Typical uses of Canvas Group are:

* Fading in or out a whole window by adding a Canvas Group on the GameObject of the Window and control its Alpha property.
* Making a whole set of controls non-interactable ("grayed out") by adding a Canvas Group to a parent GameObject and setting its Interactable property to false.
* Making one or more UI elements not block mouse events by placing a Canvas Group component on the element or one of its parents and setting its Block Raycasts property to false.
