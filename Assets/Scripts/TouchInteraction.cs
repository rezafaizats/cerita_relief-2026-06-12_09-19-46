using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Lean.Touch;
using System;
using Unity.VisualScripting;
using System.Linq;

namespace VridayStudio.Book
{
    
    public class TouchInteraction : MonoBehaviour
    {
        public BookManager book;

        public float swipeDistanceMinimum = 150f;
        public EventSystem currentEventSystem;
        public GraphicRaycaster mainCanvas;

        public Image touchIndicator;
        private Image spawnedTouchIndicator;
        private Vector2 touchIndicatorPosition;
        public Image touchProgressIndicatorPrefab;
        private Image spawnedTouchProgressIndicator = null;
        private ITouchedObject activeTouchObject = null;

        public bool isInteractable = true;
        public bool touchStatus = true;
            
        PointerEventData pointerEventData;

        private Vector2 maxScreenSize;
        private Vector2 startFingerPos;    
        private Vector2 endFingerPos;        
        private float touchDuration = 0f;
        public float maxTouchDuration = 2f;
        public float pauseTouchDuration = 5f;
        
        public float idleDuration = 5f;
        private List<LeanFinger> allFingers = new List<LeanFinger>();
        private bool isIdle = false;
        private bool isRestarting = false;
        [SerializeField] private float currentIdleDuration = 0f;

        void Start()
        {
            LeanTouch.OnFingerUpdate += ShowTouchIndicator;
            
            LeanTouch.OnFingerDown += GetStartPosition;

            LeanTouch.OnFingerUp += FlipPage;
            LeanTouch.OnFingerUp += HideTouchIndicator;

            LeanTouch.OnFingerDown += RegisterFinger;
            LeanTouch.OnFingerUp += DeregisterFinger;

            //LeanTouch.OnFingerUpdate += UIRaycaster;

            maxScreenSize.x = Screen.width;
            maxScreenSize.y = Screen.height;
        }

        void OnDestroy() {
            LeanTouch.OnFingerUpdate -= ShowTouchIndicator;

            LeanTouch.OnFingerDown -= GetStartPosition;

            LeanTouch.OnFingerUp -= FlipPage;
            LeanTouch.OnFingerUp -= HideTouchIndicator;

            LeanTouch.OnFingerDown -= RegisterFinger;
            LeanTouch.OnFingerUp -= DeregisterFinger;

            //LeanTouch.OnFingerUpdate -= UIRaycaster;
        }

        private void FixedUpdate()
        {
            if (!isIdle && !isRestarting && book.currentBook.currentPaper == 0) return;

            currentIdleDuration += Time.deltaTime;
            if(currentIdleDuration >= idleDuration)
            {
                Debug.Log("Resetting book page...");
                isRestarting = true;
                touchStatus = false;
                book.ResetBook();
                if (book.currentBook.currentPaper == 0) {
                    currentIdleDuration = 0;
                    isRestarting = false;
                    isIdle = false;
                    touchStatus = true;
                    Debug.Log("Done resetting.");
                }
            }
        }

        public void GetStartPosition(LeanFinger finger) {
            startFingerPos = finger.StartScreenPosition;
        }

        public void FlipPage(LeanFinger finger) {
            if (!touchStatus) return;
            endFingerPos = finger.LastScreenPosition;

            float distance = Vector2.Distance(startFingerPos, endFingerPos);
            float middlePoint = maxScreenSize.x / 2;
            
            if(!isInteractable) return;

            //Right screen
            if(startFingerPos.x > middlePoint) {
                if(startFingerPos.x > endFingerPos.x && distance > swipeDistanceMinimum) {
                    book.FlipNextPage();
                }
            }
            
            //Left screen
            if(startFingerPos.x < middlePoint) {
                if(startFingerPos.x < endFingerPos.x && distance > swipeDistanceMinimum) {
                    book.FlipPreviousPage();
                }
            }
        }

        public void UIRaycaster (LeanFinger finger) {
            if(!touchStatus) return;

            pointerEventData = new PointerEventData(currentEventSystem);
            //Set the Pointer Event Position to that of the game object
            pointerEventData.position = finger.ScreenPosition;
    
            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();
    
            //Raycast using the Graphics Raycaster and mouse click position
            mainCanvas.Raycast(pointerEventData, results);

            
            if(touchDuration >= maxTouchDuration) {
                touchDuration = 0f;
                activeTouchObject.UIEvent?.Invoke();
                StartCoroutine(PauseTouch(pauseTouchDuration));
                
                Destroy(spawnedTouchProgressIndicator);
                spawnedTouchProgressIndicator = null;
            }

            if(results.Count > 0) {
                if(!results[0].gameObject.TryGetComponent(out ITouchedObject touched)) {
                    touchDuration = 0f;

                    if(spawnedTouchProgressIndicator != null)
                        spawnedTouchProgressIndicator.gameObject.SetActive(false);
                }
                else {
                    if(activeTouchObject == touched) {
                        touchDuration += Time.deltaTime;

                        if(spawnedTouchProgressIndicator == null) {
                            spawnedTouchProgressIndicator = Instantiate(touchProgressIndicatorPrefab, spawnedTouchIndicator.rectTransform);
                            spawnedTouchProgressIndicator.fillAmount = 0f;
                        }
                        else {
                            spawnedTouchProgressIndicator.gameObject.SetActive(true);
                            spawnedTouchProgressIndicator.rectTransform.position = spawnedTouchIndicator.rectTransform.position;
                            spawnedTouchProgressIndicator.fillAmount = touchDuration / maxTouchDuration;
                        }
                    }
                    else if (activeTouchObject != touched) {
                        activeTouchObject = touched;
                        touchDuration = 0f;

                        if(spawnedTouchProgressIndicator != null)
                            spawnedTouchProgressIndicator.gameObject.SetActive(false);
                    }
                    else {
                        activeTouchObject.UIEvent?.Invoke();

                        touchDuration = 0f;

                        if(spawnedTouchProgressIndicator != null)
                            spawnedTouchProgressIndicator.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void ShowTouchIndicator(LeanFinger touch) {
            if(spawnedTouchIndicator == null) {
                spawnedTouchIndicator = Instantiate(touchIndicator, mainCanvas.transform);
                spawnedTouchIndicator.transform.position = touch.ScreenPosition;
            }
            else {
                spawnedTouchIndicator.transform.position = touch.ScreenPosition;
            }
        }

        public void HideTouchIndicator(LeanFinger touch) {
            if(spawnedTouchIndicator == null) return;

            Destroy(spawnedTouchIndicator.gameObject);
            spawnedTouchIndicator = null;
        }

        private void RegisterFinger(LeanFinger finger)
        {
            if (allFingers.Contains(finger)) return;
            allFingers.Add(finger);
            isIdle = false;
            touchStatus = true;
            currentIdleDuration = 0f;
            Debug.Log("Finger registered " + finger);
        }

        private void DeregisterFinger(LeanFinger finger)
        {
            if (!allFingers.Contains(finger)) return;

            allFingers.Remove(finger);
            Debug.Log("Finger deregistered " + finger);

            if(allFingers.Count == 0)
            {
                isRestarting = false;
                isIdle = true;
                Debug.Log("Finger empty!");
            }
        }

        private IEnumerator ResetPage()
        {
            isRestarting = true;
            touchStatus = false;
            yield return new WaitForSeconds(idleDuration);
            book.ResetBook();
            isRestarting = false;
            touchStatus = true;
        }

        public IEnumerator PauseTouch(float duration = 2f) {
            if(!touchStatus) yield return null;
            touchStatus = false;
            yield return new WaitForSeconds(duration);
            touchStatus = true;
        }

        public void SetInteraction(bool status) {
            isInteractable = status;
        }
    }

}