using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum EnemyAIStates {
        Idle,
        StartingTurn
    }

    UnitManager target = null;

    UnitManager unit;
    private void Start() {
        unit = GetComponent<UnitManager>();
    }


    public IEnumerator StartingTurn() {
        Debug.Log("enemy starting turn");

        bool playerInRange = FindClosestPlayer();

        yield return new WaitWhile(() => unit.isMoving);

        if(playerInRange) {
            yield return new WaitForSeconds(1);
            AttackTarget();
        }

        yield return new WaitForSeconds(1);
        GameManager.instance.FinishTurn();
    }


    bool FindClosestPlayer() {
        int distance = 100;

        foreach(UnitManager player in GameManager.instance.GetPlayableUnitsList()) {
            int newDist = Map.instance.GeneratePathTo(player.posX, player.posY);
            if(newDist < distance) {
                distance = newDist;
                target = player;
            }
        }

        Map.instance.GeneratePathTo(target.posX, target.posY);
        bool isInRange = Map.instance.ShortenCurrentPath(unit.movementPoints);

        if(distance > 1)
            Map.instance.InitializeMovement();

        return isInRange;
    }

    void AttackTarget() {
        GameManager.instance.activeSkill = 1;
        Tile tile = GameObject.Find("Hex_" + target.posX + "_" + target.posY).GetComponent<Tile>();
        GameManager.instance.GetActiveUnit().InteractOnTile(tile);
    }
}
