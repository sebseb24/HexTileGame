using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    Vector3 destination = Vector3.zero;
    float speed = 4;

    [Header("Stats")]
    public int maxMovementPoints = 3;
    public int movementPoints;
    public int maxActionPoints = 6;
    public int actionPoints;
    public int curHp;
    public int maxHp = 50;
    public int attackDamage = 10;
    public int healValue = 15;

    public int posX;
    public int posY;

    public bool isMoving = false;

    List<Node> currentPath = null;

    IUnitClasses unitClass; 

    public AnimatorHandler animatorHandler;
    

    void Start()
    {
        animatorHandler = GetComponent<AnimatorHandler>();
        transform.position = Map.instance.TileCoordToWorldCoord(posX, posY);
        curHp = maxHp;
        movementPoints = maxMovementPoints;
        actionPoints = maxActionPoints;
    }

    private void Update() {
        if(currentPath != null) {
            Move();
        }
    }

    public IUnitClasses GetUnitClass() {
        return unitClass;
    }

    public void InitializeUnit(IUnitClasses unitClass, int posX, int posY) {
        this.unitClass = unitClass;
        this.posX = posX;
        this.posY = posY;
    }

    public void InitializeTurn() {
        movementPoints = maxMovementPoints;
        actionPoints = maxActionPoints;

        UI.instance.UpdateMovementPointsText(movementPoints);
        UI.instance.UpdateActionPointsText(actionPoints);
    }

    public void InitializeMovement(List<Node> path) {
        isMoving = true;
        animatorHandler.anim.SetBool("isRunning", true);
        destination = Map.instance.TileCoordToWorldCoord(path[0].x, path[0].y);
        this.currentPath = path;
    }

    public void Move() {
        // Move towards destination
        Vector3 dir = destination - transform.position;
        Vector3 velocity = dir.normalized * speed * Time.deltaTime;
        transform.forward = dir;
        float step =  speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, destination, step);

        if(Vector3.Distance(transform.position, destination) < 0.001f) {
            posX = currentPath[0].x;
            posY = currentPath[0].y;
            movementPoints--;
            UI.instance.UpdateMovementPointsText(movementPoints);

            currentPath.RemoveAt(0);

            if(currentPath.Count > 0) {
                // Change to next destination
                destination = Map.instance.TileCoordToWorldCoord(currentPath[0].x, currentPath[0].y);
            }

            else {
                // Destination reached
                animatorHandler.anim.SetBool("isRunning", false);
                currentPath = null;
                isMoving = false;
            }
        }
    }

    public void InteractOnTile(Tile tile) {
        UnitManager target = GameManager.instance.UnitOnTile(tile);
        unitClass.CastSkill(this, target, GameManager.instance.activeSkill, tile);                     
    }

    public void AttackTarget(UnitManager target, int apCost) {
        Vector3 dir = Map.instance.TileCoordToWorldCoord(target.posX, target.posY) - transform.position;
        transform.forward = dir;

        target.TakeDamage(attackDamage);

        actionPoints -= apCost;
    }

    public void TakeDamage(int damage) {
        animatorHandler.PlayTargetAnimation("TakeDamage");
        curHp -= damage;
        if(curHp <= 0)
            Die();
    }

    public void BoostDamage(int value) {
        attackDamage += value;
    }

    public void ChangePosition(Tile tile) {
        posX = tile.x;
        posY = tile.y;
        transform.position = Map.instance.TileCoordToWorldCoord(tile.x, tile.y);
    }

    public void Heal(int value) {
        curHp += value;

        if(curHp > maxHp)
            curHp = maxHp;
    }

    public void BoostAP(int value) {
        actionPoints += value;
    }

    public void Die() {
        Debug.Log("This unit is dead");
    }

    public void PlayTargetAnimation(string anim) {
        animatorHandler.PlayTargetAnimation(anim);
    }
}
