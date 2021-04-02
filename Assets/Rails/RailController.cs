using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RailController : MonoBehaviour {
    [SerializeField] Camera cam;
    [SerializeField] Tilemap tilemap;
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
    Vector3Int mousePos;
    Vector3 mouseWorldPoint;
    
    void Start() {
        tileGrid = new TileType[tileDimensions.x, tileDimensions.y];
        
        for(int i = 0; i < tileGrid.GetLength(0); i++) {
            for(int j = 0; j < tileGrid.GetLength(1); j++) {

            }
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
            drawHelper = false;
        }
        else {
            helper.sprite = null;
        }
    }

    void UpdateTile(Vector3Int position, bool updateNeighbors) {

    }

    public void GetInputs(ref int tracksHeld) {
        mouseWorldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3Int(Mathf.FloorToInt(mouseWorldPoint.x), Mathf.FloorToInt(mouseWorldPoint.y), 0);
        
        tileIndex += Mathf.FloorToInt(Input.mouseScrollDelta.y);
        if(tileIndex < 0) {
            tileIndex = tileTypes.Length - 1;
        }
        else if(tileIndex >= tileTypes.Length) {
            tileIndex = 0;
        }

        if((tracksHeld > 0 || tilemap.GetTile(mousePos)) && Input.GetMouseButtonDown(0)) {
            if(!tilemap.GetTile(mousePos)) {
                tracksHeld--;
            }
            //tilemap.SetTile(mousePos, tileBase[tileIndex]);
        }
        
        if(Input.GetMouseButtonDown(1) && tilemap.GetTile(mousePos)) {
            tilemap.SetTile(mousePos, null);
            tracksHeld++;
        }
        
        drawHelper = true;
    }
}
