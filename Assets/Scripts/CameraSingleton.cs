using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VridayStudio.Core
{
    public class CameraSingleton : MonoBehaviour
    {
        public static CameraSingleton Instance { get; private set; }

        public KeyCode keyboardRestartKey = KeyCode.Space;
        public KeyCode keyboardResolutionKey = KeyCode.F12;
        public Vector2 appResolution = new Vector2(1168, 1080);

        private void Awake() 
        { 
            // If there is an instance, and it's not me, delete myself.
            
            if (Instance != null && Instance != this) 
            { 
                Destroy(this.gameObject); 
            } 
            else 
            { 
                Instance = this; 
                DontDestroyOnLoad(this.gameObject);
            }
        }

        void Update() {
            if(Input.GetKeyDown(keyboardRestartKey)) RestartScene();
            if(Input.GetKeyDown(keyboardResolutionKey)) ResetResolution();
        }

        public void RestartScene() {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        public void ResetResolution() {
            Screen.SetResolution((int)appResolution.x, (int)appResolution.y, FullScreenMode.MaximizedWindow);
        }

    }
}