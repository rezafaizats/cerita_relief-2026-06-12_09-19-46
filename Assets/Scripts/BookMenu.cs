using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace VridayStudio.Book
{    
    public class BookMenu : MonoBehaviour
    {
        public GameObject bookPanel;
        public GameObject closeBookButton;

        [SerializeField] private Transform openBookButton;

        private Tween currentTween;

        void Start() {
        }
        public void SetBookPanelVisibility(bool status) {
            bookPanel.SetActive(status);
            closeBookButton.SetActive(status);
        }

        public void SetYoyoAnimation(bool val = true) {
            print("Play Animation");
            DOTween.Init();
            if(val && currentTween == null) 
                currentTween = openBookButton.DOScale(1.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }

        void OnDestroy() {
            currentTween.Kill();
        }

    }
   
}