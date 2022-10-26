using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;
using Aokoro;
using Aokoro.Sequencing;

namespace Realit.Reader.Managers
{
    public class GameNotifications : Singleton<GameNotifications>
    {
        [SerializeField]
        GameObject topRight;


        public void TriggerNotification(string title, string content, float duration, float time)
        {
            SequencerBuilder.Begin()
                .WaitForSeconds(time)
                .Do(() => TriggerNotification(title, content, duration))
                .Build()
                .AddDependances(this)
                .Play(SequenceUpdateType.Update);
        }


        public void TriggerNotification(string title, string content, float duration)
        {
            NotificationManager notification = Instantiate(topRight, transform).GetComponent<NotificationManager>();

            notification.title = title;
            notification.description = content;
            notification.timer = duration;
            notification.UpdateUI();
            notification.OpenNotification();
        }

        protected override void OnExistingInstanceFound(GameNotifications existingInstance)
        {

        }
    }
}