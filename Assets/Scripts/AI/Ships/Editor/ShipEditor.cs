using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FreeSpace
{

    [CustomEditor (typeof (Ship))]
    public class ShipEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI ();
            Ship shipScript = (Ship)target;

            if (shipScript.stateMachine != null)
                if (shipScript.stateMachine.state != null)
                    EditorGUILayout.TextField ("State", shipScript.stateMachine.state.ToString ());
        }
    }

    [CustomEditor (typeof (BomberShip))]
    public class BomberShipEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI ();
            BomberShip shipScript = (BomberShip)target;

            if (shipScript.stateMachine != null)
                if (shipScript.stateMachine.state != null)
                    EditorGUILayout.TextField ("State", shipScript.stateMachine.state.ToString ());
        }
    }

    [CustomEditor (typeof (BansheeShip))]
    public class BansheeShipEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI ();
            BansheeShip shipScript = (BansheeShip)target;

            if (shipScript.stateMachine != null)
                if (shipScript.stateMachine.state != null)
                    EditorGUILayout.TextField ("State", shipScript.stateMachine.state.ToString ());
        }
    }

    [CustomEditor (typeof (ProtonShip))]
    public class ProtonShipEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI ();
            ProtonShip shipScript = (ProtonShip)target;

            if (shipScript.stateMachine != null)
                if (shipScript.stateMachine.state != null)
                    EditorGUILayout.TextField ("State", shipScript.stateMachine.state.ToString ());
        }
    }

}