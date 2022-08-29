# Physics 2D Raycaster

The 2D Raycaster raycasts against 2D objects in the scene. This allows messages to be sent to 2D physics objects that implement event interfaces.  The Camera GameObject needs to be used and will be added to the GameObject if the Physics 3D Raycaster is not added to the Camera GameObject.

For more Raycaster information see [Raycasters](Raycasters.md).

## Properties

|**_Property:_** |**_Function:_** |
|:---|:---|
|__Event Camera__ | The camera that will generate rays for this raycaster. |
|__Priority__ | Priority of the caster relative to other casters. |
|__Sort Order Priority__ | Priority of the raycaster based upon sort order. |
|__Render Order Priority__ | Priority of the raycaster based upon render order. |
