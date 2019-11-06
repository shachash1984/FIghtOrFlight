using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class Character : MonoBehaviour {

    #region Fields
    static public float maxEnemyDistance = 12f;
    [Range(0, 100)] public float HP;
    [Range(0, 100)] public float AP;
    public bool isSelected;
    public bool isAlive;
    public bool hasFlag = false;
    public Text _characterHPText;
    public Text _characterAPText;
    [Space]
    /*[Header("Stats")]
    [Range(0, 1)] public float strength;
    [Range(0, 1)] public float craft;
    [Range(0, 1)] public float speed;
    [Range(0, 1)] public float dexterity;
    [Range(0, 1)] public float stamina;*/
    [SerializeField] private Vector3 _initialPosition;
    [SerializeField] private Vector3 _initialRotation;
    [SerializeField] private PlayerType _playerType;
    [SerializeField] private Player _player;
    [SerializeField] private Animator _animator;
    [SerializeField] private Slider _HPSlider;
    [SerializeField] private Slider _APSlider;
    [SerializeField] private GameObject _halo;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private Flag _flagBeingCarried;    
    [SerializeField] private Vector3 _initialTextPosition;
    private List<Character> _enemiesInRange = new List<Character>();
    private bool isHighlighted = false;
    #endregion


    void Start()
    {
        Init();
        StartCoroutine(IdleAnimation());
        StartCoroutine(RegenAP());
    }

    public void OnMouseDown()
    {
        
        if (GameManager.S.GetCurrentPlayer().GetPlayerType().Equals(_playerType))
        {
            if (!isHighlighted && _player.selectedCharacter != this)
            {
                if (_player.selectedCharacter)
                    _player.selectedCharacter.ToggleHighlight(false);
                ToggleHighlight(true);
                _player.SetSelectedCharacter(this);
                UIManager.S.ToggleActionButtons(true);
                MapManager.S.HideAllPaths();
            }
            else if (isHighlighted)
            {
                ToggleHighlight(false);
                _player.SetSelectedCharacter(null);
                UIManager.S.ToggleActionButtons(false);
                MapManager.S.HideAllPaths();
            }
        }        
    }

    #region Methods
    
    public void Init()
    {
        gameObject.SetActive(true);
        SetHP(100f);
        SetAP(100f);
        transform.position = _initialPosition;
        transform.rotation = Quaternion.Euler(_initialRotation);
    }

    public void ToggleHighlight(bool highlight)
    {
        isHighlighted = highlight;
        _halo.SetActive(highlight);
    }

    public IEnumerator Move()
    {
        Vector3 wantedPos = MapManager.S.selectedMapTile.transform.position;
        wantedPos.y = 1f;
        Vector3 currentRot = transform.rotation.eulerAngles;
        transform.DOLookAt(wantedPos, 0.5f);
        yield return new WaitForSeconds(0.5f);
        transform.DOMove(wantedPos, 0.75f);
        yield return new WaitForSeconds(0.75f);
        transform.DORotate(currentRot, 0.5f);
        yield return new WaitForSeconds(0.5f);
        GameManager.S.SetMovementCoroutineStatus(null);
    }

    public IEnumerator Fly()
    {
        Vector3 wantedPos = MapManager.S.selectedMapTile.transform.position;
        wantedPos.y = 1f;
        Vector3 currentRot = transform.rotation.eulerAngles;
        transform.DOLookAt(wantedPos, 0.5f);
        yield return new WaitForSeconds(0.5f);
        transform.DOJump(wantedPos, 10f, 1, 2f);
        yield return new WaitForSeconds(2f);
        transform.DORotate(currentRot, 0.5f);
        yield return new WaitForSeconds(0.5f);
        GameManager.S.SetMovementCoroutineStatus(null);
    }

    public IEnumerator MeleeAttack(Character characterToAttack)
    {
        Vector3 tileToAttack = MapManager.S.selectedMapTile.transform.position;
        Vector3 currentRot = transform.rotation.eulerAngles;
        tileToAttack.y = 1f;
        transform.DOLookAt(tileToAttack, 0.5f);
        yield return new WaitForSeconds(0.5f);
        //play attack animation
        //play characterToAttack TakeDamageAnimation
        characterToAttack.TakeDamage(Random.Range(10f, 14f)*AP/100f);
        transform.DORotate(currentRot, 0.5f);
        yield return new WaitForSeconds(0.5f);
        GameManager.S.SetMovementCoroutineStatus(null);
        //reduce character AP
        //play damageTextAnimation
    }

    public IEnumerator RangeAttack(Character characterToAttack)
    {
        Vector3 tileToAttack = MapManager.S.selectedMapTile.transform.position;
        Vector3 currentRot = transform.rotation.eulerAngles;
        tileToAttack.y = 1f;
        transform.DOLookAt(tileToAttack, 0.5f);
        yield return new WaitForSeconds(0.5f);
        //play attack animation
        //play characterToAttack TakeDamageAnimation
        characterToAttack.TakeDamage(Random.Range(4f, 8f) * AP / 100f);
        transform.DORotate(currentRot, 0.5f);
        yield return new WaitForSeconds(0.5f);
        GameManager.S.SetMovementCoroutineStatus(null);
        //reduce character AP
        //play damageTextAnimation
    }

    public void TakeDamage(float damage)
    {
        //show damage text
        //play take damage animation
        SetHP(HP-damage);
        if(HP <= 0f)
        {
            HP = 0f;
            Die();
        }
    }    

    public Flag GetCharacterFlag()
    {
        return _flagBeingCarried;
    }

    public void SetCharacterFlag(Flag f)
    {
        _flagBeingCarried = f;
    }

    private IEnumerator IdleAnimation()
    {
        while (true)
        {
            _animator.SetInteger("Random", Random.Range(1, 4));
            yield return new WaitForSeconds(Random.Range(4f, 8f));
        }
    }
    
    private IEnumerator RegenAP()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(TriggerCharacterText(_characterAPText, "+5 AP"));
        while (!GameManager.S.gameOver)
        {
            yield return new WaitForSeconds(30f);
            StartCoroutine(TriggerCharacterText(_characterAPText, "+5 AP"));
            yield return new WaitForSeconds(5f);
            SetAP(AP + 5f);
        }
    }

    private void Animate(ActionType at)
    {

    }

    public PlayerType GetPlayerType()
    {
        return _playerType;
    }

    public void ToggleFlag(bool flag)
    {
        hasFlag = flag;
    }

    public void SetHP(float newHP)
    {
        HP = newHP;
        if (HP > 100f)
            HP = 100f;
        _HPSlider.DOValue(HP, 0.5f);
    }

    public void SetAP(float newAP)
    {
        AP = newAP;
        if (AP > 100f)
            AP = 100f;
        _APSlider.DOValue(AP, 0.5f);

    }
    
    public void Heal()
    {

    }

    public void PlaySound(SoundType st)
    {

    }

    public void Die()
    {
        //play explosion
        //show died text
        if(hasFlag)

        gameObject.SetActive(false);
    }

    public int GetCurrentMapTile()
    {
        return 0;
    }

    public void SetCurrentMapTile(int newTile)
    {

    }

    public float GetDistanceFromEnemy(Character enemy)
    {
        return Vector3.Distance(transform.position, enemy.transform.position);
    }

    public List<Character> GetCharactersInRange()
    {
        List<Character> charactersInRange = new List<Character>();
        float currentDistance;
        for (int i = 0; i < GameManager.S.characters.Length; i++)
        {
            if(GameManager.S.characters[i] != this &&GameManager.S.characters[i].gameObject.activeSelf)
            {
                
                currentDistance = GetDistanceFromEnemy(GameManager.S.characters[i]);
                //Debug.Log("<color=yellow>" + GameManager.S.characters[i] + "Distance: " + currentDistance + "</color>");
                if (currentDistance < maxEnemyDistance)
                    charactersInRange.Add(GameManager.S.characters[i]);
            }
                     
        }
        return charactersInRange;
    }

    public void UseAP(ActionType at)
    {
        switch (at)
        {
            case ActionType.Move:
                SetAP(AP - 2f);
                break;
            case ActionType.Fly:
                SetAP(AP - 8f);
                break;
            case ActionType.Melee:
                SetAP(AP - 2f);
                break;
            case ActionType.Range:
                SetAP(AP - 8f);
                break;
            case ActionType.CaptureFlag:
                break;
            default:
                break;
        }
    }

    public void SetCharacterLayer(Character c, int _layer)
    {
        c.gameObject.layer = _layer;
    }

    public IEnumerator TriggerCharacterText(Text t, string amt)
    {
        t.text = amt;
        t.transform.rotation = Quaternion.Euler(GameManager.S.currentPlayer.transform.rotation.eulerAngles);
        t.DOFade(1f, 0.5f);
        t.transform.DOLocalMoveY(5f, 3f);
        yield return new WaitForSeconds(2f);
        t.DOFade(0f, 0.5f);
        yield return new WaitForSeconds(1f);
        t.transform.localPosition = _initialTextPosition;
    }

    
    #endregion


}
