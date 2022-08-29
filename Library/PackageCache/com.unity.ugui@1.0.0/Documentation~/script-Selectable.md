# Selectable Base Class

The Selectable Class is the base class for all the interaction components and it handles the items that are in common.

|**Property:** |**Function:** |
|:---|:---|
|**Interactable** | This determines if this component will accept input.  When it is set to false interaction is disabled and the transition state will be set to the disabled state. |
|**Transition** |Within a selectable component there are several [Transition Options](script-SelectableTransition.md) depending on what state the selectable is currently in. The different states are: normal, highlighted, pressed and disabled. |
|**Navigation** |There are also a number of [Navigation Options](script-SelectableNavigation.md) to control how keyboard navigation of the controls is implemented.  |
