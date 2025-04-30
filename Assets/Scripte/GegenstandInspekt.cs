using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using Yarn.Unity;
using UnityEngine.UI;

public class GegenstandInspekt : MonoBehaviour
{
    public Image UIInspectImage;
    public List<Sprite> InspectImages;

    //<<Inspect (Name des objektes auf das dieses code drauf ist, in meinem fall Gegenstand) (Bildname)>>
    //<<StopInspecting (Name des objektes auf das dieses code drauf ist, in meinem fall Gegenstand)>>


    void Start()
    {
        StopInspecting();
    }

    //[YarnCommand("Inspect")]
    public void Inspect(string BildName)
    {
        if(InspectImages.Find(x => x.name == BildName) == true)
        {
            UIInspectImage.sprite = InspectImages.Find(x => x.name == BildName);
            UIInspectImage.enabled = true;
        }else{
            Debug.Log("Es fehlt "+ BildName + "in der liste");
        }
    }

    //[YarnCommand("StopInspecting")]
    public void StopInspecting()
    {
        UIInspectImage.enabled = false;
    }

}
