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
    
    [Header("Object Selection")]
    [Tooltip("Higher weights appear more often. Zero excludes this object from offers.")]
    [SerializeField, Min(0f)] private float selectionWeight = 1f;
    
    [SerializeField] private ObjectEffectDefinition[] effects =
        new ObjectEffectDefinition[0];

    public string DisplayName => displayName;
    public ObjectCategory Category => category;
    public GameObject Prefab => prefab;
    public GameObject PreviewPrefab => previewPrefab != null ? previewPrefab : prefab;
    public float SelectionWeight => selectionWeight;
    public IReadOnlyList<ObjectEffectDefinition> Effects => effects;
}