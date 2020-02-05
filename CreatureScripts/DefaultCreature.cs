using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultCreature : MonoBehaviour
{
	// Start is called before the first frame update
	//Transform creatureTransform;

	public enum AttackType { Melee, Missile, Heal };
	[SerializeField]
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
    protected int size;
    [SerializeField]
	protected float detectRange;
	[SerializeField]
    protected int manaCost;
    [SerializeField]
    protected Vector3 speed = new Vector3( 0.05f, 0, 0 );
	[SerializeField]
	protected int buttonNum;
	[SerializeField]
	protected GameObject projectile;

	private float pushDistance = 1.0f;
	private float deathDelay = 0.05f;
	private Vector3 start;
	private Vector3 end;
  
	protected Vector3 currentPosition;
    protected bool attackFlag = false;
    protected bool moveFlag = true;
	
	protected bool Enemy = false;

    //script instances
	protected GameControl gameControl;
	protected CombatControl combatControl;
	protected Animator animControl;
	void Start()
    {
		animControl = GetComponent<Animator>();
	}

	
	protected void MoveTo(Vector3 st, Vector3 ed)
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
	public virtual void SetCreature(Vector3 st, Vector3 ed, int _buttonNum, int lane, GameControl.Sides sideCheck)
	{
		InitCreature();
		laneNum = lane;
		gameObject.tag = "Lane" + lane;
		gameObject.layer = 15 - lane;
		side = sideCheck;
		detectRange = attackRange;
		maxHp = hp;
		buttonNum = _buttonNum;
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
				combatControl.MeleeAttack(currentPosition, attackRange, attackDamage, size, laneNum, side);
				if (animControl != null)
					animControl.SetBool("onAttack", true);
			}
			else if (creatureAttackType == AttackType.Missile)
			{
				combatControl.MissileAttack(currentPosition, projectile, attackDamage, laneNum, side);
				//animation
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

	public virtual void DamageTaken(int damage, int size)
	{
		hp -= damage;
		if (hp <= 0)
		{
			InvokeRepeating("Dead", deathDelay, attackSpeed);
			return;
		}

		if (damage > 0)
		{
			if (size > this.size)
			{
				OnPushed();
			}
		}
		else if(hp > maxHp)
		{
			hp = maxHp;
		}
		
		
	}

    protected virtual void OnPushed()
    {
        if(side == GameControl.Sides.Friendly)
        {
            gameObject.transform.position -= new Vector3(pushDistance, 0, 0);
        }
        else
        {
            gameObject.transform.position += new Vector3(pushDistance, 0, 0);
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

    public int GetManaCost()
    {
        return manaCost;
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