using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {


    #region Fields
    static public SoundManager S;
    [SerializeField] private List<AudioClip> soundEffects;
    private Dictionary<string, AudioClip> audioDict = new Dictionary<string, AudioClip>();
    #endregion

    #region MonoBehaviour Callbacks
    void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
    }
    #endregion

    #region Methods
    public void Init()
    {

    }

    public void PlaySound(SoundType sound)
    {

    }

    public void StopSound()
    {

    }

    public void ToggleBGMusic(bool play)
    {

    }
    #endregion

}
