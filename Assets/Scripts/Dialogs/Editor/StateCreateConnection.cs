using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DialogEditor
{
    public class StateCreateConnection : State
    {
        public StateCreateConnection(Context context)
            : base(context)
        {
        }

        public override void Paint()
        {
            Vector2 mousePos = m_context.GetMousePosition(Event.current);
            DialogNode targetNode = m_context.GetNodeAtPosition(mousePos);

            if (targetNode != null && m_context.SelectedNode.CanConnectTo(targetNode))
                m_context.DrawConnection(m_context.SelectedNode, targetNode);
            else {
                var fromRect = m_context.SelectedNode.Bounds.TransformedBy(m_context.Dialog.EditorTransform);
                var sourcePoint = new Vector2(fromRect.center.x, fromRect.yMax);
                Handles.color = Color.yellow;
                Handles.DrawLine(sourcePoint, Event.current.mousePosition, Window.ConnectionLineThickness);
            }
        }

        public override void OnLeftMouseDown(Event e)
        {
            m_context.State = new StateNormal(m_context);

            Vector2 mousePos = m_context.GetMousePosition(e);
            DialogNode overNode = m_context.GetNodeAtPosition(mousePos);

            if (overNode != null && m_context.SelectedNode.CanConnectTo(overNode)) {
                Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Create connection");
                if (m_context.SelectedNode.Next == null)
                    m_context.SelectedNode.Next = new List<DialogNode>();
                m_context.SelectedNode.Next.Add(overNode);
            }

            GUI.changed = true;
        }

        public override void OnMouseMove(Event e)
        {
            GUI.changed = true;
        }

        public override void OnMouseDrag(Event e)
        {
            GUI.changed = true;
        }
    }
}
