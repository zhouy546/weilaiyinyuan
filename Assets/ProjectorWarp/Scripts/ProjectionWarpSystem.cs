using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Xml;
using SimpleJSON;
using UnityEngine.EventSystems;

namespace MultiProjectorWarpSystem
{
    [ExecuteInEditMode]
    public class ProjectionWarpSystem : MonoBehaviour
    {
        public enum CameraArragement
        {
            HORIZONTAL_ORTHOGRAPHIC = 1,
            VERTICAL_ORTHOGRAPHIC = 2,
            HORIZONTAL_PERSPECTIVE = 3,
            VERTICAL_PERSPECTIVE = 4
        }


        [Header("Projection Settings")]
        public string defaultCalibrationFile;
        public CameraArragement arrangement;
        public Vector2 renderTextureSize;
        public int xDivisions;
        public int yDivisions;
        public int projectorCount;

        [Header("Edit Mode")]
        public int selectedMesh;

        [Header("Debug")]
        public bool showMouseCursor;
        public bool showProjectionWarpGUI;
        public bool showIconLabels;

        [Header("Reference Game Objects")]
        public Transform projectionCamerasContainer;
        public Transform sourceCamerasContainer;
        public RectTransform projectionUIContainer;
        public GameObject fileIOContainer;
        public NotificationMessage notificationMessage;
        public List<Material> fadeMaterials;
        public CalibrationManager calibrationManager;


        [Header("Cameras & UI")]
        public List<Camera> sourceCameras;
        public List<ProjectionMesh> projectionCameras;
        public List<int> targetDisplays;

        [Header("Distances")]
        public Vector2 overlap;
        public float viewportSize;
        public float near;
        public float far;
        public float fieldOfView;

        public float aspectRatio;

        public float projectionCameraSpace;

        [Header("File IO")]
        public string saveCalibrationFile;

        


        public void UpdateFilename()
        {
            saveCalibrationFile = calibrationManager.filename.text;
        }

        
        void OnEnable()
        {

            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem));
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            UpdateCursor();
            UpdateProjectionWarpGUI();


            //bind enter press event on input field
            calibrationManager.filename.onEndEdit.RemoveAllListeners();
            calibrationManager.filename.onEndEdit.AddListener(val =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    saveCalibrationFile = calibrationManager.filename.text;
                }
            });
            
            /*
            if (projectorCount != sourceCameras.Count)
            {
                DestroyCameras();
                InitCameras();

            }
            */

            DestroyCameras();
            InitCameras();
            UpdateSourceCameras();


        }

        public void AssignReferences()
        {

            for (int i = 0; i < projectorCount; i++)
            {
                if (sourceCameras[i] == null || projectionCameras[i] == null) continue;
                projectionCameras[i].meshRenderer.sharedMaterial.mainTexture = sourceCameras[i].targetTexture;
            }
        }
        void Start()
        {
            LoadCalibration(Application.streamingAssetsPath + "/projector_calibration.json");
            Debug.Log(Application.streamingAssetsPath + "/projector_calibration.json");
            // Debug.Log(Application.dataPath + "/projector_calibration.json.json");
            //if (defaultCalibrationFile.Length > 0)
            //{
            //    LoadCalibration(defaultCalibrationFile);
            //}
            //else if (File.Exists(Application.dataPath + "/projector_calibration.json"))
            //{
        
            //    LoadCalibration(Application.dataPath + "/projector_calibration.json");

            //    Debug.Log(Application.dataPath + "/projector_calibration.json");
            //}
            // LoadCalibration(Application.dataPath + "/../default_calibration.json");
            AssignReferences();
            SelectProjector(0, false);
        }

        public ProjectionMesh GetCurrentProjectionCamera()
        {
            if (selectedMesh < 0 || selectedMesh > 7) return null;

            return projectionCameras[selectedMesh];
        }
        public void UpdateCursor()
        {
            Cursor.visible = showMouseCursor;
            calibrationManager.mouseSelectedHighlight.SetActive(showMouseCursor);
        }

        public void UpdateProjectionWarpGUI()
        {
            calibrationManager.canvas.enabled = showProjectionWarpGUI;

            if (Application.isPlaying)
            {
                if (showProjectionWarpGUI) EventSystem.current.sendNavigationEvents = false;
                else EventSystem.current.sendNavigationEvents = true;
            }
            
        }

        public void SetEditMode(ProjectionMesh.MeshEditMode mode)
        {
            if (GetCurrentProjectionCamera() == null) return;
            
            GetCurrentProjectionCamera().SetEditMode(mode);
            
            switch (mode)
            {
                case ProjectionMesh.MeshEditMode.NONE:
                    calibrationManager.SetButtonState(CalibrationManager.MenuState.NONE);
                    break;
                case ProjectionMesh.MeshEditMode.CORNERS:
                    calibrationManager.SetButtonState(CalibrationManager.MenuState.CORNERS);
                    break;
                case ProjectionMesh.MeshEditMode.ROWS:
                    calibrationManager.SetButtonState(CalibrationManager.MenuState.ROWS);
                    break;
                case ProjectionMesh.MeshEditMode.COLUMNS:
                    calibrationManager.SetButtonState(CalibrationManager.MenuState.COLUMNS);
                    break;
                case ProjectionMesh.MeshEditMode.POINTS:
                    calibrationManager.SetButtonState(CalibrationManager.MenuState.POINTS);
                    break;
                default:
                    break;
            }
            

        }
        public void DeactivateAllSelection()
        {
            for(int i = 0; i < projectionCameras.Count; i++)
            {
                projectionCameras[i].DeactivateSelection();
            }

        }
        public void SelectProjector(int projectorIndex, bool animateIndex)
        {
            if (projectorCount <= projectorIndex) return;

            ProjectionMesh mesh = null;

            if (selectedMesh >= 0 && selectedMesh <= 7)
            {
                GetCurrentProjectionCamera().DeactivateSelection();
            }

            selectedMesh = projectorIndex;
            mesh = GetCurrentProjectionCamera();
            calibrationManager.currentProjectorText.text = (projectorIndex + 1).ToString();
            if (animateIndex) mesh.ShowProjectorIndex();


            //check to see if in blend, white balance or help mode
            if (calibrationManager.state == CalibrationManager.MenuState.BLEND ||
                calibrationManager.state == CalibrationManager.MenuState.WHITE_BALANCE ||
                calibrationManager.state == CalibrationManager.MenuState.HELP)
            {
                //cache the current mode
                CalibrationManager.MenuState state = calibrationManager.state;
                //reset selections
                SetEditMode(ProjectionMesh.MeshEditMode.NONE);

                //reapply current mode
                calibrationManager.SetButtonState(state);
            }
            else
            {
                //use last used edit mode
                SetEditMode(mesh.editMode);
            }


            //update UI values by applying only to sliders
            calibrationManager.topRangeSlider.value = mesh.topFadeRange;
            calibrationManager.topChokeSlider.value = mesh.topFadeChoke;
            calibrationManager.bottomRangeSlider.value = mesh.bottomFadeRange;
            calibrationManager.bottomChokeSlider.value = mesh.bottomFadeChoke;
            calibrationManager.leftRangeSlider.value = mesh.leftFadeRange;
            calibrationManager.leftChokeSlider.value = mesh.leftFadeChoke;
            calibrationManager.rightRangeSlider.value = mesh.rightFadeRange;
            calibrationManager.rightChokeSlider.value = mesh.rightFadeChoke;

            calibrationManager.redSlider.value = mesh.tint.r * 255;
            calibrationManager.greenSlider.value = mesh.tint.g * 255;
            calibrationManager.blueSlider.value = mesh.tint.b * 255;
        }


        public void SelectNextProjector()
        {
            int meshIndex = selectedMesh;
            meshIndex++;
            if (meshIndex > projectorCount - 1) meshIndex = 0;
            SelectProjector(meshIndex, true);
        }
        void Update()
        {
#if UNITY_EDITOR
            AssignReferences();
#endif

            /*
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray;
                RaycastHit hitInfo;
                for (int i = 0; i < projectionCameras.Count; i++)
                {
                    ray = projectionCameras[i].targetCamera.ScreenPointToRay(Input.mousePosition);
                    hitInfo = new RaycastHit();

                    if (Physics.Raycast(ray.origin, ray.direction, out hitInfo))
                    {
                        int editVertexIndex = -1;
                        if (hitInfo.collider.name.Substring(0, 3) == "Top")
                        {
                            //top collider
                            editVertexIndex = int.Parse(hitInfo.collider.name.Substring(11, hitInfo.collider.name.Length - 11));
                        }
                        else
                        {
                            //bottom collider
                            editVertexIndex = (xDivisions + 1) + int.Parse(hitInfo.collider.name.Substring(14, hitInfo.collider.name.Length - 14));
                        }
                        projectionCameras[i].SetEditVertex(editVertexIndex);
                        projectionUIs[i].controlPointIndexSlider.value = editVertexIndex;
                        projectionCameras[i].OffsetRefresh();

                    }
                }
            }
            */

        }

        public void UpdateProjectionCameras()
        {

            for (int i = 0; i < projectionCameras.Count; i++)
            {
                if (projectionCameras[i] == null) continue;
                projectionCameras[i].transform.parent.localPosition = new Vector3((float)i * (projectionCameras[i].width + projectionCameraSpace), 0f, 0f);
                projectionCameras[i].width = renderTextureSize.x / 100f;
                projectionCameras[i].height = renderTextureSize.y / 100f;
                projectionCameras[i].xDivisions = xDivisions;
                projectionCameras[i].yDivisions = yDivisions;
                projectionCameras[i].targetCamera.orthographicSize = viewportSize;

                projectionCameras[i].CreateBaseGridLines();
            }

        }

        public float HorizontalToVerticalFOV(float hFov)
        {
            float hFovRad = hFov * Mathf.Deg2Rad;
            float vFovRad = Mathf.Atan(Mathf.Tan(hFovRad * 0.5f) / aspectRatio) * 2f;
            float vFov = vFovRad * Mathf.Rad2Deg;
            return vFov;
        }

        public void UpdateSourceCameras()
        {   
            aspectRatio = (float)renderTextureSize.x/ (float)renderTextureSize.y;
            
            float viewportHeight = viewportSize * 2;
            float viewportWidth = viewportHeight * aspectRatio;

            float singleFieldOfViewH;
            float singleFieldOfViewV;
            float startAngle;
            float compressedArcAngle;

            switch (arrangement)
            {
                case CameraArragement.HORIZONTAL_ORTHOGRAPHIC:
                case CameraArragement.VERTICAL_ORTHOGRAPHIC:
                    for (int i = 0; i < sourceCameras.Count; i++)
                    {
                        sourceCameras[i].nearClipPlane = near;
                        sourceCameras[i].farClipPlane = far;

                        //ortho cameras will be locked into target resolution
                        sourceCameras[i].orthographic = true;
                        sourceCameras[i].orthographicSize = viewportSize;
                    }
                    break;
                case CameraArragement.HORIZONTAL_PERSPECTIVE:
                case CameraArragement.VERTICAL_PERSPECTIVE:
                    break;
                default:
                    break;
            }

            switch (arrangement)
            {
                case CameraArragement.HORIZONTAL_ORTHOGRAPHIC:

                    float startX = (-(sourceCameras.Count / 2f) * viewportWidth) + (viewportWidth / 2f) + ((overlap.x / 2f) * (sourceCameras.Count - 1));
                    
                    float offsetX = 0f;
                    for (int i = 0; i < sourceCameras.Count; i++)
                    {
                        offsetX = startX + (i * viewportWidth) - (i * overlap.x);
                        sourceCameras[i].transform.localPosition = new Vector3(
                            offsetX,
                            0,
                            0);
                        sourceCameras[i].transform.localEulerAngles = Vector3.zero;
                    }
                    break;
                case CameraArragement.VERTICAL_ORTHOGRAPHIC:

                    float startY = (-(sourceCameras.Count / 2f) * viewportHeight) + (viewportHeight / 2f) + ((overlap.y / 2f) * (sourceCameras.Count - 1));
                    float offsetY = 0f;
                    for (int i = 0; i < sourceCameras.Count; i++)
                    {
                        offsetY = startY + (i * viewportHeight) - (i * overlap.y);
                        sourceCameras[i].transform.localPosition = new Vector3(
                            0,
                            offsetY,
                            0);
                        sourceCameras[i].transform.localEulerAngles = Vector3.zero;
                    }
                    break;
                case ProjectionWarpSystem.CameraArragement.HORIZONTAL_PERSPECTIVE:
                    singleFieldOfViewH = (fieldOfView / sourceCameras.Count) + overlap.x;
                    singleFieldOfViewV = HorizontalToVerticalFOV(singleFieldOfViewH);

                    startAngle = -(fieldOfView / 2f) + (singleFieldOfViewH / 2f);
                    compressedArcAngle = -startAngle * 2f;

                    for (int i = 0; i < sourceCameras.Count; i++)
                    {
                        if (sourceCameras[i] == null) continue;
                        sourceCameras[i].nearClipPlane = near;
                        sourceCameras[i].farClipPlane = far;

                        //calculate from field of view total
                        sourceCameras[i].orthographic = false;
                        sourceCameras[i].fieldOfView = singleFieldOfViewV;

                        sourceCameras[i].transform.localPosition = Vector3.zero;

                        if (projectorCount > 1)
                        {
                            sourceCameras[i].transform.localEulerAngles = new Vector3(0, startAngle + (i * (compressedArcAngle / (projectorCount - 1))), 0);
                        }
                        else
                        {
                            sourceCameras[i].transform.localEulerAngles = new Vector3(0, startAngle, 0);
                        }


                    }
                    break;
                case ProjectionWarpSystem.CameraArragement.VERTICAL_PERSPECTIVE:
                    singleFieldOfViewV = (fieldOfView / sourceCameras.Count) + overlap.y;
                    startAngle = -(fieldOfView / 2f) + (singleFieldOfViewV / 2f);
                    compressedArcAngle = -startAngle * 2f;

                    for (int i = 0; i < sourceCameras.Count; i++)
                    {
                        sourceCameras[i].nearClipPlane = near;
                        sourceCameras[i].farClipPlane = far;

                        //calculate from field of view total
                        sourceCameras[i].orthographic = false;
                        sourceCameras[i].fieldOfView = singleFieldOfViewV;

                        sourceCameras[i].transform.localPosition = Vector3.zero;
                        if (projectorCount > 1)
                        {
                            sourceCameras[i].transform.localEulerAngles = new Vector3(startAngle + (i * (compressedArcAngle / (projectorCount - 1))), 0, 0);
                        }
                        else
                        {
                            sourceCameras[i].transform.localEulerAngles = new Vector3(startAngle, 0, 0);
                        }
                    }
                    break;
                default:
                    break;
            }
        }


        public void DestroyCameras()
        {

            int count;
            
            count = sourceCamerasContainer.childCount;
            for (int i = 0; i < count; i++)
            {
                DestroyImmediate(sourceCamerasContainer.GetChild(0).gameObject);
            }
            sourceCameras = new List<Camera>();

            targetDisplays = new List<int>();

            count = projectionCamerasContainer.childCount;
            for (int i = 0; i < count; i++)
            {
                DestroyImmediate(projectionCamerasContainer.GetChild(0).gameObject);
            }
            projectionCameras = new List<ProjectionMesh>();
        }
        
        public void InitCameras()
        {        
            
            //build source render texture cameras array
            GameObject sourceCamera;
            Camera camera;
            RenderTexture renderTexture;

            for (int i = 0; i < projectorCount; i++)
            {
                sourceCamera = Instantiate(Resources.Load("Prefabs/Source Camera", typeof(GameObject))) as GameObject;
                camera = sourceCamera.GetComponent<Camera>();
                sourceCameras.Add(camera);
                sourceCamera.name = "Source Camera " + (i + 1);
                sourceCamera.transform.SetParent(sourceCamerasContainer);
                renderTexture = new RenderTexture((int)renderTextureSize.x, (int)renderTextureSize.y, 24, RenderTextureFormat.ARGB32);
                renderTexture.antiAliasing = 8;
                renderTexture.Create();
                targetDisplays.Add(i);
                camera.targetTexture = renderTexture;
                camera.targetDisplay = targetDisplays[i];
            }

            //build final render cameras
            GameObject projectionCamera;
            ProjectionMesh projectionMesh;

            for (int i = 0; i < projectorCount; i++)
            {
                projectionCamera = Instantiate(Resources.Load("Prefabs/Projection Camera", typeof(GameObject))) as GameObject;
                projectionMesh = projectionCamera.transform.GetChild(0).GetComponent<ProjectionMesh>();
                
                projectionCameras.Add(projectionMesh);
                projectionMesh.meshIndex = i;
                projectionCamera.name = "Projection Camera " + (i + 1);
                projectionCamera.transform.SetParent(projectionCamerasContainer);
                projectionCamera.transform.localPosition = new Vector3((float)i * (projectionMesh.width + projectionCameraSpace), 0f, 0f);

                projectionMesh.projectorIndexText.text = (i + 1).ToString();
                projectionMesh.xDivisions = xDivisions;
                projectionMesh.yDivisions = yDivisions;
                //projectionMesh.prevXDivision = xDivisions;
                //projectionMesh.prevYDivision = yDivisions;
                projectionMesh.width = renderTextureSize.x / 100f;
                projectionMesh.height = renderTextureSize.y / 100f;
                projectionCamera.GetComponent<Camera>().targetDisplay = targetDisplays[i];

                projectionMesh.CreateMesh();

                Material projectionImage = Instantiate(Resources.Load("Materials/Projection Image", typeof(Material))) as Material;
                projectionImage.name = "Projection Image " + (i + 1);
                projectionImage.mainTexture = sourceCameras[i].targetTexture;

                projectionMesh.meshRenderer.material = projectionImage;

            }

            UpdateProjectionCameras();

        }

        #region File IO
        public void SaveCalibrationUsingInput(GameObject input)
        {
            SaveCalibration(input.GetComponent<InputField>().text);
            notificationMessage.messageText.text = "Calibration has been saved";
            notificationMessage.Show();
        }
        public void LoadCalibrationUsingInput(GameObject input)
        {
            if (LoadCalibration(input.GetComponent<InputField>().text))
            {
                notificationMessage.messageText.text = "Calibration has been loaded";
            }
            else
            {
                notificationMessage.messageText.text = "Error loading calibration";
            }

            notificationMessage.Show();
        }
        public void SaveCalibration(string path)
        {
            if (path == null || path.Length == 0) return;
            //        Debug.Log(Application.dataPath+"/"+path);
            string json = "";
            json += "{";

            #region Global Settings
            json += "\"Version\": \"" + "2.0.0" + "\",";
            json += "\"Arrangement\":" + (int)arrangement + ",";
            json += "\"TextureWidth\":" + (int)renderTextureSize.x + ",";
            json += "\"TextureHeight\":" + (int)renderTextureSize.y + ",";
            json += "\"XDivisions\":" + xDivisions + ",";
            json += "\"YDivisions\":" + yDivisions + ",";
            json += "\"OverlapX\":" + overlap.x + ",";
            json += "\"OverlapY\":" + overlap.y + ",";
            json += "\"ViewportSize\":" + viewportSize + ",";
            json += "\"FieldOfView\":" + fieldOfView + ",";
            json += "\"Near\":" + near + ",";
            json += "\"Far\":" + far + ",";
            json += "\"Spacing\":" + projectionCameraSpace + ",";
            json += "\"Cameras\":";
            #endregion

            json += "[";

            for (int i = 0; i < projectorCount; i++)
            {
                ProjectionMesh projectionMesh = projectionCameras[i];
                json += "{";

                #region Edge Blending & White Balance
                json += "\"LeftFadeRange\":" + projectionMesh.leftFadeRange + ",";
                json += "\"LeftFadeChoke\":" + projectionMesh.leftFadeChoke + ",";
                json += "\"RightFadeRange\":" + projectionMesh.rightFadeRange + ",";
                json += "\"RightFadeChoke\":" + projectionMesh.rightFadeChoke + ",";
                json += "\"TopFadeRange\":" + projectionMesh.topFadeRange + ",";
                json += "\"TopFadeChoke\":" + projectionMesh.topFadeChoke + ",";
                json += "\"BottomFadeRange\":" + projectionMesh.bottomFadeRange + ",";
                json += "\"BottomFadeChoke\":" + projectionMesh.bottomFadeChoke + ",";
                json += "\"Tint\":" + "{ \"r\":" + projectionMesh.tint.r + ",\"g\":" + projectionMesh.tint.g + ",\"b\":" + projectionMesh.tint.b + "},";

                #endregion

                #region Offsets
                json += "\"Offset\":";
                json += "{";
                json += "\"Corner\":";
                json += "[";
                for (int j = 0; j < 4; j++)
                {
                    json += projectionMesh.cornerOffset[j].x;
                    json += ",";
                    json += projectionMesh.cornerOffset[j].y;
                    if(j<3) json += ",";
                }
                json += "]";
                json += ",";
                /*
                json += "\"Row\":";
                json += "[";
                for (int j = 0; j < yDivisions+1; j++)
                {
                    json += projectionMesh.rowOffset[j].x;
                    json += ",";
                    json += projectionMesh.rowOffset[j].y;
                    if (j < yDivisions) json += ",";
                }
                json += "]";
                json += ",";
                json += "\"Column\":";
                json += "[";
                for (int j = 0; j < xDivisions+1; j++)
                {
                    json += projectionMesh.columnOffset[j].x;
                    json += ",";
                    json += projectionMesh.columnOffset[j].y;
                    if (j < xDivisions) json += ",";
                }
                json += "]";
                json += ",";
                */              
                json += "\"Point\":";
                json += "[";
                int pointCount = (xDivisions + 1) * (yDivisions + 1);
                for (int j = 0; j < pointCount; j++)
                {
                    json += projectionMesh.pointOffset[j].x;
                    json += ",";
                    json += projectionMesh.pointOffset[j].y;
                    if (j < pointCount-1) json += ",";
                }
                json += "]";

               
                #endregion
                
                json += "}";
                json += "}";
                if (i < projectorCount - 1) json += ",";
            }
            
            json += "]";
          

            json += "}";

            var sr = File.CreateText(Application.dataPath + "/../" + path);
            sr.WriteLine(json);
            sr.Close();

            Debug.Log(path + " has been saved.");
        }

        public bool LoadCalibration(string path)
        {
            if (path == null || path.Length == 0) return false;

            string json = "";
            try
            {
                string line;

                StreamReader theReader = new StreamReader(path, Encoding.Default);
                using (theReader)
                {

                    do
                    {
                        line = theReader.ReadLine();

                        if (line != null)
                        {
                            json += line;
                        }
                    }
                    while (line != null);
                    theReader.Close();

                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("{0}\n", e.Message);
                Debug.Log(e.Message);
                return false;

            }
            var N = JSON.Parse(json);
            fieldOfView = N["FieldOfView"].AsFloat;
            projectorCount = N["Cameras"].Count;
            renderTextureSize = new Vector2(N["TextureWidth"].AsInt, N["TextureHeight"].AsInt);
            xDivisions = N["XDivisions"].AsInt;
            yDivisions = N["YDivisions"].AsInt;
            arrangement = (CameraArragement)N["Arrangement"].AsInt;
            overlap = new Vector2(N["OverlapX"].AsFloat, N["OverlapY"].AsFloat);
            viewportSize = N["ViewportSize"].AsFloat;
            near = N["Near"].AsFloat;
            far = N["Far"].AsFloat;
            projectionCameraSpace = N["Spacing"].AsFloat;

            DestroyCameras();
            InitCameras();


            for (int i = 0; i < projectorCount; i++)
            {
                ProjectionMesh projectionMesh = projectionCameras[i];
                JSONNode cameraNode = N["Cameras"][i];
                projectionMesh.leftFadeRange = cameraNode["LeftFadeRange"].AsFloat;
                projectionMesh.leftFadeChoke = cameraNode["LeftFadeChoke"].AsFloat;
                projectionMesh.rightFadeRange = cameraNode["RightFadeRange"].AsFloat;
                projectionMesh.rightFadeChoke = cameraNode["RightFadeChoke"].AsFloat;
                projectionMesh.topFadeRange = cameraNode["TopFadeRange"].AsFloat;
                projectionMesh.topFadeChoke = cameraNode["TopFadeChoke"].AsFloat;
                projectionMesh.bottomFadeRange = cameraNode["BottomFadeRange"].AsFloat;
                projectionMesh.bottomFadeChoke = cameraNode["BottomFadeChoke"].AsFloat;
                projectionMesh.tint = new Color(cameraNode["Tint"]["r"].AsFloat, cameraNode["Tint"]["g"].AsFloat, cameraNode["Tint"]["b"].AsFloat);
                JSONNode cornerNode = cameraNode["Offset"]["Corner"];
                
                for (int j = 0; j < 4; j++)
                {
                    projectionMesh.cornerOffset[j] = new Vector2(cornerNode[j * 2].AsFloat, cornerNode[(j * 2) + 1].AsFloat);
                }
                /*
                JSONNode rowNode = cameraNode["Offset"]["Row"];
                
                for (int j = 0; j < yDivisions+1; j++)
                {
                    projectionMesh.rowOffset[j] = new Vector2(rowNode[j * 2].AsFloat, rowNode[(j * 2) + 1].AsFloat);
                }

                JSONNode columnNode = cameraNode["Offset"]["Column"];
                
                for (int j = 0; j < xDivisions+1; j++)
                {
                    projectionMesh.columnOffset[j] = new Vector2(columnNode[j * 2].AsFloat, columnNode[(j * 2) + 1].AsFloat);
                }
                */
                JSONNode pointNode = cameraNode["Offset"]["Point"];
                for (int j = 0; j < (xDivisions + 1)*(yDivisions+1); j++)
                {
                    projectionMesh.pointOffset[j] = new Vector2(pointNode[j * 2].AsFloat, pointNode[(j * 2) + 1].AsFloat);
                }
                

                projectionMesh.CreateMesh();
                projectionMesh.BlendRefresh();
                //projectionMesh.OffsetRefresh();
                projectionMesh.UpdateUI();
            }
            
            defaultCalibrationFile = path;
            UpdateSourceCameras();
            Debug.Log(path + " has been loaded.");

            return true;
        }

        #endregion


    }

}
