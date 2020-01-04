using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultCreature : MonoBehaviour
{
    // Start is called before the first frame update
    //Transform creatureTransform;
	bool side;
	int laneNum;
	float speed;
	int hp;
	float attackSpeed;
	float attackRange;
	Vector3 start;
	Vector3 end;
	bool moveFlag = true;
	public GameControl gameControl;
	void Start()
    {
		
		attackSpeed = 2.0f;
		side = false;
		speed = 0.1f;
		InvokeRepeating("DetectEnemy", 0.5f, attackSpeed);
		start = new Vector2(10.0f, 0.0f);
		end = new Vector2(-10.0f, 0.0f);
		MoveTo(start, end);
		
		Instantiate(1, side, 1);

	}

	void MoveTo(Vector2 st, Vector2 ed)
	{
		start = st;
		end = ed;
		transform.position = start;
	}

	void DetectEnemy()
	{
		//moveFlag = GameControl.Detect();
		Debug.Log("detect");
	}
	void Instantiate(int lane, bool sideCheck, int creatureType)
	{
		
		side = sideCheck;
		laneNum = lane;
		if(!side)
		{
			speed *= -1;
			transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
		}
	}

	void SetCreature()
	{
		
	}

	void DamageTaken(int damage)
	{
		hp -= damage;
		if (hp <= 0)
			dead();
	}
	void dead()
	{
		//GameControl.dead(this);
		Destroy(this);
	}

	// Update is called once per frame
	void Update()
    {
        if(moveFlag)
		{
			if(side && transform.position.x <= end.x)
			{
				moveFlag = true;
				transform.position = new Vector2(transform.position.x + speed, transform.position.y);
			}
			else if(!side && transform.position.x >= end.x)
			{
				moveFlag = true;
				transform.position = new Vector2(transform.position.x + speed, transform.position.y);
			}
			else
			{
				moveFlag = false;
			}
		}
    }
}
