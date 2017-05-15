using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//DEMO: when object collide all faces interacting should dissapear
public class Demo : MonoBehaviour {

    public Vector3[] newVertices;
    public Vector2[] newUV;
    public int[] newTriangles;

    List<Vector3> vert;
    List<Vector2> uv;
    List<int> tris;

    List<Vector3> removedTrisPoints;

    bool test = true;


    void OnCollisionStay(Collision collision)
    {
        GameObject go = collision.gameObject;
        Mesh meshhit = go.GetComponent<MeshFilter>().mesh;

        DestroyAllTrisInPath(meshhit);

        print(vert.Count + ", " + uv.Count);


        //redraw
        meshhit.Clear();

        meshhit.vertices = vert.ToArray();
        meshhit.uv = uv.ToArray();
        meshhit.triangles = tris.ToArray();

        meshhit.Optimize();
        meshhit.RecalculateNormals();

        MeshCollider mc = go.GetComponent<MeshCollider>();

        mc.sharedMesh = null;
        mc.sharedMesh = meshhit;

        for (int i = 0; i < meshhit.triangles.Length; i++)
        {
            Debug.DrawLine(meshhit.vertices[meshhit.triangles[i]], transform.position);
        }
    }

    private void DestroyAllTrisInPath(Mesh hit)
    {

        vert = new List<Vector3>(hit.vertices);
        uv = new List<Vector2>(hit.uv);
        tris = new List<int>(hit.triangles);

        if (test)
        {
            newVertices = vert.ToArray();
            newUV = uv.ToArray();
            newTriangles = tris.ToArray();
            test = false;
        }


        //remove all hitted tris
        removedTrisPoints = RaycastRemoveAllTris(transform.position, Vector3.down, 1000, ref tris, ref vert, ref uv);

        if (removedTrisPoints.Count == 0)
            return;

        //TODO: learn more about basic, simple, really simple vectors
    }

    private static List<Vector3> RaycastRemoveAllTris(Vector3 origin, Vector3 direction, int distance, ref List<int> tris, ref List<Vector3> vert, ref List<Vector2> uv)
    {//returns REMOVED triangles NOT RESULT
        List<Vector3> removedVertices = new List<Vector3>();

        RaycastHit[] hit = Physics.RaycastAll(origin, direction, distance);

        if (hit != null)
        {
            foreach (var data in hit)
            {
                if (data.triangleIndex < uv.Count && data.triangleIndex < tris.Count)
                {
                    removedVertices.Add(vert[ tris[data.triangleIndex]]);
                    removedVertices.Add(vert[tris[data.triangleIndex+1]]);
                    removedVertices.Add(vert[tris[data.triangleIndex+2]]);

                    int tcount = tris.Count;
                    tris = new List<int>(removeTriangle(data.triangleIndex, tris.ToArray()));

                    if(tris.Count<tcount)
                        uv.RemoveAt(data.triangleIndex);
                }
            }
        }
        return removedVertices;
    }

    static int[] removeTriangle(int triangle, int[] tris)
    {
        for (int i = triangle * 3; i < tris.Length - 3; ++i)
        {
            if (tris[i] == -1) break;
            tris[i] = tris[i + 3];
            print(i);
        }
        return tris;
    }
}