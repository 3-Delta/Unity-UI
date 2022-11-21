using System;
using System.Timers;

using UnityEngine;

namespace Logic.Hotfix.Fixed {
    [Serializable]
    public class UIBaseWithLayout<TLayout> : FUIBase where TLayout : UILayoutBase, new() {
        [SerializeField] protected TLayout layout;

        protected override void OnLoaded(Transform transform) {
            base.OnLoaded(transform);

            this.layout = new TLayout();
            this.layout.TryBind(transform);
        }
    }

    // 自动关闭类型的UI
    [Serializable]
    public class UIBaseWithScheduleClose<TLayout> : UIBaseWithLayout<TLayout> where TLayout : UILayoutBase, new() {
        protected virtual float CloseDuration {
            get { 
                return 3.0f;
            }
        }

        protected virtual bool Repeated {
            get {
                return false;
            }
        }

        protected virtual Action CloseAction {
            get {
                return CloseSelf;
            }
        }

        private void OnTimeCompleted(object _, ElapsedEventArgs __) {
            CloseAction?.Invoke();
        }

        protected Timer closeTimer;
        protected override void OnOpened() {
            base.OnOpened();

            closeTimer?.Close();
            closeTimer = new Timer(CloseDuration);
            closeTimer.Elapsed += OnTimeCompleted;
            closeTimer.AutoReset = Repeated;

            closeTimer.Enabled = true;
        }

        protected override void OnClose() {
            closeTimer?.Close();
            closeTimer?.Dispose();

            base.OnClose();
        }
    }
}