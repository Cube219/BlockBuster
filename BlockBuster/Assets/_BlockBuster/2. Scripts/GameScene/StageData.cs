using UnityEngine;
using System.Collections.Generic;

namespace GameScene {

	public class StageData{

		private List<List<int[]>> stages = new List<List<int[]>>();

		public StageData()
		{
			// Stage 1
			List<int[]> stage1 = new List<int[]>();
			stage1.Add(new int[] { 10, 00, 10, 00, 10, 00, 10, 00, 10, 00, 10 });
			stage1.Add(new int[] { 00, 11, 00, 11, 00, 11, 00, 11, 00, 11, 00 });
			stage1.Add(new int[] { 00, 00, 12, 00, 12, 00, 12, 00, 12, 00, 00 });
			stage1.Add(new int[] { 00, 00, 00, 13, 00, 13, 00, 13, 00, 00, 00 });
			stage1.Add(new int[] { 00, 00, 00, 00, 14, 00, 14, 00, 00, 00, 00 });
			stage1.Add(new int[] { 00, 00, 00, 00, 00, 15, 00, 00, 00, 00, 00 });
			stages.Add(stage1);
			// Stage 2
			List<int[]> stage2 = new List<int[]>();
			stage2.Add(new int[] { 10, 00, 10, 00, 10, 00, 10, 00, 10, 00, 10 });
			stage2.Add(new int[] { 00, 11, 00, 11, 00, 11, 00, 11, 00, 11, 00 });
			stage2.Add(new int[] { 00, 00, 12, 00, 12, 00, 12, 00, 12, 00, 00 });
			stage2.Add(new int[] { 00, 00, 00, 13, 00, 13, 00, 13, 00, 00, 00 });
			stage2.Add(new int[] { 00, 00, 00, 00, 14, 00, 14, 00, 00, 00, 00 });
			stage2.Add(new int[] { 00, 00, 00, 00, 00, 15, 00, 00, 00, 00, 00 });
			stages.Add(stage1);
		}
	}
}