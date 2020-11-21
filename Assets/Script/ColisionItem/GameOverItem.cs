using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverItem : MonoBehaviour, IBadEvent
{
    public void BadEvent()
    {
        throw new System.NotImplementedException();
        //ゲームオーバー処理
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
