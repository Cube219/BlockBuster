﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameScene.UI {

	public class CustomText:MonoBehaviour {

		public Sprite Num0;
		public Sprite Num1;
		public Sprite Num2;
		public Sprite Num3;
		public Sprite Num4;
		public Sprite Num5;
		public Sprite Num6;
		public Sprite Num7;
		public Sprite Num8;
		public Sprite Num9;
		public Sprite Comma;

		public Color color;
		public enum Alignment { Left, Center, Right};
		public Alignment alignment = Alignment.Left;
		public bool hasComma = false;

		private string text;
		private List<GameObject> nums = new List<GameObject>();
		private List<GameObject> commas = new List<GameObject>();

		void Start()
		{
			changeText(0);
		}

		// 색 변경
		public void ChangeColor(Color color)
		{
			this.color = color;
			foreach(GameObject t in nums) {
				t.GetComponent<Image>().color = color;
			}
			foreach(GameObject t in commas) {
				t.GetComponent<Image>().color = color;
			}
		}

		// 텍스트 변경
		public void changeText(int num)
		{
			// 글자 수 계산
			int numCount = 0;

			int tNum = num;
			while(tNum > 0) {
				numCount++;
				tNum /= 10;
			}

			if(num == 0)
				numCount = 1;

			// 글자 쪼갬
			int[] piecesOfNum = new int[numCount];
			int pieceIndex = numCount - 1;
			while(pieceIndex >= 0) {
				piecesOfNum[pieceIndex] = num % 10;
				num /= 10;
				pieceIndex--;
			}
			
			// 글자 수에 따라서 숫자 표시하는 gameObject들을 제거/추가
			if(numCount < nums.Count) {
				while(numCount != nums.Count) {
					Destroy(nums[nums.Count - 1]);
					nums.RemoveAt(nums.Count - 1);
				}
			} else if(numCount > nums.Count) {
				while(numCount != nums.Count) {
					GameObject t = new GameObject("Num");
					t.AddComponent<RectTransform>();
					t.AddComponent<CanvasRenderer>();
					t.AddComponent<Image>();

					t.GetComponent<RectTransform>().sizeDelta = new Vector2(45, 65);
					t.GetComponent<Image>().sprite = Num0;
					t.GetComponent<Image>().color = color;

					Instantiate(t);
					t.transform.SetParent(this.transform);
					t.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

					nums.Add(t);
				}
			}

			// 만약 콤마를 넣는다면 계산
			if(hasComma == true) {
				int commaCount = (numCount - 1) / 3;

				// 콤마 수에 따라서 콤마 표시하는 gameObject들을 제거/추가
				if(commaCount < commas.Count) {
					while(commaCount != commas.Count) {
						Destroy(commas[commas.Count - 1]);
						commas.RemoveAt(commas.Count - 1);
					}
				} else if(commaCount > commas.Count) {
					while(commaCount != commas.Count) {
						GameObject t = new GameObject("Comma");
						t.AddComponent<RectTransform>();
						t.AddComponent<CanvasRenderer>();
						t.AddComponent<Image>();

						t.GetComponent<RectTransform>().sizeDelta = new Vector2(14, 65);
						t.GetComponent<Image>().sprite = Comma;
						t.GetComponent<Image>().color = color;

						Instantiate(t);
						t.GetComponent<RectTransform>().SetParent(this.transform);
						t.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

						commas.Add(t);
					}
				}
			}

			// 글자/콤마들 배치
			int commaIndex = 0;

			switch(alignment) {
				case Alignment.Left:// 왼쪽 정렬(앞에서부터 채움)
					for(int i = 0; i < numCount; i++) {
						ChangeNumSprite(nums[i], piecesOfNum[i]);
						// 숫자 : 자릿수(45) + 콤마가 쓰인 수(15)
						nums[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(45 * i + 14 * commaIndex, 0);

						// 콤마가 쓰여야 하는 자리수면 콤마 추가
						if((numCount - i - 1) % 3 == 0 && numCount - i - 1 != 0) {
							// 앞의 숫자에 +30
							commas[commaIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(45 * i + 14 * commaIndex + 30, 0);
							commaIndex++;
						}
					}
					break;

				case Alignment.Center:// 가운데 정렬
					if(numCount % 2 == 1) { // 글자수가 홀수인 경우
						for(int i=0; i<numCount; i++) {
							ChangeNumSprite(nums[i], piecesOfNum[i]);
							// 가운데를 0으로 함
							nums[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(45 * (i - (numCount - 1) / 2), 0);
						}
					} else { // 글자수가 짝수인 경우
						for(int i = 0; i < numCount; i++) {
							ChangeNumSprite(nums[i], piecesOfNum[i]);
							// 가운데에서 왼쪽을 0으로 하고 전체적으로 -22.5 밈
							nums[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(45 * (i - (numCount/2-1)) - 22.5f, 0);
						}
					}
					break;

				case Alignment.Right:// 오른쪽 정렬(뒤에서부터 채움)
					for(int i = 0; i < numCount; i++) {
						ChangeNumSprite(nums[i], piecesOfNum[numCount - i - 1]);
						// 숫자 : 자릿수(45) + 콤마가 쓰인 수(15)
						nums[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-45 * i - 14 * commaIndex, 0);

						// 콤마가 쓰여야 하는 자리수면 콤마 추가
						if((i+1) % 3 == 0 && i != numCount-1) {
							// 뒤의 숫자에 -30
							commas[commaIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(-45 * i - 14 * commaIndex - 30, 0);
							commaIndex++;
						}
					}
					break;
			}
		}

		// 해당 숫자로 스프라이트 변경
		private void ChangeNumSprite(GameObject num, int v)
		{
			switch(v) {
				case 0:
					num.GetComponent<Image>().sprite = Num0;
					break;

				case 1:
					num.GetComponent<Image>().sprite = Num1;
					break;

				case 2:
					num.GetComponent<Image>().sprite = Num2;
					break;

				case 3:
					num.GetComponent<Image>().sprite = Num3;
					break;

				case 4:
					num.GetComponent<Image>().sprite = Num4;
					break;

				case 5:
					num.GetComponent<Image>().sprite = Num5;
					break;

				case 6:
					num.GetComponent<Image>().sprite = Num6;
					break;

				case 7:
					num.GetComponent<Image>().sprite = Num7;
					break;

				case 8:
					num.GetComponent<Image>().sprite = Num8;
					break;

				case 9:
					num.GetComponent<Image>().sprite = Num9;
					break;
			}
		}
	}
}