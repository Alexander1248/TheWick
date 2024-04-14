using UnityEditor;
using UnityEngine.UIElements;

namespace SentenceGraph
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        private Editor _editor;

        public void UpdateSelection(NodeView view)
        {
            Clear();
            _editor = Editor.CreateEditor(view.Sentence);
            Add(new IMGUIContainer(() => _editor.OnInspectorGUI()));
        }
    }
}