using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RequestBundle : Request<RequestBundle> {
    public List<RequestBundle> requestBundles = new List<RequestBundle>();
}