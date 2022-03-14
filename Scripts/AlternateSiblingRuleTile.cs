using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/Tiles/AlternateSiblingRuleTile")]
public class AlternateSiblingRuleTile : RuleTile<AlternateSiblingRuleTile.Neighbor> {
    public int thisGroup;
    public int innerGroup;
    public int outerGroup;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int SiblingInner = 3;
        public const int SiblingOuter = 4;
    }
    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        AlternateSiblingRuleTile ruleTile = tile as AlternateSiblingRuleTile;
        switch (neighbor)
        {
            case TilingRuleOutput.Neighbor.This:
                return tile == this || ruleTile && ruleTile.thisGroup == thisGroup;
            case Neighbor.SiblingInner:
                return ruleTile && ruleTile.thisGroup == innerGroup;
            case Neighbor.SiblingOuter:
                return ruleTile && ruleTile.thisGroup == outerGroup;
        }
        return base.RuleMatch(neighbor, tile);
    }
}