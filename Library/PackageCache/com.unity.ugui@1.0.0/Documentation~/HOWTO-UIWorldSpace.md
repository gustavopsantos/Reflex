# Creating a World Space UI

The UI system makes it easy to create UI that is positioned in the world among other 2D or 3D objects in the Scene.

Start by creating a UI element (such as an Image) if you don't already have one in your scene by using GameObject > UI > Image. This will also create a Canvas for you.


## Set the Canvas to World Space

Select your Canvas and change the Render Mode to World Space.

Now your Canvas is already positioned in the World and can be seen by all cameras if they are pointed at it, but it is probably huge compared to other objects in your Scene. We'll get back to that.


## Decide on a resolution

First you need to decide what the resolution of the Canvas should be. If it was an image, what should the pixel resolution of the image be? Something like 800x600 might be a good starting point. You enter the resolution in the Width and Height values of the Rect Transform of the Canvas. It's probably a good idea to set the position to 0,0 at the same time.


## Specify the size of the Canvas in the world

Now you should consider how big the Canvas should be in the world. You can use the Scale tool to simply scale it down until it has a size that looks good, or you can decide how big it should be in meters.

If you want it to have a specific width in meters, you can can calculate the needed scale by using meter_size / canvas_width. For example, if you want it to be 2 meters wide and the Canvas width is 800, you would have 2 / 800 = 0.0025. You then set the Scale property of the Rect Transform on the Canvas to 0.0025 for both X, Y, and Z in order to ensure that it's uniformly scaled.

Another way to think of it is that you are controlling the size of one pixel in the Canvas. If the Canvas is scaled by 0.0025, then that is also the size in the world of each pixel in the Canvas.

## Position the Canvas

Unlike a Canvas set to Screen Space, a World Space Canvas can be freely positioned and rotated in the Scene. You can put a Canvas on any wall, floor, ceiling, or slanted surface (or hanging freely in the air of course). Just use the normal Translate and Rotate tools in the toolbar.


## Create the UI

Now you can begin setting up your UI elements and layouts the same way you would with a Screen Space Canvas.
