using UnityEngine;
using System;
using UniRx;

/// <summary>
/// キャラクターの状態（ステート）
/// </summary>
namespace GameState
{
    /// <summary>
    /// ステートの実行を管理するクラス
    /// </summary>
    public class GameStateProcessor
    {
        //ステート本体
        public ReactiveProperty<GameState> State { get; set; } = new ReactiveProperty<GameState>();

        //実行ブリッジ
        public void Execute() => State.Value.Execute();
    }

    /// <summary>
    /// ステートのクラス
    /// </summary>
    public abstract class GameState
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
    /// オープニング処理
    /// </summary>
    public class GameStateOpning : GameState
    {
        public override string GetStateName()
        {
            return "State:Opning";
        }
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    public class GameStateStart : GameState
    {
        public override string GetStateName()
        {
            return "State:Start";
        }
    }

    /// <summary>
    /// ゲームクリア処理
    /// </summary>
    public class GameStateClear : GameState
    {
        public override string GetStateName()
        {
            return "State:Clear";
        }
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public class GameStateOver : GameState
    {
        public override string GetStateName()
        {
            return "State:Over";
        }

        public override void Execute()
        {
            Debug.Log("なにか特別な処理をしたいときは派生クラスにて処理をしても良い");
            ExecAction?.Invoke();
        }
    }
}