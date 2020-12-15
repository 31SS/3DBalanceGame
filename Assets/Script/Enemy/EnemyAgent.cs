using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.MLAgents;
using CharacterState;
using UniRx;
using Unity.Barracuda;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class EnemyAgent : Agent
{
    //変更前のステート名
    protected string _prevStateName;
    
    //ステート
    public StateProcessor StateProcessor { get; set; } = new StateProcessor();
    public CharacterStateIdle StateIdle { get; set; } = new CharacterStateIdle();
    public CharacterStateRun StateRun { get; set; } = new CharacterStateRun();
    public CharacterStateAir StateAir { get; set; } = new CharacterStateAir();
    public CharacterStateAttack StateAttack { get; set; } = new CharacterStateAttack();
    public  CharacterStateClear StateClear { get; set; } = new CharacterStateClear();
    public CharacterStateDie StateDie { get; set; } = new CharacterStateDie();
    
    public Transform target;
    public GameObject start;
    public GameObject[] goalRespawnPos;
    private Rigidbody m_AgentRb;
    private Rigidbody m_StartRb;
    private Rigidbody m_TargetRb;

    [SerializeField] protected Animator _animator;
    
    [SerializeField] protected bool isGround;
    
    private RaycastHit _isGroundHit;
    private Vector3 latestPos;
    private Ray isGroundRay;
    [SerializeField] protected Vector3 target_dir;
    [SerializeField] protected float magnitude;

    [SerializeField] protected float _distance = 0.54f;
    [SerializeField] protected float RUN_SPEED;
    [SerializeField] protected float JUMP_POWER;
    [SerializeField] protected float SMOOTH;
    
    protected void Awake()
    {
        StateProcessor.State.Value = StateIdle;
        StateIdle.ExecAction = Idle;
        StateRun.ExecAction = Run;
        StateAir.ExecAction = Air;
        StateAttack.ExecAction = Attack;
        StateClear.ExecAction = Clear;
        StateDie.ExecAction = Die;
        isGround = false;
    }

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_StartRb = start.GetComponent<Rigidbody>();
        m_TargetRb = target.GetComponent<Rigidbody>();
    }
    

    protected void Start()
    {
    //ステートの値が変更されたら実行処理を行うようにする
        StateProcessor.State
            .Where(_ => StateProcessor.State.Value.GetStateName() != _prevStateName)
            .Subscribe(_ =>
            {
                Debug.Log("Now State:" + StateProcessor.State.Value.GetStateName());
                _prevStateName = StateProcessor.State.Value.GetStateName();
                StateProcessor.Execute();
            })
            .AddTo(this);
    }

    public override void OnEpisodeBegin()
    {
        ResetGoal();
        if (transform.localPosition.y < -1f)
        {
            transform.localPosition = m_StartRb.transform.localPosition;
            transform.localRotation = Quaternion.Euler(0,180,0);
            m_AgentRb.velocity = Vector3.zero;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.transform.position);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(Physics.Raycast(isGroundRay, out _isGroundHit));
        sensor.AddObservation(GroundCheck(isGroundRay) ? 1 : 0);
    }
    
    // public override void OnActionReceived(ActionBuffers actions)
    // {
    //     Move(actions.DiscreteActions, transform);
    //     
    //     if ((!Physics.Raycast(m_AgentRb.position, Vector3.down, 20)))
    //     {
    //         SetReward(-1f);
    //         EndEpisode();
    //     }
    // }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        
        Move(actions.DiscreteActions, transform);
        
        if (transform.localPosition.y < -1f)
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut.Clear();
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 2;
        }
        discreteActionsOut[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    public void OnCollisionEnter(Collision col)
    {
        // var colGameObject = col.gameObject;
        // var clearTarget = colGameObject.GetComponent<IClearEvent>();
        // var badTarget = colGameObject.GetComponent<IBadEvent>();
        // if (badTarget != null)
        // {
        //     badTarget.BadEvent();
        //     StateProcessor.State.Value = StateDie;
        // }
        // else if (clearTarget != null)
        // {
        //     clearTarget.ClearEvent();
        //     StateProcessor.State.Value = StateClear;
        // }
        if (col.gameObject.CompareTag("Goal"))
        {
            SetReward(1f);
            EndEpisode();
        }
    }

    void ResetGoal()
    {
        var randomPosNum = Random.Range(0, goalRespawnPos.Length - 1);
        
        m_TargetRb.transform.position = goalRespawnPos[randomPosNum].transform.position;
    }

    protected  virtual void FixedUpdate()
    {
        var localTransform = transform;
        isGroundRay = GenerateRay(localTransform);
        isGround = GroundCheck(isGroundRay);
        _animator.SetBool("is_Ground", isGround);
    }

    

    Ray GenerateRay(Transform nowTransform)
    {
        var rayPosition = nowTransform.position + new Vector3(0, 0.5f, 0);
         var isGroundRay = new Ray(rayPosition, nowTransform.up * -1);
         return isGroundRay;
    }
    

    bool GroundCheck(Ray isGroundRay)
    {
        if (!Physics.Raycast(isGroundRay, out _isGroundHit, _distance)) return false;
        if (!_isGroundHit.collider.CompareTag("Ground")) return false;
        return true;
    }

    public void Move(ActionSegment<int> act, Transform localTransform)
    {
        AddReward(-1 / MaxStep);

        var dirToGoForwardAction = act[0];
        var dirToGoSideAction = act[1];
        var jumpAction = act[2];
        var dirToGo_X = 0;
        var dirToGo_Z = 0;
        
        var localTransformPosition = localTransform.position;
        var nowSpeed_X = math.abs((localTransformPosition.x - latestPos.x) / Time.deltaTime);
        var nowSpeed_Z = math.abs((localTransformPosition.z - latestPos.z) / Time.deltaTime);
        latestPos = localTransformPosition;

        if (dirToGoForwardAction == 1)
            dirToGo_Z = -1;
        else if (dirToGoForwardAction == 2)
            dirToGo_Z= 1;
        if (dirToGoSideAction == 1)
            dirToGo_X = -1;
        else if (dirToGoSideAction == 2)
            dirToGo_X = 1;
        Vector3 moveVector3 = new Vector3(dirToGo_X, 0, dirToGo_Z);
        if (moveVector3.magnitude > 1)
        {
            moveVector3.Normalize();
        }
        m_AgentRb.AddForce(moveVector3 * RUN_SPEED);

        _animator.SetFloat("Horizontal_Vel",math.abs(moveVector3.x));
        _animator.SetFloat("Vertical_Vel",math.abs(moveVector3.z));
        _animator.SetFloat("Run_Speed",moveVector3.magnitude);
        
        
        if (isGround)
        {
            if (jumpAction == 1)
            {
                _animator.SetTrigger("is_Jump");
                m_AgentRb.velocity = Vector3.up * JUMP_POWER;
                AddReward(-0.01f);
            }
            else if (nowSpeed_X > 0.1 || nowSpeed_Z > 0.1)
            {
                StateProcessor.State.Value = StateRun;
                if( moveVector3.magnitude > 0)
                {
                    Quaternion rotation = Quaternion.LookRotation(moveVector3);
                    localTransform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * SMOOTH);
                }
            }
            else
            {
                StateProcessor.State.Value = StateIdle;
                _animator.SetTrigger("is_Idle");
                m_AgentRb.angularVelocity = new Vector3(0f, 0f, 0f);
            }
        }
        else
        {
            StateProcessor.State.Value = StateAir;
            if ((nowSpeed_X > 0.1 || nowSpeed_Z > 0.1))
            {
                if(moveVector3.magnitude > 0)
                {
                    Quaternion rotation = Quaternion.LookRotation(moveVector3);
                    localTransform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * SMOOTH);
                }
            }
        }

        // if (localTransform.position.y < -10)
        // {
        //     StateProcessor.State.Value = StateDie;
        // }
    }
    
    public void Idle()
    {
        _animator.SetBool("is_Run", false);
    }
    public void Run()
    {
        _animator.SetBool("is_Run", true);
    }
    public void Air()
    {
        _animator.SetBool("is_Run", false);
    }

    public void Attack()
    {
        _animator.SetBool("is_Run", false);
    }

    public void Clear()
    {
        _animator.SetBool("is_Run", false);
        _animator.SetTrigger("Cleared");
    }
    
    public void Die()
    {
        _animator.SetBool("is_Run", false);
        // Destroy(gameObject);
    }
}