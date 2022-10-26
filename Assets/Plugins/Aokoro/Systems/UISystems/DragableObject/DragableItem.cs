using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine.Events;

namespace Aokoro.UI
{
    public struct DragContext
    {
        public DragableSlot slot;
        public Vector2 screenPosition;

        public DragContext(DragableSlot slot, Vector2 screenPosition)
        {
            this.slot = slot;
            this.screenPosition = screenPosition;
        }
    }

    public class DragableItem : UIItem, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {   
        [HorizontalLine]
        [SerializeField, BoxGroup("Hover")]
        private CanvasGroup picker;
        [SerializeField, BoxGroup("Hover")]
        private RectTransform zone;
        [SerializeField, BoxGroup("Hover")]
        private float elasticity;
        [SerializeField, BoxGroup("Hover")]
        private float elasticitySmoothness = 5;
        [SerializeField, BoxGroup("Hover")]
        private float minSize = .95f;
        [HorizontalLine]
        [SerializeField, BoxGroup("Drag")]
        private CanvasGroup dragger;
        [SerializeField, BoxGroup("Drag")]
        private RectTransform draggerRect;
        [SerializeField, BoxGroup("Drag")]
        private bool moveIconToDragEnd;
        [HorizontalLine]
        [SerializeField, BoxGroup("Slots")]
        private bool slotless = false;
        [SerializeField, BoxGroup("Slots"), HideIf(nameof(slotless))]
        private List<DragableSlot> slots;
        [SerializeField, ReadOnly, BoxGroup("Slots"), HideIf(nameof(slotless))]
        private DragableSlot hoverSlot;
        [HorizontalLine]
        [SerializeField, BoxGroup("Events")]
        private UnityEvent<Vector2> onDragBegins;
        [SerializeField, BoxGroup("Events")]
        private UnityEvent<Vector2> onDrag;
        [SerializeField, BoxGroup("Events")]
        private UnityEvent<Vector2> onDragEnds;
        public bool IsHoveringSlot => hoverSlot != null && !slotless;

        private Vector2 hoverScreenPos;
        public Vector2 DragScreenPos => dragScreenPos;
        private Vector2 dragScreenPos;


        private bool hovering;
        private bool dragging;

        private Tweener scaleTween;

        protected override void Awake()
        {
            dragger.alpha = 0;

            base.Awake();
        }

        protected override void OnUpdate()
        {
            if (dragging)
            {
                dragger.transform.position = hoverScreenPos;
            }
            else
            {
                //Default position is zone center
                Vector2 center = zone.position;
                Vector2 targetPos = center;

                if (hovering)
                {
                    //Offset towards cursor
                    Vector2 pointerPos = Pointer.current.position.ReadValue();
                    Vector2 offset = Vector2.ClampMagnitude(pointerPos - center, elasticity);
                    targetPos += offset;
                }
                //Smooth moving towards position
                picker.transform.position = Vector2.Lerp(picker.transform.position, targetPos, elasticitySmoothness * Time.deltaTime);
            }
        }

        #region Pointer Events
        public void OnPointerEnter(PointerEventData data)
        {
            if (scaleTween != null && scaleTween.IsPlaying())
                scaleTween.Kill();

            scaleTween = picker.transform.DOScale(minSize, .25f);
            hovering = true;
        }

        public void OnPointerExit(PointerEventData data)
        {
            if(scaleTween != null && scaleTween.IsPlaying())
                scaleTween.Kill();

            scaleTween = picker.transform.DOScale(1, .25f);
            hovering = false;
        }

        #endregion

        #region Drag Events

        
        public void OnDrag(PointerEventData data)
        {
            hoverScreenPos = data.position;

            if (!slotless)
            {
                CheckIfHoversSlot(data);
            }
            
            onDrag?.Invoke(hoverScreenPos);
            data.Use();
        }

        public void CheckIfHoversSlot(PointerEventData data)
        {
            if (hoverSlot != null && Hovers(hoverSlot, data))
                return;

            foreach (var slot in slots)
            {
                if (hoverSlot == slot)
                    continue;

                if (Hovers(slot, data))
                {
                    //New slot
                    if (hoverSlot != slot)
                    {
                        //If was on a slot before
                        if (hoverSlot != null)
                            hoverSlot.OnDragableItemExit(this);

                        hoverSlot = slot;
                        slot.OnDragableItemEnter(this);

                    }
                    return;
                }
            }
            //If was on a slot before
            if (hoverSlot != null)
                hoverSlot.OnDragableItemExit(this);

            hoverSlot = null;

            bool Hovers(DragableSlot slot, PointerEventData data)
            {
                //Create a list of Raycast Results
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(data, results);
                return results.Count != 0 && results[0].gameObject == slot.Zone.gameObject;
            }
        }

        public void OnBeginDrag(PointerEventData data)
        {
            dragging = true;
            dragger.alpha = 1;
            picker.alpha = 0;

            onDragBegins.Invoke(data.position);
            if(!slotless)
                DragableSlot.OnItemStartsDrag?.Invoke(slots, this);
            
            data.Use();
        }

        public void OnEndDrag(PointerEventData data)
        {
            dragging = false;
            //Animation
            dragger.alpha = 0;
            picker.alpha = 1;

            if (moveIconToDragEnd)
                picker.transform.position = dragger.transform.position;
            else
                picker.transform.position = zone.position;

            //Position
            dragScreenPos = data.position;

            if (!slotless)
            {
                //Last one i promise
                CheckIfHoversSlot(data);
            }

            onDragEnds.Invoke(data.position);

            if (!slotless)
            {
                //Events
                DragableSlot.OnItemEndsDrag?.Invoke(hoverSlot, this);
            }
            //Cleaning
            hoverSlot = null;
            data.Use();
        }

#endregion

        #region Slots
        
        public void AssignSlots(params DragableSlot[] slots) => this.slots = new List<DragableSlot>(slots);
        public void AddSlot(DragableSlot slot) => this.slots.Add(slot);
        public void RemoveSlot(DragableSlot slot) => this.slots.Remove(slot);

        #endregion
    }
}
