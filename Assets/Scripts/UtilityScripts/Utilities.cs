using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

public class Utilities
{
    public static int Id = 0;
    public const float OBJECT_LERP_TIME = 0.1f;
    public const float OBJECT_SHAKE_TIME = 0.05f;
    const float OBJECT_SHAKE_MAGNITUDE = 0.01f;

    public static int QuaternionToTurnCount(Quaternion quat)
    {
        if (Vector3.Distance(quat.eulerAngles, new Vector3(0, 270, 0)) < 0.1f) return 3;
        if (Vector3.Distance(quat.eulerAngles, new Vector3(0, 180, 0)) < 0.1f) return 2;
        if (Vector3.Distance(quat.eulerAngles, new Vector3(0, 90, 0)) < 0.1f) return 1;
        return 0;
    }

    public static Vector3 Vec3Pow(Vector3 vec, float pow) {
        return new Vector3(Mathf.Pow(vec.x, pow), Mathf.Pow(vec.y, pow), Mathf.Pow(vec.z, pow));
    }

    public static string Parenthize<t>(t input) => "(" + input + ")";

    public static async Task LerpToAndBack(GameObject obj, Vector3 target, float time = OBJECT_LERP_TIME) {
        Vector3 initObjPos = obj.transform.position;
        Quaternion initObjRot = obj.transform.rotation;
        await LerpObject(obj, target, obj.transform.rotation, time : time);
        await LerpObject(obj, initObjPos, initObjRot, time : time);
    }

    public static async Task LerpObject(GameObject obj, Transform target, float time = OBJECT_LERP_TIME) {
        if (obj == null) {
            Debug.Log("cancelled shake because of null obj");
            return;
        }

        Vector3 initObjPos = obj.transform.position;
        Quaternion initObjRot = obj.transform.rotation;

        float startTime = Time.time;
        while (Time.time - startTime < time) {
            if (obj == null) {
                Debug.Log("cancelled shake because of null obj");
                return;
            }

            float completed = (Time.time - startTime) / time;

            obj.transform.position = Vector3.Lerp(initObjPos, target.position, completed);
            obj.transform.rotation = Quaternion.Lerp(initObjRot, target.rotation, completed);
            
            await Task.Delay(1);
        }

        obj.transform.position = target.position;
        obj.transform.rotation = target.rotation;
    }

    public static async Task LerpObject(GameObject obj, Vector3 targetPos, Quaternion targetRot, float time = OBJECT_LERP_TIME) {
        if (obj == null) {
            Debug.Log("cancelled shake because of null obj");
            return;
        }

        Vector3 initObjPos = obj.transform.position;
        Quaternion initObjRot = obj.transform.rotation;

        float startTime = Time.time;
        while (Time.time - startTime < time) {
            if (obj == null) {
                Debug.Log("cancelled shake because of null obj");
                return;
            }

            float completed = (Time.time - startTime) / time;

            obj.transform.position = Vector3.Lerp(initObjPos, targetPos, completed);
            obj.transform.rotation = Quaternion.Lerp(initObjRot, targetRot, completed);
            
            await Task.Delay(1);
        }

        obj.transform.position = targetPos;
        obj.transform.rotation = targetRot;
    }

    public static async Task LerpObjectLocal(GameObject obj, Vector3 targetPos, Quaternion targetRot, float time = OBJECT_LERP_TIME) {
        if (obj == null) {
            Debug.Log("cancelled shake because of null obj");
            return;
        }

        Vector3 initObjPos = obj.transform.localPosition;
        Quaternion initObjRot = obj.transform.localRotation;

        float startTime = Time.time;
        while (Time.time - startTime < time) {
            if (obj == null) {
                Debug.Log("cancelled shake because of null obj");
                return;
            }

            float completed = (Time.time - startTime) / time;

            obj.transform.localPosition = Vector3.Lerp(initObjPos, targetPos, completed);
            obj.transform.localRotation = Quaternion.Lerp(initObjRot, targetRot, completed);
            
            await Task.Delay(1);
        }

        obj.transform.localPosition = targetPos;
        obj.transform.localRotation = targetRot;
    }

    public static async Task LerpScale(GameObject obj, Vector3 targetScale, float time = OBJECT_LERP_TIME) {
        if (obj == null) return;

        Vector3 initScale = obj.transform.localScale;

        float startTime = Time.time;
        while (Time.time - startTime < time) {
            if (obj == null) return;

            float completed = (Time.time - startTime) / time;

            obj.transform.localScale = Vector3.Lerp(initScale, targetScale, completed);
            
            await Task.Delay(1);
        }
    }

    public static async Task ShakeObject(GameObject obj) {
        Vector3 initPos = obj.transform.localPosition;
        Quaternion initRot = obj.transform.localRotation;
        Vector3 targetPos = initPos + OBJECT_SHAKE_MAGNITUDE * 1f * Camera.main.scaledPixelWidth
        * new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0).normalized;
        Quaternion targetRot = Quaternion.Euler(0, 0, OBJECT_SHAKE_MAGNITUDE * UnityEngine.Random.Range(-1f, 1f));

        await LerpObjectLocal(obj, targetPos, targetRot, OBJECT_SHAKE_TIME / 2);
        await LerpObjectLocal(obj, initPos, initRot, OBJECT_SHAKE_TIME / 2);
    }
}