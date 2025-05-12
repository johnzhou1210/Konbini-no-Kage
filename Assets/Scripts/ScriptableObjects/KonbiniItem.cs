using UnityEngine;

[CreateAssetMenu(fileName = "NewKonbiniItem", menuName = "Konbini/Item")]
public class KonbiniItem : ScriptableObject
{
    public string itemName;
    public int price;
    public Material itemMaterial;
}