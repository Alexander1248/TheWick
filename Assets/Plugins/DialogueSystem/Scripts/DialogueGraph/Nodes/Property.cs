
using Plugins.DialogueSystem.Scripts.DialogueGraph.Attributes;
using UnityEngine;

namespace Plugins.DialogueSystem.Scripts.DialogueGraph.Nodes
{
    [OutputPort(typeof(Property),"Property")]
    public abstract class Property : AbstractNode
    {
        static Property()
        {
            NodeColors.Colors.Add(typeof(Property), new Color(1, 1, 1));
        }
        public abstract void OnDrawStart(Dialogue dialogue, Storyline node);
        public abstract void OnDrawEnd(Dialogue dialogue, Storyline storyline);
        public abstract void OnDelayStart(Dialogue dialogue, Storyline storyline);
        public abstract void OnDelayEnd(Dialogue dialogue, Storyline storyline);
    }
}