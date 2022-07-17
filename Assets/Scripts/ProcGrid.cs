using UnityEngine;
using System.Collections;

//This grid is going to need to be displayed, without these components 
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProcGrid : MonoBehaviour {
    private Mesh mesh;
    
    public int xSize, ySize; //The size of our mesh

    private Vector3[] vertices; //A list of vertices for us to place our meshes onto our grid.
    //We settle the position of each vertex first then focus on the actual triangles of the mesh.
    
    private void Generate() {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";
        
        //Vertices can be reused but not at every point. There is one more vertex than there are tiles.
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length]; //Beginning to make an albedo map for detail
        //The normal determines up, the tangent tells us what is to the right.
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);//Usually it is not objectively to the right like it is
        //here. But because our surface is flat, it will work fine.
        for (int i = 0, y = 0; y <= ySize; y++) {
            for (int x = 0; x <= xSize; x++, i++) {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x/xSize, (float)y/ySize);
                tangents[i] = tangent;
            }
        }
        mesh.vertices = vertices; //Now that we have established the vertices, we pass them to the mesh here
        mesh.uv = uv;
        mesh.tangents = tangents;
        
        //Drawing the triangles.
        int[] triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); //If we didn't do it this way, we'd have to fill another vector array for each
        //triangle's normal. I'm not gonna do that if that's all the same to everyone here.
    }
    
    private void Awake () {
        Generate();
    }
    
    //Highlight the locations of the vertices to visualize what is happening.
    private void OnDrawGizmos () {
        //It'd be a good idea to make sure that the vertices array of Vector3s actually exists before
        //trying to draw something based on the data contained in it.
        if (vertices == null) {
            return;
        }
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++) {
            Gizmos.DrawSphere(transform.TransformPoint(vertices[i]), 0.1f);
        }
    }
}