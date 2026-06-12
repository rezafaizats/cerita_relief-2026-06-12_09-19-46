using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ChangeBook : MonoBehaviour
{

    [Scene] public string targetScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeBooks() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(targetScene);
    }

}
