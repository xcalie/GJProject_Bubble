using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CheckColor
{
    public static Color CheckNowColor(BubbleType type)
    {
        switch (type)
        {
            case BubbleType.Red:
                return ContentBubble.BubbleColorRed;
            case BubbleType.Yellow:
                return ContentBubble.BubbleColorYellow;
            case BubbleType.Orange:
                return ContentBubble.BubbleColorOrange;
            case BubbleType.Green:
                return ContentBubble.BubbleColorGreen;
            default:
                return ContentBubble.BubbleColorNone;
        }
    }

    public static string CheckCombineType(BubbleType type)
    {
        switch (type)
        {
            case BubbleType.Red:
                return ContentBubble.BubblePrefab + ContentBubble.BubbleCombinePrefab + ContentBubble.BubbleCombineRed;
            case BubbleType.Yellow:
                return ContentBubble.BubblePrefab + ContentBubble.BubbleCombinePrefab + ContentBubble.BubbleCombineYellow;
            case BubbleType.Orange:
                return ContentBubble.BubblePrefab + ContentBubble.BubbleCombinePrefab + ContentBubble.BubbleCombineOrange;
            case BubbleType.Green:
                return ContentBubble.BubblePrefab + ContentBubble.BubbleCombinePrefab + ContentBubble.BubbleCombineGreen;
            default:
                return ContentBubble.BubblePrefab + ContentBubble.BubbleCombinePrefab + ContentBubble.BubbleCombineNone;
        }
    }

    public static string CheckFloatType(BubbleType type)
    {
        switch (type)
        {
            case BubbleType.Red:
                return ContentBubble.BubblePrefab + ContentBubble.BubbleFloatPrefab + ContentBubble.BubbleFloatRed;
            case BubbleType.Yellow:
                return ContentBubble.BubblePrefab + ContentBubble.BubbleFloatPrefab + ContentBubble.BubbleFloatYellow;
            case BubbleType.Orange:
                return ContentBubble.BubblePrefab + ContentBubble.BubbleFloatPrefab + ContentBubble.BubbleFloatOrange;
            case BubbleType.Green:
                return ContentBubble.BubblePrefab + ContentBubble.BubbleFloatPrefab + ContentBubble.BubbleFloatGreen;
            default:
                return ContentBubble.BubblePrefab + ContentBubble.BubbleFloatPrefab + ContentBubble.BubbleFloatNone;
        }
    }
}
