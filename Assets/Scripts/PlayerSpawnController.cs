using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class PlayerSpawnController : MonoBehaviour
{
    private Animator checkpointButton;
    public AudioSource checkpointInitialize;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        DontDestroyOnLoad(gameObject);
        checkpointButton = GameObject.Find("CheckpointButton").GetComponent<Animator>();
        checkpointButton.Play("CheckpointPopdownAnimation");
        checkpointInitialize.Play();
    }
}