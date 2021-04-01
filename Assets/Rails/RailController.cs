using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RailController : MonoBehaviour {
    [SerializeField] List<TileBase> tileBase;
    [SerializeField] Camera cam;
    [SerializeField] Tilemap tilemap;
    [SerializeField] ITilemap iTilemap;
    [SerializeField] SpriteRenderer helper;

    bool drawHelper = false;
    Vector3Int mousePos;
    Vector3 mouseWorldPoint;
    int tileIndex = 0;

    void Update() {
        if(drawHelper) {
            TileData td = new TileData();
            tileBase[tileIndex].GetTileData(mousePos, iTilemap, ref td);
            helper.sprite = td.sprite;
            helper.transform.position = mousePos;
            helper.transform.Translate(new Vector3(0.5f, 0.5f, 0));
            drawHelper = false;
        }
        else {
            helper.sprite = null;
        }
    }

    public void GetInputs() {
        mouseWorldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3Int(Mathf.FloorToInt(mouseWorldPoint.x), Mathf.FloorToInt(mouseWorldPoint.y), 0);
        
        if(Input.GetMouseButtonDown(0)) {
            tilemap.SetTile(mousePos, tileBase[tileIndex]);
        }

        drawHelper = true;
    }

}
