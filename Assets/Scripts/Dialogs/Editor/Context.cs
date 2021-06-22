using UnityEditor;
using UnityEngine;

namespace DialogEditor
{
    public class Context
    {
        public Dialog Dialog;
        public DialogNode SelectedNode;
        public State State;

        public Vector2 GetMousePosition(Event e)
        {
            return e.mousePosition.InverseTransformedBy(Dialog.EditorTransform);
        }

        public DialogNode GetNodeAtPosition(Vector2 pos)
        {
            if (Dialog.Nodes == null)
                return null;

            foreach (var node in Dialog.Nodes) {
                if (node.Bounds.Contains(pos))
                    return node;
            }

            return null;
        }

        public void DrawConnection(DialogNode sourceNode, DialogNode targetNode)
        {
            var sourceRect = sourceNode.Bounds.TransformedBy(Dialog.EditorTransform);
            var sourcePoint = new Vector2(sourceRect.center.x, sourceRect.yMax);

            var targetRect = targetNode.Bounds.TransformedBy(Dialog.EditorTransform);
            var targetPoint = new Vector2(targetRect.center.x, targetRect.yMin);

            Handles.color = Color.yellow;
            Handles.DrawLine(sourcePoint, targetPoint, Window.ConnectionLineThickness);
        }
    }
}
