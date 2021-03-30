using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int enemyHP = 3;

    //I could create a slider for hp using UI
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            EnemyTakeDamage();
        }
    }

    void EnemyTakeDamage()
    {
        enemyHP -= 1;
        if (enemyHP <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
