using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace g3
{
	public class HashGraph
	{
		public static double VectorToHashRoundCount { get; set; } = 10;
		public DGraph2 Graph => _graph; //maybe need to return cloned graph for allow synchronization with hashes
		protected DGraph2 _graph = new();

		public int EdgesCount => _graph.EdgeCount;
		public int VertexesCount => _graph.VertexCount;
		public IEnumerable<Vector2d> Vectors => GetVectors();
		public HashSet<Vector2d> Junctions => GetJunctionVectors();

		protected Dictionary<int, int> _hashToVertexIdMap = new();
		protected Dictionary<int, int> _vertexIdToHashMap = new();

		//protected Dictionary<int, int> _hashToEdgeIdMap = new();

		public HashGraph()
		{

		}

		public HashGraph(Vector2d edgeStart, Vector2d edgeEnd)
		{
			AppendEdge(edgeStart, edgeEnd);
		}


		public HashGraph FindConnectedEdges(HashGraph connection)
		{
			HashGraph result = new();

			foreach (var vHash in connection.GetVertexHashes())
			{
				if (TryGetVertexId(vHash, out int vId))
				{
					Vector2d v = GetVector(vId);
					foreach (int vIdNeigh in Graph.VtxVerticesItr(vId))
					{
						Vector2d vNeigh = GetVector(vIdNeigh);

						result.AppendEdge(v, vNeigh);
					}
				}
			}

			return result;
		}

		public List<int> FindConnectedEdgesIds(HashGraph connection)
		{
			List<int> result = new();

			foreach (var vHash in connection.GetVertexHashes())
			{
				if (TryGetVertexId(vHash, out int vId) == false)
				{
					continue;
				}

				foreach (int vIdNeigh in Graph.VtxEdgesItr(vId))
				{
					result.Add(vIdNeigh);
				}
			}

			return result;
		}

		//Serving Methods

		public void AppendGraphOnlyEdges(HashGraph toAppend)
		{
			foreach (var edgeId in toAppend.Graph.EdgeIndices())
			{
				Vector2d start = Vector2d.Zero;
				Vector2d end = Vector2d.Zero;

				if (toAppend.Graph.GetEdgeV(edgeId, ref start, ref end) == false)
				{
					throw new Exception("Edge is not exist");
				}

				AppendEdge(start, end);
			}
		}

		public int GetVertexEdgesCountByHash(int vHash)
		{
			return _graph.GetVtxEdgeCount(_hashToVertexIdMap[vHash]);
		}

		public IEnumerable<(Vector2d left, Vector2d right)> GetEdgesVectors()
		{
			foreach (var eId in _graph.EdgeIndices())
			{
				yield return GetEdgeV(eId);
			}
		}

		public IEnumerable<Segment2d> GetEdgesSegments()
		{
			foreach (var eId in _graph.EdgeIndices())
			{
				yield return _graph.GetEdgeSegment(eId);
			}
		}

		public IEnumerable<int> GetEdgesIndices(int vHash)
		{
			if (_hashToVertexIdMap.ContainsKey(vHash) == false)
			{
				return Enumerable.Empty<int>();
			}

			return _graph.GetVtxEdges(_hashToVertexIdMap[vHash]);
		}

		public IEnumerable<Vector2d> GetNeighborVectors(Vector2d v)
		{
			return GetNeighborVectorsByHash(ToHash(v));
		}

		public IEnumerable<Vector2d> GetNeighborVectors(int vId)
		{
			foreach (var nVId in _graph.VtxVerticesItr(vId))
			{
				yield return _graph.GetVertex(nVId);
			}
		}

		public IEnumerable<Vector2d> GetNeighborVectorsByHash(int vHash)
		{

			if (TryGetVertexId(vHash, out int vId))
			{
				foreach (var nVId in _graph.VtxVerticesItr(vId))
				{
					yield return _graph.GetVertex(nVId);
				}
			}
		}

		public (Vector2d left, Vector2d right) GetEdgeV(int eId)
		{
			Vector2d left = Vector2d.Zero;
			Vector2d right = Vector2d.Zero;
			if (_graph.GetEdgeV(eId, ref left, ref right) == false)
			{
				throw new Exception($"Edge {eId} not found");
			}

			return (left, right);
		}

		public bool TryGetEdgeV(int eId, [MaybeNullWhen(false)] out Vector2d one, [MaybeNullWhen(false)] out Vector2d two)
		{
			one = Vector2d.Zero;
			two = Vector2d.Zero;
			return _graph.GetEdgeV(eId, ref one, ref two);
		}


		public IEnumerable<int> GetVertexHashes()
		{
			return _hashToVertexIdMap.Keys;
		}

		public int GetVertexId(int hashVertex)
		{
			return _hashToVertexIdMap[hashVertex];
		}
		public int GetVertexId(Vector2d vertex)
		{
			return GetVertexId(ToHash(vertex));
		}

		public int GetVertexHash(int vId)
		{
			return _vertexIdToHashMap[vId];
		}

		public IEnumerable<Vector2d> GetVectors()
		{
			return Graph.Vertices();
		}

		public Vector2d GetVector(int vId)
		{
			return Graph.GetVertex(vId);
		}

		public Vector2d GetVectorByHash(int hash)
		{
			return Graph.GetVertex(_hashToVertexIdMap[hash]);
		}

		public HashSet<Vector2d> GetJunctionVectors()
		{
			var graph = Graph;
			HashSet<Vector2d> junctions = new();
			foreach (int vId in graph.VertexIndices())
			{
				if (graph.IsJunctionVertex(vId))
				{
					junctions.Add(GetVector(vId));
				}
			}

			return junctions;
		}

		public HashSet<int> GetJunctionsIds()
		{
			var graph = Graph;
			HashSet<int> junctionsIds = new();
			foreach (int vId in graph.VertexIndices())
			{
				if (graph.IsJunctionVertex(vId))
				{
					junctionsIds.Add(vId);
				}
			}

			return junctionsIds;
		}

		public HashSet<int> GetJunctionsHashes()
		{
			var graph = Graph;
			HashSet<int> junctionsHashes = new();
			foreach (int vId in graph.VertexIndices())
			{
				if (graph.IsJunctionVertex(vId))
				{
					junctionsHashes.Add(_vertexIdToHashMap[vId]);
				}
			}

			return junctionsHashes;
		}

		public bool ContainsVertexId(int vId)
		{
			return _vertexIdToHashMap.ContainsKey(vId);
		}

		public bool ContainsVertexHash(int hash)
		{
			return _hashToVertexIdMap.ContainsKey(hash);
		}

		public bool ContainsVertex(Vector2d vector)
		{
			return ContainsVertexHash(ToHash(vector));
		}

		public bool TryGetVertexId(int hashVertex, out int result)
		{
			return _hashToVertexIdMap.TryGetValue(hashVertex, out result);
		}
		public bool TryGetVertexId(Vector2d vector, [MaybeNullWhen(false)] out int result)
		{
			return TryGetVertexId(ToHash(vector), out result);
		}

		public bool TryGetVertexHash(int vId, out int result)
		{
			return _vertexIdToHashMap.TryGetValue(vId, out result);
		}

		public bool TryGetVector(int vId, [MaybeNullWhen(false)] out Vector2d vec)
		{
			if (Graph.IsVertex(vId))
			{
				vec = Graph.GetVertex(vId);
				return true;
			}

			vec = default(Vector2d);
			return false;
		}

		public bool TryGetVectorByHash(int vHash, [MaybeNullWhen(false)] out Vector2d vec)
		{
			if (TryGetVertexId(vHash, out int vId) == false)
			{
				vec = default(Vector2d);
				return false;
			}

			vec = Graph.GetVertex(vId);
			return true;
		}

		public int AppendVertex(Vector2d vec)
		{
			return GetOrAddVertex(vec);
		}

		public int AppendEdge(Vector2d edgeStart, Vector2d edgeEnd)
		{
			int vid0 = GetOrAddVertex(edgeStart);
			int vid1 = GetOrAddVertex(edgeEnd);
			return Graph.AppendEdge(vid0, vid1);
		}

		public int AppendEdge(int edgeStartHash, int edgeEndHash)
		{
			if (TryGetVertexId(edgeStartHash, out int vid0) && TryGetVertexId(edgeEndHash, out int vid1))
			{
				return Graph.AppendEdge(vid0, vid1);
			}

			return -1;
		}

		public int SetVertex(int vId, Vector2d newPos)
		{
			int newHash = HashGraph.ToHash(newPos);

			if (_vertexIdToHashMap.TryGetValue(vId, out int vHash))
			{
				_graph.SetVertex(vId, newPos);
				_hashToVertexIdMap.Remove(vHash);

				_vertexIdToHashMap[vId] = newHash;
				_hashToVertexIdMap[newHash] = vId;

				return newHash;
			}

			throw new Exception($"Can`t find vertex with {vHash} hash");
		}

		public int SetVertexByHash(int vHash, Vector2d newPos)
		{
			if (TryGetVertexId(vHash, out int vId))
			{
				int newHash = ToHash(newPos);

				_hashToVertexIdMap.Remove(vHash);

				//if there is no point in new position, just move existing point and refresh hash map
				if (TryGetVertexId(newHash, out int newId) == false)
				{
					_graph.SetVertex(vId, newPos);

					_vertexIdToHashMap[vId] = newHash;
					_hashToVertexIdMap[newHash] = vId;

					return newHash;
				}

				//If there exists point in new position, need to reconnect all connected edges
				var edgesIds = _graph.GetVtxEdges(vId).ToArray();

				bool isVertexRemoved = false;
				foreach (var edgeId in edgesIds)
				{
					var edgeSeg = _graph.GetEdgeSegment(edgeId);

					var twoHash = ToHash(edgeSeg.P0);
					if (twoHash == vHash)
					{
						twoHash = ToHash(edgeSeg.P1);
					}

					if (newHash != twoHash)
					{
						int eIdNew = AppendEdge(newHash, twoHash);
					}

					_graph.RemoveEdge(edgeId, true);
					isVertexRemoved = true;
				}

				if (isVertexRemoved == false)
				{
					_graph.RemoveVertex(vId, true);
				}

				_vertexIdToHashMap.Remove(vId);

				return newHash;
			}

			throw new Exception($"Cant find vertex with {vHash} hash");
		}

		public bool RemoveVertex(int vId)
		{
			if (Graph.RemoveVertex(vId, true) == MeshResult.Ok)
			{
				if (TryGetVertexHash(vId, out int vHash))
				{
					_hashToVertexIdMap.Remove(vHash);
					_vertexIdToHashMap.Remove(vId);
				}

				return true;
			}

			return false;
		}

		public bool RemoveVertexByHash(int vHash)
		{
			if (TryGetVertexId(vHash, out int vId) && Graph.RemoveVertex(vId, true) == MeshResult.Ok)
			{
				_hashToVertexIdMap.Remove(vHash);
				_vertexIdToHashMap.Remove(vId);

				return true;
			}

			return false;
		}

		protected int GetOrAddVertex(Vector2d vertex)
		{
			var vertexHash = ToHash(vertex);

			if (!_hashToVertexIdMap.TryGetValue(vertexHash, out int vid))
			{
				vid = Graph.AppendVertex(vertex);

				_vertexIdToHashMap.Add(vid, vertexHash);
				_hashToVertexIdMap.Add(vertexHash, vid);
			}

			return vid;
		}

		//This is so slow. Need to clone it faster
		public HashGraph Clone()
		{
			HashGraph newGraph = new()
			{
				_graph = _graph.Clone(),
				_hashToVertexIdMap = new(_hashToVertexIdMap),
				_vertexIdToHashMap = new(_vertexIdToHashMap),
			};

			return newGraph;

			foreach (var seg in GetEdgesSegments())
			{
				newGraph.AppendEdge(seg.P0, seg.P1);
			}

			foreach (var v in Vectors)
			{
				newGraph.AppendVertex(v);
			}

			return newGraph;
			return new HashGraph()
			{
				_graph = new DGraph2(_graph),
				_hashToVertexIdMap = new(_hashToVertexIdMap),
				_vertexIdToHashMap = new(_vertexIdToHashMap),
			};
		}

		//TODO: Math.Round may give negative effect for performance.
		//But in some cases (then double value after decimal point in period) without rounding hash function return little bit wrong results.
		public static int ToHash(Vector2d vector)
		{
			unchecked // Overflow is fine, just wrap
			{
				VectorToHashRoundCount = 10;
				int hash = (int)2166136261;
				// Suitable nullity checks etc, of course :)
				int x = (int)Math.Round(vector.x * VectorToHashRoundCount);
				int y = (int)Math.Round(vector.y * VectorToHashRoundCount);

				hash = (hash * 16777619) ^ x.GetHashCode();
				hash = (hash * 16777619) ^ y.GetHashCode();
				return hash;
			}

			//int hash1 = (int)(vector.x * VectorToHashRoundCount);
			//int hash2 = (int)(vector.y * VectorToHashRoundCount);
			//return HashCode.Combine(hash1, hash2);

			return HashCode.Combine((int)(vector.x * VectorToHashRoundCount), (int)(vector.y * VectorToHashRoundCount));
		}

		public static int ToHash(Vector2d vector, double roundCount)
		{
			unchecked // Overflow is fine, just wrap
			{
				roundCount = 10;
				int hash = (int)2166136261;
				// Suitable nullity checks etc, of course :)
				int x = (int)Math.Round(vector.x * roundCount);
				int y = (int)Math.Round(vector.y * roundCount);

				hash = (hash * 16777619) ^ x.GetHashCode();
				hash = (hash * 16777619) ^ y.GetHashCode();
				return hash;
			}
		}

		public static bool EqualHashes(Vector2d one, Vector2d two)
		{
			return ToHash(one) == ToHash(two);
		}
		//protected int ToHash(SharedEdge edge) => HashCode.Combine(edge.Start, edge.End);
		////private int ToHash(Corner corner) => HashCode.Combine((int)corner.X * Defaults.HashTolerance, (int)corner.Y * Defaults.HashTolerance);
		//protected int ToHash(g3.Vector2d vector) => HashCode.Combine((int)vector.x * HashTolerance, (int)vector.y * HashTolerance);
		//protected int ToHash(SharpSkeleton.Primitives.Tuple3d vector) => HashCode.Combine((int)vector.X * HashTolerance, (int)vector.Y * HashTolerance);
		//protected int ToHash(SharpSkeleton.Primitives.Tuple2d vector) => HashCode.Combine((int)vector.X * HashTolerance, (int)vector.Y * HashTolerance);
	}
}
