using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetCatalogue;

public class StandardHazard : Hazard
{
    private void Awake()
    {
        type = HazardType.Standard;
    }
}
