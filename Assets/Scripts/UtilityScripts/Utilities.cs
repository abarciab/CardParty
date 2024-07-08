using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
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

    public static IEnumerator LerpToAndBack(GameObject obj, Vector3 target, float time = OBJECT_LERP_TIME) {
        Vector3 initObjPos = obj.transform.position;
        Quaternion initObjRot = obj.transform.rotation;
        yield return GameManager.i.StartCoroutine(LerpObject(obj, target, obj.transform.rotation, time : time));
        yield return GameManager.i.StartCoroutine(LerpObject(obj, initObjPos, initObjRot, time : time));
    }

    public static IEnumerator LerpObject(GameObject obj, Transform target, float time = OBJECT_LERP_TIME) {
        if (obj == null) {
            Debug.Log("cancelled shake because of null obj");
            yield break;
        }

        Vector3 initObjPos = obj.transform.position;
        Quaternion initObjRot = obj.transform.rotation;

        float startTime = Time.time;
        while (Time.time - startTime < time) {
            if (obj == null) {
                Debug.Log("cancelled shake because of null obj");
                yield break;
            }

            float completed = (Time.time - startTime) / time;

            obj.transform.position = Vector3.Lerp(initObjPos, target.position, completed);
            obj.transform.rotation = Quaternion.Lerp(initObjRot, target.rotation, completed);
            
            yield return 0;
        }

        obj.transform.position = target.position;
        obj.transform.rotation = target.rotation;
    }

    public static IEnumerator LerpObject(GameObject obj, Vector3 targetPos, Quaternion targetRot, float time = OBJECT_LERP_TIME) {
        if (obj == null) {
            Debug.Log("cancelled shake because of null obj");
            yield break;
        }

        Vector3 initObjPos = obj.transform.position;
        Quaternion initObjRot = obj.transform.rotation;

        float startTime = Time.time;
        while (Time.time - startTime < time) {
            if (obj == null) {
                Debug.Log("cancelled shake because of null obj");
                yield break;
            }

            float completed = (Time.time - startTime) / time;

            obj.transform.position = Vector3.Lerp(initObjPos, targetPos, completed);
            obj.transform.rotation = Quaternion.Lerp(initObjRot, targetRot, completed);
            
            yield return 0;
        }

        obj.transform.position = targetPos;
        obj.transform.rotation = targetRot;
    }

    public static IEnumerator LerpObjectLocal(GameObject obj, Vector3 targetPos, Quaternion targetRot, float time = OBJECT_LERP_TIME) {
        if (obj == null) {
            Debug.Log("cancelled shake because of null obj");
            yield break;
        }

        Vector3 initObjPos = obj.transform.localPosition;
        Quaternion initObjRot = obj.transform.localRotation;

        float startTime = Time.time;
        while (Time.time - startTime < time) {
            if (obj == null) {
                Debug.Log("cancelled shake because of null obj");
                yield break;
            }

            float completed = (Time.time - startTime) / time;

            obj.transform.localPosition = Vector3.Lerp(initObjPos, targetPos, completed);
            obj.transform.localRotation = Quaternion.Lerp(initObjRot, targetRot, completed);
            
            yield return 0;
        }

        obj.transform.localPosition = targetPos;
        obj.transform.localRotation = targetRot;
    }

    public static IEnumerator LerpScale(GameObject obj, Vector3 targetScale, float time = OBJECT_LERP_TIME) {
        if (obj == null) yield break;

        Vector3 initScale = obj.transform.localScale;

        float startTime = Time.time;
        while (Time.time - startTime < time) {
            if (obj == null) yield break;

            float completed = (Time.time - startTime) / time;

            obj.transform.localScale = Vector3.Lerp(initScale, targetScale, completed);
            
            yield return 0;
        }
    }

    public static IEnumerator ShakeObject(GameObject obj) {
        Vector3 initPos = obj.transform.localPosition;
        Quaternion initRot = obj.transform.localRotation;
        Vector3 targetPos = initPos + OBJECT_SHAKE_MAGNITUDE * 1f * Camera.main.scaledPixelWidth
        * new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
        Quaternion targetRot = Quaternion.Euler(0, 0, OBJECT_SHAKE_MAGNITUDE * Random.Range(-1f, 1f));

        yield return GameManager.i.StartCoroutine(LerpObjectLocal(obj, targetPos, targetRot, OBJECT_SHAKE_TIME / 2));
        yield return GameManager.i.StartCoroutine(LerpObjectLocal(obj, initPos, initRot, OBJECT_SHAKE_TIME / 2));
    }
}