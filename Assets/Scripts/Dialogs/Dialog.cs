using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "OTUS/Dialog")]
public class Dialog : ScriptableObject, ISerializationCallbackReceiver
{
    public List<DialogNode> RootNodes { get; private set; }

    [HideInInspector] public int NextUniqueId;
    [HideInInspector] public SimpleTransform2D EditorTransform = new SimpleTransform2D();
    [HideInInspector] public List<DialogNode> Nodes;

    public void OnBeforeSerialize()
    {
        if (Nodes != null) {
            foreach (var node in Nodes) {
                if (node.NextIds != null)
                    node.NextIds.Clear();
                else
                    node.NextIds = new List<int>();

                if (node.Next != null) {
                    foreach (var next in node.Next)
                        node.NextIds.Add(next.UniqueId);
                }
            }
        }
    }

    public void OnAfterDeserialize()
    {
        if (RootNodes == null)
            RootNodes = new List<DialogNode>();
        else
            RootNodes.Clear();

        if (Nodes != null) {
            var dict = new Dictionary<int, DialogNode>();
            var nodesWithoutParent = new HashSet<DialogNode>();
            foreach (var node in Nodes) {
                dict[node.UniqueId] = node;
                nodesWithoutParent.Add(node);
            }

            foreach (var node in Nodes) {
                if (node.Next != null)
                    node.Next.Clear();
                else
                    node.Next = new List<DialogNode>();

                if (node.NextIds != null) {
                    foreach (var nextId in node.NextIds) {
                        var childNode = dict[nextId];
                        node.Next.Add(childNode);
                        nodesWithoutParent.Remove(childNode);
                    }
                }
            }

            foreach (var node in nodesWithoutParent)
                RootNodes.Add(node);
        }
    }
}
