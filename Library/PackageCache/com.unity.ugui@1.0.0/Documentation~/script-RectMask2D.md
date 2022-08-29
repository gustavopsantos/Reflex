# RectMask2D

A **RectMask2D** is a masking control similar to the **Mask** control. The mask restricts the child elements to the rectangle of the parent element. Unlike the standard Mask control it has some limitations, but it also has a number of performance benefits.


## Description

A common use of a RectMask2D is to show small sections of a larger area. Using the RectMask2D to frame this area.

The limitations of RectMask2D control are:

- It only works in 2D space
- It will not properly mask elements that are not coplanar

The advantages of RectMask2D are:

- It does not use the stencil buffer
- No extra draw calls
- No material changes
- Fast performance
