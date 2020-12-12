using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using CharacterState;
using UniRx;
using UnityEngine.EventSystems;

public abstract class BaseCharacter : MonoBehaviour
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
    
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] protected Animator _animator;
    
    [SerializeField] protected bool isGround;

    protected PlayerInput _playerInput;
    protected RaycastHit _isGroundHit;
    protected Vector3 latestPos;
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
    
    protected void Update()
    {
        _playerInput.Inputting();
    }

    protected  virtual void FixedUpdate()
    {
        var localTransform = transform;
        var isGroundRay = GenerateRay(localTransform);
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

    protected abstract void Move(Transform localTransform);

    protected bool GroundCheck(Ray isGroundRay)
    {
        if (!Physics.Raycast(isGroundRay, out _isGroundHit, _distance)) return false;
        if (!_isGroundHit.collider.CompareTag("Ground")) return false;
        return true;
    }
    
    // private bool GroundCheck()
    // {
    //     if (!Physics.Raycast(_isGroundRay, out _isGroundHit, distance)) return false;
    //     if (!_isGroundHit.collider.CompareTag("Ground")) return false;
    //     return true;
    // }
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
