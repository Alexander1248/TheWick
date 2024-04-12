using System;
using System.Collections.Generic;
using System.Linq;
using NPC.SentenceNodes;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SentenceGraph
{
    public class SentenceGraphView : GraphView
    {
        public Action<NodeView> OnNodeSelected;
        private NPC.SentenceGraph _graph;

        public new class UxmlFactory : UxmlFactory<SentenceGraphView, UxmlTraits> { }

        public SentenceGraphView()
        {
            Insert(0, new GridBackground());
            
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SentenceGraph/SentenceTreeEditor.uss");
            styleSheets.Add(styleSheet);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // base.BuildContextualMenu(evt);
            evt.menu.AppendAction("Update", _ => PopulateView(_graph));
            var types = TypeCache.GetTypesDerivedFrom<Sentence>();
            foreach (var type in types.Where(type => type != typeof(DialogueRoot)))
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", _ => CreateNode(type));
        }

        private void CreateNode(Type type)
        {
            Sentence node = _graph.CreateNode(type);
            CreateNodeView(node);
        }

        private NodeView FindNodeView(Sentence sentence)
        {
            return GetNodeByGuid(sentence.guid) as NodeView;
            
        }

        public void PopulateView(NPC.SentenceGraph graph)
        {
            _graph = graph;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (_graph.root == null)
            {
                _graph.root = graph.CreateNode(typeof(DialogueRoot)) as DialogueRoot;
                EditorUtility.SetDirty(_graph);
                AssetDatabase.SaveAssets();
            }
            
            _graph.nodes.ForEach(CreateNodeView);
            
            _graph.nodes.ForEach(n =>
            {
                var from = FindNodeView(n);
                for (var i = 0; i < n.next.Length; i++)
                    if (n.next[i] != null)
                        AddElement(from.Outputs[i].ConnectTo(FindNodeView(n.next[i]).Input));
            });
            
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            graphViewChange.elementsToRemove?.ForEach(elem =>
            {
                switch (elem)
                {
                    case NodeView nodeView:
                        if (nodeView.Sentence == _graph.root) 
                            _graph.root = null;
                        _graph.DeleteNode(nodeView.Sentence);
                        break;
                    case Edge edge:
                    {
                        var from = edge.output.node as NodeView;
                        if (from == null) return;

                        var to = edge.input.node as NodeView;
                        if (to == null) return;
                        _graph.RemoveLink(from.Sentence, to.Sentence);
                        break;
                    }
                }
            });

            graphViewChange.edgesToCreate?.ForEach(edge =>
            {
                var from = edge.output.node as NodeView;
                if (from == null) return;

                var to = edge.input.node as NodeView;
                if (to == null) return;
                
                var index = 0;
                for (var i = 0; i < from.Outputs.Length; i++)
                    if (from.Outputs[i] == edge.output)
                    {
                        index = i;
                        break;
                    }
                NPC.SentenceGraph.AddLink(from.Sentence, index, to.Sentence);
            });

            return graphViewChange;
        }

        private void CreateNodeView(Sentence sentence)
        {
            NodeView nodeView = new NodeView(sentence, _graph);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort => 
                endPort.direction != startPort.direction
                && endPort.node != startPort.node).ToList();
        }
        
        
    }
}