using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace g3
{
	public struct HashEdge
	{
		public HashGraph HashGraph => First.HashGraph;
		public int EId { get; set; }
		public HashVertex First { get; set; }
		public HashVertex Last { get; set; }
		public Segment2d Segment => new Segment2d(First.V, Last.V);

		public HashEdge(int eId, HashVertex prev, HashVertex next)
		{
			EId = eId;
			First = prev;
			Last = next;
		}

		public HashEdge SwapVertexes() => new(EId, Last, First);
	}

	public struct HashVertex
	{
		public HashGraph HashGraph { get; set; }
		public Vector2d V { get; set; } = Vector2d.MinValue;
		public int Id { get; set; } = -1;
		public int Hash { get; set; } = -1;
		public Dictionary<int, HashVertex> Neighbors => GetEdgeIdNeighbors();

		public HashVertex(HashGraph hashGraph)
		{
			HashGraph = hashGraph;
		}

		public HashVertex(HashGraph hashGraph, Vector2d v, int id, int hash) : this(hashGraph)
		{
			V = v;
			Id = id;
			Hash = hash;
		}

		public Dictionary<int, HashVertex> GetEdgeIdNeighbors()
		{
			Dictionary<int, HashVertex> result = new();
			var edgeIds = HashGraph.Graph.GetVtxEdges(Id);

			foreach(int eId in edgeIds)
			{
				result.Add(eId, GetNeighborVertex(eId));
			}

			return result;
		}

		public HashVertex GetNeighborVertex(int eId)
		{
			var edgeV = HashGraph.Graph.GetEdgeV(eId);
			if (edgeV == DGraph.InvalidEdgeV)
			{
				throw new Exception("Invalid edge id");
			}

			int nVId = edgeV.a == Id ? edgeV.b : edgeV.a;
			return HashGraph.GetHashVertex(nVId);
		}

		public static HashVertex FromGraph(HashGraph hashGraph, int vId)
		{
			return new() { Id = vId, V = hashGraph.GetVector(vId), Hash = hashGraph.GetVertexHash(vId) };
		}

		public static HashVertex FromGraph(HashGraph hashGraph, Vector2d v)
		{
			int vId = hashGraph.GetVertexId(v);
			return new() { V = v, Id = vId, Hash = hashGraph.GetVertexHash(vId) };
		}

		public override bool Equals(object obj)
		{
			return obj is HashVertex vertex &&
				   Hash == vertex.Hash;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Hash);
		}

		public static bool operator ==(HashVertex left, HashVertex right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(HashVertex left, HashVertex right)
		{
			return !(left == right);
		}
	}
}
