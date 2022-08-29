# Horizontal Layout Group

The Horizontal Layout Group component places its child layout elements next to each other, side by side. Their widths are determined by their respective minimum, preferred, and flexible widths according to the following model:

* The minimum widths of all the child layout elements are added together and the spacing between them is added as well. The result is the mimimum width of the Horizontal Layout Group.
* The preferred widths of all the child layout elements are added together and the spacing between them is added as well. The result is the preferred width of the Horizontal Layout Group.
* If the Horizontal Layout Group is at its minimum width or smaller, all the child layout elements will also have their minimum width.
* The closer the Horizontal Layout group is to its preferred width, the closer each child layout element will also get to their preferred width.
* If the Horizontal Layout Group is wider than its preferred width, it will distribute the extra available space proportionally to the child layout elements according to their respective flexible widths.

For more information about minimum, preferred, and flexible width, see the documentation on [Auto Layout](UIAutoLayout.md).

## Properties

![](images/UI_HorizontalLayoutGroupInspector.png)

|**Property:** |**Function:** |
|:---|:---|
|**Padding** |The padding inside the edges of the layout group. |
|**Spacing** |The spacing between the layout elements. |
|**Child Alignment** |The alignment to use for the child layout elements if they don't fill out all the available space. |
|**Control Child Size** |Whether the Layout Group controls the width and height of its child layout elements.|
|**Use Child Scale** |Whether the Layout Group considers the scale of its child layout elements when sizing and laying out elements. <br/><br/> **Width** and **Height**  correspond to the **Scale > X** and **Scale > Y** values in each child layout element's [Rect Transform](class-RectTransform.md) component. <br/><br/>You cannot animate the Scale values using the [Animator Controller](https://docs.unity3d.com/Manual/class-AnimatorController.html) |
|**Child Force Expand** |Whether to force the child layout elements to expand to fill additional available space. |
