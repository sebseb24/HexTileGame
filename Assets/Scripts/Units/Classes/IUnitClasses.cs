using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitClasses
{
    void CastSkill(UnitManager source, UnitManager target, int skill, Tile tile);
    void CalculateAttackRange(int skill);
}
