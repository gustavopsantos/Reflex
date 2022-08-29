# Aspect Ratio Fitter

## Properties

![](images/UI_AspectRatioFitterInspector.png)

|**Property:** |**Function:** |
|:---|:---|
|**Aspect Mode** |How the rectangle is resized to enforce the aspect ratio. |
|&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;**None** |Do not make the rect fit the aspect ratio. |
|&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;**Width Controls Height** |The height is automatically adjusted based on the width. |
|&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;**Height Controls Width** |The width is automatically adjusted based on the height. |
|&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;**Fit In Parent** |The width, height, position, and anchors are automatically adjusted to make the rect fit inside the rect of the parent while keeping the aspect ratio. The may be some space inside the parent rect which is not covered by this rect. |
|&#160;&#160;&#160;&#160;&#160;&#160;&#160;&#160;**Envelope Parent** |The width, height, position, and anchors are automatically adjusted to make the rect cover the entire area of the parent while keeping the aspect ratio. This rect may extend further out than the parent rect. |
|**Aspect Ratio** |The aspect ratio to enforce. This is the width divided by the height. |


## Description

The Aspect Ratio Fitter functions as a layout controller that controls the size of its own layout element. It can adjust the height to fit the width or vice versa, or it can make the element fit inside its parent or envelope its parent. The Aspect Ratio Fitter does not take layout information into account such as minimum size and preferred size.

It's worth keeping in mind that when a Rect Transform is resized - whether by an Aspect Ratio Fitter or something else - the resizing is around the pivot. This means that the pivot can be used to control the alignment of the rectangle. For example, a pivot placed at the top center will make the rectangle grow evenly to both sides, and only grow downwards while the top edge remain at its position.
