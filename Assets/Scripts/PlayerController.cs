using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PlayerController : MonoBehaviour
{
    public SaveFile so;
    private GameManager gameMan;
    private InputManager inputMan;
    private TransitionManager tranMan;
    private BoxCollider playerColl;
    private AudioSource playerAudS; 
    private GameObject secondaryAxis;
    private List<GameObject> genreCosmetics;
    [HideInInspector] public Rigidbody playerRigB;
    [HideInInspector] public MeshFilter playerMeshF;
    [HideInInspector] public MeshRenderer playerMeshR;
    [HideInInspector] public GameObject closestTile;

    [Header("Components")]
    public GameObject bulletPrefab;
    public GameObject deathPrefab;
    public List<AudioClip> playerClips;
    public GameObject dashEffect;

    [Header("General Vars")]
    public float currentHP;
    public float maxHP;
    public Image hpBar;
    public Image extraBar;
    public Image platBar;
    public Image shotBar;
    public Image rpgBar;
    public LayerMask hazardMask;
    public GameObject objective;
    public GameObject arrows;
    private GameObject tempArrow;

    [Header("General Movement")]
    public float xForce;
    public float yForce;
    public float maxXVelocity;
    public float maxYVelocity;

    [Header("General Timers")]
    public float toggleTimer;
    public float toggleDelayTime;

    [Header("Platformer Movement")]
    public float jumpForce;
    public float dashForce;
    public float maxJumpVelocity;
    public float maxFallVelocity;

    [Header("Platformer Trackers")]
    public int jumpCount;
    public int maxJumps;
    public int wallJumpCount;
    public int maxWallJumps;
    public int dashCount;
    public int maxDashes;
    private Vector3 wallNorm;

    [Header("Platformer Timers")]
    public float jumpTimer;
    public float jumpDelayTime;
    public float wallJumpTimer;
    public float wallJumpDelayTime;
    public float dashTimer;
    public float dashDelayTime;
    //For double tap 
    public float lastDashTime = 0;
    public float dashSpeed = 0.2f;

    [Header("Platformer Data")]
    public bool grounded;
    public bool walled;
    public LayerMask groundingMask;

    [Header("Shooter Timers")]
    public float shootTimer;
    public float shootDelayTime;
    public float rollTimer;
    public float rollDelayTime;
    public float damageTimer;
    public float damageDelayTime;

    [Header("Shooter Data")]
    public float rollMultiplier;
    public bool rolling;

    [Header("Genre-Control Data")]
    public States.GameGenre playerPrimaryGenre;
    public States.GameGenre playerSubGenre;
    public delegate void ControlDelegate();
    public ControlDelegate controlDel;

    /*============================================================================
     * DEFAULT UNITY METHODS
     ============================================================================*/
    private void Start()
    {
        so = SaveManager.Load();
        if (so.lastPosition != new Vector3(0.0f, 0.0f, 0.0f))
        {
            transform.position = so.lastPosition;
        }
        gameMan = GameObject.Find("[MANAGER]").GetComponent<GameManager>();
        inputMan = GameObject.Find("[MANAGER]").GetComponent<InputManager>();
        tranMan = GameObject.Find("[MANAGER]").GetComponent<TransitionManager>();
        playerColl = GetComponent<BoxCollider>();
        hpBar = GameObject.Find("Canvas").transform.Find("GamePanel").transform.Find("HealthBase").GetChild(1).GetComponent<Image>();
        extraBar = GameObject.Find("Canvas").transform.Find("GamePanel").transform.Find("DashBase").GetChild(1).GetComponent<Image>();
        playerAudS = GetComponent<AudioSource>();
        playerRigB = GetComponent<Rigidbody>();
        playerMeshF = gameObject.transform.GetChild(0).GetComponent<MeshFilter>();
        playerMeshR = gameObject.transform.GetChild(0).GetComponent<MeshRenderer>();
        secondaryAxis = gameObject.transform.GetChild(1).gameObject;
        genreCosmetics = new List<GameObject>();
        for (int i = 0; i < gameObject.transform.GetChild(2).childCount; i++)
        {
            genreCosmetics.Add(gameObject.transform.GetChild(2).GetChild(i).gameObject);
        }
        SetDefaultValues();
        objective = GameObject.FindGameObjectWithTag("Objective");
        Debug.Log("OBJ with tage == " + objective);
        tempArrow = Instantiate(arrows);
        tempArrow.transform.SetParent(GameObject.Find("Canvas").transform, false);
        tempArrow.SetActive(false);
        if (gameMan.genreMain == States.GameGenre.None)
        {
            if (GameObject.Find("Canvas").transform.Find("GamePanel").transform.Find("PlatBase").GetChild(0).GetComponent<Image>() != null)
                platBar = GameObject.Find("Canvas").transform.Find("GamePanel").transform.Find("PlatBase").GetChild(0).GetComponent<Image>();
            if (GameObject.Find("Canvas").transform.Find("GamePanel").transform.Find("ShotBase").GetChild(0).GetComponent<Image>() != null)
                shotBar = GameObject.Find("Canvas").transform.Find("GamePanel").transform.Find("ShotBase").GetChild(0).GetComponent<Image>();
            if (GameObject.Find("Canvas").transform.Find("GamePanel").transform.Find("RPGBase").GetChild(0).GetComponent<Image>() != null)
                rpgBar = GameObject.Find("Canvas").transform.Find("GamePanel").transform.Find("RPGBase").GetChild(0).GetComponent<Image>();
            if (platBar != null)
            {
                if (so.platDone)
                {
                    platBar.color = new Color(0.0f, 1.0f, 0.0f);
                }
                else
                {
                    platBar.color = new Color(1.0f, 1.0f, 1.0f);
                }
            }
            if (shotBar != null)
            {
                if (so.shotDone)
                {
                    shotBar.color = new Color(0.0f, 1.0f, 0.0f);
                }
                else
                {
                    shotBar.color = new Color(1.0f, 1.0f, 1.0f);
                }
            }
            if (rpgBar != null)
            {
                if (so.rpgDone)
                {
                    rpgBar.color = new Color(0.0f, 1.0f, 0.0f);
                }
                else
                {
                    rpgBar.color = new Color(1.0f, 1.0f, 1.0f);
                }
            }
        }
        lastDashTime = Time.deltaTime;
    }

    private void Update()       //Handles live/uneven changes
    {
        if (!gameMan.paused)
        {
            TimerUpdate();
            CursorUpdate();
        }
        if (objective != null)
        {
            if (playerPrimaryGenre == States.GameGenre.Platformer || playerPrimaryGenre == States.GameGenre.None)
            {
                //Debug.Log("Objective good");
                Vector3 screenpos = Camera.main.WorldToScreenPoint(objective.transform.position);
                if (screenpos.z > 0 &&
                    screenpos.x > 0 && screenpos.x < Screen.width &&
                    screenpos.y > 0 && screenpos.y < Screen.height)
                {

                }
                else
                {
                    if (screenpos.z < 0) screenpos *= -1;
                    Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 4;
                    screenpos -= screenCenter;
                    float angle = Mathf.Atan2(screenpos.y, screenpos.x);
                    angle -= 90 * Mathf.Deg2Rad;

                    float cos = Mathf.Cos(angle);
                    float sin = -Mathf.Sin(angle);
                    screenpos = screenCenter + new Vector3(sin * 150, cos * 150, 0);

                    float m = cos / sin;

                    Vector3 screenBounds = screenCenter * 0.8f;

                    if (cos > 0) screenpos = new Vector3(screenpos.y / m, screenpos.y, 0);
                    else screenpos = new Vector3(-screenpos.y / m, -screenpos.y, 0);

                    if (screenpos.x > screenBounds.x) screenpos = new Vector3(screenpos.x, screenpos.x * m, 0);
                    else if (screenpos.x < screenBounds.x) screenpos = new Vector3(screenpos.x, -screenpos.x * m, 0);

                    screenpos += screenCenter;
                    tempArrow.transform.localPosition = screenpos;
                    tempArrow.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
                }
            }
            else
            {
                //Debug.Log("Objective good");
                Vector3 screenpos = Camera.main.WorldToScreenPoint(objective.transform.position);
                if (screenpos.z > 0 &&
                    screenpos.x > 0 && screenpos.x < Screen.width &&
                    screenpos.y > 0 && screenpos.y < Screen.height)
                {

                }
                else
                {
                    if (screenpos.z < 0) screenpos *= -1;
                    Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 3;
                    screenpos -= screenCenter;
                    float angle = Mathf.Atan2(screenpos.y, screenpos.x);
                    angle -= 90 * Mathf.Deg2Rad;

                    float cos = Mathf.Cos(angle);
                    float sin = -Mathf.Sin(angle);
                    screenpos = screenCenter + new Vector3(sin * 150, cos * 150, 0);

                    float m = cos / sin;

                    Vector3 screenBounds = screenCenter * 0.9f;

                    if (cos > 0) screenpos = new Vector3(screenpos.y / m, screenpos.y, 0);
                    else screenpos = new Vector3(-screenpos.y / m, -screenpos.y, 0);

                    if (screenpos.x > screenBounds.x) screenpos = new Vector3(screenpos.x, screenpos.x * m, 0);
                    else if (screenpos.x < screenBounds.x) screenpos = new Vector3(screenpos.x, -screenpos.x * m, 0);

                    screenpos += screenCenter;
                    tempArrow.transform.localPosition = screenpos;
                    tempArrow.transform.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
                }
            }
        }
    }

    private void FixedUpdate()  //Handles steady/stable changes
    {
        if (!gameMan.paused && !gameMan.dead && controlDel != null) { controlDel(); }
        MiscFUpdate();
        //Delegate uses current genre control schema
        //Actions currently use keyhold value(not KEYDOWN) since input may get dropped, might revise later
    }

    private void OnCollisionEnter(Collision collision)
    {
        grounded = GroundCheck();
        walled = WallCheck();
        EnemyCheck(collision);
        jumpCount = grounded ? 0 : jumpCount;
        wallJumpCount = grounded ? 0 : wallJumpCount;
    }

    private void OnCollisionExit(Collision collision)
    {
        grounded = GroundCheck();
        walled = WallCheck();
    }

    /*============================================================================
     * MOVEMENT UPDATE METHODS
     ============================================================================*/
    public void PlatformerMoveUpdate()
    {
        if (!grounded)  //Airborne check
        {
            playerRigB.AddForce(Physics.gravity, ForceMode.Force);
            if (playerRigB.velocity.y < maxFallVelocity) { playerRigB.velocity = new Vector3(playerRigB.velocity.x, maxFallVelocity, playerRigB.velocity.z); }
            else if (playerRigB.velocity.y > maxJumpVelocity) { playerRigB.velocity = new Vector3(playerRigB.velocity.x, maxJumpVelocity, playerRigB.velocity.z); }
        }

        if (inputMan.inputAct4 == 1 && jumpTimer <= 0) //Jump check
        {
            if (!grounded && walled && wallJumpCount < maxWallJumps)
            {
                playerAudS.PlayOneShot(playerClips[2]);
                wallJumpCount++;
                Vector3 jumpDir = (wallNorm + transform.up).normalized;
                playerRigB.AddForce(jumpDir * 50000.0f, ForceMode.Force);
                wallJumpTimer = wallJumpDelayTime;
                jumpTimer = jumpDelayTime;
            }
            else if (jumpCount < maxJumps)
            {
                playerAudS.PlayOneShot(playerClips[2]);
                jumpCount++;
                playerRigB.AddForce(new Vector3(0, jumpForce * jumpCount, 0), ForceMode.Impulse);
                jumpTimer = jumpDelayTime;
            }
        }

        if ((inputMan.inputFire1_D || inputMan.inputAct6_D || (inputMan.inputSubmit_D && playerRigB.velocity.x != 0)) && dashCount < 1)   //Dash check
        {
            GameObject temp = Instantiate(dashEffect, transform.position, transform.rotation);
            playerRigB.AddForce(new Vector3(inputMan.inputX * dashForce, 0, 0), ForceMode.VelocityChange);
            dashCount++;
            dashTimer = dashDelayTime;
            lastDashTime = Time.deltaTime;
        }
        else if (inputMan.inputX_D)
        {
            lastDashTime = Time.deltaTime;
        }
        extraBar.fillAmount = -(dashTimer - dashDelayTime) / dashDelayTime;

        if (dashTimer >= dashDelayTime) dashCount = 0;

        if (wallJumpTimer <= 0)    //X move check
        {
            playerRigB.AddForce(new Vector3(inputMan.inputX * xForce, 0, 0), ForceMode.VelocityChange);
        }

        float xClampVel = (inputMan.inputX == 0) ? 0 : Mathf.Clamp(Mathf.Abs(playerRigB.velocity.x), 0, maxXVelocity) * Mathf.Sign(playerRigB.velocity.x);
        playerRigB.velocity = new Vector3(xClampVel, playerRigB.velocity.y, playerRigB.velocity.z);
    }

    public void ShooterMoveUpdate()
    {
        if (inputMan.inputAct4 == 1 && rollTimer <= 0) //Roll check (current mapping == q)
        {
            rollTimer = rollDelayTime;
            StartCoroutine(Roll());
        }
        /*else if (rollX != 0 || rollY != 0)  //IMPLEMENT ASAP TO FOLLOW SINGLE ROLL DIRECTION
        {

        }*/
        if (!rolling)
        {
            extraBar.fillAmount = -(rollTimer - rollDelayTime) / rollDelayTime;
            playerRigB.AddForce(new Vector3(inputMan.inputX * xForce, 0, 0), ForceMode.VelocityChange);
            playerRigB.AddForce(new Vector3(0, 0, inputMan.inputY * yForce), ForceMode.VelocityChange);
            float xClampVel = (inputMan.inputX == 0) ? 0 : Mathf.Clamp(Mathf.Abs(playerRigB.velocity.x), 0, maxXVelocity) * Mathf.Sign(playerRigB.velocity.x);  //X move check
            float yClampVel = (inputMan.inputY == 0) ? 0 : Mathf.Clamp(Mathf.Abs(playerRigB.velocity.z), 0, maxYVelocity) * Mathf.Sign(playerRigB.velocity.z);  //Y move check
            Utils.XYMoveRecalc(ref xClampVel, ref yClampVel);
            playerRigB.velocity = new Vector3(xClampVel, playerRigB.velocity.y, yClampVel);
        }
    }

    public void RPGMoveUpdate()
    {
        float xClampVel = 0, yClampVel = 0, zClampVel = 0;
        if (inputMan.inputX != 0)  //X move check
        {
            float zSin = Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * transform.localEulerAngles.z));
            float zCos = Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * transform.localEulerAngles.z));
            playerRigB.AddForce(new Vector3(inputMan.inputX * xForce * zCos, 0, 0), ForceMode.VelocityChange);
            //playerRigB.AddForce(new Vector3(0, inputMan.inputX * xForce * zSin, 0), ForceMode.VelocityChange);
            playerRigB.AddForce(new Vector3(0, inputMan.inputX * xForce * Mathf.Sin(Mathf.Deg2Rad * transform.localEulerAngles.z), 0), ForceMode.VelocityChange);
            xClampVel = Mathf.Clamp(Mathf.Abs(playerRigB.velocity.x), 0, maxXVelocity * zCos) * Mathf.Sign(playerRigB.velocity.x);
            yClampVel = Mathf.Clamp(Mathf.Abs(playerRigB.velocity.y), 0, maxXVelocity * zSin) * Mathf.Sign(playerRigB.velocity.y);
        }
        else if (inputMan.inputY != 0)  //Z move check
        {
            float xSin = Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad * transform.localEulerAngles.x));
            float xCos = Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad * transform.localEulerAngles.x));
            playerRigB.AddForce(new Vector3(0, 0, inputMan.inputY * yForce * xCos), ForceMode.VelocityChange);
            playerRigB.AddForce(new Vector3(0, inputMan.inputY * yForce * -Mathf.Sin(Mathf.Deg2Rad * transform.localEulerAngles.x), 0), ForceMode.VelocityChange);
            zClampVel = Mathf.Clamp(Mathf.Abs(playerRigB.velocity.z), 0, maxYVelocity * xCos) * Mathf.Sign(playerRigB.velocity.z);
            yClampVel = Mathf.Clamp(Mathf.Abs(playerRigB.velocity.y), 0, maxYVelocity * xSin) * Mathf.Sign(playerRigB.velocity.y);
        }

        

        if (closestTile != null)
        {
            closestTile.GetComponent<GridUnit>().occupied = inputMan.inputX == 0 && inputMan.inputY == 0;
            transform.position = closestTile.GetComponent<GridUnit>().occupied ? closestTile.transform.position : transform.position;
        }  



        playerRigB.velocity = new Vector3(xClampVel, yClampVel, zClampVel);
    }

    public void HubMoveUpdate()
    {
        playerRigB.AddForce(new Vector3(inputMan.inputX * xForce, 0, 0), ForceMode.VelocityChange);
        playerRigB.AddForce(new Vector3(0, inputMan.inputY * yForce, 0), ForceMode.VelocityChange);
        float xClampVel = (inputMan.inputX == 0) ? 0 : Mathf.Clamp(Mathf.Abs(playerRigB.velocity.x), 0, maxXVelocity) * Mathf.Sign(playerRigB.velocity.x);  //X move check
        float yClampVel = (inputMan.inputY == 0) ? 0 : Mathf.Clamp(Mathf.Abs(playerRigB.velocity.y), 0, maxYVelocity) * Mathf.Sign(playerRigB.velocity.y);  //Y move check
        Utils.XYMoveRecalc(ref xClampVel, ref yClampVel);
        playerRigB.velocity = new Vector3(xClampVel, yClampVel, playerRigB.velocity.z);
    }

    /*============================================================================
     * GAMEPLAY UPDATE METHODS
     ============================================================================*/
    private void CursorUpdate() //Cursor related changes
    {
        if (playerPrimaryGenre == States.GameGenre.Shooter)
        {
            float aimAngle = ((Mathf.Atan2(inputMan.inputMY - Screen.height / 2, inputMan.inputMX - Screen.width / 2) * Mathf.Rad2Deg) + 360) % 360;
            transform.eulerAngles = (playerPrimaryGenre == States.GameGenre.Platformer) ? new Vector3(-aimAngle, 90, -90) : new Vector3(0, -aimAngle + 90, 0);

        }
        if (inputMan.inputFire1 == 1 && shootTimer <= 0 && playerPrimaryGenre == States.GameGenre.Shooter)
        {
            playerAudS.PlayOneShot(playerClips[4]);
            Instantiate(bulletPrefab, transform.position, secondaryAxis.transform.rotation).GetComponent<Bullet>().Init(gameObject.tag);
            shootTimer = shootDelayTime;
        }

    }

    private void TimerUpdate()  //Timer related changes
    {
        jumpTimer -= jumpTimer > 0 ? Time.deltaTime : 0;
        wallJumpTimer -= wallJumpTimer > 0 ? Time.deltaTime : 0;
        dashTimer -= dashTimer > 0 ? Time.deltaTime : 0;
        shootTimer -= shootTimer > 0 ? Time.deltaTime : 0;
        rollTimer -= rollTimer > 0 ? Time.deltaTime : 0;
        toggleTimer -= toggleTimer > 0 ? Time.deltaTime : 0;
        damageTimer -= damageTimer > 0 ? Time.deltaTime : 0;

        dashCount = dashTimer <= 0 ? 0 : dashCount;
    }

    private void MiscFUpdate()   //Misc changes
    {
        if( playerPrimaryGenre == States.GameGenre.Shooter && damageTimer <= 0 && currentHP < maxHP)
        {
            currentHP += 0.01f;
            currentHP = currentHP > maxHP ? maxHP : currentHP;
        }
    }

    public void GenreCosmeticUpdate(int index)  //Cosmetic changes
    {
        foreach (GameObject g in genreCosmetics) { g.SetActive(false); }
        if (index > -1 && index < genreCosmetics.Count) { genreCosmetics[index].SetActive(true); }
    }

    public float HealthUpdate(float change) //Health changes
    {
        currentHP += change;
        if (currentHP <=  0 && !gameMan.dead)
        {
            currentHP = 0;
            gameMan.dead = true;
            playerRigB.constraints = RigidbodyConstraints.FreezeAll;
            playerMeshF.mesh = null;
            GenreCosmeticUpdate(-1);
            playerAudS.PlayOneShot(playerClips[6]);
            Destroy(Instantiate(deathPrefab, transform.position, transform.rotation), 4.0f);
            StartCoroutine(Respawn());
        }
        damageTimer = change < 0 ? damageDelayTime : damageTimer;
        hpBar.fillAmount = currentHP / maxHP;
        return currentHP;
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2f);
        SaveManager.Save(so);
        if (playerPrimaryGenre == States.GameGenre.Platformer)
        {
            tranMan.SceneSwitch("PlatformerWorld");
        }
        else if (playerPrimaryGenre == States.GameGenre.Shooter)
        {
            tranMan.SceneSwitch("ShooterWorld");
        }
        else if (playerPrimaryGenre == States.GameGenre.RPG)
        {
            tranMan.SceneSwitch("RPGWorld");
        }
    }

    /*============================================================================
     * COLLISION METHODS
     ============================================================================*/
    private bool GroundCheck()
    {
        Vector3 rightPos = transform.position + new Vector3(0.275f, 0, 0);
        Vector3 leftPos = transform.position + new Vector3(-0.275f, 0, 0);

        bool castContact = false;
        castContact = castContact | Physics.Raycast(rightPos, -transform.up, 0.4f, groundingMask);  //RIGHT CAST
        castContact = castContact | Physics.Raycast(leftPos, -transform.up, 0.4f, groundingMask);   //LEFT CAST
        return castContact;
    }

    private bool WallCheck()
    {
        wallNorm = Vector3.zero;
        float closestDist = 100.0f;
        Vector3[] sides = { transform.position + new Vector3(0, 0.275f, 0), transform.position + new Vector3(0, -0.275f, 0) };
        RaycastHit hit;

        foreach (Vector3 v in sides)
        {
            if (Physics.Raycast(v, transform.right, out hit, 0.4f, groundingMask))
            {
                if (hit.distance < closestDist)
                {
                    closestDist = hit.distance;
                    wallNorm = hit.normal;
                }
            }
            if (Physics.Raycast(v, -transform.right, out hit, 0.4f, groundingMask))
            {
                if (hit.distance < closestDist)
                {
                    closestDist = hit.distance;
                    wallNorm = hit.normal;
                }
            }
        }
        return wallNorm != Vector3.zero;
    }

    private bool EnemyCheck(Collision coll)
    {
        if (playerPrimaryGenre == States.GameGenre.Platformer)
        {
            if (coll.gameObject.tag == "Enemy" || coll.gameObject.tag == "Boss")
            {
                Vector3 rightPos = transform.position + new Vector3(0.25f, 0, 0);
                Vector3 leftPos = transform.position + new Vector3(-0.25f, 0, 0);
                if (Physics.Raycast(rightPos, -transform.up, 0.4f, hazardMask)   //RIGHT CAST
                                | Physics.Raycast(leftPos, -transform.up, 0.4f, hazardMask))  //LEFT CAST
                {
                    if (coll.gameObject.tag == "Enemy")
                    {
                        PlatformerEnemy pScript = coll.collider.GetComponent<PlatformerEnemy>();
                        pScript.TakeDamage(pScript.GetHealth());
                    }
                    else
                    {
                        BossEnemy bScript = coll.collider.gameObject.GetComponent<BossEnemy>();
                        bScript.TakeDamage(80.0f);
                    }
                    playerRigB.AddForce(transform.up * 25000.0f, ForceMode.Force);
                }
                else
                {
                    if (coll.gameObject.tag == "Enemy")
                    {
                        PlatformerEnemy pScript = coll.collider.GetComponent<PlatformerEnemy>();
                        HealthUpdate(-pScript.GetAtkDamage());
                    }
                    else
                    {
                        BossEnemy bScript = coll.collider.GetComponent<BossEnemy>();
                        HealthUpdate(-3);
                    }
                    Vector3 pushDir = Vector3.right * (transform.position.x >= coll.transform.position.x ? 1 : -1);
                    playerRigB.AddForce((10 * pushDir + Vector3.up).normalized * 30000.0f, ForceMode.Force);
                }
                return true;
            }
            return false;
        }
        return false;
    }

    /*============================================================================
     * COROUTINES
     ============================================================================*/
    private IEnumerator Roll()
    {
        float tempMaxX = maxXVelocity, tempMaxY = maxXVelocity;
        maxXVelocity /= rollMultiplier;
        maxYVelocity /= rollMultiplier;
        yield return new WaitForSeconds(0.5f);
        rolling = true;
        maxXVelocity = tempMaxX * rollMultiplier;
        maxYVelocity = tempMaxY * rollMultiplier;

        playerRigB.AddForce(new Vector3(inputMan.inputX * xForce * rollMultiplier, 0, 0), ForceMode.VelocityChange);
        playerRigB.AddForce(new Vector3(0, 0, inputMan.inputY * yForce * rollMultiplier), ForceMode.VelocityChange);
        float xClampVel = (inputMan.inputX == 0) ? 0 : Mathf.Clamp(Mathf.Abs(playerRigB.velocity.x), 0, maxXVelocity) * Mathf.Sign(playerRigB.velocity.x);
        float yClampVel = (inputMan.inputY == 0) ? 0 : Mathf.Clamp(Mathf.Abs(playerRigB.velocity.z), 0, maxYVelocity) * Mathf.Sign(playerRigB.velocity.z);
        Utils.XYMoveRecalc(ref xClampVel, ref yClampVel);
        playerRigB.velocity = new Vector3(xClampVel, playerRigB.velocity.y, yClampVel);
        yield return new WaitForSeconds(0.25f);
        rolling = false;
        maxXVelocity = tempMaxX;
        maxYVelocity = tempMaxY;
    }

    /*============================================================================
     * MISC METHODS
     ============================================================================*/
    private void SetDefaultValues()
    {
        maxHP = 10.0f;
        currentHP = maxHP;
        
        xForce = 4.0f;
        yForce = 4.0f;
        jumpForce = 150.0f;
        dashForce = 80.0f;

        maxXVelocity = 6.0f;
        maxYVelocity = 6.0f;
        maxJumpVelocity = 6.0f;
        maxFallVelocity = -12.0f;

        jumpCount = 0;
        maxJumps = 2;
        wallJumpCount = 0;
        maxWallJumps = 6;
        dashCount = 0;
        maxDashes = 1;

        jumpTimer = 0;
        jumpDelayTime = 0.5f;
        wallJumpTimer = 0;
        wallJumpDelayTime = 0.2f;
        dashTimer = 0;
        dashDelayTime = 1.0f;
        shootTimer = 0;        
        shootDelayTime = 0.5f;
        toggleTimer = 0;
        toggleDelayTime = 1.0f;
        rollTimer = 0;
        rollDelayTime = 2.0f;
        damageTimer = 0;
        damageDelayTime = 3.0f;

        rollMultiplier = 4.0f;
        rolling = false;

        grounded = false;
        walled = false;
    }
}