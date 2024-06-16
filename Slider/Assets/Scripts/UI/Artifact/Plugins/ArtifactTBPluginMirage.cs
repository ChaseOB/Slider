using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactTBPluginMirage : ArtifactTBPlugin
{
    //disable mirage button called on click based on bool flag and whnever a move is made
    //if desertMirage is true, do any mirage things. but if false, revert to normal behavior and don't enable the foggy vfx for the tile
    public int mirageIslandId;
    public int buttonIslandId;
    public ArtifactTBPluginLaser myLaserPlugin;
    [SerializeField] private List<ArtifactTBPluginLaser> laserPlugins;
    [SerializeField] private List<Sprite> mirageSprites;
    [SerializeField] private STile stile;
    [SerializeField] private ButtonMirage buttonMirage;
    public MagiLaser mirageDinoLaser;

    private void OnEnable()
    {
        DesertArtifact.MoveMadeOnArtifact += MoveMadeOnArtifact;
    }
    private void OnDisable()
    {
        DesertArtifact.MoveMadeOnArtifact -= MoveMadeOnArtifact;
    }

    public void MirageOnClick()
    {
        button.SelectButton();
    }

    private void MoveMadeOnArtifact(object sender, System.EventArgs e)
    {
        CheckForMirage();
    }

    private void CheckForMirage()
    {
        if (!MirageSTileManager.GetInstance().MirageEnabled) return; 

        DisableMirageButton();
        (int, int) cords = (button.x, button.y);
        DesertArtifact artifact = (DesertArtifact)UIArtifact.GetInstance();
        mirageIslandId = artifact.currGrid[cords];

        if (mirageIslandId < 8) 
        {
            EnableMirageButton(mirageIslandId, false);
        }
    }

    public void EnableMirageButton(int id, bool fromLoad)
    {
        //print("enabling mirage button " + id);
        //print(DesertGrid.GetGridString()); //4 * (2-y) + x
        mirageIslandId = id;
        button.SetEmptySprite(mirageSprites[mirageIslandId - 1]);
        button.SetIslandSprite(mirageSprites[mirageIslandId - 1]);
        button.SetSpriteToIsland();
        stile.sliderCollider.isTrigger = true;
        buttonMirage.SetMirageEnabled(true);
        SendLaserData(fromLoad);
    }

    private void SendLaserData(bool fromLoad)
    {
        //When is this a source?
        //0. dino laser is on
        //1. this is mirage tile 4
        //2. the tile to the left is either the real tile 7 or mirage tile 7
        //mirage 7 if currGrid is 7
        //real 7 if gridString is 7 4 * (2-y) + x

        DesertArtifact artifact = (DesertArtifact)UIArtifact.GetInstance();

        if(button.x > 0)
        {
            int x = button.x - 1;
            int y = button.y;
            (int, int) cords = (x, y);
            int tileLeft = UIArtifact.GetButton(x, y).islandId;
            bool real7left = tileLeft == 7;
            bool mirage7left = fromLoad ? 
                MirageSTileManager.GetInstance().MirageTileAtLocation(7, x, y) :
                artifact.currGrid[cords] == 7 && (tileLeft == 8 || tileLeft == 9);
            bool dinoButtLeft = mirage7left || real7left;
            if(dinoButtLeft)
            {
                print("dino butt left real " + real7left +  " mirage " + mirage7left );
            }
            ArtifactTBPluginLaser laserPlugin = laserPlugins[mirageIslandId - 1];
            var laser = mirageIslandId == 4 ? mirageDinoLaser : null;
            myLaserPlugin.GetLaserUIData().CopyDataFromMirageSource(laserPlugin.GetLaserUIData(), dinoButtLeft, laser);
        }
    }

    public void DisableMirageButton()
    {
        button.RestoreDefaultEmptySprite();
        button.RestoreDefaultIslandSprite();
        button.SetSpriteToIslandOrEmpty();
        MirageSTileManager.GetInstance().DisableMirageTile(mirageIslandId);
        mirageIslandId = 0;
        stile.sliderCollider.isTrigger = false;
        buttonMirage.SetMirageEnabled(false);
        myLaserPlugin.GetLaserUIData()?.ClearDataOnMirageDisable();

        DesertArtifact.MirageDisappeared?.Invoke(this, new System.EventArgs());
    }
}
