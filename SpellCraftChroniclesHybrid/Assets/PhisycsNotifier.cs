using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhisycsNotifier : MonoBehaviour
{
    public float platformtimer = 0.5f;
    public CharacterStateMachine NotificationTarget;

    public void Start()

    {
        NotificationTarget.notifier = this;
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Floor"))
        {
            NotificationTarget.touchesFloor = false;
            NotificationTarget.gameObject.transform.parent = null; // this is to un-stick the player to the platform
        }
        if (collision.transform.CompareTag("Actor"))
        {
            NotificationTarget.touchActor = false;
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        platformtimer -= Time.deltaTime;
        if (platformtimer <= 0)
        {
            if (collision.transform.CompareTag("Floor"))
            {
                NotificationTarget.touchesFloor = true;
            }
            platformtimer = 0.5f;
        }
    }

    // never put triggers on the player or they will boh take damage
    public void OnTriggerEnter2D(Collider2D colider)
    {
        if (colider.transform.CompareTag("Actor"))
        {
            if (colider.gameObject.transform.parent.gameObject.GetComponent<StatBlock>() != gameObject.transform.parent.gameObject.GetComponent<StatBlock>())
            {
                //NotificationTarget.touchActor = true;
                //NotificationTarget.damageSource = colider.gameObject.transform.parent.gameObject.GetComponent<StatBlock>();
            
                var otherObject = colider.gameObject.GetComponent<PhisycsNotifier>();

                if (otherObject.NotificationTarget.curState != CharacterStateMachine.PossibleStates.DealDamage)
                {
                    int damage = NotificationTarget.characterStats.Damage - otherObject.NotificationTarget.characterStats.Defense;
                    if (damage > 0)
                        otherObject.NotificationTarget.characterStats.HP -= damage;
                }
            }
        }

        StatBlock statblock = colider.gameObject.GetComponent<StatBlock>();
        if (statblock != null)
        {
            int damage = statblock.Damage - NotificationTarget.characterStats.Defense;
            if (damage > 0)
                NotificationTarget.characterStats.HP -= damage;
        }
    }

    public void OnTriggerStay2D(Collider2D colider)
    {
        StatBlock statblock = colider.gameObject.GetComponent<StatBlock>();
        if (statblock != null)
        {
            int damage = statblock.Damage - NotificationTarget.characterStats.Defense;
            if (damage > 0)
                NotificationTarget.characterStats.HP -= damage;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Floor"))
        {
            NotificationTarget.touchesFloor = true;
            NotificationTarget.gameObject.transform.parent = collision.transform; // this is to stick the player to the platform
        }
        else
        {
            NotificationTarget.TouchSomething = true;
            Vector2 normal = (collision.collider.bounds.center - gameObject.GetComponent<Collider2D>().bounds.center).normalized;
            if (collision.transform.CompareTag("Actor"))
            {
                NotificationTarget.touchActor = true;
                NotificationTarget.damageSource = collision.gameObject.transform.parent.gameObject.GetComponent<StatBlock>();
            }
            else
            {
                
                if (NotificationTarget.curState == CharacterStateMachine.PossibleStates.Bouncing)
                {
                    NotificationTarget.ragdollPhisics.gravityScale += 0.2f;
                    if (normal.x > 0)
                    {
                        gameObject.transform.parent.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else gameObject.transform.parent.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
        }
    }
  

}
