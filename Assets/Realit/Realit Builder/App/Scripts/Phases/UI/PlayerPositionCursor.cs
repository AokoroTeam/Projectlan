using Aokoro.UI;
using UnityEngine;

namespace Realit.Builder.App.Edition.UI
{
    [AddComponentMenu("Realit/Builder/Edition/UI/Player position cursor")]
    public class PlayerPositionCursor : MonoBehaviour
    {
        private DragableItem item;

        private void Awake()
        {
            item = GetComponent<DragableItem>();
        }


        public void OnDrag(Vector2 screenPosition)
        {
            PlayerPositionPreview.Instance.PreviewPlayer(screenPosition);
        }

        public void OnDragEnd(Vector2 screenPosition)
        {
            PlayerPositionPreview.Instance.SetPlayerPositionWithScreenPoint(screenPosition);
        }
    }
}