using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Pathfinding;

public class EnemyPathfinding : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject _player;
    public float speed = 1f;
    float number;
    private Transform _playerTransform;

    private Rigidbody2D rb;
    private Vector2 movingPosition;
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = _player.GetComponent<Transform>();

        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //move towards Player 
        movingPosition = Vector2.MoveTowards(transform.position, _playerTransform.position, speed * Time.deltaTime);

        float yNumber = Random.Range(-1f, 1f);
        if (yNumber > 0.6f)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - 0.01f);
        }
        else if (yNumber < -0.6f)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + 0.01f);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, _playerTransform.position, speed * Time.deltaTime);
        }
        //transform.position = Vector2.MoveTowards(transform.position, _playerTransform.position, speed * Time.deltaTime);
        /*a* pseudocode
                      
        list of open nodes
        list of closed nodes

        while()
        {

        if(currentNode.position == player.position)
        {
            return
        }
        }
         */
    }


}
