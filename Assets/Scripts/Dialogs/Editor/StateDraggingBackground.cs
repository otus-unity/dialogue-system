using UnityEditor;
using UnityEngine;

namespace DialogEditor
{
    public class StateDraggingBackground : State
    {
        public StateDraggingBackground(Context context)
            : base(context)
        {
        }

        public override void OnMouseUp(Event e)
        {
            m_context.State = new StateNormal(m_context);
            GUI.changed = true;
        }

        public override void OnMouseDrag(Event e)
        {
            m_context.Dialog.EditorTransform.Offset += e.delta / m_context.Dialog.EditorTransform.Scale;
            EditorUtility.SetDirty(m_context.Dialog);
            GUI.changed = true;
        }
    }
}
