using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMissile : MonoBehaviour
{
    public int attackDamage;
    public float attackRange;
    public Vector3 direction;

    private Vector3 position;
    private GameControl.Sides side;
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("DestroyItself", 4.0f, 30.0f);
    }

    void DestroyItself()
    {

        CancelInvoke("DestroyItself");
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position += direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enter");
        if (collision.gameObject.GetComponent<DefaultCreature>().side != this.side)
        {
            collision.gameObject.GetComponent<DefaultCreature>().DamageTaken(attackDamage);
            Destroy(this.gameObject);
        }
    }

    public void SetMissile(Vector3 currentPosition, GameControl.Sides side)
    {
        this.gameObject.transform.position = currentPosition;
        this.side = side;
        if(this.side == GameControl.Sides.Hostile)
        {
            direction *= -1;
            transform.localScale = new Vector3(-1.0f * transform.localScale.x, 1.0f * transform.localScale.y, 1.0f);
        }
    }
}
