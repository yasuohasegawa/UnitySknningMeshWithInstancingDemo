using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneData {
	public float[] position;
	public float[] color;
	public float[] uvs;
	public int[] index;
	public int[] boneIndices;
	public float[] weights;

	public PlaneData () {
		position = new float[]{
			-0.5f, 0f, 0f,
			-0.5f, 1f, 0f,
			-0.5f, 1.5f, 0f,
			0.5f, 1.5f, 0f,
			0.5f, 1f, 0f,
			0.5f, 0f, 0f
		};
		color = new float[]{
			1.0f, 0.0f, 0.0f, 1.0f,
			0.0f, 1.0f, 0.0f, 1.0f,
			0.0f, 0.0f, 1.0f, 1.0f,

			1.0f, 0.0f, 0.0f, 1.0f,
			0.0f, 1.0f, 0.0f, 1.0f,
			0.0f, 0.0f, 1.0f, 1.0f,

			1.0f, 0.0f, 0.0f, 1.0f,
			0.0f, 1.0f, 0.0f, 1.0f,
			0.0f, 0.0f, 1.0f, 1.0f,

			1.0f, 0.0f, 0.0f, 1.0f,
			0.0f, 1.0f, 0.0f, 1.0f,
			0.0f, 0.0f, 1.0f, 1.0f
		};

		uvs = new float[]{
			0f, 0f,
			0f, 0.5f,
			0f, 1f,
			1f, 1f,
			1f, 0.5f,
			1f, 0f
		};

		index = new int[]{
			0, 1, 5,
			1, 4, 5,
			1, 2, 4,
			2, 3, 4
		};

		boneIndices = new int[]{
			0, 0, 0, 0,
			1, 0, 0, 0,
			1, 0, 0, 0,
			1, 0, 0, 0,
			1, 0, 0, 0,
			0, 0, 0, 0
		};

		weights = new float[]{
			1.0f, 0.0f, 0.00f,
			0.5f, 0.5f, 0.00f,
			1.0f, 0.0f, 0.00f,
			1.0f, 0.0f, 0.00f,
			0.5f, 0.5f, 0.00f,
			1.0f, 0.0f, 0.00f
		};

	}
}