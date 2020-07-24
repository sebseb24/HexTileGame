using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct UnitLineup {
    public IUnitClasses unitClass;
    public int posX;
    public int posY;
    public int prefab;

    public UnitLineup(IUnitClasses unitClass, int prefab, int posX, int posY) {
        this.unitClass = unitClass;
        this.prefab = prefab;
        this.posX = posX;
        this.posY = posY;
    }
} 

public class UnitsCreator : MonoBehaviour
{
    public static UnitsCreator instance = null;

    [Header("Components")]
    public UnitManager[] prefabs;

    static IUnitClasses warrior = new WarriorClass();
    static IUnitClasses healer = new HealerClass();
    static IUnitClasses hunter = new HunterClass();
    static IUnitClasses enemyMinion = new EnemyMinionClass();

    UnitLineup[] lineup = new UnitLineup[] {
        new UnitLineup(warrior, 1, 0, 1),
        new UnitLineup(enemyMinion, 2, 4, 1),
        new UnitLineup(enemyMinion, 2, 4, 3)
    };


    void Awake()
    {
        if(instance == null) 
            instance = this;
        else if(instance != this)
            Destroy(gameObject);

        GenerateUnits();
    }


    void GenerateUnits() {
        foreach(UnitLineup v in lineup) {
            UnitManager unit = Instantiate(prefabs[v.prefab], this.transform);
            unit.InitializeUnit(v.unitClass, v.posX, v.posY);
        }  
    }
}
