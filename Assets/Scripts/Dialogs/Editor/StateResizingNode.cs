using UnityEditor;
using UnityEngine;

namespace DialogEditor
{
    public class StateResizingNode : State
    {
        readonly DialogNode m_node;
        Vector2 m_size;

        public StateResizingNode(Context context, DialogNode node, Vector2 size)
            : base(context)
        {
            m_node = node;
            m_size = node.Bounds.size;
        }

        public override void OnMouseUp(Event e)
        {
            m_context.State = new StateNormal(m_context);
            GUI.changed = true;
        }

        public override void OnMouseDrag(Event e)
        {
            Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Resize Node");
            m_size += e.delta / m_context.Dialog.EditorTransform.Scale;
            Vector2 size = m_size;
            if (size.x < 50)
                size.x = 50;
            if (size.y < 50)
                size.y = 50;
            m_node.Bounds.size = size;
            GUI.changed = true;
        }
    }
}
