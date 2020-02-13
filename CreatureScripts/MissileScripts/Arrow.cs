﻿using System.Collections;
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
}
