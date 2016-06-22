namespace GooglePlayGames
{
    using GooglePlayGames.OurUtils;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.SocialPlatforms;

    public class PlayGamesUserProfile : IUserProfile
    {
        private string mAvatarUrl;
        private string mDisplayName;
        private Texture2D mImage;
        private volatile bool mImageLoading;
        private string mPlayerId;

        internal PlayGamesUserProfile(string displayName, string playerId, string avatarUrl)
        {
            this.mDisplayName = displayName;
            this.mPlayerId = playerId;
            this.mAvatarUrl = avatarUrl;
            this.mImageLoading = false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            if (!typeof(object).IsSubclassOf(typeof(PlayGamesUserProfile)))
            {
                return false;
            }
            return this.mPlayerId.Equals(((PlayGamesUserProfile) obj).mPlayerId);
        }

        public override int GetHashCode() => 
            (typeof(PlayGamesUserProfile).GetHashCode() ^ this.mPlayerId.GetHashCode());

        [DebuggerHidden]
        internal IEnumerator LoadImage() => 
            new <LoadImage>c__Iterator0 { <>f__this = this };

        protected void ResetIdentity(string displayName, string playerId, string avatarUrl)
        {
            this.mDisplayName = displayName;
            this.mPlayerId = playerId;
            this.mAvatarUrl = avatarUrl;
            this.mImageLoading = false;
        }

        public override string ToString() => 
            $"[Player: '{this.mDisplayName}' (id {this.mPlayerId})]";

        public string AvatarURL =>
            this.mAvatarUrl;

        public string id =>
            this.mPlayerId;

        public Texture2D image
        {
            get
            {
                if ((!this.mImageLoading && (this.mImage == null)) && !string.IsNullOrEmpty(this.AvatarURL))
                {
                    UnityEngine.Debug.Log("Starting to load image: " + this.AvatarURL);
                    this.mImageLoading = true;
                    PlayGamesHelperObject.RunCoroutine(this.LoadImage());
                }
                return this.mImage;
            }
        }

        public bool isFriend =>
            true;

        public UserState state =>
            UserState.Online;

        public string userName =>
            this.mDisplayName;

        [CompilerGenerated]
        private sealed class <LoadImage>c__Iterator0 : IDisposable, IEnumerator, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal PlayGamesUserProfile <>f__this;
            internal WWW <www>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        if (string.IsNullOrEmpty(this.<>f__this.AvatarURL))
                        {
                            UnityEngine.Debug.Log("No URL found.");
                            this.<>f__this.mImage = Texture2D.blackTexture;
                            this.<>f__this.mImageLoading = false;
                            goto Label_0104;
                        }
                        this.<www>__0 = new WWW(this.<>f__this.AvatarURL);
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_010B;
                }
                if (!this.<www>__0.isDone)
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                if (this.<www>__0.error == null)
                {
                    this.<>f__this.mImage = this.<www>__0.texture;
                }
                else
                {
                    this.<>f__this.mImage = Texture2D.blackTexture;
                    UnityEngine.Debug.Log("Error downloading image: " + this.<www>__0.error);
                }
                this.<>f__this.mImageLoading = false;
            Label_0104:
                this.$PC = -1;
            Label_010B:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current =>
                this.$current;

            object IEnumerator.Current =>
                this.$current;
        }
    }
}

