using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour {

    #region Fields
    static public AppManager S;
    
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

    public void StartGame()
    {
        GameManager.S.Init();
        UIManager.S.ToggleMainMenu(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    #endregion
}
