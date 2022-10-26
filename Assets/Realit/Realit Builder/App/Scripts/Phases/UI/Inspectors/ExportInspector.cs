using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Realit.Builder.App.Edition.UI.Inspectors
{
    [AddComponentMenu("Realit/Builder/Edition/UI/Inspectors/Export")]
    public class ExportInspector : Aokoro.UI.UIItem
    {
        [SerializeField]
        private TMP_InputField projectNameInputField;
        [SerializeField]
        private Button button;

        public string ProjectName
        {
            set
            {
                ProjectDataBuilder.ProjectName = value;
            }
        }


#if IS_BUILDER
        public void Export() => RealitBuilder.Instance.Build();
#endif

        protected override void OnUpdate()
        {
            button.interactable = RealitBuilder.CanBuild;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            projectNameInputField.SetTextWithoutNotify(ProjectDataBuilder.ProjectName);
        }
    }
}