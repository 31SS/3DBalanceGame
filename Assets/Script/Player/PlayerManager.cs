﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using CharacterState;
using UniRx;
using Unity.Mathematics;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour
{
    //変更前のステート名
    private string _prevStateName;
    
    //ステート
    public StateProcessor StateProcessor { get; set; } = new StateProcessor();
    public CharacterStateIdle StateIdle { get; set; } = new CharacterStateIdle();
    public CharacterStateRun StateRun { get; set; } = new CharacterStateRun();
    public CharacterStateAir StateAir { get; set; } = new CharacterStateAir();
    public CharacterStateAttack StateAttack { get; set; } = new CharacterStateAttack();
    public  CharacterStateClear StateClear { get; set; } = new CharacterStateClear();
    public CharacterStateDie StateDie { get; set; } = new CharacterStateDie();
    
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _animator;
    
    [SerializeField] private bool isGround;

    private PlayerInput _playerInput;
    private RaycastHit _isGroundHit;
    private Vector3 latestPos;
    [SerializeField] private Vector3 target_dir;
    [SerializeField] private float magnitude;

    [SerializeField] private float _distance = 0.54f;
    [SerializeField] private float RUN_SPEED;
    [SerializeField] private float JUMP_POWER;
    [SerializeField] private float SMOOTH;
    [SerializeField] private Vector3 speed;
    private void Awake()
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
    
    void Start()
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
    
    void Update()
    {
        _playerInput.Inputting();
    }

    protected  virtual void FixedUpdate()
    {
        // var localTransform = transform;
        // var localTransformPosition = localTransform.position;
        // _rayPosition = localTransformPosition + new Vector3(0, 0.5f, 0);
        // _isGroundRay = new Ray(_rayPosition, localTransform.up * -1);
        // Debug.DrawRay(_isGroundRay.origin, _isGroundRay.direction * distance, Color.red);
        // isGround = GroundCheck();
        // _animator.SetBool("is_Ground", isGround);
        //
        // var nowSpeed_X = Math.Abs((localTransformPosition.x - latestPos.x) / Time.deltaTime);
        // var nowSpeed_Z = Math.Abs((localTransformPosition.z - latestPos.z) / Time.deltaTime);
        // latestPos = localTransformPosition; 
        //
        // Vector3 moveVector3 = new Vector3(_playerInput.X, 0, _playerInput.Z);
        // if (moveVector3.magnitude > 1)
        // {
        //     moveVector3.Normalize();
        // }
        // _rb.AddForce(moveVector3 * RUN_SPEED);
        //
        // _animator.SetFloat("Horizontal_Vel",Math.Abs(moveVector3.x));
        // _animator.SetFloat("Vertical_Vel",Math.Abs(moveVector3.z));
        // _animator.SetFloat("Run_Speed",moveVector3.magnitude);
        //
        //
        // if (isGround)
        // {
        //     if (_playerInput.Jump == true)
        //     {
        //         _animator.SetTrigger("is_Jump");
        //         _rb.velocity = Vector3.up * JUMP_POWER;
        //     }
        //     else if (nowSpeed_X > 0.1 || nowSpeed_Z > 0.1)
        //     {
        //         StateProcessor.State.Value = StateRun;
        //         if( moveVector3.magnitude > 0)
        //         {
        //             Quaternion rotation = Quaternion.LookRotation(moveVector3);
        //             localTransform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * SMOOTH);
        //         }
        //     }
        //     else
        //     {
        //         StateProcessor.State.Value = StateIdle;
        //         _animator.SetTrigger("is_Idle");
        //         _rb.angularVelocity = new Vector3(0f, 0f, 0f);
        //     }
        // }
        // else
        // {
        //     StateProcessor.State.Value = StateAir;
        //     if ((nowSpeed_X > 0.1 || nowSpeed_Z > 0.1))
        //     {
        //         if(moveVector3.magnitude > 0)
        //         {
        //             Quaternion rotation = Quaternion.LookRotation(moveVector3);
        //             localTransform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * SMOOTH);
        //         }
        //     }
        // }
        //
        // if (localTransform.position.y < -10)
        // {
        //     StateProcessor.State.Value = StateDie;
        // }
        var localTransform = transform;
        var isGroundRay = GenerateRay(localTransform);
        Debug.DrawRay(isGroundRay.origin, isGroundRay.direction * _distance, Color.red);
        isGround = GroundCheck(isGroundRay);
        _animator.SetBool("is_Ground", isGround);
        Move(localTransform);
    }

    private void OnCollisionEnter(Collision col)
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

    private Ray GenerateRay(Transform nowTransform)
    {
        var rayPosition = nowTransform.position + new Vector3(0, 0.5f, 0);
         var isGroundRay = new Ray(rayPosition, nowTransform.up * -1);
         return isGroundRay;
    }

    private void Move(Transform localTransform)
    {
        var localTransformPosition = localTransform.position;
        var nowSpeed_X = math.abs((localTransformPosition.x - latestPos.x) / Time.deltaTime);
        var nowSpeed_Z = math.abs((localTransformPosition.z - latestPos.z) / Time.deltaTime);
        latestPos = localTransformPosition; 
        
        Vector3 moveVector3 = new Vector3(_playerInput.X, 0, _playerInput.Z);
        if (moveVector3.magnitude > 1)
        {
            moveVector3.Normalize();
        }
        _rb.AddForce(moveVector3 * RUN_SPEED);

        _animator.SetFloat("Horizontal_Vel",math.abs(moveVector3.x));
        _animator.SetFloat("Vertical_Vel",math.abs(moveVector3.z));
        _animator.SetFloat("Run_Speed",moveVector3.magnitude);
        
        
        if (isGround)
        {
            if (_playerInput.Jump == true)
            {
                _animator.SetTrigger("is_Jump");
                _rb.velocity = Vector3.up * JUMP_POWER;
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
                _rb.angularVelocity = new Vector3(0f, 0f, 0f);
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
    
    private bool GroundCheck(Ray isGroundRay)
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
