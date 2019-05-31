using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MultiProjectorWarpSystem
{
    public class CalibrationManager : MonoBehaviour
    {
        public enum MenuState
        {
            NONE,
            CORNERS,
            ROWS,
            COLUMNS,
            POINTS,
            BLEND,
            WHITE_BALANCE,
            HELP
        }
        public MenuState state;

        [Header("Shelf Icons")]
        public ProjectionWarpSystem system;
        public Canvas canvas;
        public InputField filename;
        public Text currentProjectorText;

        [Header("Button Selected Highlights")]
        public GameObject displayLabel;
        public GameObject pointGroupLabel;
        public GameObject blendLabel;
        public GameObject whiteLabel;
        public GameObject mouseLabel;
        public GameObject ioLabel;
        public GameObject helpLabel;

        [Header("Button Selected Highlights")]
        public GameObject cornerSelectedHighlight;
        public GameObject rowSelectedHighlight;
        public GameObject columnSelectedHighlight;
        public GameObject pointSelectedHighlight;
        public GameObject blendSelectedHighlight;
        public GameObject whiteSelectedHighlight;
        public GameObject mouseSelectedHighlight;
        public GameObject helpSelectedHighlight;

        [Header("Hotkey Instructions")]
        public ScrollRect helpScrollView;
        public ScrollRect cornerScrollView;
        public ScrollRect rowScrollView;
        public ScrollRect columnScrollView;
        public ScrollRect pointScrollView;
        public RectTransform blendView;
        public RectTransform whiteBalanceView;

        [Header("Edge Blending UI")]
        public InputField topRangeInputField;
        public InputField topChokeInputField;
        public InputField bottomRangeInputField;
        public InputField bottomChokeInputField;
        public InputField leftRangeInputField;
        public InputField leftChokeInputField;
        public InputField rightRangeInputField;
        public InputField rightChokeInputField;

        public Slider topRangeSlider;
        public Slider topChokeSlider;
        public Slider bottomRangeSlider;
        public Slider bottomChokeSlider;
        public Slider leftRangeSlider;
        public Slider leftChokeSlider;
        public Slider rightRangeSlider;
        public Slider rightChokeSlider;

        [Header("White Balance UI")]
        public InputField redInputField;
        public InputField greenInputField;
        public InputField blueInputField;

        public Slider redSlider;
        public Slider greenSlider;
        public Slider blueSlider;

        void Start()
        {
            SetButtonState(MenuState.NONE);
            mouseSelectedHighlight.SetActive(system.showMouseCursor);
        }

        void Update()
        {

        }

        #region Edge Blending Callbacks
        public void OnTopRangeSliderChanged(float value)
        {
            system.GetCurrentProjectionCamera().topFadeRange = value;
            topRangeInputField.text = value.ToString();
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnTopChokeSliderChanged(float value)
        {
            system.GetCurrentProjectionCamera().topFadeChoke = value;
            topChokeInputField.text = value.ToString();
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnBottomRangeSliderChanged(float value)
        {
            system.GetCurrentProjectionCamera().bottomFadeRange = value;
            bottomRangeInputField.text = value.ToString();
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnBottomChokeSliderChanged(float value)
        {
            system.GetCurrentProjectionCamera().bottomFadeChoke = value;
            bottomChokeInputField.text = value.ToString();
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnLeftRangeSliderChanged(float value)
        {
            system.GetCurrentProjectionCamera().leftFadeRange = value;
            leftRangeInputField.text = value.ToString();
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnLeftChokeSliderChanged(float value)
        {
            system.GetCurrentProjectionCamera().leftFadeChoke = value;
            leftChokeInputField.text = value.ToString();
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnRightRangeSliderChanged(float value)
        {
            system.GetCurrentProjectionCamera().rightFadeRange = value;
            rightRangeInputField.text = value.ToString();
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnRightChokeSliderChanged(float value)
        {
            system.GetCurrentProjectionCamera().rightFadeChoke = value;
            rightChokeInputField.text = value.ToString();
            system.GetCurrentProjectionCamera().UpdateBlend();
        }

        public void OnTopRangeInputChanged(string value)
        {
            float.TryParse(value, out system.GetCurrentProjectionCamera().topFadeRange);
            topRangeSlider.value = system.GetCurrentProjectionCamera().topFadeRange;
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnTopChokeInputChanged(string value)
        {
            float.TryParse(value, out system.GetCurrentProjectionCamera().topFadeChoke);
            topChokeSlider.value = system.GetCurrentProjectionCamera().topFadeChoke;
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnBottomRangeInputChanged(string value)
        {
            float.TryParse(value, out system.GetCurrentProjectionCamera().bottomFadeRange);
            bottomRangeSlider.value = system.GetCurrentProjectionCamera().bottomFadeRange;
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnBottomChokeInputChanged(string value)
        {
            float.TryParse(value, out system.GetCurrentProjectionCamera().bottomFadeChoke);
            bottomChokeSlider.value = system.GetCurrentProjectionCamera().bottomFadeChoke;
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnLeftRangeInputChanged(string value)
        {
            float.TryParse(value, out system.GetCurrentProjectionCamera().leftFadeRange);
            leftRangeSlider.value = system.GetCurrentProjectionCamera().leftFadeRange;
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnLeftChokeInputChanged(string value)
        {
            float.TryParse(value, out system.GetCurrentProjectionCamera().leftFadeChoke);
            leftChokeSlider.value = system.GetCurrentProjectionCamera().leftFadeChoke;
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnRightRangeInputChanged(string value)
        {
            float.TryParse(value, out system.GetCurrentProjectionCamera().rightFadeRange);
            rightRangeSlider.value = system.GetCurrentProjectionCamera().rightFadeRange;
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnRightChokeInputChanged(string value)
        {
            float.TryParse(value, out system.GetCurrentProjectionCamera().rightFadeChoke);
            rightChokeSlider.value = system.GetCurrentProjectionCamera().rightFadeChoke;
            system.GetCurrentProjectionCamera().UpdateBlend();
        }



        #endregion

        #region White Balance Callbacks
        public void OnRedSliderChanged(float value)
        {
            Color tint = system.GetCurrentProjectionCamera().tint;
            system.GetCurrentProjectionCamera().tint = new Color(value / 255f, tint.g, tint.b);
            
            redInputField.text = value.ToString();
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnGreenSliderChanged(float value)
        {
            Color tint = system.GetCurrentProjectionCamera().tint;

            system.GetCurrentProjectionCamera().tint = new Color(tint.r, value / 255f, tint.b);
            greenInputField.text = value.ToString();
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        public void OnBlueSliderChanged(float value)
        {
            Color tint = system.GetCurrentProjectionCamera().tint;

            system.GetCurrentProjectionCamera().tint = new Color(tint.r, tint.g, value / 255f);
            blueInputField.text = value.ToString();
            system.GetCurrentProjectionCamera().UpdateBlend();
        }

        public void OnRedInputChanged(string value)
        {
            Color tint = system.GetCurrentProjectionCamera().tint;
            float red = 0f;
            float.TryParse(value, out red);

            system.GetCurrentProjectionCamera().tint = new Color(red / 255f, tint.g, tint.b);
            redSlider.value = red;
            system.GetCurrentProjectionCamera().UpdateBlend();
        }

        public void OnGreenInputChanged(string value)
        {
            Color tint = system.GetCurrentProjectionCamera().tint;
            float green = 0f;
            float.TryParse(value, out green);

            system.GetCurrentProjectionCamera().tint = new Color(tint.r, green / 255f, tint.b);
            greenSlider.value = green;
            system.GetCurrentProjectionCamera().UpdateBlend();
        }

        public void OnBlueInputChanged(string value)
        {
            Color tint = system.GetCurrentProjectionCamera().tint;
            float blue = 0f;
            float.TryParse(value, out blue);

            system.GetCurrentProjectionCamera().tint = new Color(tint.r, tint.g, blue / 255f);
            blueSlider.value = blue;
            system.GetCurrentProjectionCamera().UpdateBlend();
        }
        #endregion

        public void HideAllExclusiveModeSelections()
        {
            cornerSelectedHighlight.SetActive(false);
            rowSelectedHighlight.SetActive(false);
            columnSelectedHighlight.SetActive(false);
            pointSelectedHighlight.SetActive(false);
            blendSelectedHighlight.SetActive(false);
            whiteSelectedHighlight.SetActive(false);
            helpSelectedHighlight.SetActive(false);
        }

        public void HideAllViews()
        {
            cornerScrollView.gameObject.SetActive(false);
            rowScrollView.gameObject.SetActive(false);
            columnScrollView.gameObject.SetActive(false);
            pointScrollView.gameObject.SetActive(false);
            helpScrollView.gameObject.SetActive(false);
            blendView.gameObject.SetActive(false);
            whiteBalanceView.gameObject.SetActive(false);
        }

        public void SetButtonState(MenuState s)
        {
            HideAllExclusiveModeSelections();
            HideAllViews();

            switch (s) {
                case MenuState.CORNERS:
                    cornerSelectedHighlight.SetActive(true);
                    cornerScrollView.gameObject.SetActive(true);
                    break;
                case MenuState.ROWS:
                    rowSelectedHighlight.SetActive(true);
                    rowScrollView.gameObject.SetActive(true);
                    break;
                case MenuState.COLUMNS:
                    columnSelectedHighlight.SetActive(true);
                    columnScrollView.gameObject.SetActive(true);
                    break;
                case MenuState.POINTS:
                    pointSelectedHighlight.SetActive(true);
                    pointScrollView.gameObject.SetActive(true);
                    break;
                case MenuState.BLEND:
                    blendView.gameObject.SetActive(true);
                    blendSelectedHighlight.SetActive(true);
                    break;
                case MenuState.WHITE_BALANCE:
                    whiteBalanceView.gameObject.SetActive(true);
                    whiteSelectedHighlight.SetActive(true);
                    break;
                case MenuState.HELP:
                    helpSelectedHighlight.SetActive(true);
                    helpScrollView.gameObject.SetActive(true);
                    break;
                case MenuState.NONE:
                    break;
                default:
                    break;
            }

            state = s;

        }

        void OnEnable()
        {

        }

        #region Shelf Callbacks
        public void OnToggleMouseCursorDisplay()
        {
            system.showMouseCursor = !system.showMouseCursor;
            system.UpdateCursor();
        }
        public void OnToggleDisplaySelect()
        {
            system.SelectNextProjector();
        }
        public void OnToggleCornerSelect()
        {

            if (system.GetCurrentProjectionCamera().editMode ==  ProjectionMesh.MeshEditMode.CORNERS)
            {
                system.SetEditMode(ProjectionMesh.MeshEditMode.NONE);
            }
            else
            {
                system.SetEditMode(ProjectionMesh.MeshEditMode.CORNERS);
            }
        }
        public void OnToggleRowSelect()
        {
            if (system.GetCurrentProjectionCamera().editMode == ProjectionMesh.MeshEditMode.ROWS)
            {
                system.SetEditMode(ProjectionMesh.MeshEditMode.NONE);
            }
            else
            {
                system.SetEditMode(ProjectionMesh.MeshEditMode.ROWS);
            }
            
        }
        public void OnToggleColumnSelect()
        {
            if (system.GetCurrentProjectionCamera().editMode == ProjectionMesh.MeshEditMode.COLUMNS)
            {
                system.SetEditMode(ProjectionMesh.MeshEditMode.NONE);
            }
            else
            {
                system.SetEditMode(ProjectionMesh.MeshEditMode.COLUMNS);
            }
        }
        public void OnTogglePointSelect()
        {
            if (system.GetCurrentProjectionCamera().editMode == ProjectionMesh.MeshEditMode.POINTS)
            {
                system.SetEditMode(ProjectionMesh.MeshEditMode.NONE);
            }
            else
            {
                system.SetEditMode(ProjectionMesh.MeshEditMode.POINTS);
            }
        }

        public void OnToggleBlend()
        {
            system.SetEditMode(ProjectionMesh.MeshEditMode.NONE);

            if (state == MenuState.BLEND)
            {
                SetButtonState(MenuState.NONE);
            }
            else
            {
                SetButtonState(MenuState.BLEND);
            }
        }
        public void OnToggleWhiteBalance()
        {
            system.SetEditMode(ProjectionMesh.MeshEditMode.NONE);

            if (state == MenuState.WHITE_BALANCE)
            {
                SetButtonState(MenuState.NONE);
            }
            else
            {
                SetButtonState(MenuState.WHITE_BALANCE);
            }
        }
        public void OnToggleHelp()
        {
            system.SetEditMode(ProjectionMesh.MeshEditMode.NONE);
            if ( state== MenuState.HELP)
            {
                SetButtonState(MenuState.NONE);
            }
            else
            {
                SetButtonState(MenuState.HELP);
            }
        }

        #endregion

        public void ShowIconLabels()
        {
            displayLabel.SetActive(true);
            pointGroupLabel.SetActive(true);
            blendLabel.SetActive(true);
            whiteLabel.SetActive(true);
            mouseLabel.SetActive(true);
            ioLabel.SetActive(true);
            helpLabel.SetActive(true);
        }
        public void HideIconLabels()
        {
            displayLabel.SetActive(false);
            pointGroupLabel.SetActive(false);
            blendLabel.SetActive(false);
            whiteLabel.SetActive(false);
            mouseLabel.SetActive(false);
            ioLabel.SetActive(false);
            helpLabel.SetActive(false);
        }
    }

}