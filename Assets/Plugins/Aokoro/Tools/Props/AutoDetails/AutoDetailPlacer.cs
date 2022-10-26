using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Aokoro.Tools.AutoDetails
{
    public class AutoDetailPlacer : MonoBehaviour
    {
        [SerializeField, Expandable]
        private AutoDetailList detailList;
        [SerializeField]
        private Transform[] detailsPositions;

        [SerializeField]
        private bool generateOnAwake;

        private Transform _detailsParent;
        private GameObject[] instiantiatedDetails;

        [ShowNativeProperty]
        private Transform DetailsParent
        {
            get
            {
                if (_detailsParent == null)
                {
                    _detailsParent = transform.Find("AutoDetails");
                    if (_detailsParent == null)
                    {
                        _detailsParent = new GameObject("AutoDetails").transform;
                        _detailsParent.SetParent(transform);
                        _detailsParent.SetAsFirstSibling();
                        _detailsParent.localPosition = Vector3.zero;
                        _detailsParent.localRotation = Quaternion.identity;
                    }
                }

                return _detailsParent;
            }
        }
        private void Awake()
        {
            if (generateOnAwake)
            {
                GenerateDetails();
            }
        }

        [Button]
        public void GenerateDetails()
        {
            CleanDetails();
            instiantiatedDetails = new GameObject[detailsPositions.Length];

            AutoDetail[] details = detailList.details;
            int detailsCount = details.Length;
            int totalRng = 0;

            //Total rng
            for (int i = 0; i < detailsCount; i++)
                totalRng += details[i].probability;

            //Going through every place where to spwan an item
            for (int i = 0; i < detailsPositions.Length; i++)
            {
                Transform detailPosition = detailsPositions[i];
                int rng = Random.Range(0, totalRng);
                int currentIndex = 0;

                //Searching the choosed detaim
                for (int j = 0; j < detailsCount; j++)
                {
                    AutoDetail detail = details[j];
                    currentIndex += detail.probability;
                    //Creating the choosed detail
                    if(currentIndex >= rng)
                    {
#if UNITY_EDITOR
                        if (PrefabUtility.IsPartOfAnyPrefab(detail.source))
                        {
                            instiantiatedDetails[i] = PrefabUtility.InstantiatePrefab(detail.source,DetailsParent) as GameObject;
                            instiantiatedDetails[i].transform.SetPositionAndRotation(detailPosition.position, detailPosition.rotation);
                        }
                        else
#endif
                            instiantiatedDetails[i] = Instantiate(detail.source, detailPosition.position, detailPosition.rotation, DetailsParent);
                        break;
                    }
                }
            }
        }

        [Button]
        public void GenerateDetailsForEveryone()
        {
            var detailPlacers = GameObject.FindObjectsOfType<AutoDetailPlacer>(false);
            for (int i = 0; i < detailPlacers.Length; i++)
                detailPlacers[i].GenerateDetails();
        }


        [Button]
        public void CleanDetails()
        {

            if (instiantiatedDetails != null && instiantiatedDetails.Length > 0)
            {
                int length = instiantiatedDetails.Length;
                for (int i = 0; i < length; i++)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        DestroyImmediate(instiantiatedDetails[i]);
                    else
#endif
                        Destroy(instiantiatedDetails[i]);
                }
            }

        }
        [Button]
        public void CleanDetailsForEveryone()
        {
            var detailPlacers = GameObject.FindObjectsOfType<AutoDetailPlacer>(false);
            for (int i = 0; i < detailPlacers.Length; i++)
                detailPlacers[i].CleanDetails();
        }
    }
}