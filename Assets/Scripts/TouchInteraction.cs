using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Lean.Touch;

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

        void Start()
        {
            LeanTouch.OnFingerUpdate += ShowTouchIndicator;
            
            LeanTouch.OnFingerDown += GetStartPosition;

            LeanTouch.OnFingerUp += FlipPage;
            LeanTouch.OnFingerUp += HideTouchIndicator;

            LeanTouch.OnFingerUpdate += UIRaycaster;

            maxScreenSize.x = Screen.width;
            maxScreenSize.y = Screen.height;
        }

        void OnDestroy() {
            LeanTouch.OnFingerUpdate -= ShowTouchIndicator;

            LeanTouch.OnFingerDown -= GetStartPosition;

            LeanTouch.OnFingerUp -= FlipPage;
            LeanTouch.OnFingerUp -= HideTouchIndicator;
            
            LeanTouch.OnFingerUpdate -= UIRaycaster;
        }  
        public void GetStartPosition(LeanFinger finger) {
            startFingerPos = finger.StartScreenPosition;
        }

        void Update() {

        }
        public void FlipPage(LeanFinger finger) {
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