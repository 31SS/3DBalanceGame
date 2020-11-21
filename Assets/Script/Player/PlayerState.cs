using UnityEngine;
using System;
using UniRx;

/// <summary>
/// キャラクターの状態（ステート）
/// </summary>
namespace CharacterState
{
    /// <summary>
    /// ステートの実行を管理するクラス
    /// </summary>
    public class StateProcessor
    {
        //ステート本体
        public ReactiveProperty<CharacterState> State { get; set; } = new ReactiveProperty<CharacterState>();

        //実行ブリッジ
        public void Execute() => State.Value.Execute();
    }

    /// <summary>
    /// ステートのクラス
    /// </summary>
    public abstract class CharacterState
    {
        //デリゲート
        public Action ExecAction { get; set; }

        //実行処理
        public virtual void Execute()
        {
            ExecAction?.Invoke();
        }

        //ステート名を取得するメソッド
        public abstract string GetStateName();
    }

    //=================================================================================
    //以下状態クラス
    //=================================================================================

    /// <summary>
    /// 何もしていない状態
    /// </summary>
    public class CharacterStateIdle : CharacterState
    {
        public override string GetStateName()
        {
            return "State:Idle";
        }
    }

    /// <summary>
    /// 走っている状態
    /// </summary>
    public class CharacterStateRun : CharacterState
    {
        public override string GetStateName()
        {
            return "State:Run";
        }
    }

    /// <summary>
    /// 跳んでいる状態
    /// </summary>
    public class CharacterStateAir : CharacterState
    {
        public override string GetStateName()
        {
            return "State:Air";
        }
    }

    /// <summary>
    /// 攻撃している状態
    /// </summary>
    public class CharacterStateAttack : CharacterState
    {
        public override string GetStateName()
        {
            return "State:Attack";
        }

        public override void Execute()
        {
            Debug.Log("なにか特別な処理をしたいときは派生クラスにて処理をしても良い");
            ExecAction?.Invoke();
        }
    }
}