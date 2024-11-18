using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseBrowserInWin : MonoBehaviour
{
    [SerializeField] GameObject gameObject;
   public void close()
    {
        Destroy(gameObject);
    }
}
