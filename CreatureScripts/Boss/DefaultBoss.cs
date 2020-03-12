using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBoss : PlayerBase
{
    [SerializeField]
    private static int phase;
    [SerializeField]
    private static int maxPhase;

    private DefaultBoss bossGameObject;

    private void Start()
    {
        // Find Gamecontrol
        gameControl = GameObject.Find("GameControl").GetComponent<GameControl>();


        if (laneNum == (PublicLevel.usingLaneNum - 1))
        {
            DefaultBoss _bossGameObject = null;
            GameObject[] bossObjects = GameObject.FindGameObjectsWithTag("Boss");
            foreach (GameObject bossObject in bossObjects)
            {
                if (bossObject.GetComponent<DefaultCreature>().GetLaneNum() == (PublicLevel.usingLaneNum / 2))
                {
                    _bossGameObject = bossObject.GetComponent<DefaultBoss>();
                    bossObject.transform.GetChild(0).localScale = new Vector3(0.3f, 0.04f, 1.0f);
                }
                else
                {
                    // make other boss parts invisible
                    bossObject.GetComponent<SpriteRenderer>().enabled = false;
                    bossObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                }
            }

            for(int i = 0; i < PublicLevel.usingLaneNum; ++i)
            {
                bossObjects[i].tag = "Lane" + i.ToString();
                bossObjects[i].GetComponent<DefaultBoss>().SetBossObject(_bossGameObject);
                // set boss healthbar
                bossObjects[i].GetComponent<DefaultBoss>().hostileHealthBar = _bossGameObject.gameObject.transform.GetChild(0).gameObject;
            }

            SetMaxPhase(3);
            hostileCurrentHealth = hostileMaxHealth;
        }
    }

    // set new phase for boss
    protected virtual void SetMaxPhase(int _phase)
    {
        maxPhase = _phase;
        phase = maxPhase - 1;
    }

    // set new phase for boss
    protected virtual void PhaseUp()
    {
        phase--;
    }

    // bosses detect all lanes
    protected override void DetectEnemy()
    {
        bool detectCheck = false;
        currentPosition = transform.position;
        for(int i = 0; i < PublicLevel.usingLaneNum; ++i)
        {
            if(combatControl.SearchCreature(currentPosition, detectRange, i, side))
            {
                detectCheck = true;
                break;
            }
        }
        if (detectCheck)
        {
            moveFlag = false;
            attackFlag = true;
        }
        else
        {
            attackFlag = false;
            moveFlag = true;
        }
    }

    // boss damage are taken only on bossObject
    public override void DamageTaken(int damage, int size)
    {
        if(damage < 0)
        {
            return;
        }
        else
        {
            hostileCurrentHealth -= damage;
            base.CalculateHealth();

            if(hostileCurrentHealth < hostileMaxHealth * phase / maxPhase)
            {
                PhaseUp();
            }
        }
    }

    // bosses attack attacks all lanes
    public void BossAttack()
    {
        for(int i = 0; i < PublicLevel.usingLaneNum; ++i)
        {
            combatControl.MeleeAttack(currentPosition, attackRange, attackDamage, size, i, side);
        }
        if (animControl != null)
            animControl.SetBool("onAttack", true);

    }

    // attack mechanism of boss
    protected override void Attack()
    {
        if (attackFlag)
        {
            bossGameObject.BossAttack();
        }
        else
        {
            if (animControl != null)
                animControl.SetBool("onAttack", false);
        }
    }

    // special skill of boss
    protected virtual void BossSkill(int i)
    {

    }

    public void SetBossObject(DefaultBoss _bossGameObject)
    {
        bossGameObject = _bossGameObject;
    }

    public void SetHostileHealthBar(GameObject _hostileHealthBar)
    {
        hostileHealthBar = _hostileHealthBar;
    }
}
