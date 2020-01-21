using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    protected float destroyDelay;
    // Start is called before the first frame update

    private void FixedUpdate()
    {
        Destroy(gameObject, destroyDelay);
    }
}
