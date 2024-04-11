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
            var types = TypeCache.GetTypesDerivedFrom<Sentence>();
            foreach (var type in types)
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", 
                    (a) => CreateNode(type));
        }

        private void CreateNode(Type type)
        {
            Sentence node = _graph.CreateNode(type);
            CreateNodeView(node);
        }

        public void PopulateView(NPC.SentenceGraph graph)
        {
            _graph = graph;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
            
            _graph.nodes.ForEach(CreateNodeView);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    if (elem is NodeView nodeView)
                    {
                        
                    }
                });
            }
            return graphViewChange;
        }

        private void CreateNodeView(Sentence sentence)
        {
            NodeView nodeView = new NodeView(sentence, _graph);
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