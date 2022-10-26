using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG;
using UnityEngine.Events;
using System;

namespace Aokoro.UI
{
    public class DragableSlot : UIItem
    {
        public static Action<List<DragableSlot>, DragableItem> OnItemStartsDrag;
        public static Action<DragableSlot, DragableItem> OnItemEndsDrag;

        [SerializeField]
        private RectTransform zone;
        public RectTransform Zone => zone;

        [SerializeField]
        private UnityEvent<DragableSlot, DragableItem> OnItemDrop;
        [SerializeField]
        private UnityEvent<DragableSlot, DragableItem> OnItemEnter;
        [SerializeField]
        private UnityEvent<DragableSlot, DragableItem> OnItemExit;


        private void OnValidate()
        {

            if (zone == null)
                zone = transform as RectTransform;

        }

        protected override void Awake()
        {
            if(zone == null)
                zone = transform as RectTransform;

            base.Awake();
        }
        protected override void OnEnable()
        {
            OnItemStartsDrag += DragableSlot_OnItemStartsDrag;
            OnItemEndsDrag += DragableSlot_OnItemEndsDrag;

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            OnItemStartsDrag += DragableSlot_OnItemStartsDrag;
            OnItemEndsDrag -= DragableSlot_OnItemEndsDrag;

            base.OnDisable();
        }

        protected override void OnUpdate()
        {

        }


        private void DragableSlot_OnItemStartsDrag(List<DragableSlot> compatibleSlots, DragableItem item)
        {
            if(compatibleSlots.Contains(this))
            {
                //Enters waiting state
            }
        }

        private void DragableSlot_OnItemEndsDrag(DragableSlot slot, DragableItem item)
        {
            if(slot == this)
                OnItemDrop.Invoke(this, item);


            //Enters sleep state
        }


        public void OnDragableItemEnter(DragableItem item)
        {
            OnItemEnter.Invoke(this, item);

            //Enters hover state state
        }
        public void OnDragableItemExit(DragableItem item)
        {
            OnItemExit.Invoke(this, item);

            //Enters waiting state
        }

    }
}
