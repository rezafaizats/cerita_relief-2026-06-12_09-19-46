using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public abstract class BaseSequence : MonoBehaviour
{
        public UnityEvent onStart;
        public UnityEvent onUpdate;
        public UnityEvent onFinished;

        public abstract void OnStartSequence();
        
        public abstract Sequence GetSequences();
        
        protected void StartCallback() => onStart?.Invoke();
        protected void FinishedCallback() => onFinished?.Invoke();
        protected void UpdateCallback() => onUpdate?.Invoke();
}
