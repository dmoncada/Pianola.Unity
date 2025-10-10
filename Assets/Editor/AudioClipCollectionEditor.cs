using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pianola.Editor
{
    [CustomEditor(typeof(AudioClipCollection))]
    public class AudioClipCollectionEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            var button = new Button(TryFillFromSelection);
            button.text = "Fill audio clip infos from selection";
            container.Add(button);

            InspectorElement.FillDefaultInspector(container, serializedObject, this);

            return container;
        }

        private void TryFillFromSelection()
        {
            var collection = target as AudioClipCollection;

            var clips = new List<AudioClip>();

            foreach (var selectedObject in Selection.objects)
            {
                if (selectedObject is AudioClip clip)
                {
                    clips.Add(clip);
                }
            }

            Undo.RecordObject(collection, "Fill audio clips");

            collection.SetClips(clips);
            EditorUtility.SetDirty(collection);
            AssetDatabase.SaveAssets();
        }
    }
}
