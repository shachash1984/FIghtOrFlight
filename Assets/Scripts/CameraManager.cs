using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour {

    #region Fields
    static public CameraManager S;
    [SerializeField] private float _cameraSpeed = 10f;
    [SerializeField] private Vector3 _mapViewPosGood;
    [SerializeField] private Vector3 _mapViewPosBad;
    private int _closestCharacterTile;
    [SerializeField] private Vector3 _offsetFromCharacter;
    [SerializeField] private Vector3 _goodPlayerOffset = new Vector3(0f, 16f, -12f);
    [SerializeField] private Vector3 _badPlayerOffset = new Vector3(0f, 16f, 12f);
    private Vector3 _goodPlayerRotation = new Vector3(40f, 0f, 0f);
    private Vector3 _badPlayerRotation = new Vector3(40f, 180f, 0f);
    [SerializeField] private Player _currentPlayer;
    #endregion

    #region MonoBehaviour callbacks
    void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
    }

    
    void Start()
    {
        SetCurrentPlayer(GameManager.S.currentPlayer);
        Init();
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
            MoveCamera();
    }
    #endregion

    #region Methods
    public void Init()
    {
        MoveCamera(GetWantedCameraTransform(_currentPlayer));
    }

    public void MoveCamera()
    {
        int mult = 0;
        if (_currentPlayer.GetPlayerType().Equals(PlayerType.Good))        
            mult = 1;           
        else        
            mult = -1;          

        if (Input.GetKey(KeyCode.W))
            transform.position += mult * (Vector3.forward * Time.deltaTime * _cameraSpeed);
        if (Input.GetKey(KeyCode.A))
            transform.position += mult * (Vector3.left * Time.deltaTime * _cameraSpeed);
        if (Input.GetKey(KeyCode.S))
            transform.position += mult * (Vector3.back * Time.deltaTime * _cameraSpeed);
        if (Input.GetKey(KeyCode.D))
            transform.position += mult * (Vector3.right * Time.deltaTime * _cameraSpeed);

    }

    public void MoveCamera(Transform t)
    {       
        transform.DOMove(t.position, 0.5f);
        transform.DORotate(t.rotation.eulerAngles, 0.5f);
    }

    public Transform GetWantedCameraTransform(Player p)
    {
        GameObject go = new GameObject();
        Transform camTransform = go.transform;        
        if (_currentPlayer.GetPlayerType() == PlayerType.Good)
        {
            camTransform.position = _mapViewPosGood;
            _offsetFromCharacter = _goodPlayerOffset;
            camTransform.rotation = Quaternion.Euler(_goodPlayerRotation);
        }
        else
        {
            camTransform.position = _mapViewPosBad;
            _offsetFromCharacter = _badPlayerOffset;
            camTransform.rotation = Quaternion.Euler(_badPlayerRotation);
        }
        if (_currentPlayer.selectedCharacter != null)
            camTransform.position = _currentPlayer.selectedCharacter.transform.position + _offsetFromCharacter;
        Destroy(go);
        return camTransform;
    }

    public void SetCurrentPlayer(Player p)
    {
        _currentPlayer = p;
    }
    #endregion

}
