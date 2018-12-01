using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMind : MonoBehaviour
{
    public CharacterStateMachine charCtrl;
    public StatBlock stats;
    public SimpleHealthBar Hpbar;
    Transform HpLoc;
    public bool paused;
    public float powerCharge = 3;
    public Image superBar;
    public Image superBarBG;
    public Image[] powerCharges;
    internal bool delayOneFrame;

    // Use this for initialization
    void Start ()
    {
        charCtrl = gameObject.GetComponent<CharacterStateMachine>();
        stats = gameObject.GetComponent<StatBlock>();
        HpLoc = transform.Find("hpLocation");
    }

    public void OnDamageCollisionDelegate(Collider2D thingYouHit)
    {
        Debug.Log("OnPointerDownDelegate called.");
    }

    // Update is called once per frame
    void Update()
    {
        if (paused)
            return;
        if (delayOneFrame)
        {
            delayOneFrame = false;
            return;
        }
        if (powerCharge < 3)
        {
            powerCharge += Time.deltaTime/8;
            int fullAmount = Mathf.FloorToInt(powerCharge);
            for (int i = 1; i <= 3; i++)
            {
                if(i< powerCharge)
                    powerCharges[i-1].fillAmount = 100;
                else
                {
                    if(i-powerCharge <=1)
                        powerCharges[i-1].fillAmount = powerCharge-fullAmount;
                    else powerCharges[i - 1].fillAmount = 0;
                }
            }
        }
        if (Hpbar != null)
        {
            Hpbar.transform.parent.transform.position = HpLoc.position;//Camera.main.WorldToScreenPoint(HpLoc.position);
            Hpbar.UpdateBar(stats.HP, 100);
            if (charCtrl.curState == CharacterStateMachine.PossibleStates.DealDamage)
            {
                superBar.fillAmount = charCtrl.DamageModeTimer / 2;
                superBarBG.fillAmount = 100;
            }
            else
            {
                superBar.fillAmount = 0;
                superBarBG.fillAmount = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //charCtrl.Attack();
            if (charCtrl.curState == CharacterStateMachine.PossibleStates.Bouncing || charCtrl.curState == CharacterStateMachine.PossibleStates.DealDamage)
            {
                charCtrl.exitBounce = true;
            }
            else charCtrl.aimFlag = !charCtrl.aimFlag;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (powerCharge > 1)
            {
                if (charCtrl.curState == CharacterStateMachine.PossibleStates.Bouncing)
                {
                    charCtrl.enterDamageMode = true;
                    powerCharge -= 1;
                }
                else
                {
                    charCtrl.DamageModeTimer = 0;
                }
            }
        }

        if (Input.GetAxis("Horizontal") != 0)
        {
            charCtrl.RunningFlag = true;
        }
        else charCtrl.RunningFlag = false;

        charCtrl.xAccel = Input.GetAxis("Horizontal");

        if (charCtrl.dead)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
        //if (!charCtrl.Flop)
        // {
        //    charCtrl.Run(Input.GetAxis("Horizontal"));
        // }

        //else
        //  {
        //      var v3 = Input.mousePosition;
        //      v3.z = 0;
        //     v3 = Camera.main.ScreenToWorldPoint(v3);
        //     charCtrl.RotateTowards(v3);
        // }
    }
}
