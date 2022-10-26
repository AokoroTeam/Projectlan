using Aokoro.UI.ControlsDiplaySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Michsky.UI.ModernUIPack;
using Aokoro.UI;

namespace Realit.Reader.Features.SliceView
{
    [AddComponentMenu("Realit/Reader/Features/SliceView/UI")]
    public class SliceView_UI : FeatureComponent<SliceView>
    {
        private const string windowName = "SliceViewWindow";

        CD_Displayer displayer;
        int lastWindow;

        protected override void Awake()
        {
            displayer = GetComponent<CD_Displayer>();
            base.Awake();
        }

        protected override void OnFeatureInitiate()
        {
            Aokoro.UI.WindowManager.MainUI.AddWindow(windowName, gameObject);

            displayer.AssignActionProvider(feature.Player, false);
        }

        protected override void OnFeatureStarts()
        {
            Aokoro.UI.WindowManager.MainUI.OpenWindow(windowName);
            displayer.Show();
        }
        protected override void OnFeatureEnds()
        {
            displayer.Hide();
            Aokoro.UI.WindowManager.MainUI.OpenWindow(Aokoro.UI.WindowManager.MainUI.DefaultWindow);
        }
    }
}