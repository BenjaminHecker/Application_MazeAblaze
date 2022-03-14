using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetCatalogue;

public class StandardBlock : Block
{
    private void Awake()
    {
        type = BlockType.Standard;
    }
}
