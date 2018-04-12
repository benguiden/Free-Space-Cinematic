namespace FreeSpace {

    [System.Serializable]
    public class StateMachine {

        public State state;

        public StateMachine() { }

        public StateMachine(State _state) {
            state = _state;
        }

        public void Update() {
            if (state != null)
                state.Update();
        }

        public void ChangeState(State newState) {
            if (state != null) {
                state.Exit();
            }
            state = newState;

            if (state != null) {
                state.Enter();
            }
        }

    }

    [System.Serializable]
    public class ShipStateMachine : StateMachine {

        public Ship ship;

        public ShipStateMachine(Ship _ship) {
            ship = _ship;
        }

        public ShipStateMachine(State _state, Ship _ship) {
            state = _state;
            ship = _ship;
        }

    }

}