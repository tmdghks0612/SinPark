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
        
    }

    // Update is called once per frame
    void Update()
    {
        position += direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<DefaultCreature>().DamageTaken(attackDamage);
        Destroy(this.gameObject);
    }

    public void SetMissile(Vector3 currentPosition, GameControl.Sides side)
    {
        this.position = this.gameObject.transform.position;
        position = currentPosition;
        this.side = side;
    }
}
