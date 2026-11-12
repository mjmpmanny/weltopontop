using System.Collections.Generic;
using UnityEngine;

public enum ObjectCategory
{
    Item,
    PowerUp,
    Buildable
}

[CreateAssetMenu(
    fileName = "NewObjectDefinition",
    menuName = "WELTOP/Objects/Object Definition")]
public class ObjectDefinition : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private ObjectCategory category;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject previewPrefab;
    [SerializeField] private ObjectEffectDefinition[] effects =
        new ObjectEffectDefinition[0];

    public string DisplayName => displayName;
    public ObjectCategory Category => category;
    public GameObject Prefab => prefab;
    public GameObject PreviewPrefab => previewPrefab != null ? previewPrefab : prefab;
    public IReadOnlyList<ObjectEffectDefinition> Effects => effects;
}