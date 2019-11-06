using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour {

    public List<MapTile> _tiles;
    [SerializeField] private Player _player;
    private float _bound = 25f;

   
    public Player GetCurrentPlayer()
    {
        return _player;
    }

    public void SetCurrentPlayer(Player p)
    {
        _player = p;
    }

    public void HideOutOfBoundTiles()
    {
        if (Mathf.Abs(transform.position.x) < 17 && Mathf.Abs(transform.position.z) < 17)
        {
            foreach (MapTile t in _tiles)
            {
                t.gameObject.SetActive(true);
            }
            return;
        }            
        foreach (MapTile t in _tiles)
        {
            if (Mathf.Abs(t.transform.position.x) > 25 || Mathf.Abs(t.transform.position.z) > 25)
                t.gameObject.SetActive(false);
            else
                t.gameObject.SetActive(true);
        }
    }

    public void HideObstacleTiles()
    {
        for (int i = 0; i < _tiles.Count; i++)
        {
            for (int j = 0; j < MapManager.S.obstacles.Length; j++)
            {
                if (_tiles[i].transform.position.x == MapManager.S.obstacles[j].x && _tiles[i].transform.position.z == MapManager.S.obstacles[j].z)
                    _tiles[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowEnemyTiles()
    {
        List<Character> charactersInRange = _player.selectedCharacter.GetCharactersInRange();
        if(charactersInRange.Count > 0)
        {
            for (int i = 0; i < _tiles.Count; i++)
            {
                for (int j = 0; j < charactersInRange.Count; j++)
                {
                    if (_tiles[i].transform.position.x != charactersInRange[j].transform.position.x || _tiles[i].transform.position.z != charactersInRange[j].transform.position.z)
                        _tiles[i].gameObject.SetActive(false);
                    else
                    {
                        _player.selectedCharacter.SetCharacterLayer(charactersInRange[j], 2);
                    }
                }
            }
            _player.selectedCharacter.SetCharacterLayer(_player.selectedCharacter, 2);
        }
        else
        {
            foreach (MapTile mt in _tiles)
            {
                mt.gameObject.SetActive(false);
            }
                   
            //show message "no enemies close enough to attack"
        }
    }

    public void HideEnemyTiles()
    {
        List<Character> charactersInRange = _player.selectedCharacter.GetCharactersInRange();
        if (charactersInRange.Count > 0)
        {
            for (int i = 0; i < _tiles.Count; i++)
            {
                for (int j = 0; j < charactersInRange.Count; j++)
                {
                    if (_tiles[i].transform.position.x == charactersInRange[j].transform.position.x && _tiles[i].transform.position.z == charactersInRange[j].transform.position.z)
                        _tiles[i].gameObject.SetActive(false);
                    else
                    {
                        _player.selectedCharacter.SetCharacterLayer(charactersInRange[j], 2);
                    }
                }
            }
            _player.selectedCharacter.SetCharacterLayer(_player.selectedCharacter, 2);
        }
    }

    public void ShowFlagTile()
    {
        
        foreach (MapTile mt in _tiles)
        {
            if (MapManager.S.ComparePositionToTile(_player._myFlag, mt) || MapManager.S.ComparePositionToTile(_player._enemyFlag, mt))
            {
                mt.gameObject.SetActive(true);
            }               
            else
                mt.gameObject.SetActive(false);
        }
    }

    public void HideFlagTile()
    {
        foreach (MapTile mt in _tiles)
        {
            if (MapManager.S.ComparePositionToTile(_player._myFlag, mt) || MapManager.S.ComparePositionToTile(_player._enemyFlag, mt))
                mt.gameObject.SetActive(false);
        }
    }
}
