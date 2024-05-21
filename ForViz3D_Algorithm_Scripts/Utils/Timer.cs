using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Utils
{
    public abstract class Timer
    {
        protected float _initialTime;
        protected float Time { get; set; }
        public bool IsRunning { get; private set; }

        public float InProgress => Time / _initialTime;

        public Action OnTimerStart = delegate { };
        public Action<GameObject> OnTimerStartGO = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value)
        {
            _initialTime = value;
            IsRunning = false;
        }

        public void Start()
        {
            Time = _initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                OnTimerStop.Invoke();
            }
        }

        public void Pause() => IsRunning = false;
        public void Resume() => IsRunning = true;

        public abstract void Tick(float deltaTime);

    }

    public class StopWatch : Timer
    {
        public StopWatch(float value) : base(value)
        {
        }
        public override void Tick(float deltaTime)
        {
            Time += deltaTime;
        }
        public float GetFinishTime()
        {
            return Time;
        }
    }
    
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value) : base(value) { }

        public override void Tick(float deltaTime)
        {
            if (IsRunning && Time > 0)
            {
                Time -= deltaTime;
            }

            if (IsRunning && Time <= 0)
            {
                Stop();
            }
        }

        public bool IsFinished => Time <= 0;

        public void Reset() => Time = _initialTime;

        public void Reset(float newTime)
        {
            _initialTime = newTime;
            Reset();
        }

        public void ResetAndStart()
        {
            Reset();
            Start();
        }
    }
}
