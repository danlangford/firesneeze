using System;
using UnityEngine;

[Serializable]
public class ScreenPoint
{
    [Tooltip("An offset in world coordinates.")]
    public Vector2 Offset = Vector2.zero;
    [Tooltip("Defines the location of a layout on screen.")]
    public ScreenPointType Point = ScreenPointType.Center;

    public Vector3 ToWorldPosition(GameObject source)
    {
        Vector3 position = source.transform.position;
        if (this.Point == ScreenPointType.Center)
        {
            position = UI.Camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        }
        GuiWindowLocation window = UI.Window as GuiWindowLocation;
        if (window != null)
        {
            switch (this.Point)
            {
                case ScreenPointType.Discard:
                    position = window.layoutDiscard.transform.position;
                    break;

                case ScreenPointType.Recharge:
                    position = window.layoutRecharge.transform.position;
                    break;

                case ScreenPointType.Banish:
                    position = window.layoutBanish.transform.position;
                    break;

                case ScreenPointType.Bury:
                    position = window.layoutBury.transform.position;
                    break;

                case ScreenPointType.Blessing:
                    position = window.layoutBlessings.transform.position;
                    break;

                case ScreenPointType.Summoner:
                    position = window.layoutSummoner.transform.position;
                    break;

                case ScreenPointType.Location:
                    position = window.layoutExplore.transform.position;
                    break;

                case ScreenPointType.Encounter:
                    position = window.layoutLocation.transform.position;
                    break;

                case ScreenPointType.DraggedCard:
                {
                    Card draggedCard = window.GetDraggedCard();
                    if (draggedCard != null)
                    {
                        position = draggedCard.transform.position;
                    }
                    break;
                }
            }
        }
        return new Vector3(position.x + this.Offset.x, position.y + this.Offset.y, source.transform.position.z);
    }
}

