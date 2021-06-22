using UnityEngine;

namespace DialogEditor
{
    public abstract class State
    {
        protected readonly Context m_context;

        protected State(Context context)
        {
            m_context = context;
        }

        public virtual void Paint() {}
        public virtual void OnLeftMouseDown(Event e) {}
        public virtual void OnMiddleMouseDown(Event e) {}
        public virtual void OnRightMouseDown(Event e) {}
        public virtual void OnMouseUp(Event e) {}
        public virtual void OnMouseMove(Event e) {}
        public virtual void OnMouseDrag(Event e) {}
        public virtual void OnMouseWheel(Event e) {}
    }
}
