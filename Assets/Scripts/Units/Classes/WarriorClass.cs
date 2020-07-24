using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorClass : IUnitClasses
{
    int[] apCost = new int[] { 3, 2, 4 };

    public void CastSkill(UnitManager source, UnitManager target, int skill, Tile tile) {
        if(source.actionPoints >= apCost[skill-1]) {
            switch(skill) {
                case 1:
                    if(target) {
                        Debug.Log("The warrior is doing skill 1");
                        source.PlayTargetAnimation("WarriorSwordAttack");
                        source.AttackTarget(target, apCost[skill-1]);
                        UI.instance.UpdateActionPointsText(source.actionPoints);    
                    }    
                break;

                case 2:
                    if(target) {
                        Debug.Log("The warrior is doing skill 2");
                        source.PlayTargetAnimation("WarriorBoostDamage");
                        target.BoostDamage(5);

                        source.actionPoints -= apCost[skill-1];
                        UI.instance.UpdateActionPointsText(source.actionPoints); 
                    }      
                break;

                case 3:
                    if(!target) {
                        Debug.Log("The warrior is doing skill 3");
                        source.PlayTargetAnimation("WarriorBoostDamage");
                        source.ChangePosition(tile);

                        source.actionPoints -= apCost[skill-1];
                        UI.instance.UpdateActionPointsText(source.actionPoints); 
                    }
                break;
            }
        }
    }

    public void CalculateAttackRange(int skill) {
        switch(skill) {
            case 1:
                // Melee Attack
                Map.instance.CalculateAttackRangeAroundUnit(1, false);
            break;

            case 2: 
                // Boost Damage
                Map.instance.CalculateAttackRangeAroundUnit(2, true);
            break;

            case 3:
                // Jump
                Map.instance.CalculateAttackRangeAroundUnit(3, false);
            break;
        }
    }
}
