using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Pianola.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var field = new PropertyField(property, property.displayName);
            field.SetEnabled(false);
            return field;
        }
    }
}
