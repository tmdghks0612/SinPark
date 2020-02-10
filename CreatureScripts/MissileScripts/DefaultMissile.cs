using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMissile : MonoBehaviour
{
    private readonly object lock_hit = new object();

    public int attackDamage;
    public float attackRange;
    public Vector3 direction;

    private Vector3 position;
    private GameControl.Sides side;
    private bool hitFlag;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("DestroyItself", 4.0f, 30.0f);
        hitFlag = false;
    }

    void DestroyItself()
    {

        CancelInvoke("DestroyItself");
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        this.gameObject.transform.position += direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        lock(lock_hit)
        {
            if (hitFlag == false)
            {
                GameObject collided = collision.gameObject;
                if (collided.GetComponent<DefaultCreature>().GetSide() != this.side)
                {
                    if (collided.CompareTag(gameObject.tag))
                    {
                        collision.gameObject.GetComponent<DefaultCreature>().DamageTaken(attackDamage, 0);
                        hitFlag = true;
                        Destroy(this.gameObject);
                    }
                }
            }
        }
        
    }

    public void SetMissile(Vector3 currentPosition, GameControl.Sides side, int laneNum)
    {
        this.gameObject.transform.position = currentPosition;
        this.side = side;
        gameObject.tag = "Lane" + laneNum;
        gameObject.layer = 15 - laneNum;
        if (this.side == GameControl.Sides.Hostile)
        {
            direction = new Vector3(direction.x * -1, direction.y, direction.z);
            transform.localScale = new Vector3(-1.0f * transform.localScale.x, 1.0f * transform.localScale.y, 1.0f);
        }
    }
}
