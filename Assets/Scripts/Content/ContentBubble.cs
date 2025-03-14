using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ContentBubble
{
    public static float BubbleSpeedLeast = 0.1f;
    public static float BubbleSpeedMost = 0.5f;

    public static float BubbleSizeLeast = 0.1f;
    public static float BubbleSizeMost = 0.5f;


    public static Color BubbleColorRed = Color.red;
    public static Color BubbleColorYellow = new Color(1, 0.92f, 0.016f, 0.5f);
    public static Color BubbleColorOrange = new Color(1, 0.5f, 0);
    public static Color BubbleColorGreen = Color.green;
    public static Color BubbleColorNone = Color.white;

    public static float YellowBubbleTime = 4f;

    public static float BubbleClosestDistance = 0.8f;

    public static string BubblePrefab = "Prefabs/Bubble/";

    public static string BubbleFloatPrefab = "Float/";
    public static string BubbleCombinePrefab = "Combine/";

    public static string BubbleFloatRed = "RedFloat";
    public static string BubbleFloatYellow = "YellowFloat";
    public static string BubbleFloatOrange = "OrangeFloat";
    public static string BubbleFloatGreen = "GreenFloat";
    public static string BubbleFloatNone = "NoneFloat";

    public static string BubbleCombineRed = "RedCombine";
    public static string BubbleCombineYellow = "YellowCombine";
    public static string BubbleCombineOrange = "OrangeCombine";
    public static string BubbleCombineGreen = "GreenCombine";
    public static string BubbleCombineNone = "NoneCombine";
}
