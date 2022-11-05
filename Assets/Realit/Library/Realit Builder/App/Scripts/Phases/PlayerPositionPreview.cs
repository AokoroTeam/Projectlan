using Aokoro;
using Realit.Builder.App.Edition.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Realit.Builder.App.Edition
{
    public class PlayerPositionPreview : Singleton<PlayerPositionPreview>, IPhaseComponent
    {
        public Phase phase => Phase.Edition;

        public AssetReferenceGameObject playerPreview;
        public GameObject playerPreviewInstance;

        public static event Action OnEditingStarts;
        public static event Action OnEditingEnds;


        private void OnEnable()
        {
            EditionWindow.OnFunctionnalityIsDisplayed += OnFunctionnalityIsDisplayed;
            RealitBuilder.OnPhaseChanges += OnPhaseChange;
        }

        private void OnDisable()
        {
            EditionWindow.OnFunctionnalityIsDisplayed -= OnFunctionnalityIsDisplayed;
            RealitBuilder.OnPhaseChanges -= OnPhaseChange;
        }

        private void OnFunctionnalityIsDisplayed(EditionFunctionality from, EditionFunctionality to)
        {
            //Leaving
            if (from == EditionFunctionality.Player)
                ExitPlayerSettingsEdition();
            //Entering
            else if (to == EditionFunctionality.Player)
                EnterPlayerSettingsEdition();
        }

        public void EnterPlayerSettingsEdition()
        {
            if (this.IsActive())
            {
                //Debug.Log("[Player Settings] Starting edition...");
                OnEditingStarts?.Invoke();
            }
        }

        public void ExitPlayerSettingsEdition()
        {
            OnEditingEnds?.Invoke();
            //Debug.Log("[Player Settings] Ending edition...");
        }

        public void SetPlayerPositionWithScreenPoint(Vector2 screenPoint)
        {
            var ray = Camera.main.ScreenPointToRay(screenPoint);

            //If hits something
            if (Physics.Raycast(ray, out RaycastHit info))
            {
                //If valid
                if (Vector3.Dot(info.normal.normalized, Vector3.up) > .85)
                {
                    Vector3 position = info.point;
                    Quaternion rotation = playerPreviewInstance.transform.rotation;

                    PlayerDataBuilder.SetPlayerPosition(position);
                    PlayerDataBuilder.SetPlayerRotation(rotation.eulerAngles);
                    Debug.Log($"[Player Settings] Setting player position to {position}");

                    if (playerPreviewInstance != null)
                    {
                        playerPreviewInstance.transform.SetPositionAndRotation(position, rotation);
                        playerPreviewInstance.SetActive(true);
                    }
                    return;
                }
            }

            Debug.Log($"[Player Settings] Could find a valid surface for the player start");
            PlayerDataBuilder.ClearPlayerRotation();
            PlayerDataBuilder.ClearPlayerPosition();

            if (playerPreviewInstance != null)
                playerPreviewInstance.SetActive(false);
        }


        public void PreviewPlayer(Vector2 screenPosition)
        {
            if (playerPreviewInstance != null)
            {
                var ray = Camera.main.ScreenPointToRay(screenPosition);

                //If hits something and its not UI
                if (UIExtentions.Raycast(screenPosition) == 0 && Physics.Raycast(ray, out RaycastHit info))
                {
                    bool valid = Vector3.Dot(info.normal.normalized, Vector3.up) > .85;
                    if (valid)
                    {
                        playerPreviewInstance.transform.position = info.point;
                        playerPreviewInstance.SetActive(true);
                        return;
                    }
                }

                playerPreviewInstance.SetActive(false);
            }
        }

        public void OnPhaseChange(Phase to)
        {
            if (to == this.phase)
            {
                //If player not loaded
                if (playerPreview.Asset == null && !playerPreview.IsValid())
                {
                    Debug.Log("[Player Settings] Loading player preview...");
                    //Loading it first
                    playerPreview.LoadAssetAsync().Completed += ctx => InstantiatePlayerPreview();
                }
            }
        }

        private void InstantiatePlayerPreview()
        {
            Debug.Log("[Player Settings] Instantiating player preview...");
            //Instantiating it
            var asyncOperation = playerPreview.InstantiateAsync(transform);
            asyncOperation.Completed += p =>
            {
                playerPreviewInstance = p.Result;
                playerPreviewInstance.transform.SetPositionAndRotation(PlayerDataBuilder.Position, Quaternion.Euler(PlayerDataBuilder.Rotation));
                playerPreviewInstance.SetActive(PlayerDataBuilder.Instance.IsValid);
            };
        }

        protected override void OnExistingInstanceFound(PlayerPositionPreview existingInstance)
        {
            Destroy(gameObject);
        }
    }
}