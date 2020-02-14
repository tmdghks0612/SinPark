using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : DefaultMissile
{
    [SerializeField]
    protected float fallRate;

    private float startY;
    private float prevX;
    private float prevY;
    private Vector3 target;

    void Start()
    {
        InvokeRepeating("DestroyItself", 4.0f, 30.0f);
        startY = gameObject.transform.position.y;
        prevX = direction.x;
        prevY = direction.y;
    }

    protected override void Update()
    {
        if(gameObject.transform.position.y < startY)
        {
            Destroy(this.gameObject);
        }
        target = transform.position + direction * Time.deltaTime;
        transform.right = target - transform.position;
        this.gameObject.transform.position += direction * Time.deltaTime;
        prevY -= fallRate * Time.deltaTime;
        direction = new Vector3(prevX, prevY, 0);
        
    }


    public override void SetMissile(Vector3 currentPosition, GameControl.Sides side, int laneNum)
    {
        this.gameObject.transform.position = currentPosition;
        this.side = side;
        gameObject.tag = "Lane" + laneNum;
        gameObject.layer = 15 - laneNum;
        if (this.side == GameControl.Sides.Hostile)
        {
            direction = new Vector3(direction.x * -1, direction.y, direction.z);
            transform.localScale = new Vector3(1.0f * transform.localScale.x, -1.0f * transform.localScale.y, 1.0f);
        }
    }
}
