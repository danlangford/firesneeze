using System;
using UnityEngine;

public class BlackMaggaController : MonoBehaviour
{
    public BlockSetAsideCards Block;

    public void EatCard()
    {
        if (this.Block != null)
        {
            this.Block.BlackMaggaChomp();
        }
    }
}

