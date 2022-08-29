# Content Size Fitter

## Properties

![](images/UI_ContentSizeFitterInspector.png)

|**Property:** |**Function:** |
|:---|:---|
|**Horizontal Fit** |How the width is controlled. |
|&nbsp;&nbsp;&nbsp;&nbsp;Unconstrained |Do not drive the width based on the layout element. |
|&nbsp;&nbsp;&nbsp;&nbsp;Min Size |Drive the width based on the minimum width of the layout element. |
|&nbsp;&nbsp;&nbsp;&nbsp;Preferred Size |Drive the width based on the preferred width of the layout element. |
|**Vertical Fit** |How the height is controlled. |
|&nbsp;&nbsp;&nbsp;&nbsp;Unconstrained |Do not drive the height based on the layout element. |
|&nbsp;&nbsp;&nbsp;&nbsp;Min Size |Drive the height based on the minimum height of the layout element. |
|&nbsp;&nbsp;&nbsp;&nbsp;Preferred Size |Drive the height based on the preferred height of the layout element. |


## Description

The Content Size Fitter functions as a layout controller that controls the size of its own layout element. The size is determined by the minimum or preferred sizes provided by layout element components on the Game Object. Such layout elements can be Image or Text components, layout groups, or a Layout Element component.

It's worth keeping in mind that when a Rect Transform is resized - whether by a Content Size Fitter or something else - the resizing is around the pivot. This means that the direction of the resizing can be controlled using the pivot.

For example, when the pivot is in the center, the Content Size Fitter will expand the Rect Transform out equally in all directions. And when the pivot is in the upper left corner, the Content Size Fitter will expand the Rect Transform down and to the right.
