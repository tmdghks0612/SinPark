using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultCreature : MonoBehaviour
{
    // Start is called before the first frame update
    Transform creatureTransform;
	bool side;
	int laneNum;
	float speed;
	int hp;
	float attackSpeed;
	float attackRange;

    void Start()
    {
        creatureTransform = this.transform;

    }


	void Instantiate(int lane, bool sideCheck, int creatureType)
	{
		
		side = sideCheck;
		laneNum = lane;
		SetCreature(creatureType);
		if (side)
		{
			switch (laneNum)
			{
				
			}
		}
		else
		{
			speed *= -1;
			switch (laneNum)
			{
				
			}
		}
	}
	void SetStartEnd(Vector3 start, Vector3 end)
	{

	}
	void SetCreature(int creatureType)
	{

	}
	void dead()
	{

	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
