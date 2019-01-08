using nobnak.Blending.Geometry;
using nobnak.Gist.InputDevice;
using nobnak.Gist.StateMachine;
using System.Diagnostics;
using UnityEngine;

namespace nobnak.Blending.Control {

    public class EdgeControl : AbstractControl {
        public enum OperationEnum { None = 0, MoveEdge }

        protected FSM<OperationEnum> fsmOperation;

        public EdgeControl(BlendingController bcon, Blending blending,
            MouseTracker mouseTracker, BlendingController.MousePosition mouseCurr) 
            : base(bcon, blending, mouseTracker, mouseCurr) {

            fsmOperation = new FSM<OperationEnum>();

            FsmActivity.State(ActivityEnum.Inactive);
            FsmActivity.State(ActivityEnum.Active).Enter(fsm => {
                mouseTracker.OnSelectionDown += (mt, f) => {
                    switch (f) {
                        case MouseTracker.ButtonFlag.Left:
                            if (mouseCurr.TryInitEdgeMode())
                                fsmOperation.Goto(OperationEnum.MoveEdge);
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
            fsmOperation.State(OperationEnum.MoveEdge).Update(fsm => {
                if (bcon.IsOverGUI)
                    return;
                
                mouseCurr.Update();

                var duv = mouseCurr.WorldDuv;
                var screen = mouseCurr.selectedScreen;
                var data = blending.BlendingData;
                var iscreen = screen.x + screen.y * data.Screens.x;
                var trap = data.Trapeziums[iscreen];
                var iv0 = ScreenSelector.PAIR_EDGES[2 * mouseCurr.selectedEdge];
                var iv1 = ScreenSelector.PAIR_EDGES[2 * mouseCurr.selectedEdge + 1];
                var dx = ScreenSelector.ScreenSize(data.Screens);
                var worldTrap = ScreenSelector.LocalToWorld(screen.x, screen.y, trap, dx);
                var tan = (worldTrap[iv1] - worldTrap[iv0]).normalized;
                duv -= Vector2.Dot(duv, tan) * tan;
                trap[iv0] += duv;
                trap[iv1] += duv;
                trap.Clamp01();
                data.Trapeziums[iscreen] = trap;
                data.Invalidate();
            });
            fsmOperation.Init();
        }
        
	}
}
