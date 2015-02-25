using UnityEngine;
using System.Collections;

public class scrLandscape : MonoBehaviour
{
	public static scrLandscape Instance { get; private set; }

	// Handy consts for grid generation.
	public const int GRID = 100;		// Verts per dimension. (1 + cells per dimension)
	public const int CELL_SCALE = 30;							// Units per dimension of each cell.
	public const int GRID_SCALE = (GRID - 1) * CELL_SCALE;		// Units per dimension of grid.
	public const float HALF_GRID_SCALE = GRID_SCALE * 0.5f;	// Half the grid scale, handy for removing tonnes of multiplication.
	
	// Same as above except used to make the mesh collider, which should be smaller than the visible grid.
	public const int PHYS_GRID = 20;
	public const int PHYS_CELL_SCALE = 30;
	public const int PHYS_GRID_SCALE = (PHYS_GRID - 1) * PHYS_CELL_SCALE;
	public const float HALF_PHYS_GRID_SCALE = PHYS_GRID_SCALE * 0.5f;

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

	Vector3[] physVertices;
	int[] physTriangles;
	MeshCollider meshCollider;

	
	public Vector3 LowestPoint { get; private set; }	// The lowest point on the terrain.
	public Vector3 HighestPoint { get; private set; }	// The highest point on the terrain.

	// Use this for initialization
	void Start ()
	{
		Instance = this;

		vertices = new Vector3[GRID * GRID];
		uv = new Vector2[vertices.Length];
		triangles = new int[(GRID - 1) * (GRID - 1) * 6];
		meshFilter = GetComponent<MeshFilter>();

		physVertices = new Vector3[PHYS_GRID * PHYS_GRID];
		physTriangles = new int[(PHYS_GRID - 1) * (PHYS_GRID - 1) * 6];
		meshCollider = GetComponent<MeshCollider>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		GenerateVertices();
	}

	public Vector2 GetCentre() { return origin; }
	public float GetLeft() { return origin.x - HALF_GRID_SCALE; }
	public float GetRight() { return origin.x + HALF_GRID_SCALE; }
	public float GetFront() { return origin.y + HALF_GRID_SCALE; }
	public float GetBack() { return origin.y - HALF_GRID_SCALE; }
	public float GetPhysLeft() { return origin.x - HALF_PHYS_GRID_SCALE; }
	public float GetPhysRight() { return origin.x + HALF_PHYS_GRID_SCALE; }
	public float GetPhysFront() { return origin.y + HALF_PHYS_GRID_SCALE; }
	public float GetPhysBack() { return origin.y - HALF_PHYS_GRID_SCALE; }

	
	public float GetHeight(float x, float z)
	{
		float noiseHeight = GetHeightFromNoise(x, z);

		// Cast a ray down to the mesh from the height and return the hit normal.
		RaycastHit hit;
		if (Physics.Raycast(new Vector3(x, noiseHeight + 1, z), Vector3.down, out hit, 1000, 1 << gameObject.layer))
			return hit.point.y;
		return noiseHeight;
	}

	public float GetHeightFromNoise(float x, float z)
	{
		if (Uniformity == 0)
			return 0;

		// Return perlin noise, offset by the seed, with size based on flatness.
		return Mathf.PerlinNoise(x / Uniformity + Seed, z / Uniformity + Seed) * MaxHeight;
	}

	public Vector3 GetNormal(float x, float z)
	{
		// Cast a ray down to the mesh from the height and return the hit normal.
		RaycastHit hit;
		if (Physics.Raycast(new Vector3(x, GetHeight(x, z) + 1, z), Vector3.down, out hit, 1000, 1 << gameObject.layer))
			return hit.normal;

		return GetNormalFromNoise(x, z, 1.0f);
	}

	// Gets the normal from the noise information. Lower precision value = more precise, can't think of a better word for the value...
	public Vector3 GetNormalFromNoise(float x, float z, float precision)
	{
		Vector3 position = new Vector3(x, GetHeight (x, z), z);

		Vector2 dirX = new Vector2(x + precision, GetHeight(x + precision, z)) - new Vector2(x - precision, GetHeight(x - precision, z));
		Vector2 dirZ = new Vector2(GetHeight (x, z + precision), z + precision) - new Vector2(GetHeight (x, z - precision), z - precision); // .y = z, .x = y

		Vector3 normalX = new Vector3(-dirX.y, Mathf.Abs (dirX.x), 0);
		Vector3 normalZ = new Vector3(0, Mathf.Abs (dirZ.y), -dirZ.x);	// .y = z, .x = y

		return (normalX + normalZ) / 2.0f;
	}

	public bool Contains(Vector3 position)
	{
		return !(position.x < GetLeft () || position.x > GetRight () || position.z < GetBack() || position.z > GetFront ());
	}

	public bool PhysContains(Vector3 position)
	{
		return !(position.x < GetPhysLeft () || position.x > GetPhysRight () || position.z < GetPhysBack() || position.z > GetPhysFront ());
	}
	
	// Generates all vertices.
	void GenerateVertices()
	{
		LowestPoint = new Vector3(0, float.MaxValue, 0);
		HighestPoint = new Vector3(0, float.MinValue, 0);

		origin = new Vector2((int)((Player.position.x) / CELL_SCALE) * CELL_SCALE,
		                     (int)((Player.position.z) / CELL_SCALE) * CELL_SCALE);

		transform.position = new Vector3(origin.x, transform.position.y, origin.y);;
		
		float lx = 0, ly = 0, lz = 0;	// Local x, y, z.
		float wx = 0, wz = 0;	// World x, z.

		for (int x = 0; x < GRID; ++x)
		{
			lx = x * CELL_SCALE - HALF_GRID_SCALE;
			wx = lx + origin.x;
			
			for (int z = 0; z < GRID; ++z)
			{
				lz = z * CELL_SCALE - HALF_GRID_SCALE;
				wz = lz + origin.y;
				
				// Get the height using the world coordinates, not the local coordinates.
				ly = GetHeightFromNoise(wx, wz);

				// Update the lowest and highest points.
				if (ly < LowestPoint.y)
					LowestPoint = new Vector3(wx, ly, wz);
				if (ly > HighestPoint.y)
					HighestPoint = new Vector3(wx, ly, wz);
				
				int index = z * GRID + x;
				vertices[index].x = lx;
				vertices[index].y = ly;
				vertices[index].z = lz;
				uv[index].x = (float)wx / GRID;
				uv[index].y = (float)wz / GRID;
			}
		}


		for (int s = 0; s < Smooth; ++s)
		{
			// Post blur on y values.
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

		// -- Physics --

		// Physics vertices.
		for (int x = 0; x < PHYS_GRID; ++x)
		{
			lx = x * PHYS_CELL_SCALE - HALF_PHYS_GRID_SCALE;

			for (int z = 0; z < PHYS_GRID; ++z)
			{
				lz = z * PHYS_CELL_SCALE - HALF_PHYS_GRID_SCALE;

				ly = GetHeightFromNoise(lx + origin.x, lz + origin.y);
				
				int index = z * PHYS_GRID + x;
				physVertices[index].x = lx;
				physVertices[index].y = ly;
				physVertices[index].z = lz;
			}
		}

		// Physics triangles.
		for (int t = 0, v = 0; t < physTriangles.Length; t += 6)
		{
			// Triangle 1
			physTriangles[t] = v;
			physTriangles[t + 1] = v + PHYS_GRID;
			physTriangles[t + 2] = v + PHYS_GRID + 1;
			
			// Triangle 2
			physTriangles[t + 3] = v;
			physTriangles[t + 4] = v + PHYS_GRID + 1;
			physTriangles[t + 5] = v + 1;
			
			// Skip the far vertex.
			if (v % PHYS_GRID == PHYS_GRID - 2)
				v += 2;
			else
				++v;
		}

		Mesh physMesh = new Mesh();
		physMesh.vertices = physVertices;
		physMesh.triangles = physTriangles;
		physMesh.RecalculateNormals();
		//meshCollider.sharedMesh = null;
		meshCollider.sharedMesh = physMesh;

	}
}