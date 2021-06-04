using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	[Range(2, 256)]
	public int resolution = 10;

	public bool autoupdate = true;
	public ShapeSettings shapeSettings;
	public ColorSettings colorSettings;

	private ShapeGenerator shapeGenerator;

	[SerializeField, HideInInspector]
	private MeshFilter[] meshFilters;
	private PlanetFace[] planetFaces;

	private static Vector3[] normals = { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

	public void GeneratePlanet()
	{
		Initialize();
		GenerateMesh();
		GenerateColors();
	}

	public void OnShapeSettingsUpdated()
	{
		if (!autoupdate) { return; }
		Initialize();
		GenerateMesh();
	}

	public void OnColorSettingsUpdated()
	{
		if (!autoupdate) { return; }
		Initialize();
		GenerateColors();
	}

	void Initialize()
	{
		shapeGenerator = new ShapeGenerator(shapeSettings);

		if (meshFilters == null || meshFilters.Length == 0)
		{
			meshFilters = new MeshFilter[6];
		}
		planetFaces = new PlanetFace[6];

		for (int i = 0; i < 6; i++)
		{
			if (meshFilters[i] == null)
			{
				GameObject meshObj = new GameObject("face" + i);
				meshObj.transform.parent = this.transform;

				meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
				meshFilters[i] = meshObj.AddComponent<MeshFilter>();
				meshFilters[i].sharedMesh = new Mesh();
			}

			planetFaces[i] = new PlanetFace(shapeGenerator, meshFilters[i].sharedMesh, this.resolution, normals[i]);
		}
	}

	void GenerateMesh()
	{
		foreach (var face in planetFaces)
		{
			face.ConstructMesh();
		}
	}

	void GenerateColors()
	{
		foreach (var mesh in meshFilters)
		{
			mesh.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.color;
		}
	}
}
