using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetFace
{
	private ShapeGenerator shapeGenerator;
	private ColorGenerator colorGenerator;
	private Mesh mesh;
	private int resolution;

	// Vectors for orientation
	private Vector3 normDir;
	private Vector3 latDir;
	private Vector3 lonDir;

	public PlanetFace(ShapeGenerator shapeGenerator, ColorGenerator colorGenerator, Mesh mesh, int resolution, Vector3 normalDirection)
	{
		this.shapeGenerator = shapeGenerator;
		this.colorGenerator = colorGenerator;
		this.mesh = mesh;
		this.resolution = resolution;

		normDir = Vector3.Normalize(normalDirection);
		latDir = new Vector3(normDir.y, normDir.z, normDir.x);
		lonDir = Vector3.Cross(normDir, latDir);
	}

	public void ConstructMesh()
	{
		int totalVertices = resolution * resolution;
		Vector3[] vertices = new Vector3[totalVertices];
		int vertexIndex = 0;

		int totalTriangles = 2 * (resolution - 1) * (resolution - 1);
		int[] triangles = new int[3 * totalTriangles];
		int triangleIndex = 0;

		Vector2[] uv = (mesh.uv.Length == vertices.Length) ? mesh.uv : new Vector2[vertices.Length];

		for (int y = 0; y < resolution; y++)
		{
			for (int x = 0; x < resolution; x++)
			{
				Vector3 pointOnUnitSphere = PointOnUnitSphere(x, y);
				float unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);
				vertices[vertexIndex] = pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);
				uv[vertexIndex].y = unscaledElevation;

				if (x != resolution - 1 && y != resolution - 1)
				{
					triangles[triangleIndex] = vertexIndex;
					triangles[triangleIndex + 1] = vertexIndex + 1;
					triangles[triangleIndex + 2] = vertexIndex + 1 + resolution;

					triangles[triangleIndex + 3] = vertexIndex;
					triangles[triangleIndex + 4] = vertexIndex + 1 + resolution;
					triangles[triangleIndex + 5] = vertexIndex + resolution;

					triangleIndex += 6;
				}

				vertexIndex += 1;
			}
		}

		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		mesh.uv = uv;
	}

	public void UpdateUVs()
	{
		Vector2[] uv = mesh.uv;
		for (int y = 0; y < resolution; y++)
		{
			for (int x = 0; x < resolution; x++)
			{
				int i = x + y * resolution;
				Vector3 pointOnUnitSphere = PointOnUnitSphere(x, y);
				uv[i].x = colorGenerator.BiomePercentFromPoint(pointOnUnitSphere);
			}
		}
		mesh.uv = uv;
	}

	private Vector3 PointOnUnitSphere(int x, int y)
	{
		Vector2 percent = new Vector2(x, y) / (resolution - 1);
		Vector3 pointOnUnitCube = this.normDir +   // move from center
			(percent.x - 0.5f) * 2 * latDir +       // adjust in x
			(percent.y - 0.5f) * 2 * lonDir;        // adjust in y
		Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
		return pointOnUnitSphere;
	}
}
