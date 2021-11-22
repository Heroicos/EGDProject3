
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossEnemy : MonoBehaviour
{
    [SerializeField] private string type;                     //Name (Ex: TestBox)

    [SerializeField] private int phase;               //currPhase of the boss
    [SerializeField] private List<float> health;      //List of the bosses health set at the begining of each of their phase
    [SerializeField] private List<float> armor;       //List of the bosses armor set at the begining of each of their phase
    private float currHealth;
    private float currArmor;

    private bool invincible;
    private bool vulnarable;

    protected Rigidbody rgbdy;

    public void SetInvincible(bool t) { invincible = t; }
    public void SetVulnarable(bool t) { vulnarable = t; }
    public bool GetInvincible() { return invincible; }
    public bool GetVulnarablity() { return vulnarable; }


    // Start is called before the first frame update
    void Start()
    {
        Introduction();
        rgbdy = this.GetComponent<Rigidbody>();

        invincible = false;
        vulnarable = false;

        rgbdy.useGravity = true;
        phase = 1;              
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    
    public virtual void Introduction()
    {
        Debug.Log(string.Format("Indroducing Platformer Boss Enemy with health of {1}.", type, health));
    }

    void SetPhase()
    {
        currHealth = health[phase-1];
        currArmor = armor[phase - 1];
    }

    public void TakeDamage(float damage)
    {
        if (vulnarable)
        {
            if (currArmor > 0)
            {
                currArmor -= (3 / 4) * damage;
                currHealth -= (1 / 4) * damage;
                Debug.Log(string.Format("Platform Boss took {0} damage to armor and {1} damage to health", (3 / 4) * damage, (1 / 4) * damage));
            }
            else
            {
                currHealth -= damage;
                Debug.Log(string.Format("Platform Boss took {0} damage", damage));
            }

            Debug.Log(string.Format("Platform Boss Current Health and Armor: {0} & {1}", currHealth, currArmor));

            if (currHealth <= 0)
            { Death(); }
        }
    }

    public void BecomeVulnarable()
    {
        StartCoroutine("VulnarableTimer");
    }

    IEnumerator VulnarableTimer()
    {
        vulnarable = true;

        yield return new WaitForSeconds(6);

        vulnarable = false;
    }


    public void Death()
    {
        Debug.Log(string.Format("Enemy {0} destroyed", type));
        Destroy(this.gameObject);
    }
}
