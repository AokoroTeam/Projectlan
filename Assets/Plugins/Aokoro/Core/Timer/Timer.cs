using System;
using UnityEngine;

namespace Aokoro
{

    [Serializable]
    public class Timer
    {
        //Events
        public event Action OnTimerEnd;
        public event Action OnTimerStart;
        /// <summary>
        /// Return the normalized time of the timer every frame
        /// </summary>
        public event Action<Timer> OnTimerUpdate;

        /// <summary>
        /// Return true if the timer is completed
        /// </summary>
        public bool IsFinished
        {
            get
            {
                Update();
                return finished;
            }
        }

        /// <summary>
        /// Return the normalized time of the timer
        /// </summary>
        public float NormalizedTime => CurrentTime / MaxTime;

        public bool Running
        {
            get => running;
            private set
            {
                if (!running && value)
                    Update();

                running = value;
            }
        }

        private bool firstCheck;
        private bool playOnce;
        private bool finished;
        private bool running;
        private float currentTime = 0;
        public float MaxTime { get; private set; }

        public float TimeSpeed
        {
            get => timeSpeed;
            set => timeSpeed = Mathf.Clamp(value, 0, Mathf.Infinity);
        }
        private float timeSpeed = 1;

        private float lastTime;

        //A acceder pour incrementer le compteur automatiquement
        public float CurrentTime
        {
            get
            {
                Update();
                return currentTime;
            }
        }


        /// <summary>
        /// Creates a Timer
        /// </summary>
        /// <param name="duration">Duration of the timer.</param>
        public Timer(float duration) : this(duration, false, true) { }
        public Timer(float duration, bool playOnce) : this(duration, playOnce, true) { }

        /// <summary>
        /// Creates a Timer
        /// </summary>
        /// <param name="duration">Duration of the timer.</param>
        /// <param name="playOnce">True if the timer is only played once.</param>
        /// <param name="startRunning">True if the timer is started manually.</param>
        public Timer(float duration, bool playOnce, bool startRunning)
        {
            currentTime = 0;

            lastTime = Time.time;
            if (duration < Mathf.Epsilon)
            {
                Debug.LogError($"[Timers] Could not set max time to {duration}");
                MaxTime = Mathf.Epsilon;
            }
            else
                MaxTime = duration;

            this.playOnce = playOnce;
            Running = startRunning;

            finished = false;
        }

        public void Pause() => Running = false;
        public void Play() => Running = true;
        public void Update()
        {
            if (Running && lastTime != Time.time)
            {
                lastTime = Time.time;
                if (playOnce && finished)
                {
                    currentTime = MaxTime;
                    return;
                }
                else
                {
                    if (firstCheck)
                    {
                        firstCheck = false;
                        OnTimerStart?.Invoke();
                    }
                    if (finished)
                    {
                        finished = false;
                        currentTime = 0;
                        OnTimerUpdate?.Invoke(this);
                    }

                    currentTime += Time.deltaTime * TimeSpeed;

                    if (currentTime >= MaxTime)
                    {
                        finished = true;
                        OnTimerEnd?.Invoke();

                        currentTime = MaxTime;
                    }
                    else
                    {
                        finished = false;
                        OnTimerUpdate?.Invoke(this);
                    }
                }

                //Debug.Log(currentTime);
            }
        }
        /// <summary>
        /// Changes the current time of the Timer
        /// </summary>
        /// <param name="newTime"></param>
        public void ForceTime(float newTime) => currentTime = newTime;

        /// <summary>
        /// Restart the timer whitout clearing events
        /// </summary>
        public void Reset()
        {
            Running = true;
            finished = false;
            currentTime = 0;

            firstCheck = false;
            OnTimerStart?.Invoke();

        }
        /// <summary>
        /// Restart the timer and clears all events.
        /// </summary>
        public void HardReset()
        {
            firstCheck = true;
            finished = false;
            currentTime = 0;

            OnTimerEnd = null;
            OnTimerUpdate = null;
            OnTimerStart = null;
        }
        /// <summary>
        /// Restart the timer and clears all events.
        /// </summary>
        /// <param name="newMaxTime">New duration of the timer</param>
        public void HardReset(float newMaxTime)
        {
            HardReset();
            ChangeMaxTime(newMaxTime);
        }

        /// <summary>
        /// Changes the duration of the timer whitout touching is Current Time
        /// </summary>
        /// <param name="newMaxTime"></param>
        public void ChangeMaxTime(float newMaxTime) => MaxTime = newMaxTime;

        //Permet de considerer un timer en seconde. Raccourci pour avoir le temps actuel 
        //(Actualise le timer)
        public static implicit operator float(Timer timer) => timer.CurrentTime;

        public override string ToString() => CurrentTime.ToString();
    }
}