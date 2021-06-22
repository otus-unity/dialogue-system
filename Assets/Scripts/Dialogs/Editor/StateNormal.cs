using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DialogEditor
{
    public class StateNormal : State
    {
        Vector2 m_nodeCreatePosition;
        int m_nodeIndex;
        int m_nextIndex;

        public StateNormal(Context context)
            : base(context)
        {
        }

        public override void OnLeftMouseDown(Event e)
        {
            if (m_context.Dialog.Nodes == null)
                return;

            Vector2 mouse = m_context.GetMousePosition(e);
            DialogNode node = m_context.GetNodeAtPosition(mouse);
            if (node == null)
                return;

            var r = new Rect(node.Bounds.max - Window.DragHandleSize, Window.DragHandleSize);
            if (!r.Contains(mouse))
                m_context.State = new StateDraggingNode(m_context, node);
            else
                m_context.State = new StateResizingNode(m_context, node, node.Bounds.size);

            GUI.changed = true;
        }

        public override void OnMiddleMouseDown(Event e)
        {
            m_context.State = new StateDraggingBackground(m_context);
            GUI.changed = true;
        }

        public override void OnRightMouseDown(Event e)
        {
            GenericMenu menu;

            m_context.State = new StateNormal(m_context);

            Vector2 mouse = m_context.GetMousePosition(e);
            DialogNode overNode = m_context.GetNodeAtPosition(mouse);

            if (overNode != null) {
                m_context.SelectedNode = overNode;
                menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create connection"), false, CreateConnection);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete node"), false, DeleteNode);
                menu.ShowAsContext();
                return;
            }

            if (m_context.Dialog.Nodes != null) {
                m_nodeIndex = 0;
                foreach (var node in m_context.Dialog.Nodes) {
                    if (node.Next != null) {
                        Vector2 sourcePoint = new Vector2(node.Bounds.center.x, node.Bounds.yMax);
                        m_nextIndex = 0;
                        foreach (var next in node.Next) {
                            var targetRect = next.Bounds;
                            var targetPoint = new Vector2(targetRect.center.x, targetRect.yMin);

                            if (HandleUtility.DistancePointLine(mouse, sourcePoint, targetPoint) < 5.0f) {
                                menu = new GenericMenu();
                                menu.AddItem(new GUIContent("Delete connection"), false, DeleteConnection);
                                menu.ShowAsContext();
                                return;
                            }

                            ++m_nextIndex;
                        }
                    }
                    ++m_nodeIndex;
                }
            }

            m_nodeCreatePosition = mouse;
            menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create node"), false, CreateNode);
            menu.ShowAsContext();
        }

        public override void OnMouseWheel(Event e)
        {
            m_context.Dialog.EditorTransform.AdjustScale(e.delta.y * 0.05f, m_context.GetMousePosition(e));
            EditorUtility.SetDirty(m_context.Dialog);
            GUI.changed = true;
        }

        void CreateNode()
        {
            var size = new Vector2(200, 600);

            Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Create node");

            var node = new DialogNode();
            node.UniqueId = m_context.Dialog.NextUniqueId++;
            node.Bounds = new Rect(m_nodeCreatePosition - size * 0.5f, size);
            node.Next = new List<DialogNode>();
            node.NextIds = new List<int>();

            if (m_context.Dialog.Nodes == null)
                m_context.Dialog.Nodes = new List<DialogNode>();

            m_context.Dialog.Nodes.Add(node);
        }

        void DeleteNode()
        {
            Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Delete node");

            int outerIndex = m_context.Dialog.Nodes.Count;
            while (outerIndex-- > 0) {
                var node = m_context.Dialog.Nodes[outerIndex];
                if (node == m_context.SelectedNode) {
                    m_context.Dialog.Nodes.RemoveAt(outerIndex);
                    continue;
                }

                if (node.Next != null) {
                    int innerIndex = node.Next.Count;
                    while (innerIndex-- > 0) {
                        if (node.Next[innerIndex] == m_context.SelectedNode)
                            node.Next.RemoveAt(innerIndex);
                    }
                }
            }
        }

        void CreateConnection()
        {
            m_context.State = new StateCreateConnection(m_context);
        }

        void DeleteConnection()
        {
            Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Delete connection");
            m_context.Dialog.Nodes[m_nodeIndex].Next.RemoveAt(m_nextIndex);
        }
    }
}
