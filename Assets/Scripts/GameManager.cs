using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType { Good, Bad}
public enum ActionType { Move = 0, Fly = 1, Melee = 2, Range = 3, CaptureFlag = 4, LeaveFlag = 5}
public enum SoundType { Click, Move, Fly, Melee, Range, CaptureFlag, Heal, Win, Lose, CameraZoom}

public class GameManager : MonoBehaviour {

    #region Fields
    static public GameManager S;
    public bool gameOver { get; private set; }
    public bool gamePaused;
    public Player currentPlayer { get; private set; }
    public Character[] characters;
    [SerializeField] private Player[] _players;
    public Coroutine movementCoroutine;
    private Player _winningPlayer;
    public AudioSource audio;


    #endregion
    
    #region Monobehaviour Callbacks
    void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
        Init();
    }    

    
    #endregion

    #region Methods
    public void Init()
    {
        foreach (Character c in characters)
        {
            if (!c.gameObject.activeSelf)
                c.gameObject.SetActive(true);
            c.Init();
        }
        foreach (Player p in _players)
        {
            p.Init();
        }
        if (GetWinner() != null)
        {
            SetCurrentPlayer(GetWinner());
            CameraManager.S.MoveCamera(GetWinner().transform);
        }            
        else
            SetCurrentPlayer(_players[0]);
        UIManager.S.Init();
        
    }   

    

    public Coroutine GetMovementCoroutineStatus()
    {
        return movementCoroutine;
    }

    public void SetMovementCoroutineStatus(Coroutine coroutine)
    {
        movementCoroutine = coroutine;
    }

    public bool isGameOver()
    {
        return gameOver;
    }

    public void SetGameOver(bool over)
    {
        gameOver = over;
        UIManager.S.SetMainText(string.Format("{0} is the Winner!", _winningPlayer.name));
        UIManager.S.ToggleMainText(true);
        UIManager.S.ToggleRematchButton(true);
        //show text who won
        //play music
        //show rematch button
        //play fireworks
        
    }

    public void SetPause(bool pause)
    {

    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public void SetCurrentPlayer(Player p)
    {
        currentPlayer = p;
    }

    public Player GetLeadingPlayer()
    {
        return null;
    }

    public void SetLeadingPlayer(Player p)
    {

    }

    public Player GetWinner()
    {
        return _winningPlayer;
    }

    public void SetWinner(Player p)
    {        
        _winningPlayer = p;
    }

    public void SwitchTurns()
    {
        if (_players[0].Equals(currentPlayer))
            SetCurrentPlayer(_players[1]);
        else
            SetCurrentPlayer(_players[0]);
        currentPlayer.SetTotalPoints();
        UIManager.S.SetCurrentPlayerText();
    }
    #endregion

    public void ToggleCharactersLayer(bool clickable)
    {
        if (clickable)
        {
            foreach (Character c in characters)
            {
                c.SetCharacterLayer(c, 0);
            }
        }
        else
        {
            foreach (Character c in characters)
            {
                c.SetCharacterLayer(c, 2);
            }
        }
    }
}
