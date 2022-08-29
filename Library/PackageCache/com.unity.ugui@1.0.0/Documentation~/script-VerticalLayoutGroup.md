# Vertical Layout Group

The Vertical Layout Group component places its child layout elements on top of each other. Their heights are determined by their respective minimum, preferred, and flexible heights according to the following model:

* The minimum heights of all the child layout elements are added together and the spacing between them is added as well. The result is the mimimum height of the Vertical Layout Group.
* The preferred heights of all the child layout elements are added together and the spacing between them is added as well. The result is the preferred height of the Vertical Layout Group.
* If the Vertical Layout Group is at its minimum height or smaller, all the child layout elements will also have their minimum height.
* The closer the Vertical Layout group is to its preferred height, the closer each child layout element will also get to their preferred height.
* If the Vertical Layout Group is taller than its preferred height, it will distribute the extra available space proportionally to the child layout elements according to their respective flexible heights.

For more information about minimum, preferred, and flexible height, see the documentation on [Auto Layout](UIAutoLayout.md).

## Properties

![](images/UI_VerticalLayoutGroupInspector.png)

|**Property:** |**Function:** |
|:---|:---|
|**Padding** |The padding inside the edges of the layout group. |
|**Spacing** |The spacing between the layout elements. |
|**Child Alignment** |The alignment to use for the child layout elements if they don't fill out all the available space. |
|**Control Child Size** |Whether the Layout Group controls the width and height of its child layout elements.|
|**Use Child Scale** |Whether the Layout Group considers the scale of its child layout elements when sizing and laying out elements. <br/><br/> **Width** and **Height**  correspond to the **Scale > X** and **Scale > Y** values in each child layout element's [Rect Transform](class-RectTransform.md) component. <br/><br/>You cannot animate the Scale values using the [Animator Controller](https://docs.unity3d.com/Manual/class-AnimatorController.html) |
|**Child Force Expand** |Whether to force the child layout elements to expand to fill additional available space. |
