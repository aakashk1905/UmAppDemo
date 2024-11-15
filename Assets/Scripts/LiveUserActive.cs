using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveUserActive : MonoBehaviour
{
    [SerializeField] ChatInitializer chatInitializer;
    void Awake()
    {
        chatInitializer.UpdateNotificationCounter();
    }
}
