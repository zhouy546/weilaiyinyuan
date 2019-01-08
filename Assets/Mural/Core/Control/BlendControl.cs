using nobnak.Blending.Geometry;
using nobnak.Gist.InputDevice;
using nobnak.Gist.StateMachine;
using UnityEngine;

namespace nobnak.Blending.Control {

    public class BlendControl : AbstractControl {
        public enum OperationEnum { None = 0, MoveBlendWidth }

        protected FSM<OperationEnum> fsmOperation;

        public BlendControl(BlendingController bcon, Blending blending,
            MouseTracker mouseTracker, BlendingController.MousePosition mouseCurr) 
            : base(bcon, blending, mouseTracker, mouseCurr) {

            fsmOperation = new FSM<OperationEnum>();

            FsmActivity.State(ActivityEnum.Inactive);
            FsmActivity.State(ActivityEnum.Active).Enter(fsm => {
                mouseTracker.OnSelectionDown += (mt, f) => {
                    switch (f) {
                        case MouseTracker.ButtonFlag.Left:
                            if (mouseCurr.TryInitEdgeMode())
                                fsmOperation.Goto(OperationEnum.MoveBlendWidth);
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
            fsmOperation.State(OperationEnum.MoveBlendWidth).Update(fsm => {
                if (bcon.IsOverGUI)
                    return;
                
                mouseCurr.Update();

                var duv = mouseCurr.WorldDuv;
                var data = blending.BlendingData;
                var ej = new EdgeJoint(data, mouseCurr.selectedScreen, mouseCurr.selectedEdge);
                ej.Bandwidth += duv[ej.axis];
                UvMapper.UpdateUv(data.Screens, data.Edges, data.ViewportOffsets);
                data.Invalidate();
            });
            fsmOperation.Init();
        }
        
	}
}
