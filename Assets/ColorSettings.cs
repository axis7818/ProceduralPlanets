using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColorSettings : ScriptableObject
{
	public Gradient oceanGradient;
	public Material planetMaterial;
	public BiomeColorSettings biomeColorSettings;

	[HideInInspector]
	public bool foldout = true;

	[System.Serializable]
	public class BiomeColorSettings
	{
		public Biome[] biomes;
		public NoiseSettings noiseSettings;
		public float noiseOffset;
		public float noiseStrength;

		[Range(0, 1)]
		public float blendAmount;

		[System.Serializable]
		public class Biome
		{
			public Gradient gradient;
			public Color tint;

			[Range(0, 1)]
			public float startLatitude;
			[Range(0, 1)]
			public float tintPercent;
		}
	}
}
