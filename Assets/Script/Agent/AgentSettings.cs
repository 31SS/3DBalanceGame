using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSettings : MonoBehaviour
{
    [Header("Specific to 3DBalance")]
    public float agentRunSpeed;
    public float agentJumpPower;
    //when a goal is scored the ground will use this material for a few seconds.
    public Material goalScoredMaterial;
    //when fail, the ground will use this material for a few seconds.
    public Material failMaterial;

    [HideInInspector]
    public float agentJumpVelocity = 777;
    [HideInInspector]
    public float agentJumpVelocityMaxChange = 10;
}
