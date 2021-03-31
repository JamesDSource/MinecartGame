using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RailController : MonoBehaviour {
    [SerializeField] TileBase tileBase;
    [SerializeField] GameObject dummyTile;
    [SerializeField] Camera cam;
    [SerializeField] Tilemap tilemap;

    Vector3 mousePos;

    void Update() {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }
}
