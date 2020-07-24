using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterClass : IUnitClasses
{
    int[] apCost = new int[] { 3, 3, 4 };

    public void CastSkill(UnitManager source, UnitManager target, int skill, Tile tile) {
        
    }

    public void CalculateAttackRange(int skill) {
        switch(skill) {
            case 1: 
                // Range attack
                Map.instance.CalculateAttackRangeAroundUnit(4, false);
            break;

            case 2: 
                // Line attack
                Map.instance.CalculateAttackRangeInStraightLine(2, false);
            break;

            case 3:
                // Explosive attack 
                //Map.instance.CalculateAttackRangeAroundTarget(3, false);
            break;
        }
    }
}
