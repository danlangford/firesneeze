namespace GooglePlayGames.BasicApi.Multiplayer
{
    using System;

    public class Invitation
    {
        private string mInvitationId;
        private InvType mInvitationType;
        private Participant mInviter;
        private int mVariant;

        internal Invitation(InvType invType, string invId, Participant inviter, int variant)
        {
            this.mInvitationType = invType;
            this.mInvitationId = invId;
            this.mInviter = inviter;
            this.mVariant = variant;
        }

        public override string ToString() => 
            $"[Invitation: InvitationType={this.InvitationType}, InvitationId={this.InvitationId}, Inviter={this.Inviter}, Variant={this.Variant}]";

        public string InvitationId =>
            this.mInvitationId;

        public InvType InvitationType =>
            this.mInvitationType;

        public Participant Inviter =>
            this.mInviter;

        public int Variant =>
            this.mVariant;

        public enum InvType
        {
            RealTime,
            TurnBased,
            Unknown
        }
    }
}

