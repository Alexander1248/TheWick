using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SentenceGraph
{
    public class SentenceGraphEditor : EditorWindow
    {
        private SentenceGraphView _graphView;
        private InspectorView _inspectorView;
        

        [MenuItem("Window/Sentence Tree Editor")]
        public static void OpenWindow()
        {
            SentenceGraphEditor wnd = GetWindow<SentenceGraphEditor>();
            wnd.titleContent = new GUIContent("SentenceTreeEditor");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/SentenceGraph/SentenceTreeEditor.uxml");
            visualTree.CloneTree(root);
        
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/SentenceGraph/SentenceTreeEditor.uss");
            root.styleSheets.Add(styleSheet);

            _graphView = root.Q<SentenceGraphView>();
            _inspectorView = root.Q<InspectorView>();

            var save = root.Q<ToolbarButton>("save");
            if (save != null)
                save.clickable.clicked += _graphView.Save;
            
            
            _graphView.OnNodeSelected = OnNodeSelectionChanged;
            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            var graph = Selection.activeObject as NPC.SentenceGraph;
            if (graph && AssetDatabase.CanOpenAssetInEditor(graph.GetInstanceID())) 
                _graphView.PopulateView(graph);
            
        }

        private void OnNodeSelectionChanged(NodeView view)
        {
            _inspectorView.UpdateSelection(view);
        }
    }
}
