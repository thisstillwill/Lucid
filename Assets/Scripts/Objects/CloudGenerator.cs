using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CloudData
{
    public Vector3 pos;
    public Vector3 scale;
    public Quaternion rotation;
    private bool _isActive;

    public bool isActive
    {
        get
        {
            return _isActive;
        }

    }

    public int x;
    public int y;
    public float distFromCam;

    public Matrix4x4 matrix
    {
        get
        {
            return Matrix4x4.TRS(pos, rotation, scale);
        }
    }

    public CloudData(Vector3 position, Vector2 scale, Quaternion rotation, int x, int y, float distFromCam)
    {
        this.pos = position;
        this.scale = scale;
        this.rotation = rotation;
        SetActive(true);
        this.x = x;
        this.y = y;
        this.distFromCam = distFromCam;
    }

    public void SetActive(bool desState)
    {
        _isActive = desState;
    }
}
public class CloudGenerator : MonoBehaviour
{
    public Mesh cloudMesh;
    public Material cloudMaterial;

    public float cloudSize = 5;
    public float maxScale = 1;

    public float timeScale = 1;
    public float texScale = 1;

    public float minNoiseSize = 0.5f;
    public float sizeScale = 0.25f;

    public Camera cam;

    public int maxDist;

    public int batchesToCreate;

    private Vector3 prevCamPos;
    private float offsetX = 1;
    private float offsetY = 1;

    private List<List<CloudData>> batches = new List<List<CloudData>>();
    private List<List<CloudData>> batchesToUpdate = new List<List<CloudData>>();
    private void Start()
    {
        for (int batchesX = 0; batchesX < batchesToCreate; batchesX++)
        {
            for (int batchesY = 0; batchesY < batchesToCreate; batchesY++) {
                BuildCloudBatch(batchesX, batchesY);
        }
        }
    }

    private void BuildCloudBatch(int xLoop, int yLoop)
    {
        bool markBatch = false;
        List<CloudData> currBatch = new List<CloudData>();
        for (int x = 0; x < 31; x++)
        {
            for (int y = 0; y < 31; y++)
            {
                AddCloud(currBatch, x + xLoop * 31, y + yLoop * 31);
            }
        }

        markBatch = CheckForActiveBatch(currBatch);

        batches.Add(currBatch);

        if (markBatch) batchesToUpdate.Add(currBatch);
    }

    private bool CheckForActiveBatch (List<CloudData> batch)
    {
        foreach (var cloud in batch)
        {
            cloud.distFromCam = Vector3.Distance(cloud.pos, cam.transform.position);
            if (cloud.distFromCam < maxDist) return true;
        }
        return false;
    }

    private void AddCloud (List<CloudData> currBatch, int x, int y)
    {
        Vector3 position = new Vector3(transform.position.x + x * cloudSize, transform.position.y, transform.position.z + y * cloudSize);

        float distToCam = Vector3.Distance(new Vector3(x, transform.position.y, y), cam.transform.position);
        currBatch.Add(new CloudData(position, Vector3.zero, Quaternion.identity, x, y, distToCam));
    }

    private void Update()
    {
        MakeNoise();
        offsetX += Time.deltaTime * timeScale;
        offsetY += Time.deltaTime * timeScale;

    }

    void MakeNoise()
    {
        if (cam.transform.position == prevCamPos)
        {
            UpdateBatches();
        }
        else
        {
            prevCamPos = cam.transform.position;
            UpdateBatchList();
            UpdateBatches();
        }
        RenderBatches();
        prevCamPos = cam.transform.position;
    }

    private void UpdateBatches()
    {
        foreach (var batch in batchesToUpdate)
        {
            foreach(var cloud in batch)
            {
                float size = Mathf.PerlinNoise(cloud.x * texScale + offsetX, cloud.y * texScale + offsetY);
                if (size > minNoiseSize)
                {
                    float localScaleX = cloud.scale.x;

                    if (!cloud.isActive)
                    {
                        cloud.SetActive(true);
                        cloud.scale = Vector3.zero;
                    }
                    if (localScaleX < maxScale)
                    {
                        ScaleCloud(cloud, 1);
                        if (cloud.scale.x > maxScale)
                        {
                            cloud.scale = new Vector3(maxScale, maxScale, maxScale);
                        }
                    }
                }
                else if (size < minNoiseSize)
                {
                    float localScaleX = cloud.scale.x;

                    ScaleCloud(cloud, -1);

                    if (localScaleX <= 0.1)
                    {
                        cloud.SetActive(false);
                        cloud.scale = Vector3.zero;
                    }
                }
            }
        }
    }

    private void ScaleCloud (CloudData cloud, int direction)
    {
        cloud.scale += new Vector3(sizeScale * Time.deltaTime * direction, sizeScale * Time.deltaTime * direction, sizeScale * Time.deltaTime * direction);
    }

    private void UpdateBatchList()
    {
        batchesToUpdate.Clear();
        foreach (var batch in batches)
        {
            if (CheckForActiveBatch(batch))
            {
                batchesToUpdate.Add(batch);
            }
        }
    }

    private void RenderBatches()
    {
        foreach (var batch in batchesToUpdate)
        {
            Graphics.DrawMeshInstanced(cloudMesh, 0, cloudMaterial, batch.Select((a) => a.matrix).ToList());
        }
    }
}

