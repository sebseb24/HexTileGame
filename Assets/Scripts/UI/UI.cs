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
    public GameObject popupPrefab;

    public Image iconPrefab;
    public GameObject iconListParent;
    public Image activeIconBorder;
    public List<Image> iconList  = new List<Image>();
    public GameObject[] hpBarList;

    private void Awake() {
        if(instance == null) 
            instance = this;
        else if(instance != this)
            Destroy(gameObject);
    }

    void Start() {
        GameManager.instance.GetActiveUnit().GetComponent<UnitManager>().InitializeTurn();
    }

    public void CreateIcon(int cnt, Sprite sprite) {
        Image icon = Instantiate(iconPrefab, iconListParent.transform, false);
        icon.transform.position += new Vector3(0, -3-(61*cnt), 0);
        icon.sprite = sprite;
        iconList.Add(icon);
    }

    public void InitializeHealthBars() {
        hpBarList = GameObject.FindGameObjectsWithTag("HpBar");
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

    public void SpawnPopupText(Vector3 position, int value, PopupType type, string op) {
        GameObject popupTransform = Instantiate(popupPrefab, position, Quaternion.identity);
        PopupMessage popup = popupTransform.GetComponent<PopupMessage>();
        popup.Setup(value, type, op);
    }

    public void UpdateIconList() {
        activeIconBorder.transform.position = iconList[GameManager.instance.activeUnit].transform.position - new Vector3(0, 25, 0);
    }

    public void UpdateHpBar(int unit, float fillAmount) {
        hpBarList[unit].GetComponent<Slider>().value = fillAmount;
    }
}
