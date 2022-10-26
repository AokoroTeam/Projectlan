using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Aokoro.Cutscenes
{
    public class TimelineBinding
    {
        private static Dictionary<string, Component> actors;

        public static CinemachineBrain MainBrain { get; private set; }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void SearchForMainCamera()
        {
            if(actors == null)
                actors = new Dictionary<string, Component>();
            
            if (MainBrain == null)
            {
                var cam = Camera.main;
                if (cam != null && cam.TryGetComponent(out CinemachineBrain brain))
                    MainBrain = brain;
            }

        }

        public static void BindTimeline(PlayableDirector playableDirector)
        {
            TimelineAsset timeline = playableDirector.playableAsset as TimelineAsset;
            
            IEnumerable<TrackAsset> tracks = timeline.GetOutputTracks();
            //playableDirector.RebuildGraph();
            
            foreach (TrackAsset track in tracks)
            {
                string streamName = track.name.ToLower();

                if (track is CinemachineTrack)
                {
                    CinemachineTrack cinemachineTrack = track as CinemachineTrack;
                    playableDirector.SetGenericBinding(track, MainBrain);

                    foreach (TimelineClip clip in cinemachineTrack.GetClips())
                    {
                        if (clip.asset is CinemachineShot)
                        {
                            CinemachineShot shot = clip.asset as CinemachineShot;

                            if (shot.VirtualCamera.defaultValue == null && actors.TryGetValue(shot.DisplayName, out Component newBinding))
                                playableDirector.SetReferenceValue(shot.VirtualCamera.exposedName, newBinding);
                        }
                    }
                }
                else if (playableDirector.GetGenericBinding(track) == null)
                {
                    if (actors.TryGetValue(streamName, out Component newBinding))
                        playableDirector.SetGenericBinding(track, newBinding);
                }
            }

        }
        public static void ClearGraphs(PlayableDirector playableDirector)
        {
            IEnumerable<PlayableBinding> outputs = playableDirector.playableAsset.outputs;

            foreach (PlayableBinding output in outputs)
            {
                if (output.sourceObject is CinemachineTrack)
                {
                    CinemachineTrack track = output.sourceObject as CinemachineTrack;
                    playableDirector.SetGenericBinding(output.sourceObject, MainBrain);

                    foreach (TimelineClip clip in track.GetClips())
                    {
                        if (clip.asset is CinemachineShot)
                        {
                            CinemachineShot shot = clip.asset as CinemachineShot;

                            if (shot.VirtualCamera.defaultValue == null && actors.ContainsKey(shot.DisplayName))
                                playableDirector.ClearReferenceValue(shot.VirtualCamera.exposedName);
                        }
                    }
                }
                else if (output.sourceObject == null && actors.TryGetValue(output.streamName, out Component newBinding))
                {
                    playableDirector.ClearGenericBinding(output.sourceObject);
                }
            }
        }

        public static void AddComponents(ActorComponent[] components)
        {
            if (actors == null)
                actors = new Dictionary<string, Component>();

            int lenght = components.Length;
            for (int i = 0; i < lenght; i++)
            {
                ActorComponent actorComponent = components[i];
                string key = actorComponent.key.ToLower();

                if (!actors.ContainsKey(key))
                    actors.Add(key, actorComponent.component);
                else
                {
                    var conflictingComponent = actors[actorComponent.key];
                    Debug.LogError("[Timeline binding] Conflict with key " + actorComponent.key);
                    Debug.LogError("[Timeline binding] First : " + actorComponent.component.GetType(), actorComponent.component);
                    Debug.LogError("[Timeline binding] Second : " + conflictingComponent.GetType(), conflictingComponent);
                }
            }
        }
        public static void RemoveComponents(ActorComponent[] components)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode &&
                 EditorApplication.isPlaying)
                return;
#endif
            Debug.Log("a");
            if (actors == null)
                return;
            int lenght = components.Length;
            for (int i = 0; i < lenght; i++)
            {
                ActorComponent actorComponent = components[i];
                string key = actorComponent.key.ToLower();

                if (actors.ContainsKey(key))
                    actors.Remove(key);
            }
        }
    }

    [System.Serializable]
    public struct ActorComponent
    {
        public string key;
        public Component component;

        public ActorComponent(string key, Component component)
        {
            this.key = key;
            this.component = component;
        }
    }
}