using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Laser : MonoBehaviour {

	public Material LaserMaterial;
	public Material LaserPreviewMaterial;
	public float Width = 0.25f;
	bool bFiring;

	class LaserHitComparer : IComparer<RaycastHit>
	{
		public Vector3 SortDirection;

		public int Compare(RaycastHit a, RaycastHit b)
		{
			float da = Vector3.Dot(SortDirection, a.point);
			float db = Vector3.Dot(SortDirection, b.point);
			if (da > db)
				return 1;
			return -1;
		}
	}

	LaserHitComparer HitComparer = new LaserHitComparer ();

	struct LaserSegment
	{
		public Vector3 Origin;
		public Vector3 End;
		public LaserSegment(Vector3 org, Vector3 end)
		{
			Origin = org;
			End = end;
		}
	}

	const int MaxSegments = 128;
	LaserSegment[] DrawSegments = new LaserSegment[MaxSegments];
	int SegmentCount;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		SegmentCount = 0;
		bFiring = Input.GetMouseButton (0);
		TraceLaser ();
	}

	void TraceLaser() {
		Ray ray = new Ray(transform.position, transform.right);

		bool bContinue = true;
		while (bContinue) {
			bContinue = false;

			RaycastHit[] hits = Physics.RaycastAll(ray);

			HitComparer.SortDirection = ray.direction;
			Array.Sort(hits, HitComparer);

			bool bLaserDrawn = false;
			bool bStopHits = false;
			foreach (RaycastHit hit in hits) {
				var dmgCmp = hit.collider.gameObject.GetComponent<InteractsWithLaser>();
				if (null == dmgCmp)
					continue;

				var result = dmgCmp.OnLaserHit(this, !bFiring);

				switch (result.ResponseType) {
					case ELaserResponse.Continue:
						break;
					case ELaserResponse.Stop:
						AddLaserSegment(ray.origin, hit.point);
						bLaserDrawn = true;
						bStopHits = true;
						break;
					case ELaserResponse.Bounce:
						AddLaserSegment(ray.origin, hit.point);
						bLaserDrawn = true;
						ray.origin = hit.point;
						Vector3 norm = hit.normal;
						ray.direction = Vector3.Reflect(ray.direction, norm);
						bStopHits = true;
						bContinue = true;
						break;
				}

				if (bStopHits)
					break;
			}

			if (!bLaserDrawn)
				AddLaserSegment(ray.origin, ray.origin + ray.direction * 1000);
		}
	}

	void AddLaserSegment(Vector3 origin, Vector3 end) {
		if (SegmentCount == MaxSegments)
			return;
		DrawSegments [SegmentCount++] = new LaserSegment (origin, end);
	}

	void OnGUI()
	{
		for (int s = 0; s < SegmentCount; s++) {
			LaserSegment segment = DrawSegments[s];
			DrawLaser(segment.Origin, segment.End);
		}
	}

	void DrawLaser(Vector3 origin, Vector3 end)
	{
		Material mat = bFiring ? LaserMaterial : LaserPreviewMaterial;
		if (null == mat) {
			Debug.LogError ("You forgot to set a laser material.");
			return;
		}

		mat.SetPass (0);
		
		Vector3 dir = (end - origin).normalized;
		Vector3 right = 0.5f * Width * new Vector3 (dir.z, -dir.y, -dir.x).normalized;

		GL.Begin (GL.QUADS);
		GL.Color (Color.red);

		GL.Vertex (origin + right);
		GL.Vertex (origin - right);
		GL.Vertex (end - right);
		GL.Vertex (end + right);

		GL.End ();
	}
}
