using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Flag : MonoBehaviour {

    [SerializeField] private PlayerType _playerType;
    [SerializeField] private Player _player;
    public Vector3 homeBase;
    public Vector3 enemyBase;
    public bool isHome = true;
    [SerializeField] private ParticleSystem _particles;

    
    void Start()
    {
        StartCoroutine(RegenerateCharacters());
    }

    public void Init()
    {
        transform.parent = null;
        transform.position = homeBase;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
    }

    public void GoToCharacter(Character c)
    {
        transform.parent = c.transform;
        transform.DOLocalMove(new Vector3(0, -0.75f, -0.2f), 0.5f);
        isHome = false;
    }

    public void LeaveCharacter(Vector3 wantedPos)
    {
        transform.parent = null;
        transform.DOMove(wantedPos, 0.5f);

    }

    public float GetDistanceFromHome()
    {
        return Vector3.Distance(transform.position, homeBase);
    }

    public IEnumerator RegenerateCharacters()
    {
        while (!GameManager.S.gameOver)
        {
            float distance = GetDistanceFromHome();
            if (distance <= 5f)
                distance = 5f;
            float HPToRegen = 25f / distance;
            _particles.Play();
            yield return new WaitForSeconds(4f);
            foreach (Character c in _player._characters)
            {
                StartCoroutine(c.TriggerCharacterText(c._characterHPText, string.Format("+{0} HP", HPToRegen)));
            }
            yield return new WaitForSeconds(60f);
        }
        
    }

    
}
