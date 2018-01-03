using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationChara {
	public List<Bone> bones = new List<Bone> ();

	public List<Matrix4x4> combMatArr = new List<Matrix4x4>();

	private float count = 0f;

	private Matrix4x4 gmat = Matrix4x4.identity;

	private Vector3[] newPos = new Vector3[6];
	private Vector3[] newVec = new Vector3[6];

	private PlaneData m_planeData;

	public Vector3 pivot = Vector3.zero;

	private int m_startIndex = 0;


	public Vector3 position = Vector3.zero;
	public Vector3 scale = new Vector3(1.2f,0.85f,1.2f);
	public Quaternion rot = Quaternion.Euler(0f, 0f, 0f);

	public float countTime = 0.05f;

	public AnimationChara () {
	}

	public void Create(Vector3 pos, int start) {
		m_planeData = new PlaneData ();
		m_startIndex = start;

		Matrix4x4 mat0 = MatrixUtils.RotateX ((-90.0f * Mathf.PI) / 180);
		Matrix4x4 mat1 = MatrixUtils.RotateX ((90.0f * Mathf.PI) / 180);
		mat0.m30 = 0.0000f;
		mat0.m31 = 0.5f;

		mat1.m30 = 0.0000f;
		mat1.m31 = 1.5f;


		Bone rootBone = new Bone(mat0);
		bones.Add (rootBone);
		bones.Add (new Bone(mat1));

		gmat = Matrix4x4.identity;
		for (int i = 0; i < bones.Count; i++) {
			combMatArr.Add (Matrix4x4.identity);
		}

		// relationship between parent and childs
		bones[0].Add(bones[1]);


		Bone.CalcRelativeMat(bones[0], gmat);

		for(int i = 0; i<newPos.Length; i++) {
			newPos [i] = Vector3.zero;
		}

		CreatePlane (pos);
	}

	public void CreatePlane(Vector3 pos) {

		BoneMain instance = BoneMain.GetInstance ();

		instance.vertices[m_startIndex] = new Vector3 (-0.5f, 0f, 0f); //0
		instance.vertices[m_startIndex+1] = new Vector3 (-0.5f, 1f, 0f); //1
		instance.vertices[m_startIndex+2] = new Vector3 (0.5f, 0f, 0f);// 5
		instance.vertices[m_startIndex+3] = new Vector3 (-0.5f, 1f, 0f); //1
		instance.vertices[m_startIndex+4] = new Vector3 (0.5f, 1f, 0f); // 4
		instance.vertices[m_startIndex+5] = new Vector3 (0.5f, 0f, 0f);// 5

		instance.vertices[m_startIndex+6] = new Vector3 (-0.5f, 1f, 0f); //1
		instance.vertices[m_startIndex+7] = new Vector3 (-0.5f, 1.5f, 0f); //2
		instance.vertices[m_startIndex+8] = new Vector3 (0.5f, 1f, 0f); // 4
		instance.vertices[m_startIndex+9] = new Vector3 (-0.5f, 1.5f, 0f); //2
		instance.vertices[m_startIndex+10] = new Vector3 (0.5f, 1.5f, 0f);//3
		instance.vertices[m_startIndex+11] = new Vector3 (0.5f, 1f, 0f); // 4

		instance.indices[m_startIndex] = 0; //0
		instance.indices[m_startIndex+1] =  1; //1
		instance.indices[m_startIndex+2] =  5; //5
		instance.indices[m_startIndex+3] =  1; //1
		instance.indices[m_startIndex+4] =  4; //4
		instance.indices[m_startIndex+5] =  5; //5

		instance.indices[m_startIndex+6] =  1; //1
		instance.indices[m_startIndex+7] =  2; //2
		instance.indices[m_startIndex+8] =  4; //4
		instance.indices[m_startIndex+9] =  2; //2
		instance.indices[m_startIndex+10] =  3; //3
		instance.indices[m_startIndex+11] =  4; //4

		instance.uvs[m_startIndex] = new Vector2 (0f, 0f); //0
		instance.uvs[m_startIndex+1] = new Vector2 (0f, 0.6f);//1
		instance.uvs[m_startIndex+2] = new Vector2 (1f, 0f);//5
		instance.uvs[m_startIndex+3] = new Vector2 (0f, 0.6f);//1
		instance.uvs[m_startIndex+4] = new Vector2 (1f, 0.6f);//4
		instance.uvs[m_startIndex+5] = new Vector2 (1f, 0f);//5

		instance.uvs[m_startIndex+6] = new Vector2 (0f, 0.6f);//1
		instance.uvs[m_startIndex+7] = new Vector2 (0f, 1f);//2
		instance.uvs[m_startIndex+8] = new Vector2 (1f, 0.6f);//4
		instance.uvs[m_startIndex+9] = new Vector2 (0f, 1f);//2
		instance.uvs[m_startIndex+10] = new Vector2 (1f, 1f);//3
		instance.uvs[m_startIndex+11] = new Vector2 (1f, 0.6f);//4

		instance.uvs2[m_startIndex] = new Vector2 ((float)m_startIndex, 0f);
		instance.uvs2[m_startIndex+1] = new Vector2 ((float)m_startIndex+1, 0f);
		instance.uvs2[m_startIndex+2] = new Vector2 ((float)m_startIndex+2, 0f);
		instance.uvs2[m_startIndex+3] = new Vector2 ((float)m_startIndex+3, 0f);
		instance.uvs2[m_startIndex+4] = new Vector2 ((float)m_startIndex+4, 0f);
		instance.uvs2[m_startIndex+5] = new Vector2 ((float)m_startIndex+5, 0f);

		instance.uvs2[m_startIndex+6] = new Vector2 ((float)m_startIndex+6, 0f);
		instance.uvs2[m_startIndex+7] = new Vector2 ((float)m_startIndex+7, 0f);
		instance.uvs2[m_startIndex+8] = new Vector2 ((float)m_startIndex+8, 0f);
		instance.uvs2[m_startIndex+9] = new Vector2 ((float)m_startIndex+9, 0f);
		instance.uvs2[m_startIndex+10] = new Vector2 ((float)m_startIndex+10, 0f);
		instance.uvs2[m_startIndex+11] = new Vector2 ((float)m_startIndex+11, 0f);

		instance.faceCount++;

		Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
		Matrix4x4 m = Matrix4x4.identity;
		m.SetTRS(pos, rotation, scale);

		for(int i = m_startIndex; i<m_startIndex+12; i++) {
			Vector3 newVert = m.MultiplyPoint3x4 (instance.vertices[i] - pivot);
			instance.vertices [i] = newVert;
		}
		position = pos;
	}

	public void Loop() {
		BoneMain instance = BoneMain.GetInstance ();

		count += countTime;
		float s = Mathf.Sin(count);
		float a = 0.3f * s;

		for (int i = 0; i < bones.Count; i++) {
			Matrix4x4 m = MatrixUtils.RotateY (a);
			bones [i].matrixBone = bones [i].matrixInit * m;
		}

		Bone.UpdateBone(bones[0], gmat);

		for (int i = 0; i < bones.Count; i++) {
			combMatArr[i] = bones[i].matrixComb;
		}

		int index = 0;
		for (int i = 0; i < 6; i++) {
			int idxBase = index * 3;
			int idx0 = idxBase + 0;
			int idx1 = idxBase + 1;
			int idx2 = idxBase + 2;

			Matrix4x4[] comb1 = {
				Matrix4x4.identity,
				Matrix4x4.identity,
				Matrix4x4.identity,
				Matrix4x4.identity
			};
			Matrix4x4 comb2 = Matrix4x4.zero;

			for (var j = 0; j < 3; j++) {
				int boneIdx   = index * 4 + j;
				int weightIdx = index * 3 + j;

				comb1[j] = MatrixUtils.MultiplyScalar(combMatArr[m_planeData.boneIndices[boneIdx]],
					m_planeData.weights[weightIdx],comb1[j]);
			}

			// 1.0 - weight1 - weight2 - weight3
			float weight = 1.0f - (m_planeData.weights[index * 3 + 0] +
				m_planeData.weights[index * 3 + 1] +
				m_planeData.weights[index * 3 + 2]);
			comb1[3] = MatrixUtils.MultiplyScalar(combMatArr[m_planeData.boneIndices[index * 4 + 3]], weight, comb1[3]);

			for (int k = 0; k < 4; k++) {
				comb2 = MatrixUtils.Add (comb2, comb1 [k], comb2);
			}

			Vector3 pos = newPos [i];
			pos.x = m_planeData.position[idx0];
			pos.y = m_planeData.position[idx1];
			pos.z = m_planeData.position[idx2];

			pos = comb2.MultiplyVector(pos);
			newVec [index] = pos;
			index++;
		}
			
		Matrix4x4 m2 = Matrix4x4.identity;
		m2.SetTRS(position, rot, scale);

		index = 0;
		for(int i = m_startIndex; i<m_startIndex+12; i++) {
			Vector3 newVert = m2.MultiplyPoint3x4(newVec [m_planeData.index [index]] - pivot);
			instance.vertices [i] = newVert;
			instance.indices [i] = i;
			index++;
		}

	}

}
