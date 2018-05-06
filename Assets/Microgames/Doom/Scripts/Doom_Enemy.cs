using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doom_Enemy : MonoBehaviour
{

    [SerializeField]
    Vector3 targetPosition = Vector3.zero;
    [SerializeField]
    float moveSpeed = 5, damageDelay = 0.5f;
    [SerializeField]
    int damageAmount = 30, hp = 2;

    void Update()
    {
        if(Vector3.Distance(transform.position, targetPosition) < 5)
            DamagePlayer();
        else
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        transform.rotation = Camera.main.transform.rotation;
    }

    float delay = Mathf.Infinity;
    void DamagePlayer()
    {
        if(delay > damageDelay)
        {
            Doom_Player.hp -= damageAmount;
            delay = 0;
        }
        delay += Time.deltaTime;
    }

    public void DamageSelf()
    {
        hp--;
        if(hp <= 0)
            Destroy(gameObject);
        Vector3 direction = transform.position - targetPosition;
        direction.Normalize();
        StartCoroutine(Knockback(direction * 5));
    }

    IEnumerator Knockback(Vector3 force)
    {
        while(force.sqrMagnitude > 0.01f)
        {
            transform.position += force * Time.deltaTime * 10;
            force = Vector3.MoveTowards(force, Vector3.zero, Time.deltaTime * 50);
            yield return null;
        }
    }

}
