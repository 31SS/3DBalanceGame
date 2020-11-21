// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using CharacterState;
// using UniRx;
//
// public class PlayerController : MonoBehaviour
// {
//     //変更前のステート名
//     private string _prevStateName;
//
//     //ステート
//     public StateProcessor StateProcessor { get; set; } = new StateProcessor();
//     public CharacterStateIdle StateIdle { get; set; } = new CharacterStateIdle();
//     public CharacterStateRun StateRun { get; set; } = new CharacterStateRun();
//     public CharacterStateAir StateAir { get; set; } = new CharacterStateAir();
//     public CharacterStateAttack StateAttack { get; set; } = new CharacterStateAttack();
//
//
//     [SerializeField] private static readonly float RUN_SPEED = 10f;
//     [SerializeField] private static readonly float JUMP_POWER = 500f;
//     private static readonly float FloatingDistance = 0.01f;
//
//     private PlayerInput _playerInput;
//     private PlayerMover _playerMover;
//     [SerializeField] private Rigidbody2D m_rigidbody2D;
//     [SerializeField] private Animator m_animator;
//     [SerializeField] private bool m_isGround/* { get; set; }*/;
//     [SerializeField] private BoxCollider2D m_boxCollider2D;
//     [SerializeField] private LayerMask _layerMask;
//     [SerializeField] private SpriteRenderer _spriteRenderer;
//     [SerializeField] private ContactFilter2D _filter2D;
//     private RaycastHit2D _hit;
//     
//
//     private void Awake()
//     {
//         StateProcessor.State.Value = StateIdle;
//         StateIdle.ExecAction = Idle;
//         StateRun.ExecAction = Run;
//         StateAir.ExecAction = Air;
//         StateAttack.ExecAction = Attack;
//
//         _playerInput = new PlayerInput();
//         _playerMover = new PlayerMover(m_rigidbody2D);
//         //m_isGround = false;
//         //_layerMask = 
//         
//     }
//
//     private void Start()
//     {
//         //ステートの値が変更されたら実行処理を行うようにする
//         StateProcessor.State
//             .Where(_ => StateProcessor.State.Value.GetStateName() != _prevStateName)
//             .Subscribe(_ =>
//             {
//                 Debug.Log("Now State:" + StateProcessor.State.Value.GetStateName());
//                 _prevStateName = StateProcessor.State.Value.GetStateName();
//                 StateProcessor.Execute();
//             })
//             .AddTo(this);
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         _playerInput.Inputting();
//         ////Boxcastで接地判定
//         ////Boxcastが地面にめり込んでいたらBoxCheckで確認
//         //_hit = Physics2D.BoxCast(
//         //    origin: new Vector2(transform.position.x, m_boxCollider2D.transform.position.y),
//         //    size: _spriteRenderer.bounds.size,
//         //    angle: 0f,
//         //    direction: Vector2.down,
//         //    distance: FloatingDistance,
//         //    layerMask: _layerMask
//         //    );
//         //if (!m_isGround)
//         //{
//         //    m_isGround = Physics2D.OverlapBox(transform.position, _spriteRenderer.bounds.size, 0f, _layerMask);
//         //    //Debug.Log("Hey");
//
//         //}
//         ////Debug.Log(m_isGround.ToString());
//
//         m_isGround = m_rigidbody2D.IsTouching(_filter2D);
//         Debug.Log(m_isGround.ToString());
//
//         _playerMover.Move(RUN_SPEED, _playerInput.X, m_isGround, m_animator);
//         if (m_isGround)
//         {
//             if (_playerInput.Jump == true)
//             {
//                 _playerMover.Jump(m_animator, JUMP_POWER);
//                 StateProcessor.State.Value = StateAir;
//                 Debug.Log("Jump");
//                 //return;
//             }
//             else if (Mathf.Abs(_playerInput.X) > 0)
//             {
//                 StateProcessor.State.Value = StateRun;
//                 var rot = transform.rotation;
//                 transform.rotation = Quaternion.Euler(rot.x, Mathf.Sign(_playerInput.X) == 1 ? 0 : 180, rot.z);
//             }
//             else
//             {
//                 StateProcessor.State.Value = StateIdle;
//             }
//         }
//     }
//
//     private void FixedUpdate()
//     {
//         //_playerMover.Move(RUN_SPEED, _playerInput.X, m_isGround, m_animator);
//         //if (m_isGround)
//         //{
//         //    if(_playerInput.Jump == true)
//         //    {
//         //        _playerMover.Jump(m_animator, JUMP_POWER);
//         //        StateProcessor.State.Value = StateAir;
//         //        Debug.Log("Jump");
//         //        //return;
//         //    }
//         //    else if (Mathf.Abs(_playerInput.X) > 0)
//         //    {
//         //        StateProcessor.State.Value = StateRun;
//         //        var rot = transform.rotation;
//         //        transform.rotation = Quaternion.Euler(rot.x, Mathf.Sign(_playerInput.X) == 1 ? 0 : 180, rot.z);
//         //    }
//         //    else
//         //    {
//         //        StateProcessor.State.Value = StateIdle;
//         //    }
//         //}
//         //if (StateProcessor.State.Value.GetStateName() != "State:Air")
//         //{
//         //    var rot = transform.rotation;
//         //    transform.rotation = Quaternion.Euler(rot.x, Mathf.Sign(_playerInput.X) == 1 ? 0 : 180, rot.z);
//         //}
//         //if (_playerInput.Jump == true && m_isGround == true)
//         //{
//         //    _playerMover.Jump(m_animator, JUMP_POWER);
//         //    StateProcessor.State.Value = StateJump;
//         //}
//     }
//
//     public void Idle()
//     {
//
//     }
//     public void Run()
//     {
//         
//     }
//     public void Air()
//     {
//         //_playerMover.Jump(m_animator, JUMP_POWER);
//     }
//
//     public void Attack()
//     {
//
//     }
// }
