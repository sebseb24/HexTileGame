using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [Header("Components")]
    public UnitManager player;

    GameObject lastHitObject = null;
    GameObject lastHitObjectOnSkillActive = null;

    int layerMask = (1 << 8);


    void Update()
    {
        if(GameManager.instance.GetActiveUnit().IsPlayable()) {
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

            RaycastHit hitInfo;

            if(Physics.Raycast(ray, out hitInfo, Mathf.Infinity, layerMask)) {
                GameObject hitObject =  hitInfo.collider.transform.parent.parent.gameObject;
                Tile tile = hitObject.GetComponent<Tile>();

                // Hover tiles without clicking (show path)
                if(hitObject != lastHitObject && GameManager.instance.activeSkill == 0) {
                    // For performance boost, only generate path if the mouse is hovering a new tile
                    lastHitObject = hitObject;
                    Map.instance.GeneratePathTo(tile.x, tile.y);
                    Map.instance.PaintShortestPath();
                }

                if(GameManager.instance.activeSkill != 0) {
                    if(Map.instance.TileIsInRange(tile)) {
                        if(hitObject != lastHitObjectOnSkillActive) {
                            Map.instance.HoveringTile(tile, lastHitObjectOnSkillActive, false);
                            lastHitObjectOnSkillActive = hitObject;
                        }
                    }

                    else {
                        Map.instance.HoveringTile(tile, lastHitObjectOnSkillActive, true);
                    }
                }

                else {
                    lastHitObjectOnSkillActive = null;
                }

                // On mouse click
                if(Input.GetButtonDown("Fire1")) {
                    // Move to click if no skill active
                    if(GameManager.instance.activeSkill == 0) {
                        Map.instance.InitializeMovement();
                    }

                    // Click + skill active
                    else {
                        if(Map.instance.TileIsInRange(tile)) {
                            GameManager.instance.GetActiveUnit().InteractOnTile(tile);
                        }

                        Map.instance.ClearRangeTiles();
                        GameManager.instance.activeSkill = 0;
                    }       
                }
            }
        }
    }
}
