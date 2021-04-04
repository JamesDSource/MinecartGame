using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RailController : MonoBehaviour {
    [SerializeField] Camera cam;
    [SerializeField] Tilemap tilemap;
    [SerializeField] ITilemap iTilemap;
    [SerializeField] LayerMask placeCheckMask;

    [SerializeField] Vector2Int tileDimensions;

    public enum TileType {
        None,
        Straight,
        RampLeft,
        RampRight
    }
    TileType[,] tileGrid;

    [SerializeField] Sprite tileStraightSprite;
    [SerializeField] Sprite tileRampLeftSprite;
    [SerializeField] Sprite tileRampRightSprite;
    [SerializeField] SpriteRenderer helper;

    int tileIndex = 0;
    TileType[] tileTypes = {
        TileType.Straight,
        TileType.RampLeft,
        TileType.RampRight
    };

    [SerializeField] TileBase tileSMiddle;
    [SerializeField] TileBase tileSLeft;
    [SerializeField] TileBase tileSRight;
    [SerializeField] TileBase tileSSingle;
    [SerializeField] TileBase tileRLeft;
    [SerializeField] TileBase tileRRight;
    [SerializeField] TileBase tileCLeft;
    [SerializeField] TileBase tileCRight;


    TileType currentTileType = TileType.Straight;

    bool drawHelper = false;
    bool canPlace = false;
    Vector3Int mousePos;
    Vector3 mouseWorldPoint;
    
    void Start() {
        tileGrid = new TileType[tileDimensions.x, tileDimensions.y];
        List<Vector3Int> occupiedTiles = new List<Vector3Int>();

        for(int i = 0; i < tileGrid.GetLength(0); i++) {
            for(int j = 0; j < tileGrid.GetLength(1); j++) {
                var tile = tilemap.GetTile(new Vector3Int(i, j, 0));
                if( tile == tileSMiddle ||
                    tile == tileSLeft ||
                    tile == tileSRight ||
                    tile == tileSSingle) {
                    tileGrid[i, j] = TileType.Straight;
                }
                else if( tile == tileRLeft) {
                    tileGrid[i, j] = TileType.RampLeft;
                }
                else if( tile == tileRRight) {
                    tileGrid[i, j] = TileType.RampRight;
                }
                else {
                    tileGrid[i, j] = TileType.None;
                }

                if(tileGrid[i, j] != TileType.None) {
                    occupiedTiles.Add(new Vector3Int(i, j, 0));
                }
            }
        }

        // Updates each tile that is not empty
        foreach(Vector3Int tilePos in occupiedTiles) {
            UpdateTile(tilePos, true);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(tileDimensions.x/2, tileDimensions.y/2, 0), new Vector3(tileDimensions.x, tileDimensions.y, 0));
    }
    void Update() {
        if(drawHelper) {
            switch(currentTileType) {
                case TileType.Straight:
                    helper.sprite = tileStraightSprite;
                    break;
                case TileType.RampLeft:
                    helper.sprite = tileRampLeftSprite;
                    break;
                case TileType.RampRight:
                    helper.sprite = tileRampRightSprite;
                    break;
            }
            helper.transform.position = mousePos;
            helper.transform.Translate(new Vector3(0.5f, 0.5f, 0));

            if(canPlace) {
                helper.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            }
            else {
                helper.color = new Color(1.0f, 0, 0, 0.5f);
            }

            drawHelper = false;
        }
        else {
            helper.sprite = null;
        }
    }

    bool InRange(Vector3Int position) {
        return position.x >= 0 && position.x < tileGrid.GetLength(0) && position.y >= 0 && position.y < tileGrid.GetLength(1);
    }

    TileType GetTileType(Vector3Int position) {
        if(InRange(position)) {
            return tileGrid[position.x, position.y];
        }
        return TileType.None;
    }
    bool HasTile(Vector3Int position) {
        return GetTileType(position) != TileType.None;
    }

    void UpdateTile(Vector3Int position, bool updateNeighbors) {
        if(InRange(position)) {
            TileType currentType = tileGrid[position.x, position.y];
            TileBase setTileAs = null;
            switch(currentType) {
                case TileType.Straight:
                    bool rightFrom = GetTileType(position + new Vector3Int(1, 0, 0)) != TileType.None;
                    bool leftFrom = GetTileType(position + new Vector3Int(-1, 0, 0)) != TileType.None;
                    if(leftFrom && rightFrom) {
                        setTileAs = tileSMiddle;
                    }
                    else if(leftFrom) {
                        setTileAs = tileSRight;
                    }
                    else if(rightFrom) {
                        setTileAs = tileSLeft;
                    }
                    else {
                        setTileAs = tileSSingle;
                    }
                    
                    break;
                case TileType.RampLeft:
                    setTileAs = tileRLeft;
                    break;
                case TileType.RampRight:
                    setTileAs = tileRRight;
                    break;
                case TileType.None:
                    switch(GetTileType(position + new Vector3Int(0, 1, 0))) {
                        case TileType.RampLeft:
                            setTileAs = tileCRight;
                            break;
                        case TileType.RampRight:
                            setTileAs = tileCLeft;
                            break;
                    }
                    break;
            }
            tilemap.SetTile(position, setTileAs);
        }
        
        if(updateNeighbors) {
            UpdateTile(position + new Vector3Int(1, 0, 0), false);
            UpdateTile(position + new Vector3Int(-1, 0, 0), false);
            UpdateTile(position + new Vector3Int(0, 1, 0), false);
            UpdateTile(position + new Vector3Int(0, -1, 0), false);
            UpdateTile(position + new Vector3Int(1, 1, 0), false);
            UpdateTile(position + new Vector3Int(-1, 1, 0), false);
            UpdateTile(position + new Vector3Int(1, -1, 0), false);
            UpdateTile(position + new Vector3Int(-1, -1, 0), false);
        }
    }

    List<Vector3Int> GetConnecting(Vector3Int point) {
        List<Vector3Int> possibleConnections = new List<Vector3Int>();
        TileType pointType = GetTileType(point);
        switch(pointType) {
            case TileType.Straight:
                possibleConnections.Add(new Vector3Int(point.x + 1, point.y, 0));
                possibleConnections.Add(new Vector3Int(point.x - 1, point.y, 0));
                break;
            case TileType.RampLeft:
                possibleConnections.Add(new Vector3Int(point.x - 1, point.y, 0));
                possibleConnections.Add(new Vector3Int(point.x + 1, point.y - 1, 0));
                break;
            case TileType.RampRight:
                possibleConnections.Add(new Vector3Int(point.x + 1, point.y, 0));
                possibleConnections.Add(new Vector3Int(point.x - 1, point.y - 1, 0));
                break;
        }

        List<Vector3Int> returnArray = new List<Vector3Int>();
        foreach(Vector3Int connection in possibleConnections) {
            TileType connectionType = GetTileType(connection);
            if(connectionType != TileType.None) {
                returnArray.Add(connection);
            }
        }

        return returnArray;
    }

    bool IsConnection(Vector3Int startPoint, Vector3Int endPoint) {
        List<Vector3Int> connecting = GetConnecting(endPoint);
        foreach(Vector3Int connection in connecting) {
            if(CheckPath(connection, endPoint, startPoint)) {
                return true;
            }
        }
        return false;
    }
    bool CheckPath(Vector3Int point, Vector3Int from, Vector3Int lookingFor) {
        List<Vector3Int> connecting = GetConnecting(point);
        foreach(Vector3Int connection in connecting) {
            if(connection.Equals(lookingFor)) {
                return true;
            }

            if(!connection.Equals(from) && CheckPath(connection, point, lookingFor)) {
                return true;
            }
        }

        return false;
    }

    public void GetInputs(ref int tracksHeld) {
        canPlace = true;

        mouseWorldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3Int(Mathf.FloorToInt(mouseWorldPoint.x), Mathf.FloorToInt(mouseWorldPoint.y), 0);
        
        tileIndex += Mathf.FloorToInt(Input.mouseScrollDelta.y);
        if(tileIndex < 0) {
            tileIndex = tileTypes.Length - 1;
        }
        else if(tileIndex >= tileTypes.Length) {
            tileIndex = 0;
        }
        currentTileType = tileTypes[tileIndex];
        
        if(InRange(mousePos)) {
            if(Physics2D.OverlapBox(mousePos + (currentTileType == TileType.Straight ? new Vector3(0, 1, 0) : new Vector3()) + new Vector3(0.5f, 0.5f, 0), new Vector2(0.9f, 0.9f), 0, placeCheckMask)) {
                canPlace = false;
            }

            if(tracksHeld > 0 && Input.GetMouseButtonDown(0) && canPlace) {
                tileGrid[mousePos.x, mousePos.y] = currentTileType;
                UpdateTile(mousePos, true);
                tracksHeld--;
                
            }
            
            if(Input.GetMouseButtonDown(1) && HasTile(mousePos)) {
                tileGrid[mousePos.x, mousePos.y] = TileType.None;
                UpdateTile(mousePos, true);
                tracksHeld++;
            }
        }
        else {
            canPlace = false;
        }
        drawHelper = true;
    }
}
