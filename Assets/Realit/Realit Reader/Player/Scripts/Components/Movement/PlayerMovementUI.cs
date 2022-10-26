using Aokoro.UI.ControlsDiplaySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Reader.Managers;

namespace Realit.Reader.Player.Movement
{
    [AddComponentMenu("Realit/Reader/Player/Movement/UI")]
    public class PlayerMovementUI : CD_Displayer
    {
        private void Awake()
        {
            SceneManager.OnPlayerIsSetup += OnPlayerIsCreatedCallback;
        }

        private void OnPlayerIsCreatedCallback(Realit_Player player)
        {
            SceneManager.OnPlayerIsSetup -= OnPlayerIsCreatedCallback;
            PlayerCharacter playerCharacter = player.GetLivingComponent<PlayerCharacter>();
            playerCharacter.UI = this;
            AssignActionProvider(playerCharacter);
        }
    }
}