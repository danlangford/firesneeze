using System.Collections.Generic;
using UnityEngine;

public class CardPropertyRecharge : MonoBehaviour
{
    [Tooltip("only allow recharge in these situations (HolyWater cannot recharge if banished)")]
    public List<ActionType> AllowedRecharge;
    [Tooltip("if you say yes this is the destination")]
    public ActionType SuccessDestination = ActionType.Recharge;
}

