using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTrigger : MonoBehaviour
{
    private Player player;

    private void Start()
    {
        player = gameObject.GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!player.stuffOnTrigger.Contains(col))
            player.stuffOnTrigger.Add(col);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (player.stuffOnTrigger.Contains(col))
            player.stuffOnTrigger.Remove(col);
    }
}
