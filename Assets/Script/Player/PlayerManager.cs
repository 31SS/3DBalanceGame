using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using CharacterState;
using UniRx;

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
    
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Animator _animator;
    
    [SerializeField] private bool isGround;

    private PlayerInput _playerInput;
    private Ray _isGroundRay;
    private RaycastHit _isGroundHit;
    private Vector3 _rayPosition;
    private Vector3 latestPos; 
    
    [SerializeField] private float distance = 0.54f;
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
        isGround = false;
        
        _playerInput = new PlayerInput();
    }

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        _playerInput.Inputting();
    }

    private void FixedUpdate()
    {
        var localTransform = transform;
        _rayPosition = localTransform.position + new Vector3(0, 0.5f, 0);
        _isGroundRay = new Ray(_rayPosition, localTransform.up * -1);
        Debug.DrawRay(_isGroundRay.origin, _isGroundRay.direction * distance, Color.red);
        isGround = GroundCheck();
        _animator.SetBool("is_Ground", isGround);
        
        float x = _playerInput.X * RUN_SPEED;
        float z = _playerInput.Z * RUN_SPEED;
        Vector3 moveVector3 = new Vector3(_playerInput.X, 0, _playerInput.Z);
        Vector3 target_dir = new Vector3(_playerInput.X, 0, _playerInput.Z);
        if (moveVector3.magnitude > 1)
        {
            moveVector3.Normalize();
        }
        _rb.AddForce(moveVector3 * RUN_SPEED);
        //体の向きを変更
        
        _animator.SetFloat("Horizontal_Vel",Math.Abs(moveVector3.x));
        _animator.SetFloat("Vertical_Vel",Math.Abs(moveVector3.z));
        _animator.SetFloat("Run_Speed",moveVector3.magnitude);

        if (isGround)
        {
            if (_playerInput.Jump == true)
            {
                StateProcessor.State.Value = StateAir;
                _rb.velocity = Vector3.up * JUMP_POWER;
            }
            else if (Math.Abs(_rb.velocity.x) > 0.1 || Math.Abs(_rb.velocity.z) > 0.1)
            {
                StateProcessor.State.Value = StateRun;
                Quaternion rotation = Quaternion.LookRotation(target_dir);
                localTransform.rotation = Quaternion.Lerp(transform.rotation, rotation,  Time.deltaTime * SMOOTH);
                Debug.Log(("Hey"));
            }
            else
            {
                StateProcessor.State.Value = StateIdle;
                _animator.SetTrigger("is_Idle");
                
            }
        }
    }

    private bool GroundCheck()
    {
        if (!Physics.Raycast(_isGroundRay, out _isGroundHit, distance)) return false;
        if (!_isGroundHit.collider.CompareTag("Ground")) return false;
        return true;
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
        _animator.SetTrigger("is_Jump");
        _animator.SetBool("is_Run", false);
    }

    public void Attack()
    {
        _animator.SetBool("is_Run", false);
    }
    
}
