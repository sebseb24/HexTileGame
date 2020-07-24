using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Singleton
public class UI : MonoBehaviour
{
    public static UI instance = null;

    [Header("Components")]
    public TextMeshProUGUI actionPointsText;
    public TextMeshProUGUI movementPointsText;

    private void Awake() {
        if(instance == null) 
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }

    void Start() {
        GameManager.instance.GetActiveUnit().GetComponent<UnitManager>().InitializeTurn();
    }

    public void onEndTurnButtonClicked() {
        GameManager.instance.FinishTurn();
    }

    public void UpdateActionPointsText(int ap) {
        actionPointsText.text = "<b>AP:</b> " + ap;
    }

    public void UpdateMovementPointsText(int mp) {
        movementPointsText.text = "<b>MP:</b> " + mp;
    }

    public void onSkillButtonClicked(int skillClicked) {
        Map.instance.ClearCurrentPath();
        Map.instance.ClearRangeTiles();

        if(GameManager.instance.activeSkill != skillClicked) {
            GameManager.instance.GetActiveUnit().GetUnitClass().CalculateAttackRange(skillClicked);
            GameManager.instance.activeSkill = skillClicked;
        }
            
        else {
            GameManager.instance.activeSkill = 0;
        }
    }
}
