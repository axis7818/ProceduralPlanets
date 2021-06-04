using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
	Planet planet;
	Editor shapeEditor;
	Editor colorEditor;

	public override void OnInspectorGUI()
	{
		using (var check = new EditorGUI.ChangeCheckScope())
		{
			base.OnInspectorGUI();
			if (check.changed)
			{
				planet.GeneratePlanet();
			}
		}

		if (GUILayout.Button("Generate Planet"))
		{
			planet.GeneratePlanet();
		}

		DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettings.foldout, ref shapeEditor);
		DrawSettingsEditor(planet.colorSettings, planet.OnColorSettingsUpdated, ref planet.colorSettings.foldout, ref colorEditor);
	}

	private void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
	{
		if (settings == null)
		{
			return;
		}

		foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
		using (var check = new EditorGUI.ChangeCheckScope())
		{
			if (foldout)
			{
				CreateCachedEditor(settings, null, ref editor);
				editor.OnInspectorGUI();

				if (check.changed)
				{
					onSettingsUpdated();
				}
			}
		}
	}

	private void OnEnable()
	{
		planet = (Planet)target;
	}
}
