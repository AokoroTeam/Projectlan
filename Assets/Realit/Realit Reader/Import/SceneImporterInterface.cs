using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Realit.Reader.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SceneImporterInterface : MonoBehaviour
    {
        CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

        }
        private void OnEnable()
        {
            RealitReader.Instance.OnStartsLoading += Instance_OnStartsLoading;
        }

        private void OnDisable()
        {
            RealitReader.Instance.OnStartsLoading -= Instance_OnStartsLoading;
        }
        private void Instance_OnStartsLoading()
        {
            gameObject.SetActive(false);
        }

        public void TriggerLoading()
        {
            RealitReader.Instance.AskForScene();
        }

        private void Update()
        {
            canvasGroup.interactable = RealitReader.Instance.IsReadyForLoading;
        }
    }
}