using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Numbers : MonoBehaviour {
    public static float Approach(float a, float b, float magnitude) {
        if(Mathf.Abs(a - b) <= magnitude) {
            return b;
        }
        else if(a < b) {
            return a + Mathf.Abs(magnitude);
        }
        else {
            return a - Mathf.Abs(magnitude);
        }
    }
}
