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
    public int activeUnit = 0;
    public GameObject unitParent;

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


    public UnitManager GetActiveUnit() {
        return unitList[activeUnit];
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
    }

    public UnitManager UnitOnTile(Tile tile) {
        foreach(UnitManager unit in unitList) {
            if(unit.posX == tile.x && unit.posY == tile.y)
                return unit;
        }
        return null;
    }
}
