using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct UnitLineup {
    public IUnitClasses unitClass;
    public int posX;
    public int posY;
    public int prefab;
    public int sprite;
    public bool isPlayable;

    public UnitLineup(IUnitClasses unitClass, int prefab, int posX, int posY, int sprite, bool isPlayable) {
        this.unitClass = unitClass;
        this.prefab = prefab;
        this.posX = posX;
        this.posY = posY;
        this.sprite = sprite;
        this.isPlayable = isPlayable;
    }
} 

public class UnitsCreator : MonoBehaviour
{
    public static UnitsCreator instance = null;

    [Header("Components")]
    public UnitManager[] prefabs;
    public Sprite[] iconList;

    static IUnitClasses warrior = new WarriorClass();
    static IUnitClasses healer = new HealerClass();
    static IUnitClasses hunter = new HunterClass();
    static IUnitClasses enemyMinion = new EnemyMinionClass();

    UnitLineup[] lineup = new UnitLineup[] {
        new UnitLineup(warrior, 0, 0, 1, 0, true),
        new UnitLineup(enemyMinion, 1, 7, 1, 1, false),
        new UnitLineup(healer, 0, 0, 3, 0, true),
        new UnitLineup(enemyMinion, 1, 4, 3, 1, false)
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
        int cnt = 0;
        UI ui = UI.instance;
        foreach(UnitLineup v in lineup) {
            UnitManager unit = Instantiate(prefabs[v.prefab], this.transform);
            unit.InitializeUnit(v.unitClass, v.posX, v.posY, v.isPlayable);
            ui.CreateIcon(cnt, iconList[v.sprite]);
            cnt++;
        } 

        ui.InitializeHealthBars(); 
    }
}
