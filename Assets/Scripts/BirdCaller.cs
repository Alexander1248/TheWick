using NPC;
using UnityEngine;

public class BirdCaller : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    private Bird _bird;
    private bool _birdActive;
    private void Start()
    {
        _bird = GameObject.FindGameObjectWithTag("Player").GetComponent<Bird>();
    }

    public void UpdateBird()
    {
        if (dialogue.GetCurrentNarratorName() == "Player")
            ShowBird();
        else
            HideBird();
    }

    public void ShowBird()
    {
        if (_birdActive) return;
        _bird.EnableBird();
        _birdActive = true;
    }
    public void HideBird()
    {
        if (!_birdActive) return;
        _bird.DisableBird();
        _birdActive = false;
    }
}