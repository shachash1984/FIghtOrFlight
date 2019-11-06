using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapTile : MonoBehaviour {

    #region Fields
    public Vector3 position;
    public bool isHighlighted;
    public GameObject halo;
    
    #endregion

    #region MonoBehaviour Callbacks
    

    public void OnMouseDown()
    {
        
        if(!isHighlighted && MapManager.S.selectedMapTile != this)
        {
            if (MapManager.S.selectedMapTile)
                MapManager.S.selectedMapTile.ToggleHighlight(false);           
            ToggleHighlight(true);
            MapManager.S.SetSelectedMapTile(this);
            UIManager.S.ToggleGoButton(true);
        }
        else if (isHighlighted)
        {
            ToggleHighlight(false);
            MapManager.S.SetSelectedMapTile(null);
            UIManager.S.ToggleGoButton(false);
        }
    }

    
    #endregion

    #region Methods

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void ToggleHighlight(bool highlight)
    {
        isHighlighted = highlight;
        halo.SetActive(highlight);
    }

    /*public Character isCharacterOnTile(List<Character> enemies)
    {
        foreach (Character c in enemies)
        {
            if (MapManager.S.ComparePositionToSelectedTile(c))
            {
                return c;
            }
        }
        return null;
    }*/
    #endregion
}
