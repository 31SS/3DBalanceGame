using System;
using UnityEngine;
using Unity.Mathematics;
using Unity.MLAgents;
using CharacterState;
using UniRx;
using Unity.MLAgents.Sensors;


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
    
    Rigidbody m_Agentrb;
    public Transform target;
    private AgentSettings m_AgentSettings;
    
    [SerializeField] protected Animator _animator;
    
    [SerializeField] protected bool isGround;

    protected PlayerInput _playerInput;
    protected RaycastHit _isGroundHit;
    protected Vector3 latestPos;
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
        
        _playerInput = new PlayerInput();
    }

    public override void Initialize()
    {
        m_AgentSettings = FindObjectOfType<AgentSettings>();
        
        m_Agentrb.GetComponent<Rigidbody>();
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

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.transform.position);
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);
        
        sensor.AddObservation(GroundCheck(isGroundRay) ? 1 : 0);
    }

    protected void Update()
    {
        _playerInput.Inputting();
    }

    protected  virtual void FixedUpdate()
    {
        var localTransform = transform;
        isGroundRay = GenerateRay(localTransform);
        Debug.DrawRay(isGroundRay.origin, isGroundRay.direction * _distance, Color.red);
        isGround = GroundCheck(isGroundRay);
        _animator.SetBool("is_Ground", isGround);
        Move(localTransform);
    }

    protected void OnCollisionEnter(Collision col)
    {
        var colGameObject = col.gameObject;
        var clearTarget = colGameObject.GetComponent<IClearEvent>();
        var badTarget = colGameObject.GetComponent<IBadEvent>();
        if (badTarget != null)
        {
            badTarget.BadEvent();
            StateProcessor.State.Value = StateDie;
        }
        else if (clearTarget != null)
        {
            clearTarget.ClearEvent();
            StateProcessor.State.Value = StateClear;
        }
    }

    protected Ray GenerateRay(Transform nowTransform)
    {
        var rayPosition = nowTransform.position + new Vector3(0, 0.5f, 0);
         var isGroundRay = new Ray(rayPosition, nowTransform.up * -1);
         return isGroundRay;
    }
    

    protected bool GroundCheck(Ray isGroundRay)
    {
        if (!Physics.Raycast(isGroundRay, out _isGroundHit, _distance)) return false;
        if (!_isGroundHit.collider.CompareTag("Ground")) return false;
        return true;
    }

    protected void Move(Transform localTransform)
    {
        AddReward(-1 / MaxStep);
        
        var localTransformPosition = localTransform.position;
        var nowSpeed_X = math.abs((localTransformPosition.x - latestPos.x) / Time.deltaTime);
        var nowSpeed_Z = math.abs((localTransformPosition.z - latestPos.z) / Time.deltaTime);
        latestPos = localTransformPosition; 
        
        Vector3 moveVector3 = new Vector3(_playerInput.X, 0, _playerInput.Z);
        if (moveVector3.magnitude > 1)
        {
            moveVector3.Normalize();
        }
        m_Agentrb.AddForce(moveVector3 * RUN_SPEED);

        _animator.SetFloat("Horizontal_Vel",math.abs(moveVector3.x));
        _animator.SetFloat("Vertical_Vel",math.abs(moveVector3.z));
        _animator.SetFloat("Run_Speed",moveVector3.magnitude);
        
        
        if (isGround)
        {
            if (_playerInput.Jump == true)
            {
                _animator.SetTrigger("is_Jump");
                m_Agentrb.velocity = Vector3.up * JUMP_POWER;
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
                m_Agentrb.angularVelocity = new Vector3(0f, 0f, 0f);
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

        if (localTransform.position.y < -10)
        {
            StateProcessor.State.Value = StateDie;
        }
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
        Destroy(gameObject);
    }
}
