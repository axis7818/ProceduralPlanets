using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	[Range(2, 256)]
	public int resolution = 10;
	public bool autoupdate = true;
	public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back }
	public FaceRenderMask faceRenderMask;

	public ShapeSettings shapeSettings;
	private ShapeGenerator shapeGenerator = new ShapeGenerator();

	public ColorSettings colorSettings;
	private ColorGenerator colorGenerator = new ColorGenerator();

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
		shapeGenerator.UpdateSettings(shapeSettings);
		colorGenerator.UpdateSettings(colorSettings);

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

				meshObj.AddComponent<MeshRenderer>();
				meshFilters[i] = meshObj.AddComponent<MeshFilter>();
				meshFilters[i].sharedMesh = new Mesh();
			}
			meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

			planetFaces[i] = new PlanetFace(shapeGenerator, meshFilters[i].sharedMesh, this.resolution, normals[i]);
			bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
			meshFilters[i].gameObject.SetActive(renderFace);
		}
	}

	void GenerateMesh()
	{
		for (int i = 0; i < 6; i += 1)
		{
			if (meshFilters[i].gameObject.activeSelf)
			{
				planetFaces[i].ConstructMesh();
			}
		}

		colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
	}

	void GenerateColors()
	{
		colorGenerator.UpdateColors();
	}
}
