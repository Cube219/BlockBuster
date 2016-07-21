using UnityEngine;
using System.Collections.Generic;
using GameScene.Blocks;

namespace GameScene {

	public struct Stage {
		public List<int[]> stageData;
	}

	public class StageCreator:MonoBehaviour {

		private List<int[]> stage = new List<int[]>();

		// 1번째 숫자 0 없음 / 1 일반벽돌 / 2 두번벽돌 / 3 안부셔지는 벽돌
		// 2번째 숫자 0 빨강 / 1 파랑 / 2 초록 / 3 분홍 / 4 보라 / 5 노랑

		public Block normalBlock;
		public GameObject blockWorld;

		void Awake()
		{
			
			stage.Add(new int[] { 10, 00, 10, 00, 10, 00, 10, 00, 10, 00, 10 });
			stage.Add(new int[] { 00, 11, 00, 11, 00, 11, 00, 11, 00, 11, 00 });
			stage.Add(new int[] { 00, 00, 12, 00, 12, 00, 12, 00, 12, 00, 00 });
			stage.Add(new int[] { 00, 00, 00, 13, 00, 13, 00, 13, 00, 00, 00 });
			stage.Add(new int[] { 00, 00, 00, 00, 14, 00, 14, 00, 00, 00, 00 });
			stage.Add(new int[] { 00, 00, 00, 00, 00, 15, 00, 00, 00, 00, 00 });
			
			/*stage.Add(new int[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 });
			stage.Add(new int[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 });
			stage.Add(new int[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 });
			stage.Add(new int[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 });
			stage.Add(new int[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 });
			stage.Add(new int[] { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 });
			stage.Add(new int[] { 00, 00, 00, 00, 00, 15, 00, 00, 00, 00, 00 });*/
			//CreateStage();
		}

		// 스테이지 생성
		public void CreateStage(int stageNum)
		{
			for(int i=0; i<stage.Count; i++) {
				for(int j=0; j<11; j++) {
					CreateBlock(stage[i][j] / 10, stage[i][j] % 10, 1.7f/2f * j, -0.8f * i);
				}
			}
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