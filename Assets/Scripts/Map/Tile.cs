using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    public void setCoordinates(int x, int y) {
        this.x = x;
        this.y = y;
    }
}
