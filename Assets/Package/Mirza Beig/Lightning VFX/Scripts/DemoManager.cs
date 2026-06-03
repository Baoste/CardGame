using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirzaBeig.LightningVFX
{
    public class DemoManager : MonoBehaviour
    {
        new Camera camera;

        public GameObject fxPrefab;
        // public GameObject fxPrefab_performanceMode;

        [Space]

        // -1 = Unity uses the platform's default target frame rate.

        public int targetFrameRate = -1;

        [Space]

        List<GameObject> spawnedLightningList = new List<GameObject>();

        [Space]

        public Light mainLight;
        float mainLightStartIntensity;

        [Space]

        public float mainLightDimIntensity = 0.35f;
        public int fullDimLightningCount = 5;

        [Space]

        public float mainLightIntensityLerpSpeed = 1.0f;

        void Start()
        {
            camera = Camera.main;

            mainLightStartIntensity = mainLight.intensity;

        }

        //void Update()
        //{
        //    if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(1))
        //    {
        //        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        //        if (Physics.Raycast(ray, out RaycastHit raycastHitInfo, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
        //        {
        //            GameObject lightning = Instantiate(fxPrefab, raycastHitInfo.point, Quaternion.identity);
        //            StartCoroutine(ChangeMainLightIntensity());

        //            spawnedLightningList.Add(lightning);
        //        }
        //    }
        //}

        private IEnumerator ChangeMainLightIntensity()
        {
            //yield return new WaitForSeconds(0.01f);
            mainLight.intensity = float.NaN;
            yield return new WaitForSecondsRealtime(1f);
            mainLight.intensity = 1;
        }

        public void GenerateLighting(Vector3 pos)
        {
            GameObject lightning = Instantiate(fxPrefab, pos, Quaternion.identity);
            // StartCoroutine(ChangeMainLightIntensity());
            spawnedLightningList.Add(lightning);
        }

        void LateUpdate()
        {
            spawnedLightningList.RemoveAll(x => x == null);

            //float normalizedLightningCount = spawnedLightningList.Count / (float)fullDimLightningCount;
            //float mainLightTargetIntensity = Mathf.Lerp(mainLightStartIntensity, mainLightDimIntensity, normalizedLightningCount);

            //mainLight.intensity = Mathf.Lerp(mainLight.intensity, mainLightTargetIntensity, Time.deltaTime * mainLightIntensityLerpSpeed);
        }
    }
}