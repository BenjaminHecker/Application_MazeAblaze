using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AssetCatalogue;

public class MazePaletteItem : MonoBehaviour
{
    public enum ItemType
    {
        Flame,
        Target,
        Block,
        Ground,
        Hazard
    }

    [SerializeField] private ItemType item;

    [SerializeField] private BlockType block;
    [SerializeField] private GroundType ground;
    [SerializeField] private HazardType hazard;

    public ItemType Item { get { return item; } }
    public BlockType Block { get { return block; } }
    public GroundType Ground { get { return ground; } }
    public HazardType Hazard { get { return hazard; } }

    private void OnMouseDown()
    {
        MazeEditor.SetItem(this);
    }

    public void SetSelected(bool isSelected)
    {
        Image img = transform.Find("Item Background").GetComponent<Image>();

        if (isSelected)
        {
            img.color = new Color(1, 1, 1, (float) 100 / 255);
        }
        else
        {
            img.color = new Color(1, 1, 1, (float) 20 / 255);
        }
    }

    public void UpdateSprite()
    {
        if (item == ItemType.Block)
            transform.Find("Item Sprite").GetComponent<Image>().sprite = GetBlockSprite(block);
        else if (item == ItemType.Ground)
            transform.Find("Item Sprite").GetComponent<Image>().sprite = GetGroundSprite(ground);
        else if (item == ItemType.Hazard)
            transform.Find("Item Sprite").GetComponent<Image>().sprite = GetHazardSprite(hazard);
    }

    public Sprite GetSprite()
    {
        return transform.Find("Item Sprite").GetComponent<Image>().sprite;
    }
}
