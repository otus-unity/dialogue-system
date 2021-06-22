using UnityEditor;
using UnityEngine;

namespace DialogEditor
{
    public class StateDraggingNode : State
    {
        readonly DialogNode m_node;

        public StateDraggingNode(Context context, DialogNode node)
            : base(context)
        {
            m_node = node;
        }

        public override void OnMouseUp(Event e)
        {
            m_context.State = new StateNormal(m_context);
            GUI.changed = true;
        }

        public override void OnMouseDrag(Event e)
        {
            Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Drag Node");
            m_node.Bounds = new Rect(
                m_node.Bounds.min + e.delta / m_context.Dialog.EditorTransform.Scale,
                m_node.Bounds.size);
            GUI.changed = true;
        }
    }
}
