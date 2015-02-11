using UnityEngine;
using System.Collections;

public class scrLandscape : MonoBehaviour
{
	public static scrLandscape Instance { get; private set; }

	// Handy consts for grid generation.
	const int GRID = 100;		// Verts per dimension. (1 + cells per dimension)
	const int CELL_SCALE = 5;							// Units per dimension of each cell.
	const int GRID_SCALE = (GRID - 1) * CELL_SCALE;		// Units per dimension of grid.
	const float HALF_GRID_SCALE = GRID_SCALE * 0.5f;	// Half the grid scale, handy for removing tonnes of multiplication.

	// For blurring.
	Vector2[] neighbourOffsets = new Vector2[] { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(1, 1) };

	public float MaxHeight = 10;	// Maximum possible height of the terrain.
	public float Uniformity = 128;	// Size of random noise, higher = more uniform terrain.
	public int Seed = 0;	// Seed for random noise.
	public int Smooth = 0;	// Blur passes.

	public Transform Player;

	Vector2 origin;

	Vector3[] vertices;
	Vector2[] uv;
	int[] triangles;	
	MeshFilter meshFilter;
	MeshCollider meshCollider;

	// Use this for initialization
	void Start ()
	{
		Instance = this;

		meshFilter = GetComponent<MeshFilter>();
		meshCollider = GetComponent<MeshCollider>();

		vertices = new Vector3[GRID * GRID];
		uv = new Vector2[vertices.Length];
		triangles = new int[(GRID - 1) * (GRID - 1) * 6];
		
		StartCoroutine(GenerateVertices());
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public Vector2 GetCentre() { return origin + new Vector2(HALF_GRID_SCALE, HALF_GRID_SCALE); }
	public float GetLeft() { return origin.x; }
	public float GetRight() { return origin.x + GRID_SCALE; }
	public float GetFront() { return origin.y + GRID_SCALE; }
	public float GetBack() { return origin.y; }

	public float GetHeight(float x, float z)
	{
		// Return perlin noise, offset by the seed, with size based on flatness.
		return Mathf.PerlinNoise(x / Uniformity + Seed, z / Uniformity + Seed) * MaxHeight;
	}
	
	// Generates all vertices. Implemented as a coroutine so that it can be made to yield in case of heavy processing.
	IEnumerator GenerateVertices()
	{
		// Forever.
		while (true)
		{
			origin = new Vector2((int)((Player.position.x - HALF_GRID_SCALE) / CELL_SCALE) * CELL_SCALE,
			                     (int)((Player.position.z - HALF_GRID_SCALE) / CELL_SCALE) * CELL_SCALE);
			
			
			float wx = 0, wy = 0, wz = 0;	// World x, y, z.

			for (int x = 0; x < GRID; ++x)
			{
				wx = origin.x + x * CELL_SCALE;

				
				for (int z = 0; z < GRID; ++z)
				{
					wz = origin.y + z * CELL_SCALE;
					

					wy = GetHeight(wx, wz);
					
					int index = z * GRID + x;
					vertices[index].x = wx;
					vertices[index].y = wy;
					vertices[index].z = wz;
					uv[index].x = (float)wx / GRID;
					uv[index].y = (float)wz / GRID;
				}
			}


			for (int s = 0; s < Smooth; ++s)
			{
				// Post gaussian on y values.
				float[] gaussY = new float[GRID * GRID];
				for (int x = 1; x < GRID - 1; ++x)
				{
					for (int z = 1; z < GRID - 1; ++z)
					{
						int index = z * GRID + x;
						gaussY[index] = vertices[index].y;
						for (int i = 0; i < neighbourOffsets.Length; ++i) 
						{
							gaussY[index] += vertices[(z + (int)neighbourOffsets[i].y) * GRID + x + (int)neighbourOffsets[i].x].y;
						}
						gaussY[index] /= neighbourOffsets.Length + 1;
					}
				}

				for (int x = 1; x < GRID - 1; ++x)
					for (int z = 1; z < GRID - 1; ++z)
						vertices[z * GRID + x].y = gaussY[z * GRID + x];
			}


			for (int t = 0, v = 0; t < triangles.Length; t += 6)
			{
				// Triangle 1
				triangles[t] = v;
				triangles[t + 1] = v + GRID;
				triangles[t + 2] = v + GRID + 1;
				
				// Triangle 2
				triangles[t + 3] = v;
				triangles[t + 4] = v + GRID + 1;
				triangles[t + 5] = v + 1;
				
				// Skip the far vertex.
				if (v % GRID == GRID - 2)
					v += 2;
				else
					++v;
			}
			
			Mesh mesh = meshFilter.mesh;
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			meshFilter.mesh = mesh;
			meshCollider.sharedMesh = meshFilter.mesh;
			
			yield return new WaitForEndOfFrame();
		}
	}
}