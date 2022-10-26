using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Aokoro.Sequencing.Steps
{
    public class WaitActionStep : IStep
    {
        private bool called;
#pragma warning disable IDE0052 // Supprimer les membres privés non lus
        private Action action;
#pragma warning restore IDE0052 // Supprimer les membres privés non lus

        public WaitActionStep(Action callback)
        {
            action = callback;
        }

        void IStep.OnBegin()
        {
            called = false;
            action += OnAction;
        }

        void IStep.OnEnd()
        {
            called = false;
            action -= OnAction;
        }

        bool IStep.Tick(ISequencer parent) => called;

        private void OnAction() => called = true;
    }
}
