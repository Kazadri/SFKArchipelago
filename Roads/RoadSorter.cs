using System.Collections.Generic;
using System.Linq;
using SuperFantasyKingdom;
using UnityEngine;

namespace SfkArchipelago.Roads
{
    internal static class RoadSorter
    {
        private static readonly Vector2Int[] Dirs =
        {
            new Vector2Int(0, 1), new Vector2Int(1, 0),
            new Vector2Int(0, -1), new Vector2Int(-1, 0)
        };

        private static RoadGroup GroupOf(Road road)
        {
            return road.pier != null ? RoadGroup.Pier : road.city ? RoadGroup.City : RoadGroup.Overland;
        }

        public static string[] Compute(Road[] roads, RoadGroup group)
        {
            var nodes = new List<Node>();
            for (var i = 0; i < roads.Length; i++)
            {
                var r = roads[i];
                if (GroupOf(r) != group) continue;

                if (r.parts == null || r.parts.Count == 0)
                {
                    Plugin.Log.LogWarning($"Road {r.Id}: no parts at snapshot time, skipped");
                    continue;
                }

                var n = new Node
                {
                    Index = i,
                    Road = r,
                    Start = Round(r.parts[0].Position)
                };

                foreach (var p in r.parts) n.Tiles.Add(Round(p.Position));

                var last = r.parts[r.parts.Count - 1];
                n.Tiles.Add(Round(last.Position) + Dirs[last.Direction % 4]);
                nodes.Add(n);
            }

            foreach (var n in nodes)
                n.Parents = nodes
                    .Where(p => p != n && p.Start != n.Start && p.Tiles.Contains(n.Start))
                    .ToList();


            var placed = new HashSet<Node>();
            var result = new List<Node>();
            var remaining = new List<Node>(nodes);

            while (remaining.Count > 0)
            {
                var ready = remaining
                    .Where(n => n.Parents.Count == 0 || n.Parents.Any(placed.Contains))
                    .ToList();

                if (ready.Count == 0)
                {
                    Plugin.Log.LogWarning($"Dependency cycle among {remaining.Count} roads, forcing one");
                    ready = remaining;
                }

                var next = ready
                    .OrderBy(n => n.Road.requiredKingdomLevel > 0 ? 1 : 0)
                    .ThenBy(n => n.Road.requiredGlory)
                    .ThenBy(n => n.Road.requiredKingdomLevel)
                    .ThenBy(n => n.Road.cost.amount)
                    .ThenBy(n => n.Index)
                    .First();

                placed.Add(next);
                result.Add(next);
                remaining.Remove(next);
            }

            return result.Select(n => n.Road.Id).ToArray();
        }

        private static Vector2Int Round(Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }

        private class Node
        {
            public readonly HashSet<Vector2Int> Tiles = new HashSet<Vector2Int>();
            public int Index;
            public List<Node> Parents = new List<Node>();
            public Road Road;
            public Vector2Int Start;
        }
    }
}