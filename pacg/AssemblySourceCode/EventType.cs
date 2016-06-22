﻿using System;

public enum EventType
{
    None,
    OnCardEncountered,
    OnCombatEnd,
    OnCardDefeated,
    OnCardUndefeated,
    OnCardUndefeatedSequence,
    OnCardPlayed,
    OnCardRecharged,
    OnCardDiscarded,
    OnCardBuried,
    OnCardRevealed,
    OnCardActivated,
    OnCardDeactivated,
    OnPlayerDamaged,
    OnTurnStarted,
    OnCombatResolved,
    OnDiceRolled,
    OnTurnEnded,
    OnLocationClosed,
    OnLocationChange,
    OnCardDestroyed,
    OnLocationCloseAttempted,
    OnPostHorde,
    OnHandReset,
    OnDamageTaken,
    OnSecondCombat,
    OnPostAct,
    OnEndOfEndTurn,
    OnLocationExplored,
    OnExamineAnyLocation,
    OnCardEvaded,
    OnScenarioExit,
    OnCardBeforeAct,
    OnVillainIntroduced,
    OnAfterExplore,
    OnBeforeTurnStart,
    OnCallback
}
