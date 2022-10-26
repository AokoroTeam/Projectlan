using Aokoro.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Reader.Managers;

namespace Realit.Reader.Features.SliceView
{

    public class SliceView : Feature
    {
        public override string FeatureName => "Vue coupée";
        public SliceView_Player Player { get; private set; }
        public SliceView_UI UI { get; private set; }
        public SliceView_VirtualCamera VirtualCamera { get; private set; }
        public SliceView_SceneManager SceneManager { get => SliceView_SceneManager.Instance; }

        private GameObject P_PlayerComponent;
        private GameObject P_VirtualCamera;
        private GameObject P_UI;

        public SliceView(params GameObject[] ressources)
        {
            this.P_PlayerComponent = ressources[0];
            this.P_VirtualCamera = ressources[1];
            this.P_UI = ressources[2];
        }

        protected override void GenerateNeededContentOnSetup(SceneManager manager)
        {
            //Add player Component
            Player = GameObject.Instantiate(P_PlayerComponent, manager.Player.FeaturesRoot).GetComponent<SliceView_Player>();
            //Add UI
            UI = GameObject.Instantiate(P_UI, WindowManager.MainUI.WindowsParent).GetComponent<SliceView_UI>();

            //Add camera
            VirtualCamera = GameObject.Instantiate(P_VirtualCamera).GetComponent<SliceView_VirtualCamera>();
        }

        public override void CleanContentOnDestroy(SceneManager controller)
        {
            GameObject.Destroy(Player);
            GameObject.Destroy(UI.gameObject);
        }

        protected override void OnFeatureStarts()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        protected override void OnFeatureEnds()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

    }
}
