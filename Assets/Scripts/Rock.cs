using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {

	public int points = 5;
	public Material shineMaterial;
	public float borderWidth = 0.1f;

	private Mesh mesh;

	// Use this for initialization
	void Awake () {

		// Create Vector2 vertices

		List<Vector2> vertices2D = new List<Vector2> ();

		float start = Random.Range (0, 360);

		for (int i = 0; i < points; i++) {
			vertices2D.Add (Quaternion.Euler(0, 0, start + i * 360 / points) * Vector2.one * 0.5f * Random.Range(0.5f, 1.2f));
		}

		Vector2[] vertArray = vertices2D.ToArray();

		// Use the triangulator to get indices for creating triangles
		Triangulator tr = new Triangulator(vertArray);
		int[] indices = tr.Triangulate();

		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[vertArray.Length];
		for (int i = 0; i < vertices.Length; i++) {
			vertices[i] = new Vector3(vertArray[i].x, vertArray[i].y, 0);
		}

		mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = indices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		GetComponent<MeshFilter>().mesh = mesh;

		MeshRenderer mr = GetComponent<MeshRenderer> ();
		float c = (transform.position.z - 10) * 0.1f + 0.1f;
		mr.material.color = new Color(c, c, c, 1);

		// copy as child

		GameObject shine = new GameObject("shine");
		shine.transform.parent = transform;
		shine.transform.localPosition = new Vector3(0, 0, -0.2f);
		shine.transform.localScale = Vector2.one * 0.9f;
		shine.AddComponent<MeshFilter> ().sharedMesh = mesh;
		shine.AddComponent<MeshRenderer> ().material = shineMaterial;
		c = c * (1f + borderWidth);
		shine.GetComponent<MeshRenderer>().material.color = new Color(c, c, c, 1);
	}
}
