using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Singleton
public class Map : MonoBehaviour
{
    public static Map instance = null;

    [Header("Components")]
    public GameObject hexPrefab;

    private UnitManager unitScript;

    int mapSizeX = 10;
    int mapSizeY = 10;

    public float xOffset = 0.882f;
    public float zOffset = 0.764f;

    //public float yOffset = 0.469f;
    public float yOffset = 0.115f;

    Node[,] graph;
    List<Node> currentPath = null;
    List<Node> currentSkillRange = null;
    List<Node> currentSkillRangeLastRow = null;


    private void Awake() {
        if(instance == null) 
            instance = this;
        else if(instance != this)
            Destroy(gameObject);

        currentSkillRange = new List<Node>();
        currentSkillRangeLastRow = new List<Node>();

        GenerateMap();
        GeneratePathfindingGraph();
    }


    void GenerateMap() {
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {

                float xPos = x * xOffset;

                if(y%2 == 1)
                    xPos += xOffset/2f;
                
                GameObject hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(xPos, 0, y * zOffset), Quaternion.identity);
                hex_go.GetComponent<Tile>().setCoordinates(x, y);
                hex_go.name = "Hex_" + x + "_" + y;
                hex_go.transform.SetParent(this.transform);
            }
        }
    }

    void GeneratePathfindingGraph() {
        // Initialize the array
        graph = new Node[mapSizeX, mapSizeY];

        // Initialize a Node for each spot in the array
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {
                graph[x,y] = new Node(x, y);
            }
        }
        
        // Calculate their neighbours
        for (int x = 0; x < mapSizeX; x++) {
            for (int y = 0; y < mapSizeY; y++) {
                if(y%2 != 1) {  // Even 
                    if(x > 0) // left
                        graph[x,y].neighbours.Add(graph[x-1, y]);
                    if(y < mapSizeY-1 && x > 0) // top-left
                        graph[x,y].neighbours.Add(graph[x-1, y+1]);
                    if(y < mapSizeY-1) // top-right
                        graph[x,y].neighbours.Add(graph[x, y+1]);
                    if(x < mapSizeX-1) // right
                        graph[x,y].neighbours.Add(graph[x+1, y]);
                    if(y > 0) // bottom-right
                        graph[x,y].neighbours.Add(graph[x, y-1]);
                    if(y > 0 && x > 0) // bottom-left
                        graph[x,y].neighbours.Add(graph[x-1, y-1]);
                }

                else {  // Odd
                    if(x > 0) // left
                        graph[x,y].neighbours.Add(graph[x-1, y]);
                    if(y < mapSizeY-1) // top-left
                        graph[x,y].neighbours.Add(graph[x, y+1]);
                    if(x < mapSizeX-1 && y < mapSizeY-1) // top-right
                        graph[x,y].neighbours.Add(graph[x+1, y+1]);
                    if(x < mapSizeX-1) // right
                        graph[x,y].neighbours.Add(graph[x+1, y]);
                    if(x < mapSizeX-1) // bottom-right
                        graph[x,y].neighbours.Add(graph[x+1, y-1]);
                    // bottom-left
                        graph[x,y].neighbours.Add(graph[x, y-1]);
                }
            }
        }
    }

    public Vector3 TileCoordToWorldCoord(int x, int y) {
        return GameObject.Find("Hex_" + x + "_" + y).transform.position + new Vector3(0, yOffset, 0);
    }

    public void GeneratePathTo(int x, int y) { // Dijkstra algorithm
        ClearCurrentPath();
        
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        // List of unvisited nodes yet
        List<Node> unvisited = new List<Node>();

        UnitManager activeUnit = GameManager.instance.GetActiveUnit();

        Node source = graph[activeUnit.posX, activeUnit.posY];
        Node target = graph[x,y];

        dist[source] = 0;
        prev[source] = null;

        foreach(Node v in graph) {
            if(v != source) {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }

            unvisited.Add(v);
        }

        while(unvisited.Count > 0) {
            // u is going to be the unvisited node with the smallest distance.
            Node u = null;

            foreach(Node possibleU in unvisited) {
                if(u == null || dist[possibleU] < dist[u]) {
                    u = possibleU;
                }
            }

            if(u == target)
                break;

            unvisited.Remove(u);

            foreach(Node v in u.neighbours) {
                float alt = dist[u] + u.DistanceTo(v);
                if(alt < dist[v]) {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }
        // If we get here, either we found the shortest route to our target, or there is no route at all
        if(dist[target] == null) {
            // No route to our target and the source
            Debug.Log("Error: There is no route to our target from the source");
            return;
        }

        currentPath = new List<Node>();
        Node curr = target;

        // Step through the prev chain and add it to our path
        while(curr != null) {
            currentPath.Add(curr);
            curr = prev[curr];
        }

        currentPath.Reverse();
        currentPath.RemoveAt(0);

        if(currentPath.Count <= GetActiveUnit().movementPoints && !GetActiveUnit().isMoving) {
            foreach(Node v in currentPath) {
                MeshRenderer mr = GameObject.Find("Hex_" + v.x + "_" + v.y).GetComponentInChildren<MeshRenderer>();
                mr.material.color = Color.green;
            }
        }
    }

    public void ClearCurrentPath() {
        if(currentPath != null) {
            foreach(Node v in currentPath) {
                MeshRenderer mr = GameObject.Find("Hex_" + v.x + "_" + v.y).GetComponentInChildren<MeshRenderer>();
                mr.material.color = Color.white;
            }
        }
    }

    public void InitializeMovement() {
        if(!GetActiveUnit().isMoving && GetActiveUnit().movementPoints >= currentPath.Count) {
            foreach(Node v in currentPath) {
                MeshRenderer mr = GameObject.Find("Hex_" + v.x + "_" + v.y).GetComponentInChildren<MeshRenderer>();
                mr.material.color = Color.white;
            }

            GetActiveUnit().InitializeMovement(currentPath);
        }
    }

    public void CalculateAttackRangeAroundUnit(int range, bool playerIncluded) {
        currentSkillRange = new List<Node>();
        currentSkillRangeLastRow = new List<Node>();
        List<Node> tmpNodeList = new List<Node>();
        UnitManager currentUnit = GameManager.instance.GetActiveUnit();

        currentSkillRange.Add(graph[currentUnit.posX, currentUnit.posY]);

        if(range > 0) {
            foreach(Node v in graph[currentUnit.posX, currentUnit.posY].neighbours) {
                currentSkillRange.Add(v);
                currentSkillRangeLastRow.Add(v);
            }

            for(int i=1; i < range; i++) {
                foreach(Node v in currentSkillRangeLastRow) {
                    foreach(Node w in v.neighbours) {
                        if(!currentSkillRange.Exists(node => node.isEqualTo(w))) {
                            currentSkillRange.Add(w);
                            tmpNodeList.Add(w);
                        }
                    }
                }
                currentSkillRangeLastRow.AddRange(tmpNodeList);
                tmpNodeList.Clear();
            }
        }
        
        if(!playerIncluded)
            currentSkillRange.RemoveAt(0);

        PaintTilesFromRange();
    }

    public void CalculateAttackRangeInStraightLine(int range, bool playerIncluded) {
        currentSkillRange = new List<Node>();
        currentSkillRangeLastRow = new List<Node>();
        List<Node> tmpNodeList = new List<Node>();
        UnitManager currentUnit = GameManager.instance.GetActiveUnit();
        int x = currentUnit.posX;
        int y = currentUnit.posY;

        currentSkillRange.Add(graph[currentUnit.posX, currentUnit.posY]);

        if(range > 0) {
            foreach(Node v in graph[x, y].neighbours) {
                currentSkillRange.Add(v);
                currentSkillRangeLastRow.Add(v);
            }

            int val = -1;
            for(int i=1; i < range; i++) {
                if(x-(i+1) >= 0) // left
                    currentSkillRange.Add(graph[x-(i+1),y]);

                if(i%2 != 1) { // top-left / Odd
                    if(x-(i / 2) >= 0 && (y+i+1) <= mapSizeY) {
                        currentSkillRange.Add(graph[x-(i / 2), y+i+1]);
                    }
                }

                else { // Even
                    if((x-i+val) >= 0 && (y+i+1) <= mapSizeY) {
                        currentSkillRange.Add(graph[(x-i+val), y+i+1]);
                    }
                }

                if(i%2 != 1) { // top-right / Odd
                    if(x-(i / 2) >= mapSizeX && (y+i+1) <= mapSizeY) { 
                        currentSkillRange.Add(graph[x-(i / 2), y+i+1]);
                    }
                }

                else { // Even
                    if((x+i-val) <= mapSizeX && (y+i+1) <= mapSizeY) { 
                        currentSkillRange.Add(graph[(x-i-val), y+i+1]);
                    }
                }

                if(x-(i+1) <= mapSizeX) // right
                    currentSkillRange.Add(graph[x+(i+1),y]);
                
                if(i%2 != 1) { // bottom-right / Odd
                    if(x+(i / 2) <= mapSizeX && (y-i-1) >= 0) { 
                        currentSkillRange.Add(graph[x-(i / 2), y+i+1]);
                    }
                }

                else { // Even
                    if((x+i-val) <= mapSizeX && (y-i-1) >= 0) {
                        currentSkillRange.Add(graph[(x-i-val), y+i+1]);
                    }
                }
                


                // int index = 0;
                // foreach(Node v in currentSkillRangeLastRow) {   
                //     if(v.neighbours[index] != null) {
                //         currentSkillRange.Add(v.neighbours[index]);
                //         tmpNodeList.Add(v.neighbours[index]);
                //     }
                //     index++;                  
                // }
                // currentSkillRangeLastRow.AddRange(tmpNodeList);
                // tmpNodeList.Clear();
            }
        }
        
        if(!playerIncluded)
            currentSkillRange.RemoveAt(0);

        PaintTilesFromRange();
    }

    public void ClearRangeTiles() {
        foreach(Node v in currentSkillRange) {
            MeshRenderer mr = GameObject.Find("Hex_" + v.x + "_" + v.y).GetComponentInChildren<MeshRenderer>();
            mr.material.color = Color.white;
        }
    }

    public bool TileIsInRange(Tile tile) {
        return currentSkillRange.Exists(node => node.isEqualTo(new Node(tile.x, tile.y)));
    }

    UnitManager GetActiveUnit() {
        return GameManager.instance.GetActiveUnit().GetComponent<UnitManager>();
    }

    void PaintTilesFromRange() {
        foreach(Node v in currentSkillRange) {
            MeshRenderer mr = GameObject.Find("Hex_" + v.x + "_" + v.y).GetComponentInChildren<MeshRenderer>();
            mr.material.color = Color.blue;
        }
    }

    public void HoveringTile(Tile newTile, GameObject lastTile, bool reset) {
        if(lastTile != null) {
            Tile last = lastTile.GetComponent<Tile>();
            if(TileIsInRange(last)) {
                MeshRenderer mrLast = GameObject.Find("Hex_" + last.x + "_" + last.y).GetComponentInChildren<MeshRenderer>();
                mrLast.material.color = Color.blue;
            } 
        }
        
        if(!reset) {
            MeshRenderer mr = GameObject.Find("Hex_" + newTile.x + "_" + newTile.y).GetComponentInChildren<MeshRenderer>();
            mr.material.color = Color.red;   
        }

    }
}
