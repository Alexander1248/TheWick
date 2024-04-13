using System;
using NPC.SentenceNodes;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SentenceGraph
{
    public class NodeView : Node
    {
        public Action<NodeView> OnNodeSelected;
        
        public readonly Sentence Sentence;
        private readonly NPC.SentenceGraph _graph;
        public Port Input { get; private set; }
        public  Port[] Outputs { get; private set; }
        public NodeView(Sentence sentence, NPC.SentenceGraph graph)
        {
            Sentence = sentence;
            _graph = graph;
            title = sentence.name;
            viewDataKey = sentence.guid;

            style.left = Sentence.nodePos.x;
            style.top = Sentence.nodePos.y;

            CreateInputPorts();
            CreateOutputPorts();
        }

        private void CreateInputPorts()
        {
            if (Sentence is DialogueRoot) return;
            Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, null);
            inputContainer.Add(Input);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            Sentence.BuildContextualMenu(evt);
        }

        private void CreateOutputPorts()
        {
            Outputs = new Port[Sentence.next.Count];
            for (var i = 0; i < Outputs.Length; i++)
            {
                Outputs[i] = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null);
                outputContainer.Add(Outputs[i]);
            }

        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Sentence.nodePos.x = newPos.xMin;
            Sentence.nodePos.y = newPos.yMin;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }
    }
}