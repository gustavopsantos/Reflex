# Dropdown

The **Dropdown** can be used to let the user choose a single option from a list of options.

The control shows the currently chosen option. Once clicked, it opens up the list of options so a new option can be chosen. Upon choosing a new option, the list of closed again, and the control shows the new selected option. The list is also closed if the user clicks on the control itself, or anywhere else inside the Canvas.

![A Dropdown.](images/UI_DropdownExample.png)

![A Dropdown with its list of options open.](images/UI_DropdownExampleOpen.png)

![](images/UI_DropdownInspector.png)

## Properties

|**Property:** |**Function:** |
|:---|:---|
|**Interactable** | Will this component will accept input? See [Interactable](script-Selectable.md). |
|**Transition** | Properties that determine the way the control responds visually to user actions. See [Transition Options](script-SelectableTransition.md). |
|**Navigation** | Properties that determine the sequence of controls. See [Navigation Options](script-SelectableNavigation.md).|
|**Template** | The Rect Transform of the template for the dropdown list. See instructions below. |
|**Caption Text** | The Text component to hold the text of the currently selected option. (Optional) |
|**Caption Image** | The Image component to hold the image of the currently selected option. (Optional) |
|**Item Text** | The Text component to hold the text of the item. (Optional) |
|**Item Image** | The Image component to hold the image of the item. (Optional) |
|**Value** | The index of the currently selected option. 0 is the first option, 1 is the second, and so on. |
|**Options** | The list of possible options. A text string and an image can be specified for each option. |

## Events

|**Property:** |**Function:** |
|:---|:---|
|**On Value Changed** | A [UnityEvent](https://docs.unity3d.com/Manual/UnityEvents.html) that is invoked when a user has clicked one of the options in the dropdown list. |


## Details

The list of options is specified in the Inspector or can be assigned from code. For each option a text string can be specified, and optionally an image as well, if the Dropdown is setup to support it.

The button has a single event called _On Value Changed_ that responds when the user completes a click on one of the options in the list. It supports sending an integer number value that is the index of the selected option. 0 is the first option, 1 is the second, and so on.


### The template system

The Dropdown control is designed to have a child GameObject which serves as a template for the dropdown list that is shown when clicking the dropdown control. The template GameObject is inactive by default, but can be made active while editing the template to better see what's going on. A reference to the template object must be specified in the Template property of the Dropdown component.

The template must have a single item in it with a Toggle component on. When the actual dropdown list is created upon clicking the dropdown control, this item is duplicated multiple times, with one copy used for each option in the list. The parent of the item is automatically resized so it can fit all the items inside.

![A simple dropdown setup where the item is an immediate child of the template.](images/UI_DropdownHierarchySimple.png)

![A more advanced dropdown setup that includes a scrollview that enables scrolling when there are many options in the list.](images/UI_DropdownHierarchyScrolling.png)

The template can be setup in many different ways. The setup used by the GameObject > UI > Dropdown menu item includes a scroll view, such that if there are too many options to show at once, a scrollbar will appear and the user can scroll through the options. This is however not a mandatory part of the template setup.

(See the ScrollRect page for more information about setup of Scroll Views.)


### Setup of text and image support

The dropdown supports one text content and one image content for each option. Both text and image is optional. They can only be used if the Dropdown is setup to support it.

The dropdown supports text for each option when the Caption Text and Item Text properties are both setup. These are setup by default when using the GameObject > UI > Dropdown menu item.

* The Caption Text is the Text component to hold the text for the currently selected option. It is typically a child to the Dropdown GameObject.
* The Item Text is the Text component to hold the text for each option. It is typically a child to the Item GameObject.

The dropdown supports an image for each option when the Caption Image and Item Image properties are both setup. These are not setup by default.

* The Caption Image is the Image component to hold the image for the currently selected option. It is typically a child to the Dropdown GameObject.
* The Item Image is the Image component to hold the image for each option. It is typically a child to the Item GameObject.

The actual text and images used for the dropdowns are specified in the Options property of the Dropdown component, or can be set from code.


### Placement of the dropdown list

The placement of the dropdown list in relation to the dropdown control is determined by the anchoring and pivot of the Rect Transform of the Template.

By default, the list will appear below the control. This is achieved by anchoring the template to the bottom of the control. The pivot of the template also needs to be at the top, so that as the template is expanded to accommodate a variable number of option items, it only expands downwards.

The Dropdown control has simple logic to prevent that the dropdown is displayed outside the bounds of the Canvas, since this would make it impossible to select certain options. If the dropdown at its default position is not fully within the Canvas rectangle, its position in relation to the control is reversed. For example, a list that is shown below the control by default will be shown above it instead.

This logic is quite simple and has certain limitations. The dropdown template needs to be no larger than half the Canvas size minus the size of the dropdown control, otherwise there may not be room for the list at either position if the dropdown control is placed in the middle of the Canvas.
