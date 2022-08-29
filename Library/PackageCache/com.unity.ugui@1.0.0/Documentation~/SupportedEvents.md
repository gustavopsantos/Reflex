# Supported Events

The Event System supports a number of events, and they can be customized further in user custom user written Input Modules.

The events that are supported by the Standalone Input Module and Touch Input Module are provided by interface and can be implemented on a MonoBehaviour by implementing the interface. If you have a valid Event System configured the events will be called at the correct time.

- [IPointerEnterHandler](xref:UnityEngine.EventSystems.IPointerEnterHandler) - OnPointerEnter - Called when a pointer enters the object
- [IPointerExitHandler](xref:UnityEngine.EventSystems.IPointerExitHandler) - OnPointerExit - Called when a pointer exits the object
- [IPointerDownHandler](xref:UnityEngine.EventSystems.IPointerDownHandler) - OnPointerDown - Called when a pointer is pressed on the object
- [IPointerUpHandler](xref:UnityEngine.EventSystems.IPointerUpHandler)- OnPointerUp - Called when a pointer is released (called on the GameObject that the pointer is clicking)
- [IPointerClickHandler](xref:UnityEngine.EventSystems.IPointerClickHandler) - OnPointerClick - Called when a pointer is pressed and released on the same object
- [IInitializePotentialDragHandler](xref:UnityEngine.EventSystems.IInitializePotentialDragHandler) - OnInitializePotentialDrag - Called when a drag target is found, can be used to initialize values
- [IBeginDragHandler](xref:UnityEngine.EventSystems.IBeginDragHandler) - OnBeginDrag - Called on the drag object when dragging is about to begin
- [IDragHandler](xref:UnityEngine.EventSystems.IDragHandler) - OnDrag - Called on the drag object when a drag is happening
- [IEndDragHandler](xref:UnityEngine.EventSystems.IEndDragHandler) - OnEndDrag - Called on the drag object when a drag finishes
- [IDropHandler](xref:UnityEngine.EventSystems.IDropHandler) - OnDrop - Called on the object where a drag finishes
- [IScrollHandler](xref:UnityEngine.EventSystems.IScrollHandler) - OnScroll - Called when a mouse wheel scrolls
- [IUpdateSelectedHandler](xref:UnityEngine.EventSystems.IUpdateSelectedHandler) - OnUpdateSelected - Called on the selected object each tick
- [ISelectHandler](xref:UnityEngine.EventSystems.ISelectHandler) - OnSelect - Called when the object becomes the selected object
- [IDeselectHandler](xref:UnityEngine.EventSystems.IDeselectHandler) - OnDeselect - Called on the selected object becomes deselected
- [IMoveHandler](xref:UnityEngine.EventSystems.IMoveHandler) - OnMove - Called when a move event occurs (left, right, up, down)
- [ISubmitHandler](xref:UnityEngine.EventSystems.ISubmitHandler) - OnSubmit - Called when the submit button is pressed
- [ICancelHandler](xref:UnityEngine.EventSystems.ICancelHandler) - OnCancel - Called when the cancel button is pressed
