using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

namespace Applications.Utils
{
    public class GameUtil
    {
        public static bool isAttackModeOn = false;
        public static int attackIcelandIndex = -1;
        public static int[] selectedIceLand;
        public static GameObject staticSpawnContainer;
        public static int[] activatedIcelands;

        public static int numberofselected=0;

        public static void ResetAttackMode()
        {
            isAttackModeOn = false;
            attackIcelandIndex = -1;
            selectedIceLand = new int[20];
            numberofselected = 0;

            for (int i = 0; i < selectedIceLand.Length; i++)
            {
                selectedIceLand[i] = -1;
            }
        }

        public static bool CheckIsIcelandSelected(int iceLandIndex)
        {
            foreach (int index in selectedIceLand)
            {
                if (index == iceLandIndex)
                    return true;
            }

            return false;
        }
        public static T GetIcelandValueUsingKey<T>(GameObject parent, int iceLandIndex, string keyName)
        {
            if (iceLandIndex != -1)
            {
                if (keyName == "playerType")
                {
                    return (T)Convert.ChangeType(parent.transform.GetChild(iceLandIndex).gameObject.GetComponent<Iceland>().playerType, typeof(T));
                }
            }

            return (T)Convert.ChangeType("", typeof(T));
        }

        public static void GenerateAttackPlane(GameObject planeObject, Transform fromObject, Transform toObject)
        {
            // LookAt 2D
            Vector3 target = toObject.position;

            // get the angle
            Vector3 norTar = (target - planeObject.transform.position).normalized;
            float angle = Mathf.Atan2(norTar.y, norTar.x) * Mathf.Rad2Deg;

            // rotate to angle
            Quaternion rotation = new Quaternion();
            rotation.eulerAngles = new Vector3(0, 0, angle - 90);
            planeObject.transform.rotation = rotation;

            planeObject.GetComponent<AttackingPlane>().toObject = toObject.gameObject;

            //change plane's size according to total plane
            if (planeObject.GetComponent<AttackingPlane>().totalPlane <= 10)
            {
                planeObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            }

            if (planeObject.GetComponent<AttackingPlane>().totalPlane > 10 && planeObject.GetComponent<AttackingPlane>().totalPlane <= 25)
            {
                planeObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            }

            if (planeObject.GetComponent<AttackingPlane>().totalPlane > 25 && planeObject.GetComponent<AttackingPlane>().totalPlane <= 45)
            {
                planeObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            }

            if (planeObject.GetComponent<AttackingPlane>().totalPlane > 45)
            {
                planeObject.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        public static int[] CopyFromOneToAnother()
        {
            int[] newActivatedIcelands = new int[activatedIcelands.Length];

            int counter = 0;
            for (int i = 0; i < 20; i++)
            {
                //Debug.Log("ISACTIVE: " + staticSpawnContainer.transform.GetChild(i).gameObject.activeInHierarchy);

                if (staticSpawnContainer.transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    //Debug.Log("INDEX: " + i);
                    newActivatedIcelands[counter] = staticSpawnContainer.transform.GetChild(i).GetComponent<Iceland>().iceLandIndex;
                    counter++;
                }
            }

            //activatedIcelands = newActivatedIcelands;

            return newActivatedIcelands;
        }

        public static int[] GetSortedListOfPlayers(int planeToBeChecked)
        {
            int[] sortListedPlanes = new int[activatedIcelands.Length];
            int[] newActivatedIcelands = CopyFromOneToAnother();

            List<int> sortListedIcelandList = new List<int>();

            int counter = 0;

            for (int i = 0; i < newActivatedIcelands.Length; i++)
            {
                int mainTotalPlanes = staticSpawnContainer.transform.GetChild(newActivatedIcelands[i]).GetComponent<Iceland>().totalPlane;
                if (mainTotalPlanes < planeToBeChecked)
                {
                    sortListedIcelandList.Add(newActivatedIcelands[i]);
                    counter++;
                }
            }


            if (sortListedIcelandList.Count > 4)
            {
                newActivatedIcelands = new int[sortListedIcelandList.Count];
                for (int i = 0; i < sortListedIcelandList.Count; i++)
                {
                    newActivatedIcelands[i] = sortListedIcelandList[i];
                }
            }

            for (int i = 0; i < newActivatedIcelands.Length; i++)
            {
                int mainTotalPlanes = staticSpawnContainer.transform.GetChild(newActivatedIcelands[i]).GetComponent<Iceland>().totalPlane;
                for (int j = i + 1; j < newActivatedIcelands.Length; j++)
                {
                    int innerTotalPlanes = staticSpawnContainer.transform.GetChild(newActivatedIcelands[j]).GetComponent<Iceland>().totalPlane;
                    if (mainTotalPlanes > innerTotalPlanes)
                    {
                        int temp = newActivatedIcelands[i];
                        newActivatedIcelands[i] = newActivatedIcelands[j];
                        newActivatedIcelands[j] = temp;
                    }
                }
            }

            return newActivatedIcelands;
        }

        public static int[] GetOtherPlayerIndexes(string teamCode, int[] inputData, int iteration = 0)
        {
            //activatedIcelands = CopyFromOneToAnother();

            int[] newActivatedIcelands = new int[inputData.Length];

            List<int> newActivatedIcelandsList = new List<int>();

            int counter = 0;

            for (int i = 0; i < inputData.Length; i++)
            {
                if (staticSpawnContainer.transform.GetChild(inputData[i]).GetComponent<Iceland>().teamCode != teamCode || staticSpawnContainer.transform.GetChild(activatedIcelands[i]).GetComponent<Iceland>().teamCode == "0")
                {
                    newActivatedIcelandsList.Add(inputData[i]);
                    counter++;
                }
            }

            if (newActivatedIcelandsList.Count < 0)
            {
                if (iteration <= 2)
                {
                    return GetOtherPlayerIndexes(teamCode, CopyFromOneToAnother(), iteration++);
                }
            }

            newActivatedIcelands = new int[newActivatedIcelandsList.Count];
            for (int i = 0; i < newActivatedIcelandsList.Count; i++)
            {
                newActivatedIcelands[i] = newActivatedIcelandsList[i];
            }

            return newActivatedIcelands;

        }
    }
}
