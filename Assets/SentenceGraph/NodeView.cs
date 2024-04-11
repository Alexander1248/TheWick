using NPC.SentenceNodes;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace SentenceGraph
{
    public class NodeView : Node
    {
        private readonly Sentence _sentence;
        private readonly NPC.SentenceGraph _graph;
        private Port _input;
        private Port[] _outputs;
        public NodeView(Sentence sentence, NPC.SentenceGraph graph)
        {
            _sentence = sentence;
            _graph = graph;
            title = sentence.name;
            viewDataKey = sentence.guid;

            style.left = _sentence.nodePos.x;
            style.top = _sentence.nodePos.y;

            CreateInputPorts();
            CreateOutputPorts();
        }
        

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Select As Main Node", (a) =>
            {
                _graph.root = _sentence;
            });
            evt.menu.AppendAction("Delete Node", (a) =>
            {
                _graph.DeleteNode(_sentence);
                parent.Remove(this);
            });
            base.BuildContextualMenu(evt);
        }

        private void CreateInputPorts()
        {
            _input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, null);
            inputContainer.Add(_input);
        }

        private void CreateOutputPorts()
        {
            _outputs = new Port[_sentence.next.Length];
            for (var i = 0; i < _outputs.Length; i++)
            {
                _outputs[i] = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, null);
                outputContainer.Add(_outputs[i]);
            }

        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            _sentence.nodePos.x = newPos.xMin;
            _sentence.nodePos.y = newPos.yMin;
        }
    }
}