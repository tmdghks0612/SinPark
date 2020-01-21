using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultCreature : MonoBehaviour
{
	// Start is called before the first frame update
	//Transform creatureTransform;

	public enum AttackType { Melee, Missile, Heal };

	protected GameControl.Sides side;
	protected int laneNum;

	private int maxHp;
	[SerializeField]
	protected int hp;
	[SerializeField]
	protected int attackDamage;
	[SerializeField]
	protected float attackSpeed;
	[SerializeField]
	protected float attackRange;
	[SerializeField]
	protected AttackType creatureAttackType;
	[SerializeField]
	protected float detectRange;
	[SerializeField]
    protected int manaCost;
    [SerializeField]
	protected int creatureType;
    [SerializeField]
    protected Vector3 speed = new Vector3( 0.05f, 0, 0 );
	[SerializeField]
	protected int upgradeType;

	private float deathDelay = 0.05f;
	private Vector3 start;
	private Vector3 end;
  
	protected Vector3 currentPosition;
	protected bool moveFlag = true;
	protected bool attackFlag = false;
	protected bool Enemy = false;

    //script instances
	private GameControl gameControl;
	protected CombatControl combatControl;
	protected Animator animControl;
	void Start()
    {
		animControl = GetComponent<Animator>();
	}

	
	protected void MoveTo(Vector2 st, Vector2 ed)
	{
		start = st;
		end = ed;
		transform.position = start;
		moveFlag = true;
	}


	protected virtual void DetectEnemy()
	{
		bool detectCheck;
		currentPosition = transform.position;
		detectCheck = combatControl.SearchCreature(currentPosition, detectRange, laneNum, side);
		
		if(detectCheck)
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
	public virtual void SetCreature(Vector2 st, Vector2 ed, int creatureType, int upgradeType, int lane, GameControl.Sides sideCheck)
	{
		InitCreature();
		laneNum = lane;
		side = sideCheck;
		detectRange = attackRange;
		maxHp = hp;
        this.creatureType = creatureType;
		this.upgradeType = upgradeType;
		if(side == GameControl.Sides.Hostile)
		{
			speed.x *= -1;
			Enemy = true;
			transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
		}

		MoveTo(st, ed);
	}

	protected virtual void Attack()
	{
		if (attackFlag)
		{

			if (creatureAttackType == AttackType.Melee)
			{
				combatControl.MeleeAttack(currentPosition, attackRange, attackDamage, laneNum, side);
				if (animControl != null)
					animControl.SetBool("onAttack", true);
			}
			else if (creatureAttackType == AttackType.Missile)
			{
				combatControl.MissileAttack(currentPosition, creatureType, upgradeType, attackDamage, laneNum, side);
				if (animControl != null)
					animControl.SetBool("onAttack", true);
			}
			else if(creatureAttackType == AttackType.Heal)
			{
				combatControl.Heal(currentPosition, attackRange, attackDamage, laneNum, side);
				if (animControl != null)
					animControl.SetBool("onAttack", true);
			}
		}
		else
		{
			if (animControl != null)
				animControl.SetBool("onAttack", false);
		}
	}
	protected void InitCreature()
    { 
		InvokeRepeating("DetectEnemy", 0.1f, 0.1f);
		InvokeRepeating("Attack",0.1f,attackSpeed);
        //attackDamage = 3;
        //hp = 9;
	}
	public virtual void DamageTaken(int damage)
	{
		if(damage <0)
		{
			Debug.Log("Healed");
		}
		hp -= damage;
		if (hp <= 0)
		{
			InvokeRepeating("Dead", deathDelay, attackSpeed);
		}
		else if(hp > maxHp)
		{
			hp = maxHp;
		}
			

	}
	protected virtual void Dead()
	{
		combatControl.PopCreature(laneNum, side, this);
		CancelInvoke("Dead");
		CancelInvoke("DetectEnemy");
		Destroy(this.gameObject);
	}

	// Update is called once per frame
	void Update()
    {
        if(moveFlag)
		{
			if(!Enemy && transform.position.x <= end.x)
			{
				moveFlag = true;
				transform.position += speed;
			}
			else if(Enemy && transform.position.x >= end.x)
			{
				moveFlag = true;
				transform.position += speed;

			}
			else
			{
				moveFlag = false;
			}
		}
    }
	public GameControl.Sides getSide()
	{
		return side;
	}
	public void SetGameControl(GameControl GameControl)
	{
		gameControl = GameControl;
	}
	public void SetCombatControl(CombatControl CombatControl)
	{
		combatControl = CombatControl;
	}

}