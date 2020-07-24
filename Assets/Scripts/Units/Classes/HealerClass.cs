using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerClass : IUnitClasses
{
    int[] apCost = new int[] { 2, 3, 4 };

    public void CastSkill(UnitManager source, UnitManager target, int skill, Tile tile) {
        if(source.actionPoints >= apCost[skill-1]) {
            switch(skill) {
                case 1:
                    if(target) {
                        Debug.Log("The healer is doing skill 1 (Range Attack)");
                        source.AttackTarget(target, apCost[skill-1], 10);

                        UI.instance.UpdateActionPointsText(source.actionPoints);
                    }    
                break;

                case 2:
                    if(target) {
                        Debug.Log("The healer is doing skill 2 (Heal)");
                        target.Heal(source.healValue, 10);

                        source.actionPoints -= apCost[skill-1];
                        UI.instance.UpdateActionPointsText(source.actionPoints); 
                    }      
                break;

                case 3:
                    if(target) {
                        Debug.Log("The healer is doing skill 3 (Boost AP)");
                        source.BoostAP(2);

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
                // Light attack
                Map.instance.CalculateAttackRangeAroundUnit(3, false);
            break;

            case 2: 
                // Heal
                Map.instance.CalculateAttackRangeAroundUnit(2, true);
            break;

            case 3:
                // Boost AP 
                Map.instance.CalculateAttackRangeAroundUnit(2, true);
            break;
        }
    }
}
