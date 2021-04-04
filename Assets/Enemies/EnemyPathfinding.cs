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

    float distanceToAttack;
    [SerializeField]
    float distanceToPlayer;

    [SerializeField]
    Vector3 originalPosition;

    Animator batAnimator;
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = _player.GetComponent<Transform>();

        distanceToAttack = 10f;

        batAnimator = gameObject.GetComponent<Animator>();
        batAnimator.SetBool("Flying", false);
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        transform.rotation = Quaternion.identity;
        distanceToPlayer = (_playerTransform.position - this.transform.position).sqrMagnitude;
        //move towards Player
        if (distanceToPlayer < distanceToAttack)
        {
            Movement(_playerTransform.position);
            batAnimator.SetBool("Flying", true);
            //animation switch to flying
        }
        else if(distanceToPlayer > distanceToAttack)
        {
            Movement(originalPosition);
            if (transform.position == originalPosition)
            {
                batAnimator.SetBool("Flying", false);
            }
        }
    }

    void Movement(Vector3 targetedPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetedPosition, speed * Time.deltaTime);
        
        /* For Jittery Movement
        float yNumber = Random.Range(-1f, 1f);
        if (yNumber > 0.6f)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - 0.05f);
        }
        else if (yNumber < -0.6f)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + 0.05f);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, _playerTransform.position, speed * Time.deltaTime);
        }*/
    }


}
