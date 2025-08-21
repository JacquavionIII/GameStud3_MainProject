using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro ; 
// Title: Waypoint Marker | Unity
// Author: Omar A. Balfaqih (Making Games & Short Films)
// Date: 02 March 2019
// Code Version: 1.0
// Availabiltiy: https://youtu.be/oBkfujKPZw8?si=3zDAytOxoENv_mf-
public class Waypoint : MonoBehaviour
{
    public Image image;
    public Transform target;
    public TMP_Text distText;
    public Vector3 offset;

    private void Update()
    {
        float minX = image.GetPixelAdjustedRect().width / 2;
        float maxX = Screen.width - minX;

        float minY = image.GetPixelAdjustedRect().height / 2;
        float maxY = Screen.height - minY;


        Vector2 pos = Camera.main.WorldToScreenPoint(target.position + offset);

        if (Vector3.Dot((target.position - transform.position), transform.forward) < 0)
        {
            // The target is behind the player
            if (pos.x < Screen.width / 2)
            {
                pos.x = maxX;
            }
            else
            {
                pos.x = minX;
            }
        }

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        image.transform.position = pos;
        distText.text = ((int)Vector3.Distance(target.position, transform.position)).ToString() + "m";
    }
}
