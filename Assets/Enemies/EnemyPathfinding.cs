using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Pathfinding;

public class EnemyPathfinding : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject _player;
    public float speed = 1f;
    private Transform _playerTransform;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = _player.GetComponent<Transform>();
        if(_player == null)
        {
            Debug.Log("none");
        }
        else
        {
            Debug.Log("ye");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //move towards Player
        transform.position = Vector2.MoveTowards(transform.position, _playerTransform.position, speed * Time.deltaTime);
        Debug.Log(transform.position.x + "  " + _playerTransform.position.x);
    }
}
