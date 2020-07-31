using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    bool isPlayable = true;

    Vector3 destination = Vector3.zero;
    float speed = 4;

    [Header("Stats")]
    public int maxMovementPoints = 3;
    public int movementPoints;
    public int maxActionPoints = 6;
    public int actionPoints;
    public int curHp;
    public int maxHp = 90;
    public int baseAttackDamage = 10;
    public int healValue = 15;

    public int posX;
    public int posY;

    public bool isMoving = false;
    int mpUsed = 0;

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

    public bool IsPlayable() {
        return isPlayable;
    }

    public void InitializeUnit(IUnitClasses unitClass, int posX, int posY, bool isPlayable) {
        this.unitClass = unitClass;
        this.posX = posX;
        this.posY = posY;
        this.isPlayable = isPlayable;
    }

    public void InitializeTurn() {
        movementPoints = maxMovementPoints;
        actionPoints = maxActionPoints;
        mpUsed = 0;

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
                mpUsed++;
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

    public void AttackTarget(UnitManager target, int apCost, int diceRoll) {
        Vector3 dir = Map.instance.TileCoordToWorldCoord(target.posX, target.posY) - transform.position;
        transform.forward = dir;

        int damage = baseAttackDamage + Random.Range(1, diceRoll);
        actionPoints -= apCost;
        StartCoroutine(target.TakeDamage(damage));
        //target.TakeDamage(damage);

        
    }

    public IEnumerator TakeDamage(int damage) {
        yield return null;
        animatorHandler.PlayTargetAnimation("TakeDamage");
        UI.instance.SpawnPopupText(transform.position, damage, PopupType.Damage, "-");
        
        curHp -= damage;
        UI.instance.UpdateHpBar(GameManager.instance.GetUnitId(this), (float)curHp / maxHp);

        if(curHp <= 0)
            Die();
    }

    public void BoostDamage(int value, int apCost) {
        baseAttackDamage += Mathf.RoundToInt(baseAttackDamage * ((float)value/100));
        actionPoints -= apCost;
    }

    public void ChangePosition(Tile tile) {
        posX = tile.x;
        posY = tile.y;
        transform.position = Map.instance.TileCoordToWorldCoord(tile.x, tile.y);
    }

    public void Heal(int value, int diceRoll) {
        int heal = value + Random.Range(1, diceRoll);
        curHp += heal;
        if(curHp >= maxHp) {
            heal -= (curHp - maxHp);
            curHp = maxHp;
        }

        UI.instance.UpdateHpBar(GameManager.instance.GetUnitId(this), (float)curHp / maxHp);
        UI.instance.SpawnPopupText(transform.position, heal, PopupType.Heal, "+");
    }

    public void BoostAP(int value) {
        UI.instance.SpawnPopupText(transform.position, value, PopupType.AP, "+");
        maxActionPoints += value;
        actionPoints += value;
    }

    public void Die() {
        Debug.Log("This unit is dead");
        GameManager.instance.KillPlayer(this);
    }

    public void PlayTargetAnimation(string anim) {
        animatorHandler.PlayTargetAnimation(anim);
    }
}
