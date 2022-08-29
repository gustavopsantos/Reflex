# Raycasters

The Event System needs a method for detecting where current input events need to be sent to, and this is provided by the Raycasters. Given a screen space position they will collect all potential targets, figure out if they are under the given position, and then return the object that is closest to the screen. There are a few types of Raycasters that are provided:

- [Graphic Raycaster](script-GraphicRaycaster.md) - Used for UI elements, lives on a Canvas and searches within the canvas
- [Physics 2D Raycaster](script-Physics2DRaycaster.md) - Used for 2D physics elements
- [Physics Raycaster](script-PhysicsRaycaster.md) - Used for 3D physics elements

When a Raycaster is present and enabled in the scene it will be used by the Event System whenever a query is issued from an Input Module.

If multiple Raycasters are used then they will all have casting happen against them and the results will be sorted based on distance to the elements.
