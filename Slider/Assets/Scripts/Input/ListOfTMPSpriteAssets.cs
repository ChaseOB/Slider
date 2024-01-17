using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[CreateAssetMenu(fileName = "TMPSpriteAssetsList", menuName = "Scriptable Objects/TMPSpriteAssetsList")]
public class ListOfTMPSpriteAssets : ScriptableObject
{
    public List<TMP_SpriteAsset> SpriteAssets;
    public List<Controls.GamePadType> gamePadTypes;

    public TMP_SpriteAsset GetSpriteAsset(Controls.GamePadType gamePad)
    {
        int index = Math.Max(gamePadTypes.IndexOf(gamePad), 0);
        return SpriteAssets[index];
    }
}
