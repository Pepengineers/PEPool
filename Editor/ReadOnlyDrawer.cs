using PEPEngineers.PEPools.Attributes;
using UnityEditor;
using UnityEngine;

namespace PEPEngineers.PEPools.Editor
{
	/// <summary>
	///     This class contain custom drawer for ReadOnly attribute.
	/// </summary>
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	internal class ReadOnlyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property,
			GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		/// <summary>
		///     Unity method for drawing GUI in Editor
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="property">Property.</param>
		/// <param name="label">Label.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			// Saving previous GUI enabled value
			var previousGUIState = GUI.enabled;
			// Disabling edit for property
			GUI.enabled = false;
			// Drawing Property
			EditorGUI.PropertyField(position, property, label, true);
			// Setting old GUI enabled value
			GUI.enabled = previousGUIState;
		}
	}
}