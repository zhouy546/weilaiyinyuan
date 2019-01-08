using nobnak.Gist.InputDevice;
using nobnak.Gist.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Blending.Control {

    public class CornerControl : AbstractControl {
        public enum OperationEnum { None = 0, MoveCorner }

        protected FSM<OperationEnum> fsmOperation;

        public CornerControl(BlendingController bcon, Blending blending,
            MouseTracker mouseTracker, BlendingController.MousePosition mouseCurr) 
            : base(bcon, blending, mouseTracker, mouseCurr) {

            fsmOperation = new FSM<OperationEnum>(FSM.TransitionModeEnum.Immediate);

            FsmActivity.State(ActivityEnum.Inactive);
            FsmActivity.State(ActivityEnum.Active).Enter(fsm => {
                mouseTracker.OnSelectionDown += (mt, f) => {
                    switch (f) {
                        case MouseTracker.ButtonFlag.Left:
                            if (mouseCurr.TryInitVertexMode())
                                fsmOperation.Goto(OperationEnum.MoveCorner);
                            break;
                        case MouseTracker.ButtonFlag.Right:
                            break;
                    }
                };
                mouseTracker.OnSelection += (mt, f) => {
                    fsmOperation.Update();
                };
                mouseTracker.OnSelectionUp += (mt, f) => {
                    fsmOperation.Goto(OperationEnum.None);
                };
            }).Update(fsm => {
                fsmOperation.Update();
            }).Exit(fsm => {
                fsmOperation.Goto(OperationEnum.None);
                mouseTracker.Clear();
            });
            FsmActivity.Init();

            fsmOperation.State(OperationEnum.None);
            fsmOperation.State(OperationEnum.MoveCorner).Update(fsm => {
                if (bcon.IsOverGUI)
                    return;

                mouseCurr.Update();
                var duv = mouseCurr.WorldDuv;
                var data = blending.BlendingData;
                var iscreen = mouseCurr.selectedScreen.x + mouseCurr.selectedScreen.y * data.Screens.x;
                var trap = data.Trapeziums[iscreen];
                trap[mouseCurr.selectedVertex] += duv;
                trap.Clamp01();
                data.Trapeziums[iscreen] = trap;
                data.Invalidate();
            });
            fsmOperation.Init();
        }
        
	}
}
