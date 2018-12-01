using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class CharacterStateMachine : MonoBehaviour
{
    public bool playerChar;
    public Transform aimRectangle;
    public Rigidbody2D ragdollPhisics;
    public Animator ragdol;
    public PhysicsMaterial2D bouncyBall;
    public PhysicsMaterial2D normalCharacter;
    public StatBlock characterStats;
    public StatBlock damageSource;
    public PhisycsNotifier notifier;
    public AnimationCurve speedIncrease;
    public float speedIncreaseTimer;
    public List<GameObject> deleteOnDeath;
    public BodyPartCarousel deathPartSelection;
    public enum PossibleStates
    {
        Idle,
        Running,
        BounceAim,
        Bouncing,
        TakeDamage,
        DealDamage,
    }
    public delegate void State();

    private State[] stateMap;
    public PossibleStates curState = PossibleStates.Idle;

    //flags
    private bool runningFlag;
    public bool aimFlag;
    public bool bouncingFlag;
    public bool touchActor;
    public bool touchesFloor;
    public bool exitBounce;
    //paramters
    public float xAccel;
    public bool damageTaken;

    public PossibleStates lastState;
    public bool dead;
    private bool touchSomething;

    private Vector2 bounceVelocity;
    public bool enterDamageMode;
    public float DamageModeDuration = 3;
    public float DamageModeTimer;

    //utility members

    public bool RunningFlag
    {
        get
        {
            return runningFlag;
        }

        set
        {
            // We cant have you running while aiming
            if(aimFlag && value)
                return;

           runningFlag = value;
        }
    }

    public bool TouchSomething
    {
        get
        {
            bool originalVal = touchSomething;
            if (originalVal)
                touchSomething = false;
            return originalVal;
        }

        set
        {
            touchSomething = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        stateMap = new State[6];
        stateMap[0] = Idle;
        stateMap[1] = Running;
        stateMap[2] = BounceAim;
        stateMap[3] = Bouncing;
        stateMap[4] = TakeDamage;
        stateMap[5] = DealDamage;

        ragdollPhisics = transform.GetComponentInChildren<Rigidbody2D>();
        var possibleAnimators = transform.GetComponentsInChildren<Animator>();
        if(possibleAnimators.Length>1)
        ragdol = transform.GetComponentsInChildren<Animator>()[1];
        else ragdol = transform.GetComponentsInChildren<Animator>()[0];
        characterStats = transform.GetComponentInChildren<StatBlock>();
        aimRectangle = transform.Find("JumpController");
        playerChar = gameObject.GetComponent<PlayerMind>() != null;
        DamageModeTimer = DamageModeDuration;
    }

    void DoStateStransitions()
    {
        switch (curState)
        {
            case PossibleStates.Idle:
                {
                    if (touchActor)
                    {
                        curState = PossibleStates.TakeDamage;
                        lastState = PossibleStates.Idle;
                        aimFlag = false;
                        runningFlag = false;
                    }

                    if (aimFlag)
                    {
                        aimRectangle.gameObject.SetActive(true);
                        curState = PossibleStates.BounceAim;
                    }

                    if (runningFlag && touchesFloor)
                    {
                        curState = PossibleStates.Running;
                        ragdol.SetTrigger("StartRun");
                        ragdol.SetBool("IsRunning", true);
                    }

                    if (xAccel != 0)
                    {

                    }

                }
                break;
            case PossibleStates.Running:
                {
                    if (!touchesFloor)
                    {
                        runningFlag = false;
                        ragdol.SetBool("IsInAir", true);
                        ragdol.SetTrigger("StartJump");
                    }
                    if (touchActor)
                    {
                        lastState = PossibleStates.Running;
                        curState = PossibleStates.TakeDamage;
                        if (playerChar)
                        {
                            aimFlag = false;
                            runningFlag = false;
                        }
                    }

                    if (aimFlag)
                    {
                        runningFlag = false;
                        ragdollPhisics.velocity = new Vector2(0, 0);
                    }

                    if (!runningFlag)
                    {
                        curState = PossibleStates.Idle;
                        ragdol.SetBool("IsRunning", false);
                        ragdol.SetTrigger("StopRun");
                        speedIncreaseTimer = 0;
                        ragdollPhisics.velocity = new Vector2(0, ragdollPhisics.velocity.y);
                    }
                }
                break;
            case PossibleStates.BounceAim:
                {
                    if (!aimFlag)
                    {
                        curState = PossibleStates.Bouncing;
                        ragdollPhisics.velocity = (Vector2FromAngle(aimRectangle.rotation.eulerAngles.z) * 15);
                        bounceVelocity = ragdollPhisics.velocity;
                        aimRectangle.rotation = Quaternion.identity;
                        ragdollPhisics.gravityScale = 0;
                        ragdollPhisics.sharedMaterial = bouncyBall;
                        aimRectangle.gameObject.SetActive(false);
                        ragdol.SetTrigger("StartJump");
                        ragdol.SetBool("IsInAir", true);
                        ragdollPhisics.mass = 1;
                    }

                    if (touchActor)
                    {
                        curState = PossibleStates.Idle;
                        runningFlag = false;
                        aimRectangle.gameObject.SetActive(false);
                        aimFlag = false;
                    }
                }
                break;
            case PossibleStates.Bouncing:
                {
                    if (exitBounce)
                    {
                        curState = PossibleStates.Idle;
                        ragdollPhisics.velocity = Vector2.zero;
                        ragdollPhisics.sharedMaterial = normalCharacter;
                        ragdollPhisics.gravityScale = 1;
                        ragdol.SetBool("IsInAir", false);
                        ragdollPhisics.mass = 1000000;
                        exitBounce = false;
                    }

                    if (enterDamageMode)
                    {
                        ragdol.SetTrigger("StartDamage");
                        ragdol.SetBool("InDamageMode", true);
                        enterDamageMode = false;
                        curState = PossibleStates.DealDamage;
                    }
                }
                break;
            case PossibleStates.TakeDamage:
                {
                    curState = lastState;
                    damageTaken = false;
                    if (!playerChar)
                        touchActor = false;
                }
                break;
            case PossibleStates.DealDamage:
                if (exitBounce)
                {
                    DamageModeTimer = 0;
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            if (deathPartSelection)
            {
                deathPartSelection.gameObject.SetActive(true);
                deathPartSelection.enemyStats = characterStats;
                deathPartSelection.currentStats = damageSource;
                deathPartSelection.Initialise();
            }
            this.enabled = false;
            return;
        }

        // this stops characters from floating away;
        ragdollPhisics.gameObject.transform.position = new Vector3(ragdollPhisics.gameObject.transform.position.x,ragdollPhisics.gameObject.transform.position.y,0);
        transform.position = ragdollPhisics.gameObject.transform.position;
        ragdollPhisics.gameObject.transform.localPosition = Vector3.zero;

        stateMap[(int)curState]();
        DoStateStransitions();
        if (characterStats.HP <= 0)
        {
            deleteOnDeath.ForEach(e=> Destroy(e));
            ragdol.SetTrigger("Dead");
            dead = true;
        }

        if (characterStats.HpLowered)
        {
            ragdol.SetTrigger("DamageTaken");
        }
    }

    public void Idle()
    {
        if (touchesFloor)
        {
            ragdol.SetBool("IsInAir", false);
        }
        else
        {
            ragdollPhisics.velocity = new Vector2(characterStats.Speed * xAccel/3, ragdollPhisics.velocity.y);
        }
    }

    public void Running()
    {
        speedIncreaseTimer += Time.deltaTime*2;
        if (speedIncreaseTimer > 1)
            speedIncreaseTimer = 1;
        if (xAccel < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (xAccel > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        ragdollPhisics.velocity = new Vector2(characterStats.Speed * xAccel * speedIncrease.Evaluate(speedIncreaseTimer),ragdollPhisics.velocity.y); //
    }

    public void BounceAim()
    {
        var target = Input.mousePosition;
        target.z = 0;
        target = Camera.main.ScreenToWorldPoint(target);

        float AngleRad = Mathf.Atan2(target.y - aimRectangle.position.y, target.x - aimRectangle.position.x);
        // Get Angle in Degrees
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        // Rotate Object
        aimRectangle.rotation = Quaternion.Euler(0, 0, AngleDeg);
        if (AngleDeg > 90)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void Bouncing()
    {
        if (ragdollPhisics.velocity.magnitude > bounceVelocity.magnitude)
        {
            ragdollPhisics.velocity = ragdollPhisics.velocity.normalized * bounceVelocity.magnitude;
        }

        if (ragdollPhisics.velocity.magnitude < 5)
            exitBounce = true;
    }

    public void DealDamage()
    {
        DamageModeTimer -= Time.deltaTime;
        if (DamageModeTimer <= 0)
        {
            DamageModeTimer = DamageModeDuration;
            ragdol.SetBool("InDamageMode", false);
            curState = PossibleStates.Bouncing;
            transform.rotation = Quaternion.identity;
        }
        if (ragdollPhisics.velocity.magnitude < 5)
            DamageModeTimer = 0;
    }

    public void RestoreVelocity()
    {
        if (curState == PossibleStates.Bouncing || curState == PossibleStates.DealDamage)
        {
            if (ragdollPhisics.velocity.magnitude < bounceVelocity.magnitude)
            {
                ragdollPhisics.velocity = ragdollPhisics.velocity.normalized * bounceVelocity.magnitude;
            }
        }
    }
    public void TakeDamage()
    {
        if (!damageTaken)
        {
            bool enemyBouncing = damageSource.gameObject.GetComponent<CharacterStateMachine>().curState == PossibleStates.DealDamage;
            bool isPlayer = gameObject.GetComponent<PlayerMind>() != null;
            if (enemyBouncing || isPlayer)
            {
                int damage = damageSource.Damage - characterStats.Defense;
                if (damage > 0)
                    characterStats.HP -= damage;
            }

            damageSource.gameObject.GetComponent<CharacterStateMachine>().RestoreVelocity();
            damageTaken = true;
        }
    }

    //utility functions
    public Vector2 Vector2FromAngle(float a)
    {
        a *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
    }

}
