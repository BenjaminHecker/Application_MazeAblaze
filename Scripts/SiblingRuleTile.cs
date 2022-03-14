using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/Tiles/SiblingRuleTile")]
public class SiblingRuleTile : RuleTile<SiblingRuleTile.Neighbor>
{
    public int siblingGroup;

    public class Neighbor : RuleTile.TilingRule.Neighbor { }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        SiblingRuleTile ruleTile = tile as SiblingRuleTile;
        switch (neighbor)
        {
            case TilingRuleOutput.Neighbor.This:
                return ruleTile != null && ruleTile.siblingGroup == siblingGroup;
            case TilingRuleOutput.Neighbor.NotThis:
                return ruleTile == null || ruleTile.siblingGroup != siblingGroup;
        }
        return base.RuleMatch(neighbor, tile);
    }
}