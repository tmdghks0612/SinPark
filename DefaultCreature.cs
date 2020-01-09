﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultCreature : MonoBehaviour
{
	// Start is called before the first frame update
	//Transform creatureTransform;

	public enum AttackType { Melee, Missile };

	private GameControl.Sides side;
	private int laneNum;
	private Vector3 speed;

	[SerializeField]
	private int hp;
	[SerializeField]
	private int attackDamage;
	[SerializeField]
	private float attackSpeed;
	[SerializeField]
	private float attackRange;
	[SerializeField]
	private AttackType creatureAttackType;
    [SerializeField]
    private int manaCost;

	private int creatureType;
	private Vector3 start;
	private Vector3 end;
	private Vector3 currentPosition;
	private bool moveFlag = true;
	private bool Enemy = false;

    //script instances
	private GameControl gameControl;
	private CombatControl combatControl;
	private Animator animControl;
	void Start()
    {
		animControl = GetComponent<Animator>();
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
		detectCheck = combatControl.SearchCreature(currentPosition, attackRange, laneNum, side);
		if(detectCheck)
		{
			moveFlag = false;
			if(creatureAttackType == AttackType.Melee)
			{
				combatControl.MeleeAttack(currentPosition, attackRange, attackDamage, laneNum, side);
				animControl.SetBool("onAttack", true);
			}
			else if(creatureAttackType == AttackType.Missile)
			{
                combatControl.MissileAttack(currentPosition, creatureType, attackDamage, laneNum, side);
                //animation
			}
		}
		else
		{
			animControl.SetBool("onAttack", false);
			moveFlag = true;
		}
	}
	public void SetCreature(Vector2 st, Vector2 ed, int creatureType, int lane, GameControl.Sides sideCheck)
	{
		InitCreature();
		laneNum = lane;
		side = sideCheck;
        this.creatureType = creatureType;
		if(side == GameControl.Sides.Hostile)
		{
			speed.x *= -1;
			Enemy = true;
			transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
		}

		MoveTo(st, ed);
	}

	void InitCreature()
	{
		speed = new Vector3(0.05f,0,0); //Fixed Value. Should be changed later
		InvokeRepeating("DetectEnemy", 0.5f, attackSpeed);
        //attackDamage = 3;
        //hp = 9;
	}
	public void DamageTaken(int damage)
	{
		hp -= damage;
		if (hp <= 0)
			Dead();
		
	}
	void Dead()
	{
		combatControl.PopCreature(laneNum, side, this);
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
