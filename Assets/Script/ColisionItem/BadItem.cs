using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadItem : MonoBehaviour, IBadEvent
{
    public void BadEvent()
    {
        // GameManager.Instance.dispatch(GameManager.GameState.Over);
        Destroy(gameObject);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
