using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator animatorFade;

    public void loadScene(int id){
        animatorFade.Play("FadeIn", -1, 0);
        if (id == 3) Invoke("load3", 1.5f);
        else if (id == 4) Invoke("load4", 1.5f);
    }
    void load3() => SceneManager.LoadScene("FACTORY_3");
    void load4() => SceneManager.LoadScene("FACTORY_4");
}
