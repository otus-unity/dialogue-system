using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Collections.Generic;

namespace DialogEditor
{
    public sealed class Window : EditorWindow
    {
        public const float ConnectionLineThickness = 2.0f;
        public static readonly Vector2 DragHandleSize = new Vector2(10.0f, 10.0f);

        readonly Context m_context = new Context();

        Texture2D m_nodeTexture;
        Texture2D m_dragHandleTexture;
        GUIStyle m_nodeStyle;
        GUIStyle m_dragHandleStyle;

        void OnGUI()
        {
            if (Application.isPlaying) {
                CenterMessage("Dialog editor is not available in play mode.");
                return;
            }
            if (m_context.Dialog == null) {
                CenterMessage("No dialog selected.");
                return;
            }

            HandleEvents();

            DrawGrid(m_context.Dialog.EditorTransform, 20.0f, Color.gray.WithAlpha(0.3f));
            DrawGrid(m_context.Dialog.EditorTransform, 100.0f, Color.gray.WithAlpha(0.4f));

            DrawNodes();

            foreach (var node in m_context.Dialog.Nodes) {
                if (node.Next != null) {
                    foreach (var next in node.Next)
                        m_context.DrawConnection(node, next);
                }
            }

            m_context.State.Paint();

            if (GUI.changed)
                Repaint();
        }

        void CenterMessage(string message)
        {
            var text = new GUIContent(message);
            var textSize = GUI.skin.label.CalcSize(text);
            EditorGUI.LabelField(new Rect((position.size - textSize) * 0.5f, textSize), text);
        }

        void DrawGrid(SimpleTransform2D transform, float spacing, Color color)
        {
            spacing *= transform.Scale;

            float w = position.width;
            float h = position.height;
            int nx = Mathf.CeilToInt(w / spacing);
            int ny = Mathf.CeilToInt(h / spacing);

            var off = new Vector2(transform.Offset.x % spacing, transform.Offset.y % spacing);

            Handles.BeginGUI();
            Handles.color = color;

            for (int i = 0; i <= nx; i++) {
                var s = off + new Vector2(spacing * i, -spacing    );
                var e = off + new Vector2(spacing * i,  spacing + h);
                Handles.DrawLine(s, e);
            }
            for (int i = 0; i <= ny; i++) {
                var s = off + new Vector2(-spacing    , spacing * i);
                var e = off + new Vector2( spacing + w, spacing * i);
                Handles.DrawLine(s, e);
            }

            Handles.EndGUI();
        }

        void DrawNodes()
        {
            if (m_context.Dialog.Nodes == null)
                return;

            foreach (var node in m_context.Dialog.Nodes) {
                var r = node.Bounds.TransformedBy(m_context.Dialog.EditorTransform);
                GUI.Box(r, "", m_nodeStyle);

                Rect rect = r;
                rect.xMin += 10;
                rect.xMax -= 10;
                rect.yMin += 10;
                rect.yMax = rect.yMin + EditorGUIUtility.singleLineHeight;

                var oldText = node.Text;
                string newText = EditorGUI.TextField(rect, oldText);
                if (newText != oldText) {
                    Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Edit node text");
                    node.Text = newText;
                    EditorUtility.SetDirty(m_context.Dialog);
                }

                rect.y += EditorGUIUtility.singleLineHeight + 10;

                DialogNode.Action oldAction = node.NodeAction;
                DialogNode.Action newAction = (DialogNode.Action)EditorGUI.EnumPopup(rect, oldAction);
                if (oldAction != newAction) {
                    Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Change node action");
                    node.NodeAction = newAction;
                    EditorUtility.SetDirty(m_context.Dialog);
                }

                switch (newAction) {
                    case DialogNode.Action.Default:
                        break;

                    case DialogNode.Action.StartQuest: {
                        rect.y += EditorGUIUtility.singleLineHeight + 10;

                        Quest oldQuest = node.ActionQuest;
                        Quest newQuest = (Quest)EditorGUI.ObjectField(rect, oldQuest, typeof(Quest), false);
                        if (oldQuest != newQuest) {
                            Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Change action quest");
                            node.ActionQuest = newQuest;
                        }

                        break;
                    }
                }

                rect.y += EditorGUIUtility.singleLineHeight + 10;

                DialogNode.Condition oldCondition = node.NodeCondition;
                DialogNode.Condition newCondition = (DialogNode.Condition)EditorGUI.EnumPopup(rect, oldCondition);
                if (oldCondition != newCondition) {
                    Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Change node condition");
                    node.NodeCondition = newCondition;
                    EditorUtility.SetDirty(m_context.Dialog);
                }

                switch (newCondition) {
                    case DialogNode.Condition.None:
                        break;

                    case DialogNode.Condition.QuestCompleted: {
                        rect.y += EditorGUIUtility.singleLineHeight + 10;

                        Quest oldQuest = node.ConditionQuest;
                        Quest newQuest = (Quest)EditorGUI.ObjectField(rect, oldQuest, typeof(Quest), false);
                        if (oldQuest != newQuest) {
                            Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Change condition quest");
                            node.ConditionQuest = newQuest;
                            EditorUtility.SetDirty(m_context.Dialog);
                        }

                        break;
                    }
                }

                rect.y += EditorGUIUtility.singleLineHeight + 10;

                bool oldIsPlayer = node.IsPlayer;
                bool newIsPlayer = EditorGUI.ToggleLeft(rect, "Is player", oldIsPlayer);
                if (oldIsPlayer != newIsPlayer) {
                    Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Toggle is player");
                    node.IsPlayer = newIsPlayer;
                    EditorUtility.SetDirty(m_context.Dialog);
                }

                rect.y += EditorGUIUtility.singleLineHeight + 10;

                bool oldAllowReuse = node.AllowReuse;
                bool newAllowReuse = EditorGUI.ToggleLeft(rect, "Allow reuse", oldAllowReuse);
                if (oldAllowReuse != newAllowReuse) {
                    Undo.RegisterCompleteObjectUndo(m_context.Dialog, "Toggle allow reuse");
                    node.AllowReuse = newAllowReuse;
                    EditorUtility.SetDirty(m_context.Dialog);
                }

                r = new Rect(node.Bounds.max - DragHandleSize, DragHandleSize)
                    .TransformedBy(m_context.Dialog.EditorTransform);
                GUI.Box(r, "", m_dragHandleStyle);
            }
        }

        void HandleEvents()
        {
            var e = Event.current;

            switch (e.type) {
                case EventType.MouseDown:
                    switch (e.button) {
                        case 0: m_context.State.OnLeftMouseDown(e); break;
                        case 1: m_context.State.OnRightMouseDown(e); break;
                        case 2: m_context.State.OnMiddleMouseDown(e); break;
                    }
                    break;

                case EventType.MouseUp: m_context.State.OnMouseUp(e); break;
                case EventType.MouseMove: m_context.State.OnMouseMove(e); break;
                case EventType.MouseDrag: m_context.State.OnMouseDrag(e); break;
                case EventType.ScrollWheel: m_context.State.OnMouseWheel(e); break;
            }
        }

        public void LoadAsset(UnityEngine.Object asset)
        {
            m_context.Dialog = asset as Dialog;
            UpdateTitle();
            Repaint();
        }

        void UpdateTitle()
        {
            if (m_context.Dialog == null)
                titleContent = new GUIContent("<Dialog Not Open>");
            else
                titleContent = new GUIContent(((UnityEngine.Object)m_context.Dialog).name);
        }

        void OnEnable()
        {
            m_context.State = new StateNormal(m_context);
            UpdateTitle();

            m_nodeStyle = new GUIStyle();
            m_nodeTexture = TextureUtility.CreateSolidColorTexture(new Color(0.3f, 0.3f, 0.3f));
            m_nodeStyle.normal.background = m_nodeTexture;

            var f = Color.black;
            var _ = new Color(0.3f, 0.3f, 0.3f);
            m_dragHandleTexture = TextureUtility.CreateTexture(10, 10, new Color[] {
                    _,_,_,_,_,_,_,_,_,_,
                    _,_,_,_,_,_,_,_,_,_,
                    _,_,f,f,f,f,f,_,_,_,
                    _,_,f,f,f,f,_,_,_,_,
                    _,_,f,f,f,_,_,_,f,_,
                    _,_,f,f,_,_,_,f,f,_,
                    _,_,f,_,_,_,f,f,f,_,
                    _,_,_,_,_,f,f,f,f,_,
                    _,_,_,_,f,f,f,f,f,_,
                    _,_,_,_,_,_,_,_,_,_,
                });

            m_dragHandleStyle = new GUIStyle();
            m_dragHandleStyle.normal.background = m_dragHandleTexture;

            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            wantsMouseMove = true;
        }

        void OnDisable()
        {
            m_context.State = null;

            DestroyImmediate(m_nodeTexture);
            DestroyImmediate(m_dragHandleTexture);

            m_dragHandleStyle = null;
            m_dragHandleTexture = null;
            m_nodeStyle = null;
            m_nodeTexture = null;

            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
        }

        void OnUndoRedoPerformed()
        {
            Repaint();
        }

        [OnOpenAsset(1)]
        static bool OpenDialog(int assetInstanceID, int line)
        {
            var dialog = EditorUtility.InstanceIDToObject(assetInstanceID) as Dialog;
            if (dialog == null)
                return false;

            GetWindow<Window>().LoadAsset(dialog);
            return true;
        }
    }
}
