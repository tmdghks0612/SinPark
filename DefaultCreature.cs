using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultCreature : MonoBehaviour
{
    // Start is called before the first frame update
    //Transform creatureTransform;

	private GameControl.Sides side;
	private int laneNum;
	private Vector3 speed;
	private int hp;
	private int attackDamage;
	private float attackSpeed;
	private float attackRange;
	private enum attackType { Melee, Missile };
	private attackType creatureAttackType;
	private Vector3 start;
	private Vector3 end;
	private Vector3 currentPosition;
	private bool moveFlag = true;
	private bool Enemy = false;
	public GameControl gameControl;
	public CombatControl combatControl;
	public Animation anim;
	void Start()
    {

	}

	void MoveTo(Vector2 st, Vector2 ed)
	{
		start = st;
		end = ed;
		transform.position = start;
		moveFlag = true;
	}
	void DetectEnemy()
	{
		bool detectCheck;
		currentPosition = transform.position;
		detectCheck = SearchCreature(currentPosition, attackRange, laneNum, side);

		if(detectCheck)
		{
			moveFlag = false;
			if(creatureAttackType == attackType.Melee)
			{
				MeleeAttack(currentPosition, attackRange, attackDamage, laneNum, side);
			}
			else if(creatureAttackType == attackType.Missile)
			{
				
			}

		}
		else
		{
			moveFlag = true;
		}

		Debug.Log("detect");
	}
	public void SetCreature(Vector2 st, Vector2 ed, int lane, GameControl.Sides sideCheck)
	{
		InitCreature();
		laneNum = lane;
		side = sideCheck;
		if(side == GameControl.Sides.Hostile)
		{
			speed.x *= -1;
			Enemy = true;
			transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
			Debug.Log(speed.ToString());
		}

		MoveTo(st, ed);
	}

	void InitCreature()
	{
		speed = new Vector3(0.1f,0,0); //Fixed Value. Should be changed later
		attackSpeed = 2.0f; //Fixed Value. Should be changed later
		InvokeRepeating("DetectEnemy", 0.5f, attackSpeed);
	}
	void DamageTaken(int damage)
	{
		hp -= damage;
		if (hp <= 0)
			Dead();
	}
	void Dead()
	{
		combatControl.PopCreature(laneNum, side, this);
		Destroy(this);
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
}
