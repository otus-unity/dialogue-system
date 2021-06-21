using UnityEngine;

[CreateAssetMenu(menuName = "OTUS/Thumbnail")]
public sealed class Thumbnail : ScriptableObject
{
    public GameObject Prefab;
    public Sprite Icon;

    // Для использования в редакторе
    #pragma warning disable CS0414
    [SerializeField] int iconSize = 512;
    [SerializeField] float iconPreviewBrightness = 1.0f;
    [SerializeField] Vector3 iconPreviewOffset = new Vector3(0.0f, 0.0f, -2.0f);
    [SerializeField] Vector3 iconPreviewAngles = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] Vector3 iconPreviewPosition = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] Vector3 iconPreviewRotation = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] Vector3 iconPreviewScale = new Vector3(1.0f, 1.0f, 1.0f);
    [SerializeField] Color iconBackgroundColor = new Color(0.2f, 0.2f, 0.2f);
    #pragma warning restore CS0414
}
