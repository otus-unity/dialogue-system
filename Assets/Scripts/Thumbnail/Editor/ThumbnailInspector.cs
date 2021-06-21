using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(Thumbnail))]
[CanEditMultipleObjects]
public sealed class ThumbnailEditor : UnityEditor.Editor
{
    static readonly int PreviewControlID = "IconPreview".GetHashCode();

    SerializedProperty prefabProperty;
    SerializedProperty iconProperty;
    SerializedProperty iconSizeProperty;
    SerializedProperty iconPreviewBrightnessProperty;
    SerializedProperty iconPreviewOffsetProperty;
    SerializedProperty iconPreviewAnglesProperty;
    SerializedProperty iconPreviewPositionProperty;
    SerializedProperty iconPreviewRotationProperty;
    SerializedProperty iconPreviewScaleProperty;
    SerializedProperty iconBackgroundColorProperty;

    Texture2D backgroundTexture;
    GUIStyle guiStyle = new GUIStyle();

    GameObject previewGameObject;
    UnityEngine.Object currentPreviewPrefab;
    PreviewRenderUtility previewRenderUtility;
    int currentIconSize;
    float currentPreviewBrightness;
    Vector3 currentPreviewOffset;
    Vector3 currentPreviewAngles;
    Vector3 currentPreviewPosition;
    Vector3 currentPreviewRotation;
    Vector3 currentPreviewScale;
    Color currentBackgroundColor;
    bool shouldRefresh = true;

    void OnEnable()
    {
        prefabProperty = serializedObject.FindProperty("Prefab");
        iconProperty = serializedObject.FindProperty("Icon");
        iconSizeProperty = serializedObject.FindProperty("iconSize");
        iconPreviewBrightnessProperty = serializedObject.FindProperty("iconPreviewBrightness");
        iconPreviewOffsetProperty = serializedObject.FindProperty("iconPreviewOffset");
        iconPreviewAnglesProperty = serializedObject.FindProperty("iconPreviewAngles");
        iconPreviewPositionProperty = serializedObject.FindProperty("iconPreviewPosition");
        iconPreviewRotationProperty = serializedObject.FindProperty("iconPreviewRotation");
        iconPreviewScaleProperty = serializedObject.FindProperty("iconPreviewScale");
        iconBackgroundColorProperty = serializedObject.FindProperty("iconBackgroundColor");

        backgroundTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        backgroundTexture.SetPixel(0,0, Color.white);
        backgroundTexture.Apply();
        guiStyle.normal.background = backgroundTexture;
    }

    void OnDisable()
    {
        if (backgroundTexture != null) {
            DestroyImmediate(backgroundTexture);
            backgroundTexture = null;
        }

        DestroyPreview();
    }

    void DestroyPreview()
    {
        currentPreviewPrefab = null;

        if (previewRenderUtility != null) {
            previewRenderUtility.Cleanup();
            previewRenderUtility = null;
        }

        if (previewGameObject != null) {
            DestroyImmediate(previewGameObject);
            previewGameObject = null;
        }
    }

    bool ShouldRefreshPreviewObject()
    {
        return currentPreviewPrefab != prefabProperty.objectReferenceValue;
    }

    GameObject CreatePreviewObject()
    {
        GameObject go = null;
        currentPreviewPrefab = prefabProperty.objectReferenceValue;
        if (currentPreviewPrefab != null)
            go = (GameObject)Instantiate(currentPreviewPrefab);
        return go;
    }

    void DrawPreview(bool save = false)
    {
        int iconSize = iconSizeProperty.intValue;
        float iconPreviewBrightness = iconPreviewBrightnessProperty.floatValue;
        Vector3 iconPreviewOffset = iconPreviewOffsetProperty.vector3Value;
        Vector3 iconPreviewAngles = iconPreviewAnglesProperty.vector3Value;
        Vector3 iconPreviewPosition = iconPreviewPositionProperty.vector3Value;
        Vector3 iconPreviewRotation = iconPreviewRotationProperty.vector3Value;
        Vector3 iconPreviewScale = iconPreviewScaleProperty.vector3Value;
        Color iconBackgroundColor = iconBackgroundColorProperty.colorValue;

        if (shouldRefresh
                || currentIconSize != iconSize
                || currentPreviewBrightness != iconPreviewBrightness
                || currentPreviewOffset != iconPreviewOffset
                || currentPreviewAngles != iconPreviewAngles
                || currentPreviewPosition != iconPreviewPosition
                || currentPreviewRotation != iconPreviewRotation
                || currentPreviewScale != iconPreviewScale
                || currentBackgroundColor != iconBackgroundColor
                || save
                || ShouldRefreshPreviewObject()) {
            DestroyPreview();

            shouldRefresh = false;
            currentIconSize = iconSize;
            currentPreviewBrightness = iconPreviewBrightness;
            currentPreviewOffset = iconPreviewOffset;
            currentPreviewAngles = iconPreviewAngles;
            currentPreviewPosition = iconPreviewPosition;
            currentPreviewRotation = iconPreviewRotation;
            currentPreviewScale = iconPreviewScale;
            currentBackgroundColor = iconBackgroundColor;

            previewGameObject = CreatePreviewObject();
            if (previewGameObject != null)
                previewGameObject.hideFlags = HideFlags.HideAndDontSave;

            backgroundTexture.SetPixel(0, 0, iconBackgroundColor);
            backgroundTexture.Apply();
        }

        if (previewGameObject == null && !save)
            return;

        if (previewRenderUtility == null) {
            previewRenderUtility = new PreviewRenderUtility();
            previewRenderUtility.camera.fieldOfView = 30.0f;
            previewRenderUtility.camera.allowHDR = false;
            previewRenderUtility.camera.allowMSAA = false;
            previewRenderUtility.ambientColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            previewRenderUtility.lights[0].intensity = iconPreviewBrightness;
            previewRenderUtility.lights[0].transform.position = iconPreviewOffset;
            previewRenderUtility.lights[0].transform.eulerAngles = iconPreviewAngles;
            previewRenderUtility.lights[1].intensity = iconPreviewBrightness;
            previewRenderUtility.camera.transform.position = iconPreviewOffset;
            previewRenderUtility.camera.transform.eulerAngles = iconPreviewAngles;
            previewRenderUtility.camera.nearClipPlane = 0.1f;
            previewRenderUtility.camera.farClipPlane = 10.0f;
            previewRenderUtility.AddSingleGO(previewGameObject);
        }

        previewGameObject.transform.position = iconPreviewPosition;
        previewGameObject.transform.eulerAngles = iconPreviewRotation;
        previewGameObject.transform.localScale = iconPreviewScale;

        Rect previewRect = EditorGUILayout.GetControlRect(false, 200.0f);
        int previewID = GUIUtility.GetControlID(PreviewControlID, FocusType.Passive, previewRect);

        Event e = Event.current;
        EventType eventType = e.GetTypeForControl(previewID);
        bool repaint = (eventType == EventType.Repaint);

        if (repaint || save) {
            previewRenderUtility.BeginPreview(new Rect(0, 0, iconSize, iconSize), guiStyle);
            previewRenderUtility.Render(false);
            Texture texture = previewRenderUtility.EndPreview();

            if (save) {
                UnityEngine.Object obj = serializedObject.targetObject;
                string path = Path.ChangeExtension(AssetDatabase.GetAssetPath(obj), ".png");

                var rt = (RenderTexture)texture;
                int width = rt.width;
                int height = rt.height;

                RenderTexture.active = rt;
                var tex2D = new Texture2D(width, height, TextureFormat.RGB24, false);
                tex2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                File.WriteAllBytes(path, tex2D.EncodeToPNG());
                DestroyImmediate(tex2D);
                RenderTexture.active = null;

                AssetDatabase.Refresh();

                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite;
                importer.sRGBTexture = false;
                importer.alphaIsTransparency = true;
                importer.alphaSource = TextureImporterAlphaSource.None;
                importer.spritePixelsPerUnit = 100.0f;
                importer.mipmapEnabled = false;
                importer.maxTextureSize = iconSize;
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();

                iconProperty.objectReferenceValue = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
            }

            if (repaint)
                GUI.DrawTexture(previewRect, texture, ScaleMode.ScaleToFit, false);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(prefabProperty);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(iconProperty);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(iconSizeProperty);
        EditorGUILayout.PropertyField(iconPreviewBrightnessProperty);
        EditorGUILayout.PropertyField(iconPreviewOffsetProperty);
        EditorGUILayout.PropertyField(iconPreviewAnglesProperty);
        EditorGUILayout.PropertyField(iconPreviewPositionProperty);
        EditorGUILayout.PropertyField(iconPreviewRotationProperty);
        EditorGUILayout.PropertyField(iconPreviewScaleProperty);

        float oldScale = iconPreviewScaleProperty.vector3Value.x;
        float newScale = EditorGUILayout.FloatField("Icon Preview Uniform Scale", oldScale);
        if (!Mathf.Approximately(oldScale, newScale))
            iconPreviewScaleProperty.vector3Value = new Vector3(newScale, newScale, newScale);

        EditorGUILayout.PropertyField(iconBackgroundColorProperty);
        EditorGUILayout.Space();

        if (serializedObject.isEditingMultipleObjects) {
            EditorGUILayout.HelpBox("Preview is not available when editing multiple objects.",
                MessageType.Warning);
        } else {
            bool save = GUILayout.Button("Save");
            DrawPreview(save);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
