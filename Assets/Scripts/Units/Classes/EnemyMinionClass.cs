using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMinionClass : IUnitClasses
{
    int[] apCost = new int[] { 3, 2, 4 };

    public void CastSkill(UnitManager source, UnitManager target, int skill, Tile tile) {
        if(source.actionPoints >= apCost[skill-1]) {
            switch(skill) {
                case 1:
                    if(target) {
                        Debug.Log("The enemy is doing skill 1");
                        source.PlayTargetAnimation("StabAttack");
                        source.AttackTarget(target, apCost[skill-1], 20);
 
                        UI.instance.UpdateActionPointsText(source.actionPoints);    
                    }    
                break;

                case 2:
                    if(target) {
                        Debug.Log("The enemy is doing skill 2");
                    }      
                break;

                case 3:
                    if(!target) {
                        Debug.Log("The enemy is doing skill 3");
                    }
                break;
            }
        }
    }

    public void CalculateAttackRange(int skill) {
        switch(skill) {
            case 1: 
                Map.instance.CalculateAttackRangeAroundUnit(2, false);
            break;

            case 2: 
                Map.instance.CalculateAttackRangeAroundUnit(0, true);
            break;

            case 3: 
            break;
        }
    }
}
