using System.Collections;

namespace FreeSpace {

    [System.Serializable]
    public abstract class State {

        protected StateMachine stateMachine;
        protected float updateRefresh = 0.1f;

        public State(StateMachine _stateMachine) {
            stateMachine = _stateMachine;
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update();
        public abstract IEnumerator IUpdate();

    }

    [System.Serializable]
    public abstract class ShipState : State{

        public Ship ship;

        public ShipState(StateMachine _stateMachine, Ship _ship) : base(_stateMachine) {
            ship = _ship;
        }

        public abstract override void Enter();
        public abstract override void Exit();
        public abstract override void Update();
        public abstract override IEnumerator IUpdate();

    }

}
