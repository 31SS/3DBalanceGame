using System;
using UnityEngine;
using Unity.Mathematics;

public class PlayerManager2 : BaseCharacter
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Move(Transform localTransform)
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
}
