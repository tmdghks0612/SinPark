using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkError : MonoBehaviour
{
    public void SelfInactive()
    {
        gameObject.SetActive(false);
    }
}
