using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
	ColorSettings colorSettings;
	Texture2D texture;
	const int textureResolution = 50;
	INoiseFilter biomeNoiseFilter;

	public void UpdateSettings(ColorSettings colorSettings)
	{
		this.colorSettings = colorSettings;
		var biomes = colorSettings.biomeColorSettings.biomes;
		if (texture == null || texture.height != biomes.Length)
		{
			texture = new Texture2D(textureResolution, biomes.Length, TextureFormat.RGBA32, false);
		}
		biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(colorSettings.biomeColorSettings.noiseSettings);
	}

	public void UpdateElevation(MinMax elevationMinMax)
	{
		colorSettings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
	}

	public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
	{
		var biomeColorSettings = colorSettings.biomeColorSettings;
		var biomes = biomeColorSettings.biomes;

		float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
		heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - biomeColorSettings.noiseOffset) *
			biomeColorSettings.noiseStrength;
		float biomeIndex = 0;
		float blendRange = biomeColorSettings.blendAmount / 2f + 0.001f;
		for (int i = 0; i < biomes.Length; i += 1)
		{
			var biome = biomes[i];
			float distance = heightPercent - biome.startLatitude;
			float weight = Mathf.InverseLerp(-blendRange, blendRange, distance);
			biomeIndex *= (1 - weight);
			biomeIndex += i * weight;

		}
		return biomeIndex / Mathf.Max(1, biomes.Length - 1);
	}

	public void UpdateColors()
	{
		var biomes = colorSettings.biomeColorSettings.biomes;
		Color[] colors = new Color[textureResolution * biomes.Length];
		int colorIndex = 0;
		foreach (var biome in biomes)
		{
			for (int i = 0; i < textureResolution; i += 1)
			{
				Color gradientColor = biome.gradient.Evaluate(i / (textureResolution - 1f));
				colors[colorIndex] = gradientColor * (1 - biome.tintPercent) + biome.tint * biome.tintPercent;
				colorIndex += 1;
			}
		}
		Debug.Log("Updating texture colors");
		texture.SetPixels(colors);
		texture.Apply();

		colorSettings.planetMaterial.SetTexture("_texture", texture);
	}
}
