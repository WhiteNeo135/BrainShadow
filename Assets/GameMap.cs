using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    public bool can = true;

    private void OnMouseDown()
    {
        if(can) transform.GetComponent<SpriteRenderer>().color = MenuCtrl.btnColor;
    }
}
