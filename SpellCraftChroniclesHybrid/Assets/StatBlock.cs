using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBlock : MonoBehaviour
{
    public string Name;
    private int damage;
    public int Defense;
    public int LDamage;
    public int RDamage;
    public int LSpeed;
    public int RSpeed;
    [SerializeField]
    private int hP;
    private int speed;
    private bool hpLowered;
    
    public int HP
    {
        get
        {
            return hP;
        }

        set
        {
            if (value < hP)
            {
                HpLowered = true;
            }
            hP = value;
        }
    }

    public bool HpLowered
    {
        get
        {
            bool original = hpLowered;
            hpLowered = false;
            return original;
        }

        set
        {
            hpLowered = value;
        }
    }

    public int Damage
    {
        get
        {
            return LDamage + RDamage;
        }

    }

    public int Speed
    {
        get
        {
            return LSpeed + RSpeed;
        }
    }
}
