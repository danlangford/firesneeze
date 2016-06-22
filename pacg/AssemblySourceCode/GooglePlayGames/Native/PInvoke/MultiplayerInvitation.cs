﻿namespace GooglePlayGames.Native.PInvoke
{
    using GooglePlayGames.BasicApi.Multiplayer;
    using GooglePlayGames.Native.Cwrapper;
    using GooglePlayGames.OurUtils;
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class MultiplayerInvitation : BaseReferenceHolder
    {
        internal MultiplayerInvitation(IntPtr selfPointer) : base(selfPointer)
        {
        }

        internal Invitation AsInvitation()
        {
            Participant participant;
            Invitation.InvType invType = ToInvType(this.Type());
            string invId = this.Id();
            int variant = (int) this.Variant();
            using (GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant2 = this.Inviter())
            {
                participant = participant2?.AsParticipant();
            }
            return new Invitation(invType, invId, participant, variant);
        }

        internal uint AutomatchingSlots() => 
            GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_AutomatchingSlotsAvailable(base.SelfPtr());

        protected override void CallDispose(HandleRef selfPointer)
        {
            GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_Dispose(selfPointer);
        }

        internal static GooglePlayGames.Native.PInvoke.MultiplayerInvitation FromPointer(IntPtr selfPointer)
        {
            if (PInvokeUtilities.IsNull(selfPointer))
            {
                return null;
            }
            return new GooglePlayGames.Native.PInvoke.MultiplayerInvitation(selfPointer);
        }

        internal string Id() => 
            PInvokeUtilities.OutParamsToString((out_string, size) => GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_Id(base.SelfPtr(), out_string, size));

        internal GooglePlayGames.Native.PInvoke.MultiplayerParticipant Inviter()
        {
            GooglePlayGames.Native.PInvoke.MultiplayerParticipant participant = new GooglePlayGames.Native.PInvoke.MultiplayerParticipant(GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_InvitingParticipant(base.SelfPtr()));
            if (!participant.Valid())
            {
                participant.Dispose();
                return null;
            }
            return participant;
        }

        internal uint ParticipantCount() => 
            GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_Participants_Length(base.SelfPtr()).ToUInt32();

        private static Invitation.InvType ToInvType(Types.MultiplayerInvitationType invitationType)
        {
            switch (invitationType)
            {
                case Types.MultiplayerInvitationType.TURN_BASED:
                    return Invitation.InvType.TurnBased;

                case Types.MultiplayerInvitationType.REAL_TIME:
                    return Invitation.InvType.RealTime;
            }
            Logger.d("Found unknown invitation type: " + invitationType);
            return Invitation.InvType.Unknown;
        }

        internal Types.MultiplayerInvitationType Type() => 
            GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_Type(base.SelfPtr());

        internal uint Variant() => 
            GooglePlayGames.Native.Cwrapper.MultiplayerInvitation.MultiplayerInvitation_Variant(base.SelfPtr());
    }
}

