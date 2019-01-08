using nobnak.Gist.InputDevice;
using nobnak.Gist.StateMachine;
using UnityEngine;

namespace nobnak.Blending.Control {

    public class AbstractControl {
        public enum ActivityEnum { Inactive = 0, Active }

        public FSM<ActivityEnum> FsmActivity { get; protected set; }

        protected BlendingController bcon;
        protected Blending blending;
        protected MouseTracker mouseTracker;
        protected BlendingController.MousePosition mouseCurr;

        public AbstractControl(BlendingController bcon, Blending blending,
            MouseTracker mouseTracker, BlendingController.MousePosition mouseCurr) {
            this.bcon = bcon;
            this.blending = blending;
            this.mouseTracker = mouseTracker;
            this.mouseCurr = mouseCurr;

            FsmActivity = new FSM<ActivityEnum>(FSM.TransitionModeEnum.Immediate);
        }

        public bool Activity {
            get { return FsmActivity.Current == ActivityEnum.Active; }
            set {
                var next = (value ? ActivityEnum.Active : ActivityEnum.Inactive);
                FsmActivity.Goto(next);
            }
        }
        public void Update() {
            FsmActivity.Update();
        }
    }
}
