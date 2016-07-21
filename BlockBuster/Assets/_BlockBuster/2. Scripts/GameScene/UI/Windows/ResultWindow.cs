using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameScene.UI.Windows {

	public class ResultWindow:MonoBehaviour {

		public FocusOutBackground focusOutBackground;
		public GameObject scoreCustomNum;
		public GameObject newRecordBubble;
		public GameObject rankCustomNum;
		public Text rankChangeNum;
		public Image rankChangeSymbol;
		public Sprite upArrow;
		public Sprite downArrow;
		public Sprite equal;

		// 결과창 표시
		public void Show(int score, int rank, int rankChange, bool isNewRecord)
		{
			// 텍스트 갱신
			scoreCustomNum.GetComponent<CustomText>().changeText(score);
			rankCustomNum.GetComponent<CustomText>().changeText(rank);
			rankChangeNum.text = Mathf.Abs(rankChange).ToString();

			// 애니메이션 재생
			this.GetComponent<Animator>().SetTrigger("ShowTrigger");
			focusOutBackground.Show();

			// RankChange값에 따라서 재생할 ChangeSymbol 바꿈
			if(rankChange < 0) {
				this.GetComponent<Animator>().SetInteger("RankChange", 1);
				rankChangeSymbol.overrideSprite = downArrow;
			} else if(rankChange > 0) {
				this.GetComponent<Animator>().SetInteger("RankChange", 0);
				rankChangeSymbol.overrideSprite = upArrow;
			} else {
				this.GetComponent<Animator>().SetInteger("RankChange", 2);
				rankChangeSymbol.overrideSprite = equal;
			}

			// ChangeSymbol 위치 변경(정확히는 부모의 위치 변경)
			rankChangeSymbol.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(rankChangeNum.preferredWidth / 2f + 45f, -224);

			// 새 기록일 경우 NewRecordBubble 띄움
			if(isNewRecord == true)
				StartCoroutine(NewRecordDelay());
		}
		private IEnumerator NewRecordDelay()
		{
			newRecordBubble.GetComponent<Animator>().SetTrigger("HideTrigger");
			yield return new WaitForSeconds(1.9f);
			newRecordBubble.GetComponent<Animator>().SetTrigger("ShowTrigger");
		}

		// 숨김
		public void Hide()
		{
			this.GetComponent<Animator>().SetTrigger("HideTrigger");
			focusOutBackground.Hide();
		}
	}
}