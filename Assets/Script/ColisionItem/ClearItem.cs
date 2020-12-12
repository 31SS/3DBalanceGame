using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearItem : MonoBehaviour, IClearEvent
{
    public void ClearEvent()
    {
        Destroy(gameObject);
        // GameManager.Instance.dispatch(GameManager.GameState.Clear);
    }
}
