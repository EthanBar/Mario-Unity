//using UnityEngine; using UnityEditor;
// 
//  public class AutoSnap : EditorWindow
//  {
//      private Vector3 prevPosition;
//      private Vector3 prevRotation;
//      private bool doSnap = true;
//      private bool doRotateSnap = true;
//      private float snapValueX = 1;
//      private float snapValueY = 1;
//      private float snapValueZ = 1;
//      private float snapRotateValue = 1;
// 
//     private const string doSnapKey        = "AutoSnap_doSnapKey";
//     private const string doRotateSnapKey  = "AutoSnap_doRotateSnapKey";
//     private const string snapValueXKey    = "AutoSnap_snapValueXKey";
//     private const string snapValueYKey    = "AutoSnap_snapValueYKey";
//     private const string snapValueZKey    = "AutoSnap_snapValueZKey";
//     private const string snapRotateValueKey = "AutoSnap_snapRotateValueKey";
// 
//      [MenuItem( "Edit/Auto Snap %_l" )]
//      
//      static void Init()
//      {
//          AutoSnap window = (AutoSnap)EditorWindow.GetWindow( typeof( AutoSnap ) );
//          window.maxSize = new Vector2( 200, 125 );
//      }
//      
//      public void OnGUI()
//      {
//          doSnap = EditorGUILayout.Toggle( "Auto Snap", doSnap );
//          doRotateSnap = EditorGUILayout.Toggle ("Auto Snap Rotation", doRotateSnap);
//          snapValueX = EditorGUILayout.FloatField( "Snap X Value", snapValueX );
//          snapValueY = EditorGUILayout.FloatField( "Snap Y Value", snapValueY );
//          snapValueZ = EditorGUILayout.FloatField( "Snap Z Value", snapValueZ );
//          snapRotateValue = EditorGUILayout.FloatField ("Rotation Snap Value", snapRotateValue);
//      }
//      
//      public void Update()
//      {
//          doSnap = EditorGUILayout.Toggle( "Auto Snap", doSnap );
//          if ( doSnap
//              && !EditorApplication.isPlaying
//              && Selection.transforms.Length > 0
//              && Selection.transforms[0].position != prevPosition )
//          {
//              Snap();
//              prevPosition = Selection.transforms[0].position;
//          }
//      }
//  
//      private void Snap()
//      {
//          foreach ( var transform in Selection.transforms )
//          {
//              var t = transform.transform.position;
//              t.x = RoundX( t.x + 0.5f) - 0.5f;
//              t.y = RoundY( t.y + 0.5f) - 0.5f;
//              t.z = RoundZ( t.z + 0.5f) - 0.5f;
//              transform.transform.position = t;
//          }
//      }
//  
//      private void RotateSnap()
//      {
//          foreach (var transform in Selection.transforms)
//          {
//              var r = transform.transform.eulerAngles;
//              r.x = RotRound (r.x);
//              r.y = RotRound (r.y);
//              r.z = RotRound (r.z);
//              transform.transform.eulerAngles = r;
//          }
//      }
//      
//      private float RoundX( float input )
//      {
//          return snapValueX * Mathf.Round( ( input / snapValueX ) );
//      }
//      private float RoundY( float input )
//      {
//          return snapValueY * Mathf.Round( ( input / snapValueY ) );
//      }
//      private float RoundZ( float input )
//      {
//          return snapValueZ * Mathf.Round( ( input / snapValueZ ) );
//      }
//  
//      private float RotRound( float input )
//      {
//          //Debug.Log ("The division is: " + (input / snapRotateValue ) );
//          //Debug.Log ("The rounding is: " + Mathf.Round( ( input / snapRotateValue ) ) );
//          //Debug.Log ("The return is: " + (snapRotateValue * Mathf.Round( ( input / snapRotateValue ) )));
//          return snapRotateValue * Mathf.Round( ( input / snapRotateValue ) );
//      }
//  
//      public void OnEnable() {
//         if (EditorPrefs.HasKey(doSnapKey)) {
//             doSnap = EditorPrefs.GetBool(doSnapKey);
//         }
//         if (EditorPrefs.HasKey(doRotateSnapKey)) {
//             doRotateSnap = EditorPrefs.GetBool(doRotateSnapKey);
//         }
//         if (EditorPrefs.HasKey(snapValueXKey)) {
//             snapValueX = EditorPrefs.GetFloat(snapValueXKey);
//         }
//         if (EditorPrefs.HasKey(snapValueYKey)) {
//             snapValueY = EditorPrefs.GetFloat(snapValueYKey);
//         }
//         if (EditorPrefs.HasKey(snapValueZKey)) {
//             snapValueZ = EditorPrefs.GetFloat(snapValueZKey);
//         }
//         if (EditorPrefs.HasKey(snapRotateValueKey)) {
//             snapRotateValue = EditorPrefs.GetFloat(snapRotateValueKey);
//         }
// 
//         EditorApplication.update += Update;
//      }
// 
//      public void OnDisable() {
//         EditorPrefs.SetBool(doSnapKey, doSnap);
//         EditorPrefs.SetBool(doRotateSnapKey, doRotateSnap);
//         EditorPrefs.SetFloat(snapValueXKey, snapValueX);
//         EditorPrefs.SetFloat(snapValueYKey, snapValueY);
//         EditorPrefs.SetFloat(snapValueZKey, snapValueZ);
//         EditorPrefs.SetFloat(snapRotateValueKey, snapRotateValue);
//      }
//  }