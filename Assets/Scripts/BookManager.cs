using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BookCurlPro;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace VridayStudio.Book
{    
    public class BookManager : MonoBehaviour
    {
        public BookPro currentBook;
        public List<PagesInNumber> pages;
        public bool isAutoFlip = false;
        public bool changeBookAfterEndPage = false;
        public bool isInteractable = false;
        private bool isOpeningBookPanel = false;
        [SerializeField, ShowIf("isAutoFlip")] private float waitFlipDuration;
        [SerializeField, ShowIf("isAutoFlip")] private AutoFlip autoFlip;
        public BookMenu bookMenu;

        private GameObject FadeEffect;

        [Serializable] public class PagesInNumber {
            public int pagesNumber;
            public SequenceController pageSequences;
            public PageController pageLeft;
            public PageController pageRight;
        }

        // Start is called before the first frame update
        void Start()
        {
            if(isAutoFlip) StartCoroutine(FlipBook(waitFlipDuration));
            if(bookMenu is null) throw new Exception("Book menu must not be null!");
            Debug.Log("Current book page " + currentBook.currentPaper);
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightArrow)) FlipNextPage();
            if(Input.GetKeyDown(KeyCode.LeftArrow)) FlipPreviousPage();
        }

        public void ResetCurrentPage() {
            int currentPage = currentBook.currentPaper;
            print(currentBook.PreviousPaper + " " + currentPage);
            foreach (var item in pages)
                if(item.pagesNumber == currentPage) item.pageSequences.OnStartPageSequence(item.pageSequences.PlayPageSequence);
        }

        public void PlayCurrentPage() {
            int currentPage = currentBook.currentPaper;
            foreach (var item in pages)
                if(item.pagesNumber == currentPage) item.pageSequences.OnStartPageSequence(item.pageSequences.PlayPageSequence);
        }

        public void KillPreviousSequence() {
            int previousPage = Mathf.Abs(currentBook.PreviousPaper);
            print(previousPage);
            foreach (var item in pages) {
                if(item.pagesNumber == previousPage) item.pageSequences.KillCurrentSequence();
            }
        }

        IEnumerator FlipBook(float duration) {
            while (true) {
                yield return new WaitForSeconds(duration);
                FlipNextPage();
            }
        }

        public void FlipNextPage() {
            if(isOpeningBookPanel || !isInteractable) return;

            int currentPaper = currentBook.CurrentPaper;
            pages[currentPaper].pageLeft.StopVideo();
            pages[currentPaper].pageRight.StopVideo();
            if (currentPaper >= pages[pages.Count - 2].pagesNumber) bookMenu.SetYoyoAnimation(true);
            if(currentPaper < pages[pages.Count - 1].pagesNumber) autoFlip.FlipRightPage();
            currentPaper = currentBook.CurrentPaper;
            pages[currentPaper].pageLeft.StartVideo();
            pages[currentPaper].pageRight.StartVideo();
            // else if(changeBookAfterEndPage) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex == 0 ? 1 : 0);
        }

        public void FlipPreviousPage() {
            if(isOpeningBookPanel || !isInteractable) return;

            Debug.Log("Flip Previous Page.");
            int currentPaper = currentBook.CurrentPaper;
            Debug.Log("Previous page " + currentPaper);
            pages[currentPaper].pageLeft.StopVideo();
            pages[currentPaper].pageRight.StopVideo();
            if(currentPaper > pages[0].pagesNumber) autoFlip.FlipLeftPage();
            Debug.Log("current page " + currentPaper);
            currentPaper = currentBook.CurrentPaper;
            pages[currentPaper - 1].pageLeft.StartVideo();
            pages[currentPaper - 1].pageRight.StartVideo();
        }

        public void ResetBook()
        {
            int currentPaper = currentBook.CurrentPaper;
            if (currentPaper == 0) return;

            Debug.Log("Resetting book...");
            FlipPreviousPage();
        }

        public void SetBookMenuVisibility(bool status) {
            bookMenu.SetBookPanelVisibility(status);
            isOpeningBookPanel = status;
        }

        public void FadeOutEffect() {
            GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
            GameObject fade = Instantiate(FadeEffect);
            fade.transform.SetParent(mainCanvas.transform);
        }

        public void FadeInEffect() {
            GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
            GameObject fade = Instantiate(FadeEffect);
            fade.transform.SetParent(mainCanvas.transform);
        }
    }
    
}