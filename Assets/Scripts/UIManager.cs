using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour {

    #region Fields
    static public UIManager S;
    public Button selectedButton;
    public Toggle _music;
    public Toggle _sfx;
    [SerializeField] private CanvasGroup _actionsPanel;
    [SerializeField] private CanvasGroup _menuPanel;
    [SerializeField] private CanvasGroup _optionsPanel;
    [SerializeField] private CanvasGroup _creditsPanel;
    [SerializeField] private CanvasGroup _instructionsPanel;
    [SerializeField] private Button[] _actionButtons;
    [SerializeField] private Button _captureButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Text _currentPlayerText;
    [SerializeField] private Text _mainText;
    [SerializeField] private Button _rematchButton;
    
    private Player _currentPlayer;
    #endregion


    #region MonoBehaviour Callbacks
    void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
    }
    #endregion

    void Start()
    {
        Init();
    }

   
    #region Methods
    public void Init()
    {
        ToggleActionButtons(false);
        foreach (Button b in _actionButtons)
        {
            ToggleHighlightOnButton(b, false);
        }
        SetCurrentPlayer(GameManager.S.GetCurrentPlayer());
        SetCurrentPlayerText();
        ToggleCredits(false);
        ToggleMainMenu(false);
        ToggleMainText(false);
        ToggleOptions(false);
        ToggleRematchButton(false);
        
    }

    /*public void ToggleMusic(bool on)
    {
        Debug.Log("Toggle Music" + on);
        _music.isOn = on;
        if (on)
        {            
            GameManager.S.audio.Play();
            PlayerPrefs.SetInt("Music", 1);
        }            
        else
        {
            GameManager.S.audio.Stop();
            PlayerPrefs.SetInt("Music", 0);
        }
            
    }*/

    public void SetCurrentPlayerText()
    {
        _currentPlayerText.text = string.Format("Current Player: <b>{0}</b>", GameManager.S.GetCurrentPlayer().name);
    }

    public void ToggleActionButtons(bool on)
    {
        if (on)
        {
            if (GameManager.S.GetCurrentPlayer().selectedCharacter.hasFlag)
            {
                _captureButton.transform.GetChild(0).GetComponent<Text>().text = "Leave Flag";
                _captureButton.onClick.RemoveAllListeners();
                _captureButton.onClick.AddListener(() => OnActionButtonPressed(_captureButton));
                _captureButton.onClick.AddListener(() => ShowPath(5));
            }
            else
            {
                _captureButton.transform.GetChild(0).GetComponent<Text>().text = "Capture Flag";
                _captureButton.onClick.RemoveAllListeners();
                _captureButton.onClick.AddListener(() => OnActionButtonPressed(_captureButton));
                _captureButton.onClick.AddListener(() => ShowPath(4));
            }
            _actionsPanel.DOFade(1, 0.5f);
            foreach (Button b in _actionButtons)
            {
                ToggleHighlightOnButton(b, false);
            }
        }     
            
        else       
            _actionsPanel.DOFade(0, 0.5f);

        _actionsPanel.interactable = on;
        _actionsPanel.blocksRaycasts = on;
        _actionButtons[_actionButtons.Length - 1].interactable = false;
    }

    public void ToggleHighlightOnButton(Button btn, bool highlight)
    {
        if (highlight)
            btn.GetComponent<Image>().color = btn.colors.pressedColor;
        else
            btn.GetComponent<Image>().color = btn.colors.normalColor;
    }

    public void OnActionButtonPressed(Button btn)
    {
        foreach (Button b in _actionButtons)
        {
            if (b.Equals(btn))
            {
                ToggleHighlightOnButton(b, true);
                string action = b.transform.GetChild(0).GetComponent<Text>().text;
                //Debug.Log(btnName[0]);
                ActionType at = ActionType.Move;
                switch (action)
                {
                    case "Move":
                        at = ActionType.Move;
                        break;
                    case "Fly":
                        at = ActionType.Fly;
                        break;
                    case "Melee":
                        at = ActionType.Melee;
                        break;
                    case "Range Attack":
                        at = ActionType.Range;
                        break;
                    case "Capture Flag":
                        at = ActionType.CaptureFlag;
                        break;
                    case "Leave Flag":
                        at = ActionType.LeaveFlag;
                        break;
                    default:
                        break;
                }
                _currentPlayer.SetSelectedAction(at);
            }                
            else
                ToggleHighlightOnButton(b, false);
        }
        ToggleGoButton(false);
              
    }

    public void OnGoButtonPressed()
    {
        ToggleGoButton(false);
        _currentPlayer.DoAction(_currentPlayer.GetSelectedAction());
        StartCoroutine(OnActionEnded());
    }

    public void ToggleGoButton(bool on)
    {
        _actionButtons[_actionButtons.Length - 1].interactable = on;
    }

    public void ShowPath(int action)
    {
        ActionType at = (ActionType)action;
        MapManager.S.HideAllPaths();
        MapManager.S.ShowPath(_currentPlayer.selectedCharacter, at);
    }

    IEnumerator OnActionEnded()
    {
        _currentPlayer.SetTotalPoints();
        yield return new WaitUntil(() => GameManager.S.movementCoroutine == null);
        foreach (Character c in GameManager.S.characters)
        {
            c.SetCharacterLayer(c, 0);
        }
        ToggleGoButton(false);
        foreach (Button btn in _actionButtons)
        {
            ToggleHighlightOnButton(btn, false);
        }
        ToggleActionButtons(false);
        MapManager.S.HideAllPaths();
        _currentPlayer.selectedCharacter.ToggleHighlight(false);
        _currentPlayer.SetSelectedCharacter(null);
        GameManager.S.SwitchTurns();
        CameraManager.S.SetCurrentPlayer(GameManager.S.GetCurrentPlayer());
        SetCurrentPlayer(GameManager.S.GetCurrentPlayer());
        CameraManager.S.MoveCamera(CameraManager.S.GetWantedCameraTransform(_currentPlayer));
        MapManager.S.selectedMapTile.ToggleHighlight(false);
        MapManager.S.SetSelectedMapTile(null);
    }

    public void SetCurrentPlayer(Player p)
    {
        _currentPlayer = p;
    }

    public void ToggleMainMenu(bool show)
    {
        if (GameManager.S.gameOver)
            _resumeButton.gameObject.SetActive(false);
        else
            _resumeButton.gameObject.SetActive(true);

        if (show)
            _menuPanel.DOFade(1, 0.75f);
        else
            _menuPanel.DOFade(0, 0.75f);
        _menuPanel.blocksRaycasts = show;
        _menuPanel.interactable = show;
    }

    public void ToggleOptions(bool show)
    {
        if (show)
            _optionsPanel.DOFade(1, 0.75f);
        else
            _optionsPanel.DOFade(0, 0.75f);
        _optionsPanel.interactable = show;
        _optionsPanel.blocksRaycasts = show;
    }

    public void ToggleCredits(bool show)
    {
        if (show)
            _creditsPanel.DOFade(1, 0.75f);
        else
            _creditsPanel.DOFade(0, 0.75f);
        _creditsPanel.interactable = show;
        _creditsPanel.blocksRaycasts = show;
    }

    public void ToggleMainText(bool show)
    {
        if (show)
            _mainText.DOFade(1f, 0.5f);
        else
            _mainText.DOFade(0f, 0.5f);
    }

    public void SetMainText(string wantedText)
    {
        _mainText.text = wantedText;
    }

    public void ToggleRematchButton(bool show)
    {
        if (show)
        {
            _rematchButton.image.DOFade(1f, 0.5f);
            _rematchButton.GetComponentInChildren<Text>().DOFade(1f, 0.5f);
        }
        else
        {
            _rematchButton.image.DOFade(0f, 0.5f);
            _rematchButton.GetComponentInChildren<Text>().DOFade(0f, 0.5f);
        }
        _rematchButton.interactable = show;
        _rematchButton.image.raycastTarget = show;
    }

    public void ToggleInstructionPanel(bool show)
    {
        if (show)
            _instructionsPanel.DOFade(1, 0.75f);
        else
            _instructionsPanel.DOFade(0, 0.75f);
        _instructionsPanel.interactable = show;
        _instructionsPanel.blocksRaycasts = show;
    }

    #endregion

}
