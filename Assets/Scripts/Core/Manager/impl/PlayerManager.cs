using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : BaseManager<PlayerManager>
{
    private PlayerManager() { }

    private GameObject player1;
    public GameObject Player1
    {
        get
        {
            if (player1 == null)
            {
                player1 = GameObject.Find("Player1");
            }
            return player1;
        }
    }

    private GameObject player2;
    public GameObject Player2
    {
        get
        {
            if (player2 == null)
            {
                player2 = GameObject.Find("Player2");
            }
            return player2;
        }
    }
}
