namespace GooglePlayGames.Native
{
    using GooglePlayGames.OurUtils;
    using System;
    using System.Runtime.CompilerServices;

    internal static class CallbackUtils
    {
        [CompilerGenerated]
        private static void <ToOnGameThread`1>m__12<T>(T)
        {
        }

        [CompilerGenerated]
        private static void <ToOnGameThread`2>m__14<T1, T2>(T1, T2)
        {
        }

        [CompilerGenerated]
        private static void <ToOnGameThread`3>m__16<T1, T2, T3>(T1, T2, T3)
        {
        }

        internal static Action<T> ToOnGameThread<T>(Action<T> toConvert)
        {
            <ToOnGameThread>c__AnonStoreyB6<T> yb = new <ToOnGameThread>c__AnonStoreyB6<T> {
                toConvert = toConvert
            };
            if (yb.toConvert == null)
            {
                return new Action<T>(CallbackUtils.<ToOnGameThread`1>m__12<T>);
            }
            return new Action<T>(yb.<>m__13);
        }

        internal static Action<T1, T2> ToOnGameThread<T1, T2>(Action<T1, T2> toConvert)
        {
            <ToOnGameThread>c__AnonStoreyB8<T1, T2> yb = new <ToOnGameThread>c__AnonStoreyB8<T1, T2> {
                toConvert = toConvert
            };
            if (yb.toConvert == null)
            {
                return new Action<T1, T2>(CallbackUtils.<ToOnGameThread`2>m__14<T1, T2>);
            }
            return new Action<T1, T2>(yb.<>m__15);
        }

        internal static Action<T1, T2, T3> ToOnGameThread<T1, T2, T3>(Action<T1, T2, T3> toConvert)
        {
            <ToOnGameThread>c__AnonStoreyBA<T1, T2, T3> yba = new <ToOnGameThread>c__AnonStoreyBA<T1, T2, T3> {
                toConvert = toConvert
            };
            if (yba.toConvert == null)
            {
                return new Action<T1, T2, T3>(CallbackUtils.<ToOnGameThread`3>m__16<T1, T2, T3>);
            }
            return new Action<T1, T2, T3>(yba.<>m__17);
        }

        [CompilerGenerated]
        private sealed class <ToOnGameThread>c__AnonStoreyB6<T>
        {
            internal Action<T> toConvert;

            internal void <>m__13(T val)
            {
                <ToOnGameThread>c__AnonStoreyB7<T> yb = new <ToOnGameThread>c__AnonStoreyB7<T> {
                    <>f__ref$182 = (CallbackUtils.<ToOnGameThread>c__AnonStoreyB6<T>) this,
                    val = val
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yb.<>m__18));
            }

            private sealed class <ToOnGameThread>c__AnonStoreyB7
            {
                internal CallbackUtils.<ToOnGameThread>c__AnonStoreyB6<T> <>f__ref$182;
                internal T val;

                internal void <>m__18()
                {
                    this.<>f__ref$182.toConvert(this.val);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ToOnGameThread>c__AnonStoreyB8<T1, T2>
        {
            internal Action<T1, T2> toConvert;

            internal void <>m__15(T1 val1, T2 val2)
            {
                <ToOnGameThread>c__AnonStoreyB9<T1, T2> yb = new <ToOnGameThread>c__AnonStoreyB9<T1, T2> {
                    <>f__ref$184 = (CallbackUtils.<ToOnGameThread>c__AnonStoreyB8<T1, T2>) this,
                    val1 = val1,
                    val2 = val2
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(yb.<>m__19));
            }

            private sealed class <ToOnGameThread>c__AnonStoreyB9
            {
                internal CallbackUtils.<ToOnGameThread>c__AnonStoreyB8<T1, T2> <>f__ref$184;
                internal T1 val1;
                internal T2 val2;

                internal void <>m__19()
                {
                    this.<>f__ref$184.toConvert(this.val1, this.val2);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <ToOnGameThread>c__AnonStoreyBA<T1, T2, T3>
        {
            internal Action<T1, T2, T3> toConvert;

            internal void <>m__17(T1 val1, T2 val2, T3 val3)
            {
                <ToOnGameThread>c__AnonStoreyBB<T1, T2, T3> ybb = new <ToOnGameThread>c__AnonStoreyBB<T1, T2, T3> {
                    <>f__ref$186 = (CallbackUtils.<ToOnGameThread>c__AnonStoreyBA<T1, T2, T3>) this,
                    val1 = val1,
                    val2 = val2,
                    val3 = val3
                };
                PlayGamesHelperObject.RunOnGameThread(new Action(ybb.<>m__1A));
            }

            private sealed class <ToOnGameThread>c__AnonStoreyBB
            {
                internal CallbackUtils.<ToOnGameThread>c__AnonStoreyBA<T1, T2, T3> <>f__ref$186;
                internal T1 val1;
                internal T2 val2;
                internal T3 val3;

                internal void <>m__1A()
                {
                    this.<>f__ref$186.toConvert(this.val1, this.val2, this.val3);
                }
            }
        }
    }
}

