using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Singleton
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    
    [Header("Players")]
    private UnitManager[] unitList;
    public GameObject unitParent;

    public int activeUnit = 0;
    public int activeSkill = 0;

    private void Awake() {
        if(instance == null) 
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }

    private void Start() {
        InitializeUnitList();
    }

    public int GetUnitId(UnitManager unit) {
        int cnt = 0;
        foreach(UnitManager u in unitList) {
            if(u.Equals(unit))
                return cnt;
            cnt++;
        }

        return -1;
    }

    public UnitManager GetActiveUnit() {
        return unitList[activeUnit];
    }

    public List<UnitManager> GetPlayableUnitsList() {
        List<UnitManager> list = new List<UnitManager>();

        foreach(UnitManager u in unitList) {
            if(u.IsPlayable()) {
                list.Add(u);
            }
        }

        return list;
    }

    public List<UnitManager> GetEnemyUnitsList() {
        List<UnitManager> list = new List<UnitManager>();
        
        foreach(UnitManager u in unitList) {
            if(!u.IsPlayable()) {
                list.Add(u);
            }
        }

        return list;
    }

    void InitializeUnitList() {
        unitList = unitParent.GetComponentsInChildren<UnitManager>();
    }

    public void FinishTurn() {
        Map.instance.ClearRangeTiles();
        activeSkill = 0;

        if(activeUnit >= unitList.Length-1)
            activeUnit=0;
        else
            activeUnit++;

        unitList[activeUnit].InitializeTurn();
        UI.instance.UpdateIconList();

        if(!unitList[activeUnit].IsPlayable())
            StartCoroutine(unitList[activeUnit].GetComponent<EnemyAI>().StartingTurn());
    }

    public UnitManager UnitOnTile(Tile tile) {
        foreach(UnitManager unit in unitList) {
            if(unit.posX == tile.x && unit.posY == tile.y)
                return unit;
        }
        return null;
    }

    public void KillPlayer(UnitManager unit) {
        //Destroy(unit)
    }
}
