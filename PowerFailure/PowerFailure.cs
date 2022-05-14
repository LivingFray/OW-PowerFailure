using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace PowerFailure
{
    public class PowerFailure : ModBehaviour
    {
        private RingWorldScreenController _screenController;

        private RingWorldController _ringWorldController;

        private OWEmissiveRenderer _ringWorldBulb;

        private Light _ringWorldLight;

        private float _originalIntensity;

        private Color _originalBulbColour;

        private bool _initialised;

        private void Start()
        {
            _initialised = false;
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;
        }

        private void Update()
        {
            if (!_initialised) return;

            if (TimeLoop.GetSecondsElapsed() < _ringWorldController.damBreakTime) return;

            float PowerPercentage = Mathf.Max(0.0f, 1.0f - (TimeLoop.GetSecondsElapsed() - _ringWorldController.damBreakTime) / (_ringWorldController._lighthouseCollapseTime - _ringWorldController.damBreakTime));

            for (int i = 0; i < (int)_screenController._screenRenderers.Length; i++)
            {
                _screenController._screenRenderers[i].SetMaterialProperty(RingWorldScreenController.s_propID_screenFlicker, PowerPercentage);
            }

            // Rescale the power value to account for light falloff looking awful when done linearly
            float LightLevel = Mathf.Pow(PowerPercentage * _originalIntensity, 1.0f / 2.7f);

            _ringWorldLight.intensity = LightLevel;
            _ringWorldBulb.SetEmissionColor(_originalBulbColour * LightLevel);
        }

        private void OnCompleteSceneLoad(OWScene oldScene, OWScene newScene)
        {
            if (newScene == OWScene.SolarSystem)
            {
                _screenController = FindObjectOfType<RingWorldScreenController>();
                _ringWorldController = FindObjectOfType<RingWorldController>();       
                _ringWorldLight = GameObject.Find("RingWorld_Body/Sector_RingInterior/Lights_RingInterior/IP_SunLight").GetComponent<Light>();
                _ringWorldBulb = GameObject.Find("RingWorld_Body/Sector_RingInterior/Geometry_RingInterior/Structure_IP_ArtificialSun/ArtificialSun_Bulb").GetComponent<OWEmissiveRenderer>();

                _originalIntensity = _ringWorldLight.intensity;
                _originalBulbColour = _ringWorldBulb.GetOriginalEmissionColor();

                _initialised = true;
            }
            else
            {
                _screenController = null;
                _ringWorldController = null;
                _ringWorldLight = null;
                _ringWorldBulb = null;

                _initialised = false;
            }
        }

    }
}
