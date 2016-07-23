using UnityEngine;
using System.Collections.Generic;
using GameScene.Blocks;

namespace GameScene {

	public struct Stage {
		public List<int[]> stageData;
	}

	public class StageCreator:MonoBehaviour {

		private List<List<int[]>> stages = new List<List<int[]>>();
	/*	private List<int[]> stage1 = new List<int[]>();
		private List<int[]> stage2 = new List<int[]>();
		private List<int[]> stage3 = new List<int[]>();
		private List<int[]> stage4 = new List<int[]>();
		private List<int[]> stage5 = new List<int[]>();*/
		private int[] stagesNum = new int[] { 0, 21, 36, 24, 27, 36 };


		// 1번째 숫자 0 없음 / 1 일반벽돌 / 2 두번벽돌 / 3 안부셔지는 벽돌
		// 2번째 숫자 0 빨강 / 1 파랑 / 2 초록 / 3 분홍 / 4 보라 / 5 노랑

		public Block normalBlock;
		public GameObject blockWorld;

		void Awake()
		{
			List<int[]> stage0 = new List<int[]>();
			stages.Add(stage0);

			List<int[]> stage1 = new List<int[]>();
			stage1.Add(new int[] { 10, 00, 10, 00, 10, 00, 10, 00, 10, 00, 10 });
			stage1.Add(new int[] { 00, 11, 00, 11, 00, 11, 00, 11, 00, 11, 00 });
			stage1.Add(new int[] { 00, 00, 12, 00, 12, 00, 12, 00, 12, 00, 00 });
			stage1.Add(new int[] { 00, 00, 00, 13, 00, 13, 00, 13, 00, 00, 00 });
			stage1.Add(new int[] { 00, 00, 00, 00, 14, 00, 14, 00, 00, 00, 00 });
			stage1.Add(new int[] { 00, 00, 00, 00, 00, 15, 00, 00, 00, 00, 00 });
			stages.Add(stage1);

			List<int[]> stage2 = new List<int[]>();
			stage2.Add(new int[] { 10, 00, 10, 00, 10, 00, 10, 00, 10, 00, 10 });
			stage2.Add(new int[] { 11, 00, 11, 00, 11, 00, 11, 00, 11, 00, 11 });
			stage2.Add(new int[] { 12, 00, 12, 00, 12, 00, 12, 00, 12, 00, 12 });
			stage2.Add(new int[] { 13, 00, 13, 00, 13, 00, 13, 00, 13, 00, 13 });
			stage2.Add(new int[] { 14, 00, 14, 00, 14, 00, 14, 00, 14, 00, 14 });
			stage2.Add(new int[] { 15, 00, 15, 00, 15, 00, 15, 00, 15, 00, 15 });
			stages.Add(stage2);

			List<int[]> stage3 = new List<int[]>();
			stage3.Add(new int[] { 00, 10, 00, 00, 10, 00, 00, 10, 00, 00, 10 });
			stage3.Add(new int[] { 11, 00, 00, 11, 00, 00, 11, 00, 00, 11, 00 });
			stage3.Add(new int[] { 00, 14, 00, 00, 14, 00, 00, 14, 00, 00, 14 });
			stage3.Add(new int[] { 13, 00, 00, 13, 00, 00, 13, 00, 00, 13, 00 });
			stage3.Add(new int[] { 00, 12, 00, 00, 12, 00, 00, 12, 00, 00, 12 });
			stage3.Add(new int[] { 15, 00, 00, 15, 00, 00, 15, 00, 00, 15, 00 });
			stages.Add(stage3);

			List<int[]> stage4 = new List<int[]>();
			stage4.Add(new int[] { 00, 00, 10, 00, 00, 00, 00, 00, 10, 00, 00 });
			stage4.Add(new int[] { 00, 10, 00, 10, 00, 00, 00, 10, 00, 10, 00 });
			stage4.Add(new int[] { 10, 00, 10, 00, 10, 00, 10, 00, 10, 00, 10 });
			stage4.Add(new int[] { 00, 10, 00, 10, 00, 10, 00, 10, 00, 10, 00 });
			stage4.Add(new int[] { 00, 00, 10, 00, 10, 00, 10, 00, 10, 00, 00 });
			stage4.Add(new int[] { 00, 00, 00, 10, 00, 10, 00, 10, 00, 00, 00 });
			stage4.Add(new int[] { 00, 00, 00, 00, 10, 00, 10, 00, 00, 00, 00 });
			stage4.Add(new int[] { 00, 00, 00, 00, 00, 10, 00, 00, 00, 00, 00 });
			stages.Add(stage4);

			List<int[]> stage5 = new List<int[]>();
			stage5.Add(new int[] { 11, 00, 11, 00, 11, 00, 11, 00, 11, 00, 11 });
			stage5.Add(new int[] { 11, 00, 00, 00, 00, 00, 00, 00, 00, 00, 11 });
			stage5.Add(new int[] { 11, 00, 00, 10, 00, 10, 00, 10, 00, 00, 11 });
			stage5.Add(new int[] { 11, 00, 00, 10, 00, 15, 00, 10, 00, 00, 11 });
			stage5.Add(new int[] { 11, 00, 00, 10, 00, 15, 00, 10, 00, 00, 11 });
			stage5.Add(new int[] { 11, 00, 00, 10, 00, 10, 00, 10, 00, 00, 11 });
			stage5.Add(new int[] { 11, 00, 00, 00, 00, 00, 00, 00, 00, 00, 11 });
			stage5.Add(new int[] { 11, 00, 11, 00, 11, 00, 11, 00, 11, 00, 11 });
			stages.Add(stage5);
		}

		// 스테이지 생성
		public int CreateStage(int stageNum)
		{
			// 스테이지가 6 이상이면 1~5 랜덤으로
			if(stageNum >= 6) {
				stageNum = Random.Range(1, 6);
			}

			for(int i=0; i<stages[stageNum].Count; i++) {
				for(int j=0; j<11; j++) {
					CreateBlock(stages[stageNum][i][j] / 10, stages[stageNum][i][j] % 10, 1.7f / 2f * j, -0.8f * i);
				}
			}

			return stagesNum[stageNum];
		}

		// 블록 생성
		private void CreateBlock(int type, int color, float x, float y)
		{
			GameObject block = null;

			switch(type) {
				case 1:
					block = normalBlock.gameObject;
					break;
			}

			if(block != null) {
				GameObject t = (GameObject)Instantiate(block, Vector2.zero, Quaternion.identity);

				switch(color) {
					case 0:
						t.GetComponent<Block>().SetColor(Color.red);
						break;

					case 1:
						t.GetComponent<Block>().SetColor(Color.blue);
						break;

					case 2:
						t.GetComponent<Block>().SetColor(Color.green);
						break;

					case 3:
						t.GetComponent<Block>().SetColor(Color.magenta);
						break;

					case 4:
						t.GetComponent<Block>().SetColor(new Color(178f / 255f, 0, 1f));
						break;

					case 5:
						t.GetComponent<Block>().SetColor(Color.yellow);
						break;
				}

				t.transform.SetParent(blockWorld.transform);
				t.transform.localPosition = new Vector2(x, y);
			}
		}
	}
}