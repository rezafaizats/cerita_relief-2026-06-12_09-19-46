using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using DG.Tweening;

public class SequenceController : MonoBehaviour
{
    enum PlayOptions
    {
        WITH_PREVIOUS,
        AFTER_PREVIOUS
    }

    [Serializable]
    struct SequenceProperty
    {
        public BaseSequence sequence;
        public PlayOptions playOption;
    }

    [SerializeField] private SequenceProperty[] sequences;
    public UnityEvent onStartSequence;
    public UnityEvent onFinishedSequence;
    public UnityEvent onKilledSequence;

    [SerializeField] private bool playOnStart;

    private Sequence mainSeq;

    private void Start()
    {
        if(playOnStart) OnStartPageSequence(PlayPageSequence);
    }

    public void OnStartPageSequence() {
        
        mainSeq = DOTween.Sequence();

        foreach (var seq in sequences)
            seq.sequence.OnStartSequence();
        
        foreach (var seq in sequences)
        {
            var s = seq.sequence.GetSequences();
            
            switch (seq.playOption)
            {
                case PlayOptions.WITH_PREVIOUS:
                    mainSeq.Join(s);
                    break;
                case PlayOptions.AFTER_PREVIOUS:
                    mainSeq.Append(s);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        mainSeq.OnComplete(OnFinishedSequenceCallback);
    }
    public void OnStartPageSequence(Action onDoneCallback) {
        
        mainSeq = DOTween.Sequence();

        foreach (var seq in sequences)
            seq.sequence.OnStartSequence();        

        foreach (var seq in sequences)
        {
            var s = seq.sequence.GetSequences();
            
            switch (seq.playOption)
            {
                case PlayOptions.WITH_PREVIOUS:
                    mainSeq.Join(s);
                    break;
                case PlayOptions.AFTER_PREVIOUS:
                    mainSeq.Append(s);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        mainSeq.OnComplete(OnFinishedSequenceCallback);
        
        onDoneCallback?.Invoke();
    }

    public void PlayPageSequence() {

        mainSeq.Play();
        onStartSequence?.Invoke();
    }

    public void KillCurrentSequence()  {
        onKilledSequence?.Invoke();
        mainSeq.Kill();
        print("killed");
    }

    public void ResetCurrentSequence() {
        foreach (var seq in sequences)
            seq.sequence.OnStartSequence();
    }

    protected void OnFinishedSequenceCallback() => onFinishedSequence?.Invoke();
}