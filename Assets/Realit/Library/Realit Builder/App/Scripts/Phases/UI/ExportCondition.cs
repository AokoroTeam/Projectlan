using Realit.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Builder.App.Edition.UI
{
    [AddComponentMenu("Realit/Builder/Edition/UI/Export Condition")]
    public class ExportCondition : Aokoro.UI.UIItem
    {
        public DataSection section;
        public GameObject validIcon, unvalidIcon;

        protected override void OnUpdate()
        {
            switch (section)
            {
                case DataSection.Project:
                    SetValidOrNot(RealitBuilder.IsProjectSetup);
                    break;
                case DataSection.Model:
                    SetValidOrNot(RealitBuilder.IsModelSetup);
                    break;
                case DataSection.Player:
                    SetValidOrNot(RealitBuilder.IsPlayerSetup);
                    break;
            }
        }

        private void SetValidOrNot(bool valid)
        {
            validIcon.SetActive(valid);
            unvalidIcon.SetActive(!valid);
        }
    }
}
