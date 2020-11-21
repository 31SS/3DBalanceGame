using CharacterState;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameState;
using UniRx;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public enum GameState
    {
        Opening,
        Playing,
        Clear,
        Over
    }

    public GameState currentState = GameState.Opening;

    private int stage = 1;
    [SerializeField] private GameObject panel;
    [SerializeField] private Text text;
    [SerializeField] private GameObject gameClearCanvasPrefab;
    private GameObject gameClearCanvasClone;
    [SerializeField] private GameObject gameOverCanvasPrefab;
    private GameObject gameOverCanvasClone;
    private Button[] buttons;
    
    public StateProcessor StateProcessor { get; set; } = new StateProcessor();
    public CharacterStateIdle StateIdle { get; set; } = new CharacterStateIdle();
    public CharacterStateRun StateRun { get; set; } = new CharacterStateRun();
    public CharacterStateAir StateAir { get; set; } = new CharacterStateAir();
    public CharacterStateAttack StateAttack { get; set; } = new CharacterStateAttack();

    private void Start()
    {
        
    }

    // 状態による振り分け処理
    public void dispatch(GameState state)
    {
        GameState oldState = currentState;

        currentState = state;
        switch (state)
        {
            case GameState.Opening:
                GameOpening();
                break;
            case GameState.Playing:
                GameStart();
                break;
            case GameState.Clear:
                GameClear();
                break;
            case GameState.Over:
                //if (oldState == GameState.Playing)
                //{
                    GameOver();
                //}
                break;
        }
    }

    // オープニング処理
    void GameOpening()
    {
        currentState = GameState.Opening;
        //Sound.StopBgm();
    }

    void GameStart()
    {
        if (gameOverCanvasClone)
        {
            Destroy(gameOverCanvasClone);
        }
        else if (gameClearCanvasClone)
        {
            Destroy(gameClearCanvasClone);
        }
    }

    public void GameClear()
    {
        stage++;
        gameClearCanvasClone = Instantiate(gameClearCanvasPrefab);
        //後の処理はgameClearCanvasCloneで処理される。
    }
    public void GameOver()
    {
        gameOverCanvasClone = Instantiate(gameOverCanvasPrefab);
        //後の処理はgameOverCanvasCloneで処理される。
    }
    //ここからタイトルのボタン
    public void StartGame()
    {
        SceneManager.LoadScene("NowLoading");//Unityでの遷移の設定はまだしてない?
        //dispatch(GameState.Playing);
    }
    public void ExplainImage()
    {

    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("終わり");
    }
    //ここまでタイトルのボタン
    public void TitleGame()
    {
        SceneManager.LoadScene("NowLoading");
    }
}
