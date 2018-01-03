using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
Resources:
https://qiita.com/edo_m18/items/31ee6cbc3b3ff22013ae
http://marupeke296.com/DXG_No61_WhiteBoxSkinMeshAnimation.html
*/
public class BoneMain : MonoBehaviour {
	public List<Bone> bones = new List<Bone> ();
	public List<Matrix4x4> combMatArr = new List<Matrix4x4>();

	private float count = 0f;

	private Matrix4x4 gmat = Matrix4x4.identity;

	private Vector3[] newVec = new Vector3[6];

	private Vector3[] m_vertices = new Vector3[12];
	private int[] m_indices = new int[12];
	private Vector2[] m_uvs = new Vector2[12];

	private PlaneData m_planeData;

	public int faceCount = 0;
	public Vector3[] vertices;
	public int[] indices;
	public Vector2[] uvs;
	public Vector2[] uvs2;

	private List<AnimationChara> animations = new List<AnimationChara> ();


	private int MAX_CHARA = 5000;

	private Mesh m_mesh;
	private Material m_charaMat;

	[SerializeField]
	private AudioSource m_audio;

	[SerializeField]
	private GlitchEffect m_glitch;

	private int m_resolution = 1024;
	private float m_lowFreqThreshold = 11025, m_midFreqThreshold = 22050, m_highFreqThreshold = 33075, m_high2FreqThreshold = 44100;
	private float m_lowEnhance = 1f, m_midEnhance = 1.5f, m_highEnhance = 2f, m_high2Enhance = 2.5f;


	public static int MAX_VERTS = 12;
	static private BoneMain instance = null;
	static public BoneMain GetInstance() {
		return instance;
	}

	// Use this for initialization
	void Start () {
		instance = this;
		//Init ();

		m_charaMat = gameObject.GetComponent<MeshRenderer> ().materials [0];
		m_mesh = gameObject.GetComponent<MeshFilter> ().mesh;

		vertices = new Vector3[MAX_VERTS*MAX_CHARA];
		indices = new int[MAX_VERTS*MAX_CHARA];
		uvs = new Vector2[MAX_VERTS*MAX_CHARA];
		uvs2 = new Vector2[MAX_VERTS*MAX_CHARA];

		for(int i = 0; i<MAX_CHARA; i++) {
			AnimationChara a = new AnimationChara ();
			float x = Random.Range (-50f, 50f);
			float z = Random.Range (-50f, 50f);
			float y = Random.Range (-50f, 50f);
			/*
			x = (float)i;
			z = 0f;
			*/

			if(i<30){
				x = Random.Range (-5f, 5f);
				z = Random.Range (-5f, 5f);
				y = Random.Range (-5f, 5f);
			}

			a.countTime = Random.Range (0.05f, 0.08f);
			a.Create (new Vector3(x ,y ,z),i*MAX_VERTS);
			animations.Add (a);
		}

		for (int i = 0; i < animations.Count; i++) {
			animations [i].Loop ();
		}
		UpdateMesh2();
	}

	// Update is called once per frame
	void Update () {
		//Loop ();

		for (int i = 0; i < animations.Count; i++) {
			if (i < 30) {
				animations [i].Loop ();
			}
		}

		UpdateMesh2();


		float[] spectrum = m_audio.GetSpectrumData(m_resolution, 0, FFTWindow.BlackmanHarris);

		var deltaFreq = AudioSettings.outputSampleRate / m_resolution;
		float low = 0f, mid = 0f, high = 0f, high2 = 0f;

		for (var i = 0; i < m_resolution; ++i) {
			var freq = deltaFreq * i;
			if      (freq <= m_lowFreqThreshold)  low  += spectrum[i];
			else if (freq <= m_midFreqThreshold)  mid  += spectrum[i];
			else if (freq <= m_highFreqThreshold) high += spectrum[i];
			else if (freq <= m_high2FreqThreshold) high2 += spectrum[i];
		}

		low  *= m_lowEnhance;
		mid  *= m_midEnhance;
		high *= m_highEnhance;
		high2 *= m_high2Enhance;

		if (m_glitch != null) {
			m_glitch.UpdateNoise (low*0.5f);
		}

		m_charaMat.SetFloat ("_zval", high * 0.5f);
	}

	private void UpdateMesh2() {
		m_mesh.Clear ();

		m_mesh.vertices = vertices;
		m_mesh.triangles = indices;
		m_mesh.uv = uvs;
		m_mesh.uv2 = uvs2;

		m_mesh.RecalculateNormals ();
		m_mesh.RecalculateBounds ();
	}


	/*
	private void Init() {
		m_planeData = new PlaneData ();

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

		Vector2[] uvs = new Vector2[6];
		uvs [0] = new Vector2 (0f, 0f);
		uvs [1] = new Vector2 (0f, 0.6f);
		uvs [2] = new Vector2 (0f, 1f);
		uvs [3] = new Vector2 (1f, 1f);
		uvs [4] = new Vector2 (1f, 0.6f);
		uvs [5] = new Vector2 (1f, 0f);

		for (int i = 0; i < m_planeData.index.Length; i++) {
			int idx = m_planeData.index [i];
			m_uvs [i] = uvs [idx];
		}
	}

	private void Loop() {
		count += 0.05f;
		float s = Mathf.Sin(count);
		float a = 0.2f * s;

		for (int i = 0; i < bones.Count; i++) {
			Matrix4x4 m = MatrixUtils.RotateY (a);
			bones [i].matrixBone = bones [i].matrixInit * m;
		}

		Bone.UpdateBone(bones[0], gmat);

		for (int i = 0; i < bones.Count; i++) {
			combMatArr[i] = bones[i].matrixComb;
		}

		for (int i = 0; i < 6; i++) {
			int idxBase = i * 3;
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
				int boneIdx   = i * 4 + j;
				int weightIdx = i * 3 + j;

				comb1[j] = MatrixUtils.MultiplyScalar(combMatArr[m_planeData.boneIndices[boneIdx]],
					m_planeData.weights[weightIdx],comb1[j]);
			}

			// 1.0 - weight1 - weight2 - weight3
			float weight = 1.0f - (m_planeData.weights[i * 3 + 0] +
				m_planeData.weights[i * 3 + 1] +
				m_planeData.weights[i * 3 + 2]);
			comb1[3] = MatrixUtils.MultiplyScalar(combMatArr[m_planeData.boneIndices[i * 4 + 3]], weight, comb1[3]);

			for (int k = 0; k < 4; k++) {
				comb2 = MatrixUtils.Add (comb2, comb1 [k], comb2);
			}

			Vector3 pos = new Vector3(m_planeData.position[idx0],
				m_planeData.position[idx1],
				m_planeData.position[idx2]);

			pos = comb2.MultiplyVector(pos);
			newVec [i] = pos;
		}

		Quaternion rotation = Quaternion.Euler(60f, 30f, 50f);
		Matrix4x4 m2 = Matrix4x4.identity;
		m2.SetTRS(new Vector3(1f,0f,0f), rotation, new Vector3(1.5f,1f,1.5f));

		Vector3 pivot = new Vector3 (0f, 0f, 0f);

		for(int i = 0; i<m_planeData.index.Length; i++) {
			//m_vertices [i] = newVec [m_planeData.index [i]];

			Vector3 newVert = m2.MultiplyPoint3x4(newVec [m_planeData.index [i]] - pivot);
			m_vertices [i] = newVert;
			m_indices [i] = i;
		}

		UpdateMesh ();
	}

	private void UpdateMesh() {
		Mesh mesh = gameObject.GetComponent<MeshFilter> ().mesh;
		mesh.Clear ();

		mesh.vertices = m_vertices;
		mesh.triangles = m_indices;
		mesh.uv = m_uvs;

		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}
	*/
}
