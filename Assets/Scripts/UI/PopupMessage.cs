using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum PopupType {
        Damage,
        AP,
        MP,
        Heal
}

public class PopupMessage : MonoBehaviour
{
    private const float DISAPPEAR_TIMER_MAX = 0.5f;

    private TextMeshPro textMesh;
    float disappearTimer = DISAPPEAR_TIMER_MAX;
    float disappearSpeed = 2f;
    float increaseScaleAmount = 1f;
    Color textColor;
    Vector3 moveVector = new Vector3(0, 1) * 4f;

    private void Awake() {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(int value, PopupType type, string op) {
        textMesh.SetText(op + value.ToString());
        switch(type) {
            case PopupType.Damage:
                textMesh.color = Color.yellow;
            break;

            case PopupType.AP: 
                textMesh.color = Color.blue;
            break;

            case PopupType.MP:
                textMesh.color = Color.green;
            break;

            case PopupType.Heal:
                textMesh.color = Color.red;
            break;
        }
    }

    private void Update() {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 3f * Time.deltaTime;

        // First half 
        if(disappearTimer > DISAPPEAR_TIMER_MAX * .5f) {
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }

        else {
            transform.localScale -= Vector3.one * increaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if(disappearTimer<0) {
            // Start disappearing
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if(textColor.a < 0) {
                Destroy(gameObject);
            }
        }
    }
}
