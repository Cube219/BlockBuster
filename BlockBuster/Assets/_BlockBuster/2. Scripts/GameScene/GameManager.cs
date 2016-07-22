using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameScene.UI.Windows;
using UnityEngine.SceneManagement;

namespace GameScene {

	public class GameManager:MonoBehaviour {

		public static GameManager m;

		public FocusOutBackground hideScreenBackground;
		public GameObject ingameWorld;
		public Bar bar;
		public Ball ball;
		public StageCreator stageCreator;
		public GameObject stageTxt;
		public GameObject stageNumTxt;
		public Image readyTxt;
		public Image goTxt;
		public GameObject stageClearTxt;
		public GameObject gameOverTxt;
		public UI.CustomText scoreCustomText;
		public Image[] heartImages;

		public ResultWindow resultWindow;

		private enum GameState { FirstStart, Load, LateLoad, Ready, Run, Respawn, GameOver, Pause, Count};
		private GameState gameState;

		private delegate void UpdateDelegate();
		private UpdateDelegate[] updateDelegates;

		private List<Ball> balls = new List<Ball>();
		private int gameStage;
		private int score = 0;
		private int life = 1;
		private int blockNumber;

		void Awake()
		{
			m = this;
			// Update 델리게이트 설정
			updateDelegates = new UpdateDelegate[(int)GameState.Count];
			updateDelegates[(int)GameState.FirstStart] = UpdateGameFirstStart;
			updateDelegates[(int)GameState.Load] = UpdateGameLoad;
			updateDelegates[(int)GameState.LateLoad] = UpdateGameLateLoad;
			updateDelegates[(int)GameState.Ready] = UpdateGameReady;
			updateDelegates[(int)GameState.Run] = UpdateGameRun;
			updateDelegates[(int)GameState.Respawn] = UpdateGameRespawn;
			updateDelegates[(int)GameState.GameOver] = UpdateGameGameOver;
			updateDelegates[(int)GameState.Pause] = UpdateGamePause;

			gameStage = 1;
			gameState = GameState.FirstStart;
		}

		void Update()
		{
			updateDelegates[(int)gameState]();
		}

		// --------------------------
		// 스코어 얻음
		public void GetScore(int score)
		{
			this.score += score;
		}

		// 목슴 감소
		public void LoseLife()
		{
			life--;
			heartImages[life].gameObject.SetActive(false);

			if(life > 0) {
				gameState = GameState.Respawn;
			} else {
				GameOver();
			}
		}

		// 벽돌이 깨짐
		public void BreakBlock()
		{
			score += 100;

			scoreCustomText.changeText(score);

			blockNumber--;
			if(blockNumber <= 0) {
				GameClear();
			}
		}

		// 공이 죽음
		public void BallDied(Ball ball)
		{
			balls.Remove(ball);
			if(balls.Count == 0)
				LoseLife();
		}

		// 게임 오버
		public void GameOver()
		{
			gameState = GameState.GameOver;
			StartCoroutine(GameOver_c());
		}
		private IEnumerator GameOver_c()
		{
			// GameOver 애니메이션 재생
			gameOverTxt.GetComponent<Animator>().SetTrigger("ShowTrigger");
			yield return new WaitForSeconds(5.5f);

			// 점수 서버로 보냄
			ServerManager.m.Post_ScoreResult += SendScoreResult;
			ServerManager.m.Post_Score_f(UserManager.uid, UserManager.sid, score);
		}
		private void SendScoreResult(Dictionary<string, string> data)
		{
			resultWindow.Show(score, int.Parse(data["rank"]), int.Parse(data["rankChange"]), bool.Parse(data["isNewRecord"]));
		}

		// 게임 클리어
		public void GameClear()
		{
			StartCoroutine(GameClear_c());
		}
		public IEnumerator GameClear_c()
		{
			foreach(Ball b in balls) {
				b.Pause();
			}
			stageClearTxt.GetComponent<Animator>().SetTrigger("ShowTrigger");
			yield return new WaitForSeconds(4.5f);
			// 공들을 다 지움
			foreach(Ball b in balls) {
				Destroy(b.gameObject);
			}
			balls.Clear();
			// 다음 스테이지로
			gameStage++;
			gameState = GameState.Load;
		}

		// 게임 다시 시도
		public void GameRetry()
		{
			StartCoroutine(GameRetry_c());
		}
		private IEnumerator GameRetry_c()
		{
			// 결과창 닫음
			resultWindow.Hide();
			yield return new WaitForSeconds(0.6f);
			// 초기화
			gameStage = 1;
			score = 0;
			life = 3;
			scoreCustomText.changeText(score);
			foreach(Image h in heartImages) {
				h.gameObject.SetActive(true);
			}
			GameObject[] t = GameObject.FindGameObjectsWithTag("Block");
			foreach(GameObject tt in t)
				Destroy(tt);
			// 시작
			gameState = GameState.Load;
		}

		// 로비로
		public void GoLobby()
		{
			StartCoroutine(GoLobby_c());
		}
		private IEnumerator GoLobby_c()
		{
			// 화면을 가림
			hideScreenBackground.Show();
			yield return new WaitForSeconds(0.4f);
			// LobbyScene 로드
			SceneManager.LoadScene("LobbyScene");
		}

		// 게임 일시정지
		public void GamePause()
		{
			if(gameState == GameState.Run) {
				bar.Pause();
				foreach(Ball b in balls) {
					b.Pause();
				}
				Time.timeScale = 0;
				gameState = GameState.Pause;
			}
		}

		// 게임 재게
		public void GameResume()
		{
			if(gameState == GameState.Pause) {
				bar.Resume();
				foreach(Ball b in balls) {
					b.Resume();
				}
				Time.timeScale = 1;
				gameState = GameState.Run;
			}
		}

		// ----- 델리게이트 함수들 -----
		// 처음 시작됨
		private void UpdateGameFirstStart()
		{
			if(isShowScreenRunning == false)
				StartCoroutine(ShowScreen());
		}
		// 화면을 보여줌
		private bool isShowScreenRunning = false;
		private IEnumerator ShowScreen()
		{
			isShowScreenRunning = true;
			// 맨 처음에는 화면을 즉시 가림
			hideScreenBackground.ShowImmediately();
			// 화면을 보여줌
			hideScreenBackground.Hide();
			yield return new WaitForSeconds(0.4f);
			// 로드
			gameState = GameState.Load;
			isShowScreenRunning = false;
		}

		// 로드
		private void UpdateGameLoad()
		{
			// 스테이지 생성
			stageCreator.CreateStage(gameStage);
			blockNumber = 21;
			gameState = GameState.LateLoad;
		}

		// 로드 직후
		private void UpdateGameLateLoad()
		{
			if(isShowStageTxtRunning == false)
				StartCoroutine(ShowStageTxt());
		}
		// 스테이지 텍스트 표시
		private bool isShowStageTxtRunning = false;
		private IEnumerator ShowStageTxt()
		{
			isShowStageTxtRunning = true;
			// 스테이지 텍스트 표시
			stageNumTxt.GetComponent<UI.CustomText>().changeText(gameStage);
			stageTxt.GetComponent<Animator>().SetTrigger("ShowTrigger");
			yield return new WaitForSeconds(2f);

			gameState = GameState.Ready;
			isShowStageTxtRunning = false;
		}

		// 준비
		private void UpdateGameReady()
		{
			if(isShowReadyGoTxtRunning == false)
				StartCoroutine(ShowReadyGoTxt());
		}
		// 레디 / 고 텍스트 표시
		private bool isShowReadyGoTxtRunning = false;
		private IEnumerator ShowReadyGoTxt()
		{
			isShowReadyGoTxtRunning = true;
			// 공 생성
			GameObject t = (GameObject)Instantiate(ball.gameObject, Vector2.zero, Quaternion.identity);
			t.transform.SetParent(ingameWorld.transform);
			t.transform.localPosition = new Vector2(0f, -7.65f);
			balls.Add(t.GetComponent<Ball>());
			t.GetComponent<Ball>().speed = 6f + (float)gameStage / 2f;

			// 레디 표시
			readyTxt.GetComponent<Animator>().SetTrigger("ShowTrigger");
			yield return new WaitForSeconds(1.31f);
			// 고 표시
			goTxt.GetComponent<Animator>().SetTrigger("ShowTrigger");
			yield return new WaitForSeconds(1.3f);

			// 시작
			t.GetComponent<Ball>().BallStart();
			gameState = GameState.Run;
			isShowReadyGoTxtRunning = false;
		}

		// 실행중
		private void UpdateGameRun()
		{
			if(Input.GetKeyDown(KeyCode.Escape)) {
				GamePause();
			}
		}

		// 리스폰
		private void UpdateGameRespawn()
		{
			if(isRespawnDelayRunning == false)
				StartCoroutine(RespawnDelay());
		}
		private bool isRespawnDelayRunning = false;
		private IEnumerator RespawnDelay()
		{
			isRespawnDelayRunning = true;
			yield return new WaitForSeconds(3f);
			yield return StartCoroutine(ShowReadyGoTxt());
			isRespawnDelayRunning = false;
		}
		// 게임 오버
		private void UpdateGameGameOver()
		{
			if(Input.GetKeyDown(KeyCode.Escape)) {
				GoLobby();
			}
		}

		// 일시 정지
		private void UpdateGamePause()
		{
			if(Input.GetKeyDown(KeyCode.Escape)) {
				GameResume();
			}
		}
	}
}