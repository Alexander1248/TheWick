﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Plugins.DialogueSystem.Scripts.DialogueGraph.Attributes;
using Plugins.DialogueSystem.Scripts.DialogueGraph.Nodes;
using Plugins.DialogueSystem.Scripts.DialogueGraph.Nodes.StorylineNodes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;
using IList = System.Collections.IList;

namespace Plugins.DialogueSystem.Editor.DialogueGraph
{
    public class DialogueGraphView : GraphView
    {
        public Action<NodeView> onNodeSelected;
        private Scripts.DialogueGraph.DialogueGraph _graph;
        
        private readonly List<AbstractNode> _sample = new();
        private Vector2 _copyPos; 

        
        public new class UxmlFactory : UxmlFactory<DialogueGraphView, UxmlTraits> { }

        public DialogueGraphView()
        {
            Insert(0, new GridBackground());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator( new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            RegisterCallback<KeyDownEvent>(KeyDown);
            
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/DialogueSystem/DialogueGraphEditor.uss");
            styleSheets.Add(styleSheet);
        }

        private void KeyDown(KeyDownEvent evt)
        {
            var container = ElementAt(1);
            Vector3 screenMousePosition = evt.originalMousePosition;
            Vector2 worldMousePosition = screenMousePosition - container.transform.position;
            worldMousePosition *= 1 / container.transform.scale.x;
            
            switch (evt.keyCode)
            {
                case KeyCode.C:
                    if (evt.ctrlKey) Copy(worldMousePosition);
                    return;
                case KeyCode.V:
                    if (evt.ctrlKey) Paste(worldMousePosition);
                    return;
            }
        }
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var container = ElementAt(1);
            Vector3 screenMousePosition = evt.localMousePosition;
            Vector2 worldMousePosition = screenMousePosition - container.transform.position;
            worldMousePosition *= 1 / container.transform.scale.x;
            
            // base.BuildContextualMenu(evt);
            evt.menu.AppendAction("To Coordinates", _ => container.transform.position = Vector3.zero);
            evt.menu.AppendAction("Update", _ => PopulateView(_graph));
            evt.menu.AppendAction("Copy", _ => Copy(worldMousePosition));
            evt.menu.AppendAction("Paste", _ => Paste(worldMousePosition));
            evt.menu.AppendSeparator();
            
            var types = TypeCache.GetTypesDerivedFrom<AbstractNode>();
            foreach (var type in types.Where(type => !type.IsAbstract))
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                var root = type;
                while (root != null) {
                    if (root == typeof(Storyline))
                    {
                        evt.menu.AppendAction($"Storylines/{type.Name}", 
                            _ => CreateNode(type, worldMousePosition));
                        break;
                    }
                    if (root == typeof(Drawer))
                    {
                        evt.menu.AppendAction($"Drawers/{type.Name}", 
                            _ => CreateNode(type, worldMousePosition));
                        break;
                    }
                    if (root == typeof(TextContainer))
                    {
                        evt.menu.AppendAction($"TextContainers/{type.Name}", 
                            _ => CreateNode(type, worldMousePosition));
                        break;
                    }
                    if (root == typeof(BranchChoicer))
                    {
                        evt.menu.AppendAction($"BranchChoicers/{type.Name}", 
                            _ => CreateNode(type, worldMousePosition));
                        break;
                    }
                    if (root == typeof(Property))
                    {
                        evt.menu.AppendAction($"Properties/{type.Name}", 
                            _ => CreateNode(type, worldMousePosition));
                        break;
                    }
                    if (root == typeof(Value))
                    {
                        evt.menu.AppendAction($"Values/{type.Name}", 
                            _ => CreateNode(type, worldMousePosition));
                        break;
                    }
                    root = root.BaseType;
                }
            }
        }

        private void Paste(Vector2 worldMousePosition)
        {
            var clones = new Dictionary<AbstractNode, AbstractNode>();
            foreach (var node in _sample)
                clones[node] = CreateNodeCopy(node, node.nodePos + worldMousePosition - _copyPos);

            foreach (var node in _sample)
            {
                var clone = clones[node];
                foreach (var field in node.GetType().GetFields())
                {
                    if (!field.HasAttribute(typeof(InputPort))) continue;
                    
                    if (field.FieldType.IsGenericType && field.FieldType.GetInterface(nameof(IList)) != null)
                    {
                        if (field.GetValue(node) is not IList values) continue;
                        var list = (IList) Activator.CreateInstance(field.FieldType);
                        foreach (var value in values)
                            if (value is AbstractNode n)
                                list.Add(n == null ? null : clones.GetValueOrDefault(n, n));
                        field.SetValue(clone, list);
                    }
                    else
                    {
                        var value = field.GetValue(node) as AbstractNode;
                        field.SetValue(clone, value == null ? null : clones.GetValueOrDefault(value, value));
                    }
                }
                    
                if (node is not Storyline storyline) continue;
                var storylineClone = clone as Storyline;
                foreach (var key in storyline.next.Keys)
                    storylineClone!.next[key] = storyline.next[key] == null ? null : 
                        clones.GetValueOrDefault(storyline.next[key], storyline.next[key]) as Storyline;
                
            }
                
            
            UpdateView();
        }

        private void Copy(Vector2 worldMousePosition)
        {
            _sample.Clear();
            _copyPos = worldMousePosition;
            foreach (var selectable in selection)
                if (selectable is NodeView view) 
                    _sample.Add(view.node);
        }

        private void CreateNode(Type type, Vector2 position)
        {
            var node = CreateNode(type);
            node.nodePos = position;
            CreateNodeView(node);
        }
        private AbstractNode CreateNodeCopy(AbstractNode node, Vector2 position)
        {
            var copy = CreateCopy(node);
            copy.nodePos = position;
            CreateNodeView(copy);
            return copy;
        }

        private NodeView FindNodeView(AbstractNode sentence)
        {
            return GetNodeByGuid(sentence.guid) as NodeView;
            
        }

        public void PopulateView(Scripts.DialogueGraph.DialogueGraph graph)
        {
            if (graph == null) throw new Exception("Graph not exists!");
            _graph = graph;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;
            
            _graph.nodes.ForEach(CreateNodeView);
            
            _graph.nodes.ForEach(n =>
            {
                var view = FindNodeView(n);
                switch (n)
                {
                    case Storyline storyline:
                        foreach (var key in storyline.next.Keys.Where(key => storyline.next[key] != null))
                            AddElement(view.Outputs[key].ConnectTo(FindNodeView(storyline.next[key]).Inputs[0]));
                        ConnectCustomPorts(view, n);
                        return;
                    default:
                        ConnectCustomPorts(view, n);
                        return;
                }
            });
            
        }

        private void ConnectCustomPorts(NodeView view, AbstractNode n)
        {
            for (var i = 0; i < view.InputFields.Length; i++)
            {
                var type = view.InputFields[i].FieldType;
                if (type.IsGenericType && type.GetInterface(nameof(IList)) != null)
                {
                    if (view.InputFields[i].GetValue(n) is not IList values) continue;
                    foreach (var value in values)
                        if (value is AbstractNode node) 
                            AddElement(FindNodeView(node).Outputs[0].ConnectTo(view.Inputs[i + view.shift]));
                }
                else
                {
                    var value = view.InputFields[i].GetValue(n) as AbstractNode;
                    if (value) AddElement(FindNodeView(value).Outputs[0].ConnectTo(view.Inputs[i + view.shift]));
                }
            }
        }

        private void UpdateView()
        {
            PopulateView(_graph);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            int index;
            graphViewChange.elementsToRemove?.ForEach(elem =>
            {
                switch (elem)
                {
                    case NodeView nodeView:
                        DeleteNode(nodeView.node);
                        break;
                    case Edge edge:
                    {
                        if (edge.output.node is not NodeView from) return;
                        if (edge.input.node is not NodeView to) return;

                        switch (from.node)
                        {
                            case Storyline fromStoryline:
                                switch (to.node)
                                {
                                    case Storyline toDialogue:
                                        RemoveLink(fromStoryline, toDialogue);
                                        return;
                                    default:
                                        Debug.LogError("To node strange type!");
                                        return;
                                }
                            default:
                                index = -1;
                                for (var i = 0; i < to.Inputs.Length; i++)
                                    if (to.Inputs[i] == edge.input)
                                    {
                                        index = i;
                                        break;
                                    }

                                Remove(from.node, to, index - to.shift);
                                return;
                        }
                    }
                }
            });

            graphViewChange.edgesToCreate?.ForEach(edge =>
            {
                if (edge.output.node is not NodeView from) return;
                if (edge.input.node is not NodeView to) return;

                switch (from.node)
                {
                    case Storyline fromStoryline:
                        switch (to.node)
                        {
                            case Storyline toDialogue:
                                index = -1;
                                for (var i = 0; i < from.Outputs.Length; i++)
                                    if (from.Outputs[i] == edge.output)
                                    {
                                        index = i;
                                        break;
                                    }

                                Assert.IsFalse(index < 0, "WTF with Storyline connection!");
                                AddLink(fromStoryline, index, toDialogue);
                                return;
                            default:
                                Debug.LogError("To node strange type!");
                                return;
                        }
                    default:
                        index = -1;
                        for (var i = 0; i < to.Inputs.Length; i++)
                            if (to.Inputs[i] == edge.input)
                            {
                                index = i;
                                break;
                            }
                        Add(from.node, to, index - to.shift);
                        return;
                }
            });

            return graphViewChange;
        }

        private void CreateNodeView(AbstractNode sentence)
        {
            var nodeView = new NodeView(sentence)
            {
                onNodeSelected = onNodeSelected,
                onGraphViewUpdate = UpdateView
            };
            AddElement(nodeView);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort => 
                endPort.direction != startPort.direction
                && endPort.portType == startPort.portType
                && endPort.node != startPort.node).ToList();
        }
        

        public void Save()
        {
            if (_graph == null) throw new Exception("Graph not exists!");
            EditorUtility.SetDirty(_graph);
            AssetDatabase.SaveAssets();
        }
        

        private AbstractNode CreateNode(Type type)
        {
            if (_graph == null) throw new Exception("Graph not exists!");
            var node = ScriptableObject.CreateInstance(type) as AbstractNode;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            _graph.nodes.Add(node);
            if (node is DialogueRoot r)  _graph.roots.Add(r);
            
            AssetDatabase.AddObjectToAsset(node,  _graph);
            AssetDatabase.SaveAssets();
            
            return node;
        }
        private AbstractNode CreateCopy(AbstractNode node)
        {
            if (_graph == null) throw new Exception("Graph not exists!");
            var clone = node.Clone();
            clone.name = node.name;
            clone.guid = GUID.Generate().ToString();
            _graph.nodes.Add(clone);
            if (clone is DialogueRoot r)  _graph.roots.Add(r);
            
            AssetDatabase.AddObjectToAsset(clone,  _graph);
            AssetDatabase.SaveAssets();
            
            return clone;
        }

        private void DeleteNode(AbstractNode node)
        {
            if (_graph == null) throw new Exception("Graph not exists!");
            if (node is DialogueRoot r) _graph.roots.Remove(r);
            _graph.nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        private static void AddLink(Storyline from, int index, Storyline to)
        {
            from.next[index] = to;
        }

        private static void RemoveLink(Storyline from, Storyline to)
        {
            foreach (var key in from.next.Keys.Where(key => from.next[key] == to))
                from.next[key] = null;
        }
        private static void AddProperty(Storyline node, Property property)
        {
            node.properties.Add(property);
        }
        private static void RemoveProperty(Storyline node, Property property)
        {
            node.properties.Remove(property);
        }
        private static void Add(AbstractNode from, NodeView to, int index)
        {
            if (index < 0 || index >= to.InputFields.Length)
                throw new ArgumentException("Wrong argument index!");
            
            var type = to.InputFields[index].FieldType;
            if (type.IsGenericType && type.GetInterface(nameof(IList)) != null)
            {
                var value = to.InputFields[index].GetValue(to.node) as IList;
                value.Add(from);
                    
            }
            else to.InputFields[index].SetValue(to.node, from);

        }
        private static void Remove(AbstractNode from, NodeView to, int index)
        {
            if (index < 0 || index >= to.InputFields.Length)
                throw new ArgumentException("Wrong argument index!");

            var type = to.InputFields[index].FieldType;
            if (type.IsGenericType && type.GetInterface(nameof(IList)) != null)
            {
                var value = to.InputFields[index].GetValue(to.node) as IList;
                value.Remove(from);
                    
            }
            else to.InputFields[index].SetValue(to.node, null);
        }
        
    }
}