using Aokoro;
using Michsky.UI.ModernUIPack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

namespace Realit.Builder.App.Edition.UI
{
    [AddComponentMenu("Realit/Builder/Edition/UI/Inspector")]
    public class Inspector : MonoBehaviour
    {
        //To get the canvas size for animations
        private RectTransform canvasRect;
        private Canvas canvas;
        //Button script that manages the side button behavior
        private ButtonManagerBasicIcon button;

        [SerializeField, BoxGroup("Side button")]
        private Sprite openArrow;
        [SerializeField, BoxGroup("Side button")]
        private Sprite closeArrow;

        [SerializeField, BoxGroup("Content")]
        //Object that contains every content object
        private GameObject ContentParent;

        private Dictionary<EditionFunctionality, GameObject> contents;
        private bool closed = true;

        //Called on first frame
        private void Awake()
        {
            button = GetComponentInChildren<ButtonManagerBasicIcon>();
            canvas = GetComponentInParent<Canvas>();
            canvasRect = canvas.transform as RectTransform;
            contents = new Dictionary<EditionFunctionality, GameObject>();
        }

        #region Animations
        //Called by the button
        [Button]
        public void OpenOrClose() { if (closed) Open(); else Close(); }

        public void TryOpen()
        {
            if (!closed) 
                Open();
        }
        //Opens the inspector
        private void Open()
        {
            closed = false;
            RectTransform rectTransform = transform as RectTransform;
            //Translates to opened position
            rectTransform.DOKill();
            float endValue = -rectTransform.rect.width;
            rectTransform.DOAnchorPosX(endValue, 0.25f, false);

            //Changing the sprite
            button.buttonIcon = closeArrow;
            button.UpdateUI();
        }

        //Closes the inspector
        private void Close()
        {
            closed = true;
            RectTransform rectTransform = transform as RectTransform;
            //Translates to closed position
            rectTransform.DOKill();
            rectTransform.DOAnchorPosX(0, 0.25f, true);


            //Changing the sprite
            button.buttonIcon = openArrow;
            button.UpdateUI();
        }

        #endregion
        #region Content Management

        public void DisplayFunctionnality(EditionFunctionality functionality, GameObject prefab)
        {
            //Instantiates contents if was not already present
            if (!contents.TryGetValue(functionality, out GameObject content))
            {
                content = Instantiate(prefab, ContentParent.transform);
                contents.Add(functionality, content);
            }

            //Displays the correct content
            foreach (var kvp in contents)
                kvp.Value.SetActive(kvp.Key == functionality);

            Canvas.ForceUpdateCanvases();
        }

    #endregion
        //Called when become active
        private void OnEnable()
        {
            Close();
        }

    }
}