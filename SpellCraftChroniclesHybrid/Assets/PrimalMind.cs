using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimalMind : MonoBehaviour
{
    public CharacterStateMachine charCtrl;
    public StatBlock stats;
    public SimpleHealthBar Hpbar;

    public int walkDir = 1;
    Transform HpLoc;
    public bool turnsAroundOnTouch;
    public bool sentryMode;
    public int maxHP;
    public bool focusOnDeath;
    // Use this for initialization
    void Start () {
        charCtrl = gameObject.GetComponent<CharacterStateMachine>();
        stats = gameObject.GetComponent<StatBlock>();
        HpLoc = transform.Find("hpLocation");
        maxHP = stats.HP;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Hpbar!= null)
        {
            Hpbar.transform.parent.transform.position = HpLoc.position;//Camera.main.WorldToScreenPoint(HpLoc.position);
            Hpbar.UpdateBar(stats.HP, maxHP);
        }

        if (!sentryMode)
        {
            charCtrl.RunningFlag = true;
            charCtrl.xAccel = walkDir;
            if (turnsAroundOnTouch)
            {
                if (charCtrl.TouchSomething)
                    walkDir *= -1;
            }
            else
            {
                if (charCtrl.TouchSomething && !charCtrl.touchActor)
                    walkDir *= -1;
            }
        }

        if (focusOnDeath&& charCtrl.dead)
        {
            Camera.main.GetComponent<SmoothCamera2D>().target = transform;
        }
    }
}
