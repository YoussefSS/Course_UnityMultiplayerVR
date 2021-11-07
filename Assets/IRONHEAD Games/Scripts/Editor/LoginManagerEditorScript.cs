using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LoginManager))] // This is what says which script this editor script should edit
public class LoginManagerEditorScript : Editor // Editor script, so inheriting from Editor class
{
    public override void OnInspectorGUI() // Called everytime the inspector is drawn in the unity editor / like update
    {
        DrawDefaultInspector(); // Need to have this to be able to double click the script / have defaults
        EditorGUILayout.HelpBox("This script is responsible for connecting to Photon Servers.", MessageType.Info);

        LoginManager loginManager = (LoginManager)target; // Target is inherited from Editor class
        if(GUILayout.Button("Connect Anonymously")) // Creates a button with this text
        {
            loginManager.ConnectAnonymously();
        }
    }
}
