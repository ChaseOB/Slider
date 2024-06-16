using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactTBPluginLaser : ArtifactTBPlugin
{
    public GameObject[] sprites; //0 = East, 1 = north, 2 = west, 3 = south
    public GameObject[] emptysprites; //0 = East, 1 = north, 2 = west, 3 = south
    public bool[] edgeblockers;

    public LaserCenterObject centerObject;
    public ArtifactTBPluginLaser otherPortal;
    public MagiLaser laser;
    public int sourceDir;
    public int islandId;
    public int originalIslandId;
    
    public List<LaserableRockUIData> rockdata;
    public bool t5RockBS;
    public GameObject[] t5Sprites;
    public LaserUIOffMap laserUIOffMap;

    public UILaserManager uILaserManager;

    private LaserUIData laserUIData;
 
    private bool init;

    public override void Init(ArtifactTileButton button)
    {
        if (init) 
            return;
        init = true;
        laserUIData = uILaserManager.AddData(this, button);
    }

    public void InitAndFindButton()
    {
        if (init) 
            return;

        button = GetComponentInParent<ArtifactTileButton>(includeInactive:true);
        Init(button);
    }

    public LaserUIData GetLaserUIData()
    {
        if(laserUIData != null) return laserUIData;
        InitAndFindButton();
        return laserUIData;
    }

    public override void OnPosChanged()
    {
        uILaserManager.UpdateSpritesFromSource();
    }

}

    public enum LaserCenterObject
    {
        NONE,
        SOURCE,
        PORTAL,
        MIRROR_NWSE,
        MIRROR_NESW,
        BLOCK,
        NOCHANGE,
    }

[Serializable]
public class LaserableRockUIData
{
    private bool exploded;
    public ExplodableRock rock;
    public ArtifactTBPluginLaser laserUI;
    public int blockLocation;
    public LaserCenterObject centerObject;

    public void UpdateLaserUI()
    {
        LaserUIData data = laserUI.GetLaserUIData();
        if(blockLocation != -1)
            data.edgeblockers[blockLocation] = false;
        else if(centerObject != LaserCenterObject.NOCHANGE)
            data.centerObject = centerObject;
        if(data.t5RockBS)
        {
            data.t5Sprites[0].SetActive(false);
            data.t5Sprites[0].SetActive(false);
            data.t5RockBS = false;
        }
    }

    public bool CheckForUpdate()
    {
        if(!exploded && rock.isExploded)
        {
            UpdateLaserUI();
            return true;
        }
        else return false;
    }
}
