using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathManager : MonoBehaviour
{
    [SerializeField] private Animator animatorFade;

    private bool alreadyDied;
    
    public void playerDied(){
        if (alreadyDied) return;
        alreadyDied = true;
        Time.timeScale = 1;
        animatorFade.Play("InstFade", -1, 0);
        Invoke("loadCurrent", 3);
    }

    void loadCurrent() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
