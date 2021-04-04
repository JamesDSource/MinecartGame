using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int enemyHP = 3;
    public void EnemyTakeDamage()
    {
        enemyHP -= 1;
        if (enemyHP <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
