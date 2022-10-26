using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Aokoro
{
    public static class UIExtentions
    {
        private static List<RaycastResult> garbage;

        public static int Raycast(Vector2 screenPos)
        {
            return Raycast(new List<RaycastResult>(), screenPos);
        }

        public static int Raycast(List<RaycastResult> results, Vector2 screenPos)
        {
            //Ensure that the mouse isn't on a UI element
            //Set up the new Pointer Event
            EventSystem eventSystem = EventSystem.current;

            var eventData = new PointerEventData(eventSystem);

            //Set the Pointer Event Position to that of the mouse position
            eventData.position = screenPos;

            //Raycast using the Graphics Raycaster and mouse click position
            eventSystem.RaycastAll(eventData, results);

            return results.Count;
        }
        public static Rect GetWorldRect(this RectTransform rt, Vector2 scale)
        {
            // Convert the rectangle to world corners and grab the top left
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            Vector3 topLeft = corners[0];

            // Rescale the size appropriately based on the current Canvas scale
            Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);

            return new Rect(topLeft, scaledSize);
        }
        public static bool Overlaps(this RectTransform rectTrans1, RectTransform rectTrans2)
        {
            Rect rect1 = new Rect(rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width, rectTrans1.rect.height);
            Rect rect2 = new Rect(rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width, rectTrans2.rect.height);

            return rect1.Overlaps(rect2);
        }

    }
}
