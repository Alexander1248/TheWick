using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SentenceGraph
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        private Editor _editor;

        public InspectorView()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SentenceGraph/SentenceTreeEditor.uss");
            styleSheets.Add(styleSheet);
        }

        public void UpdateSelection(NodeView view)
        {
            Clear();
            _editor = Editor.CreateEditor(view.Sentence);
            Add(new IMGUIContainer(() => _editor.OnInspectorGUI()));
        }
    }
}