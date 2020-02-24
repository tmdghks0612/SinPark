using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScale : MonoBehaviour
{
    private void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        Vector2 scale = transform.localScale;
        if(cameraSize.x >= cameraSize.y)
        {
            scale.x = cameraSize.x / spriteSize.x * 3;

            scale.y = cameraSize.y / spriteSize.y / 5 * 4; 
        }
        else
        {
            scale.x *= cameraSize.x / spriteSize.x;
            scale.y *= cameraSize.y / spriteSize.y;
        }

        transform.position = new Vector2(0, cameraHeight / 10);// - scale.y/8);
        transform.localScale = scale;
        

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
