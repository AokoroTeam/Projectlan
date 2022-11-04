using Aokoro;
using TMPro;
using UnityEngine;

namespace Realit.UI
{
    public class LoadingPanel : MonoBehaviour
    {
        InfluencedProperty<(float, string)> progresses;

        [SerializeField]
        private Michsky.UI.ModernUIPack.ProgressBar bar;
        [SerializeField]
        private TextMeshProUGUI tmp_label;
        
        
        private void Awake()
        {
            progresses = new InfluencedProperty<(float, string)>((0, "..."));
            gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            bar.currentPercent = 0;
            bar.UpdateUI();
        }

        private void LateUpdate()
        {
            if (progresses.InfluencerCount == 0)
                gameObject.SetActive(false);
        }



        public void StartLoadingScreen(object key, int priority)
        {
            progresses.Subscribe(key, priority);
            gameObject.SetActive(true);
        }

        public void StopLoadingScreen(object key)
        {
            progresses.Unsubscribe(key);
            if (progresses.InfluencerCount == 0)
                gameObject.SetActive(false);
        }

        public void UpdateBar(object key, float progress, string label)
        {
            if(progresses.IsInfluencedBy(key))
                progresses.Set(key, (progress, label));


            (float, string) value = progresses.Value;

            bar.currentPercent = value.Item1;
            tmp_label.SetText(value.Item2);
            
            bar.UpdateUI();
            //Canvas.ForceUpdateCanvases();
        }

    }
}