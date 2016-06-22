using System;
using System.Collections.Generic;
using UnityEngine;

public class AlertManager : MonoBehaviour
{
    private static Dictionary<AlertType, GuiAlert> AlertInstances;
    private static Dictionary<AlertType, bool> AlertsShowing;

    private void Awake()
    {
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
    }

    public static void HandleAlerts()
    {
        Dictionary<AlertType, GuiAlert>.Enumerator enumerator = AlertInstances.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<AlertType, GuiAlert> current = enumerator.Current;
            AlertType key = current.Key;
            KeyValuePair<AlertType, GuiAlert> pair2 = enumerator.Current;
            GuiAlert alert = pair2.Value;
            if (ShouldShowAlert(key, alert))
            {
                ShowAlert(key, alert);
            }
            else
            {
                HideAlert(key, alert);
            }
        }
    }

    private static bool HasSeenAlert(AlertType type) => 
        PlayerPrefs.HasKey(type.ToString());

    private static void HideAlert(AlertType type, GuiAlert alert)
    {
        if (AlertsShowing[type])
        {
            AlertInstances[type].Show(false);
            AlertsShowing[type] = false;
        }
    }

    public static void SeenAlert(AlertType type)
    {
        if (!HasSeenAlert(type))
        {
            PlayerPrefs.SetInt(type.ToString(), 1);
        }
        Dictionary<AlertType, GuiAlert>.Enumerator enumerator = AlertInstances.GetEnumerator();
        while (enumerator.MoveNext())
        {
            KeyValuePair<AlertType, GuiAlert> pair4;
            KeyValuePair<AlertType, GuiAlert> pair5;
            KeyValuePair<AlertType, GuiAlert> current = enumerator.Current;
            if (type != ((AlertType) current.Key))
            {
                continue;
            }
            KeyValuePair<AlertType, GuiAlert> pair2 = enumerator.Current;
            if (pair2.Value.AdditionalAlerts == null)
            {
                break;
            }
            KeyValuePair<AlertType, GuiAlert> pair3 = enumerator.Current;
            if (pair3.Value.AdditionalAlerts.Length <= 0)
            {
                break;
            }
            int index = 0;
            goto Label_009F;
        Label_007E:
            pair4 = enumerator.Current;
            PlayerPrefs.SetInt(pair4.Value.AdditionalAlerts[index], 1);
            index++;
        Label_009F:
            pair5 = enumerator.Current;
            if (index < pair5.Value.AdditionalAlerts.Length)
            {
                goto Label_007E;
            }
            break;
        }
    }

    public static void SetAlerts(List<GuiAlert> alerts)
    {
        AlertInstances = new Dictionary<AlertType, GuiAlert>();
        AlertsShowing = new Dictionary<AlertType, bool>();
        if (alerts != null)
        {
            for (int i = 0; i < alerts.Count; i++)
            {
                if (!AlertInstances.ContainsKey(alerts[i].AlertType))
                {
                    alerts[i].Show(false);
                    AlertInstances.Add(alerts[i].AlertType, alerts[i]);
                    AlertsShowing.Add(alerts[i].AlertType, false);
                }
            }
        }
    }

    private static bool ShouldShowAlert(AlertType type, GuiAlert alert)
    {
        if ((alert.transform.parent == null) || alert.transform.parent.gameObject.activeInHierarchy)
        {
            if (ShouldShowAlertOfType(type))
            {
                return true;
            }
            if ((alert.AdditionalAlerts != null) && (alert.AdditionalAlerts.Length > 0))
            {
                for (int i = 0; i < alert.AdditionalAlerts.Length; i++)
                {
                    if (!PlayerPrefs.HasKey(alert.AdditionalAlerts[i]))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private static bool ShouldShowAlertOfType(AlertType type)
    {
        if (type == AlertType.Store)
        {
            return ((((ShouldShowAlertOfType(AlertType.DailyGold) || ShouldShowAlertOfType(AlertType.SeenStoreAdventures)) || (ShouldShowAlertOfType(AlertType.SeenStoreCharacters) || ShouldShowAlertOfType(AlertType.SeenStoreGold))) || ((ShouldShowAlertOfType(AlertType.SeenStoreSpecials) || ShouldShowAlertOfType(AlertType.SeenStoreStart)) || ShouldShowAlertOfType(AlertType.SeenStoreTreasurePurchase))) || ShouldShowAlertOfType(AlertType.SeenStoreTreasureReveal));
        }
        if (type == AlertType.DailyGold)
        {
            return ((Game.Network.Connected && Game.Network.CurrentUser.GoldSubAvailable) && (Game.Network.CurrentUser.GoldSubDaysRemaining > 0));
        }
        return !PlayerPrefs.HasKey(type.ToString());
    }

    private static void ShowAlert(AlertType type, GuiAlert alert)
    {
        if (!AlertsShowing[type])
        {
            AlertInstances[type].Show(true);
            AlertsShowing[type] = true;
        }
    }

    public enum AlertType
    {
        None,
        Store,
        DailyGold,
        SeenStoreStart,
        SeenStoreSpecials,
        SeenStoreAdventures,
        SeenStoreCharacters,
        SeenStoreTreasurePurchase,
        SeenStoreGold,
        SeenStoreTreasureReveal
    }
}

