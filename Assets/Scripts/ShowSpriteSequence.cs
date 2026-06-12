using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShowSpriteSequence : BaseSequence
{
        private SpriteRenderer spriteToShow;
        public bool doFade = true;
        public bool doScale;
        public bool doMove;

        [SerializeField, ShowIf("doFade")] private float fadeDuration = 1f;
        [SerializeField, ShowIf("doFade")] private Color startColor = Color.clear;
        [SerializeField, ShowIf("doFade")] private Color endColor = Color.white;
        [SerializeField, ShowIf("doScale")] private Vector3 startScale;
        [SerializeField, ShowIf("doScale")] private Vector3 endScale;
        [SerializeField, ShowIf("doScale")] private float scaleDuration;
        [SerializeField, ShowIf("doMove")] private Vector3 startPosition;
        private Vector3 currentPosition;
        [SerializeField, ShowIf("doMove")] private float moveDuration;

        public override void OnStartSequence()
        {
            if(spriteToShow == null) spriteToShow = GetComponent<SpriteRenderer>();
            StartCallback();
            
            if(doFade) spriteToShow.color = startColor;
            if(doScale) spriteToShow.transform.localScale = startScale;
            if(doMove) {
                currentPosition = spriteToShow.transform.localPosition;
                spriteToShow.transform.localPosition =  startPosition;
            }
        }

        public Sequence CreateSequence() {
            if(spriteToShow == null) spriteToShow = GetComponent<SpriteRenderer>();
            Sequence newSequence = DOTween.Sequence();

            if(doFade)
                newSequence.Join(spriteToShow.DOColor(endColor, fadeDuration));
            if(doScale)
                newSequence.Join(spriteToShow.transform.DOScale(endScale, scaleDuration));
            if(doMove)
                newSequence.Join(spriteToShow.transform.DOLocalMove(currentPosition, moveDuration));

            newSequence.OnUpdate(UpdateCallback);
            newSequence.AppendCallback(FinishedCallback);
            return newSequence;
        }

        public override Sequence GetSequences() => CreateSequence();
}