using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    internal static class MultipleDisplayUtilities
    {
        /// <summary>
        /// Converts the current drag position into a relative position for the display.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="position"></param>
        /// <returns>Returns true except when the drag operation is not on the same display as it originated</returns>
        public static bool GetRelativeMousePositionForDrag(PointerEventData eventData, ref Vector2 position)
        {
            #if UNITY_EDITOR
            position = eventData.position;
            #else
            int pressDisplayIndex = eventData.pointerPressRaycast.displayIndex;
            var relativePosition = RelativeMouseAtScaled(eventData.position);
            int currentDisplayIndex = (int)relativePosition.z;

            // Discard events on a different display.
            if (currentDisplayIndex != pressDisplayIndex)
                return false;

            // If we are not on the main display then we must use the relative position.
            position = pressDisplayIndex != 0 ? (Vector2)relativePosition : eventData.position;
            #endif
            return true;
        }

        /// <summary>
        /// A version of Display.RelativeMouseAt that scales the position when the main display has a different rendering resolution to the system resolution.
        /// By default, the mouse position is relative to the main render area, we need to adjust this so it is relative to the system resolution
        /// in order to correctly determine the position on other displays.
        /// </summary>
        /// <returns></returns>
        public static Vector3 RelativeMouseAtScaled(Vector2 position)
        {
            #if !UNITY_EDITOR && !UNITY_WSA
            // If the main display is now the same resolution as the system then we need to scale the mouse position. (case 1141732)
            if (Display.main.renderingWidth != Display.main.systemWidth || Display.main.renderingHeight != Display.main.systemHeight)
            {
                // Calculate any padding that may be added when the rendering apsect ratio does not match the system aspect ratio.
                int widthPlusPadding = Screen.fullScreen ? Display.main.renderingWidth : (int)(Display.main.renderingHeight * (Display.main.systemWidth / (float)Display.main.systemHeight));

                // Calculate the padding on each side of the screen.
                int padding = Screen.fullScreen ? 0 : (int)((widthPlusPadding - Display.main.renderingWidth) * 0.5f);
                int widthPlusRightPadding = widthPlusPadding - padding;

                // If we are not inside of the main display then we must adjust the mouse position so it is scaled by
                // the main display and adjusted for any padding that may have been added due to different aspect ratios.
                if ((position.y < 0 || position.y > Display.main.renderingHeight ||
                     position.x < 0 || position.x > widthPlusRightPadding))
                {
                    if (!Screen.fullScreen)
                    {
                        // When in windowed mode, the window will be centered with the 0,0 coordinate at the top left, we need to adjust so it is relative to the screen instead.
                        position.x -= (Display.main.renderingWidth - Display.main.systemWidth) * 0.5f;
                        position.y -= (Display.main.renderingHeight - Display.main.systemHeight) * 0.5f;
                    }
                    else
                    {
                        // Scale the mouse position
                        position.x += padding;

                        float xScale = Display.main.systemWidth / (float)widthPlusPadding;
                        float yScale = Display.main.systemHeight / (float)Display.main.renderingHeight;
                        position.x *= xScale;
                        position.y *= yScale;
                    }

                    return Display.RelativeMouseAt(position);
                }
                else
                {
                    // We are using the main display.
                    return new Vector3(position.x, position.y, 0);
                }
            }
            #endif
            return Display.RelativeMouseAt(position);
        }
    }
}
