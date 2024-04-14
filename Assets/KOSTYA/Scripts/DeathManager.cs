using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    [SerializeField] private Animator animatorFade;
    
    public void playerDied(){
        Time.timeScale = 1;
        animatorFade.Play("InstFade", -1, 0);
        Invoke("loadCurrent", 3);
    }

    void loadCurrent() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
