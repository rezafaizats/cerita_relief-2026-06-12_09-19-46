using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class ShowTextSequence : BaseSequence
{
    private TextMeshPro textToShow;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnStartSequence()
    {
        if(textToShow == null) textToShow = GetComponent<TextMeshPro>();
        StartCallback();
        
        if(doFade) textToShow.color = startColor;
        if(doScale) textToShow.transform.localScale = startScale;
        if(doMove) {
            currentPosition = textToShow.transform.position;
            textToShow.transform.position =  startPosition;
        }
    }

    public Sequence CreateSequence() {
        if(textToShow == null) textToShow = GetComponent<TextMeshPro>();
        Sequence newSeq = DOTween.Sequence();

        if(doFade) newSeq.Join(textToShow.DOColor(endColor, fadeDuration));
        if(doScale) newSeq.Join(textToShow.transform.DOScale(endScale, scaleDuration));
        if(doMove)  newSeq.Join(textToShow.transform.DOLocalMove(currentPosition, moveDuration));

        return newSeq;
    }
    
    public override Sequence GetSequences() => CreateSequence();
}
