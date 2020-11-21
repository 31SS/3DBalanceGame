// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
//
// public class GameManager : SingletonMonoBehaviour<GameManager>
// {
//    public enum GameState
//     {
//         Opening,
//         Playing,
//         Clear,
//         Over
//     }
//
//     public GameState currentState = GameState.Opening;
//
//     private int stage = 1;
//     [SerializeField] private GameObject panel;
//     [SerializeField] private Text text;
//     [SerializeField] private GameObject gameClearCanvasPrefab;
//     private GameObject gameClearCanvasClone;
//     [SerializeField] private GameObject gameOverCanvasPrefab;
//     private GameObject gameOverCanvasClone;
//     private Button[] buttons;
//
//     private void Start()
//     {
//         Sound.LoadBgm("Title", "TitleBGM");
//         Sound.LoadBgm("Main", "MainBGM");
//         Sound.LoadBgm("BossWarning", "BossWarningBGM");
//         Sound.LoadBgm("Boss", "BossBGM");
//         Sound.LoadSe("PlayerAttack1", "PlayerAttackSE1");
//         Sound.LoadSe("PlayerAttack2", "PlayerAttackSE2");
//         Sound.LoadSe("PlayerAttack3", "PlayerAttackSE3");
//         Sound.LoadSe("PlayerDamage1", "PlayerDamageSE1");
//         Sound.LoadSe("PlayerDamage2", "PlayerDamageSE2");
//         Sound.LoadSe("PlayerWin", "PlayerWinSE");
//         Sound.LoadSe("PlayerDead", "PlayerDeadSE");
//         Sound.LoadSe("TitleButton", "TitleButtonSE");
//         Sound.LoadSe("BlockChange", "BlockChangeSE");
//         Sound.LoadSe("GameClear", "GameClearBGM");
//         Sound.LoadSe("GameOver", "GameOverBGM");
//         //以下未実装
//         Sound.LoadSe("Devil", "DevilSE");
//         Sound.LoadSe("Bug", "BugSE");
//         Sound.LoadSe("Gunner", "GunnerSE");
//         Sound.LoadSe("GunnerShot", "GunnerShotSE");
//         Sound.LoadSe("Dragon", "DragonSE");//ドラゴンの断末魔
//         Sound.LoadSe("DragonFire", "DragonFireSE");
//         Sound.LoadSe("EnemyAttack", "EnemyAttackSE");//敵の体当たり
//         dispatch(GameState.Opening);
//     }
//
//     // 状態による振り分け処理
//     public void dispatch(GameState state)
//     {
//         GameState oldState = currentState;
//
//         currentState = state;
//         switch (state)
//         {
//             case GameState.Opening:
//                 GameOpening();
//                 break;
//             case GameState.Playing:
//                 GameStart();
//                 break;
//             case GameState.Clear:
//                 GameClear();
//                 break;
//             case GameState.Over:
//                 //if (oldState == GameState.Playing)
//                 //{
//                     GameOver();
//                 //}
//                 break;
//         }
//     }
//
//     // オープニング処理
//     void GameOpening()
//     {
//         currentState = GameState.Opening;
//         //Sound.StopBgm();
//         Sound.PlayBgm("Title");
//     }
//
//     void GameStart()
//     {
//         if (gameOverCanvasClone)
//         {
//             Destroy(gameOverCanvasClone);
//         }
//         else if (gameClearCanvasClone)
//         {
//             Destroy(gameClearCanvasClone);
//         }
//         //Sound.StopBgm();
//         Sound.PlayBgm("Main");
//     }
//
//     public void GameClear()
//     {
//         stage++;
//         gameClearCanvasClone = Instantiate(gameClearCanvasPrefab);
//         //後の処理はgameClearCanvasCloneで処理される。
//     }
//     public void GameOver()
//     {
//         gameOverCanvasClone = Instantiate(gameOverCanvasPrefab);
//         //後の処理はgameOverCanvasCloneで処理される。
//     }
//     //ここからタイトルのボタン
//     public void StartGame()
//     {
//         NowLoading.sceneName = "PlayerScene";
//         SceneManager.LoadScene("NowLoading");//Unityでの遷移の設定はまだしてない?
//         //dispatch(GameState.Playing);
//     }
//     public void ExplainImage()
//     {
//
//     }
//     public void ExitGame()
//     {
//         Application.Quit();
//         Debug.Log("終わり");
//     }
//     //ここまでタイトルのボタン
//     public void TitleGame()
//     {
//         NowLoading.sceneName = "TitleScene";
//         SceneManager.LoadScene("NowLoading");
//     }
// }
