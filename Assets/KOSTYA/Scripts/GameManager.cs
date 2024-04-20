using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator animatorFade;
    private GasInventory _gasInventory;

    private void Start()
    {
        _gasInventory = GameObject.FindWithTag("Player").GetComponent<GasInventory>();
    }

    // хз Time.timeScale = 1; нужен везде
    public void loadScene(int id){
        Time.timeScale = 1;
        _gasInventory.Save();
        animatorFade.Play("FadeIn", -1, 0);
        if (id == 3) Invoke("load3", 1.5f);
        else if (id == 4) Invoke("load4", 1.5f);
        else if (id == 5) Invoke("load5", 1.5f);
        else if (id == 6) Invoke("load6", 1.5f);
        else if (id == -1) Invoke("loadmenu", 3f);
    }
    void load3() {
         Time.timeScale = 1;
        SceneManager.LoadScene("FACTORY_3");
    }
    void load4() {
         Time.timeScale = 1;
        SceneManager.LoadScene("FACTORY_4");
    }
    void load5() {
         Time.timeScale = 1;
        SceneManager.LoadScene("FACTORY_5");
    }
    void load6() {
         Time.timeScale = 1;
        SceneManager.LoadScene("FACTORY_6 1");
    }

    void loadmenu() {
         Time.timeScale = 1;
        SceneManager.LoadScene("MENU");
    }
}
