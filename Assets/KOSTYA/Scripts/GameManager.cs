using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator animatorFade;

    public void loadScene3(){
        animatorFade.Play("FadeIn", -1, 0);
        Invoke("load3", 1.5f);
    }
    void load3() => SceneManager.LoadScene("FACTORY_3");
}
