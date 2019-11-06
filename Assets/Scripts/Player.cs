using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour {

    #region Fields
    public bool isWinner = false;
    public bool isLeading = false;
    public bool canPlay;
    public float totalPoints { get; private set; }
    public Flag _myFlag;
    public Flag _enemyFlag;
    public int turnsLeft;
    public Character selectedCharacter;
    public Character[] _characters;
    [SerializeField] private PlayerType _playerType;
    [SerializeField] private Slider _totalPointsSlider;
    [SerializeField] private GameObject _actionListPanel;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private Character _currentEnemy;
    
    private ActionType _selectedAction;
    
    #endregion

    #region Methods
    public void Init()
    {
        SetTotalPoints();
        _myFlag.Init();

    }

    public Character GetCurrentEnemy()
    {
        return _currentEnemy;
    }

    public void SetEnemy(Character e)
    {
        _currentEnemy = e;
    }

    public PlayerType GetPlayerType()
    {
        return _playerType;
    }

    public void SetSelectedCharacter(Character c)
    {
        foreach (Character ch in GameManager.S.characters)
        {
            ch.SetCharacterLayer(ch, 0);
        }
        selectedCharacter = c;
        CameraManager cam = CameraManager.S;
        cam.MoveCamera(cam.GetWantedCameraTransform(GameManager.S.currentPlayer));
    }

    public void DoAction(ActionType at)
    {
        selectedCharacter.UseAP(at);
        
        switch (at)
        {
            case ActionType.Move:
                GameManager.S.movementCoroutine = StartCoroutine(selectedCharacter.Move());
                break;
            case ActionType.Fly:
                GameManager.S.movementCoroutine = StartCoroutine(selectedCharacter.Fly());
                break;
            case ActionType.Melee:
                List<Character> enemies = selectedCharacter.GetCharactersInRange();
                
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (_playerType == enemies[i].GetPlayerType())                       
                        enemies.Remove(enemies[i]);
                }
                
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (MapManager.S.ComparePositionToTile(enemies[i], MapManager.S.selectedMapTile))
                    {
                        GameManager.S.movementCoroutine = StartCoroutine(selectedCharacter.MeleeAttack(enemies[i]));
                        break;
                    }
                }
                break;
            case ActionType.Range:
                List<Character> enemiesInRange = selectedCharacter.GetCharactersInRange();

                for (int i = 0; i < enemiesInRange.Count; i++)
                {
                    if (_playerType == enemiesInRange[i].GetPlayerType())
                        enemiesInRange.Remove(enemiesInRange[i]);
                }

                for (int i = 0; i < enemiesInRange.Count; i++)
                {
                    if (MapManager.S.ComparePositionToTile(enemiesInRange[i], MapManager.S.selectedMapTile))
                    {
                        GameManager.S.movementCoroutine = StartCoroutine(selectedCharacter.RangeAttack(enemiesInRange[i]));
                        break;
                    }
                }
                break;
            case ActionType.CaptureFlag:
                if (MapManager.S.ComparePositionToTile(_enemyFlag, MapManager.S.selectedMapTile))
                    GameManager.S.movementCoroutine = StartCoroutine(CaptureFlag(_enemyFlag));
                else if (MapManager.S.ComparePositionToTile(_myFlag, MapManager.S.selectedMapTile))
                    GameManager.S.movementCoroutine = StartCoroutine(CaptureFlag(_myFlag));
                break;
            case ActionType.LeaveFlag:
                Vector3 wantedFlagPos = MapManager.S.selectedMapTile.transform.position;
                if (wantedFlagPos == selectedCharacter.GetCharacterFlag().enemyBase)
                {
                    GameManager.S.SetWinner(this);
                    selectedCharacter.GetCharacterFlag().LeaveCharacter(wantedFlagPos);
                    selectedCharacter.hasFlag = false;
                    selectedCharacter.SetCharacterFlag(null);
                    GameManager.S.SetGameOver(true);
                    //start EndGame Method
                    return;
                }
                    
                selectedCharacter.GetCharacterFlag().LeaveCharacter(wantedFlagPos);
                selectedCharacter.hasFlag = false;
                selectedCharacter.SetCharacterFlag(null);
                break;
            default:
                break;
        }
    }

    IEnumerator CaptureFlag(Flag f)
    {
        if (f == _enemyFlag)
            _enemyFlag.GoToCharacter(selectedCharacter);
        else if (f == _myFlag)
            _myFlag.GoToCharacter(selectedCharacter);
        selectedCharacter.hasFlag = true;
        selectedCharacter.SetCharacterFlag(f);
        yield return new WaitForSeconds(0.5f);
        //play music
        //show message "Player 1/2 has the enemy/ his flag"
        GameManager.S.SetMovementCoroutineStatus(null);
    }

    IEnumerator LeaveFlag()
    {
        selectedCharacter.GetCharacterFlag().LeaveCharacter(MapManager.S.selectedMapTile.transform.position);
        yield return new WaitForSeconds(0.5f);
        GameManager.S.SetMovementCoroutineStatus(null);
    }

    public void SetTotalPoints()
    {
        totalPoints = 0;
        foreach (Character c in _characters)
        {
            totalPoints += (c.HP + c.AP);
        }        
        _totalPointsSlider.DOValue(totalPoints, 0.5f);
    }

    public void PlaySound(SoundType st)
    {

    }

   
    public void SetSelectedAction(ActionType at)
    {
        _selectedAction = at;
    }

    public ActionType GetSelectedAction()
    {
        return _selectedAction;
    }

    public Vector3 GetFlagPosition(Flag f)
    {
        return f.transform.position;
    }


    #endregion
}
