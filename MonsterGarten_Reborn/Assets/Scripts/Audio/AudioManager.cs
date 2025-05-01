using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private void Awake()
    {
        Instance = this;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("SoundManager");
        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        if (Instance == null)
        {
            Instance = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<AudioManager>(); ;

        }
    }
}
