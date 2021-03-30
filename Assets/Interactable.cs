using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour{
    public delegate void InteractedWith();
    public InteractedWith interactedWith;
}
