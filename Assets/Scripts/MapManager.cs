using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    #region Fields
    static public MapManager S;
    public MapTile selectedMapTile;
    public GameObject[] paths;
    public GameObject selectedPath;
    public Vector3[] obstacles;    
    private List<MapTile> _highlightedTiles;


    #endregion
    
    #region MonoBehaviour Callbacks
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
        HideAllPaths();
    }

    public void SetSelectedMapTile(MapTile mt)
    {
        selectedMapTile = mt;
    }


    public void ShowPath(Character c, ActionType a)
    {
        Vector3 wantedPathPos = c.transform.position;
        wantedPathPos.y = 0f;
        GameObject currentPath = paths[(int)a];
        currentPath.GetComponent<Path>().SetCurrentPlayer(GameManager.S.currentPlayer);
        currentPath.transform.position = wantedPathPos;
        selectedPath = currentPath;
        Path p = selectedPath.GetComponent<Path>();
        p.HideOutOfBoundTiles();
        p.HideObstacleTiles();
        switch (a)
        {
            case ActionType.Move:                
            case ActionType.Fly:
                p.HideFlagTile();
                p.HideEnemyTiles();
                break;
            case ActionType.Melee:              
            case ActionType.Range:
                p.HideFlagTile();
                p.ShowEnemyTiles();
                break;
            case ActionType.CaptureFlag:
                p.ShowFlagTile();
                break;
            case ActionType.LeaveFlag:
                p.HideEnemyTiles();
                break;
            default:
                break;
        }
        
        
        selectedPath.SetActive(true);
        

    }

    public void HideAllPaths()
    {
        for (int i = 0; i < paths.Length; i++)
        {
            paths[i].SetActive(false);
        }
        selectedPath = null;
    }

    public bool ComparePositionToTile(MonoBehaviour mb, MapTile mt)
    {
        return mt.transform.position.z == mb.transform.position.z && mt.transform.position.x == mb.transform.position.x;
    }

    
    #endregion
}
