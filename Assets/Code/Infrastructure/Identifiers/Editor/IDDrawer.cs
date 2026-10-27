using System;
using UnityEditor;
using UnityEngine;

namespace Code.Infrastructure.Identifiers.Editor
{
	[CustomPropertyDrawer(typeof(IDAttribute))]
	public class IDDrawer : PropertyDrawer
	{
		private const float ButtonWidth = 25f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.String)
			{
				EditorGUI.LabelField(position, label.text, "[ID] works only on string fields");

				return;
			}

			Rect fieldRect = new(position.x, position.y, position.width - ButtonWidth * 2, position.height);
			Rect copyRect = new(position.xMax - ButtonWidth * 2, position.y, ButtonWidth, position.height);
			Rect actionRect = new(position.xMax - ButtonWidth, position.y, ButtonWidth, position.height);

			using (new EditorGUI.DisabledScope(true))
			{
				EditorGUI.PropertyField(fieldRect, property, label);
			}

			if (string.IsNullOrEmpty(property.stringValue))
			{
				DrawAssignButton(actionRect, property);

				return;
			}

			DrawCopyButton(copyRect, property);
			DrawRegenerateButton(actionRect, property);
		}

		private static void DrawAssignButton(Rect rect, SerializedProperty property)
		{
			GUIContent content = new("+", "This asset has no ID yet. Assign it once and never change it afterwards.");

			if (GUI.Button(rect, content) == false)
				return;

			Assign(property);
			Debug.Log($"Assigned ID {property.stringValue} to {property.serializedObject.targetObject.name}");
		}

		private static void DrawCopyButton(Rect rect, SerializedProperty property)
		{
			if (GUI.Button(rect, EditorGUIUtility.IconContent("Clipboard")))
			{
				GUIUtility.systemCopyBuffer = property.stringValue;
			}
		}

		private static void DrawRegenerateButton(Rect rect, SerializedProperty property)
		{
			GUIContent content = new(EditorGUIUtility.IconContent("d_Refresh@2x"));
			content.tooltip = "Regenerate the ID. Anything that stored the old ID loses its reference to this asset.";

			if (GUI.Button(rect, content) == false)
				return;

			string assetName = property.serializedObject.targetObject.name;
			bool confirmed = EditorUtility.DisplayDialog(
				"Regenerate ID",
				$"You are about to regenerate the ID of {assetName}.\n\n"
				+ "Anything that stored the old ID loses its reference to this asset.\n\nDo you want to proceed?",
				"Yes",
				"No");

			if (confirmed == false)
				return;

			string previousId = property.stringValue;
			Assign(property);
			Debug.LogWarning($"Regenerated ID for {assetName}: {previousId} -> {property.stringValue}");
		}

		private static void Assign(SerializedProperty property)
		{
			property.stringValue = Guid.NewGuid().ToString();
			property.serializedObject.ApplyModifiedProperties();
		}
	}
}
