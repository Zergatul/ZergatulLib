using System;
using System.Collections.Generic;
using Zergatul.Algo.DataStructure;

namespace Zergatul.Algo.Geometry
{
    public class VoronoiDiagram
    {
        public Point2D[] Points { get; }
        public List<HalfEdge> HalfEdges { get; }
        public List<Vertex> Vertices { get; }
        public List<HalfEdge> Faces { get; }

        private VoronoiDiagram(Point2D[] points, List<HalfEdge> halfEdges, List<Vertex> vertices, List<HalfEdge> faces)
        {
            Points = points;
            HalfEdges = halfEdges;
            Vertices = vertices;
            Faces = faces;
        }

#if DEBUG

        public string ToPython()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("import matplotlib.pyplot as plt");
            sb.AppendLine();

            for (int i = 0; i < Points.Length; i++)
            {
                sb.AppendLine($"plt.plot({Points[i].X}, {Points[i].Y}, 'ro', markersize=2)");
            }

            sb.AppendLine();

            void initEdgePointsVis(HalfEdge h, out Point2D p1, out Point2D p2)
            {
                if (h.vertex != null && h.twin.vertex != null)
                {
                    p1 = h.vertex.Point;
                    p2 = h.twin.vertex.Point;

                }
                else if (h.vertex != null)
                {
                    p1 = h.vertex.Point;

                    Vector2D norm = (Points[h.LeftIndex] - Points[h.RightIndex]).Normalized.Rot90Ccw;
                    p2 = new Point2D(p1.X + norm.X * 1000, p1.Y + norm.Y * 1000);

                }
                else if (h.twin.vertex != null)
                {
                    p1 = h.twin.vertex.Point;

                    Vector2D norm = (Points[h.twin.LeftIndex] - Points[h.twin.RightIndex]).Normalized.Rot90Ccw;
                    p2 = new Point2D(p1.X + norm.X * 1000, p1.Y + norm.Y * 1000);
                }
                else
                {
                    Point2D pl = Points[h.LeftIndex], pr = Points[h.RightIndex];

                    Vector2D norm = (pl - pr).Normalized.Rot90Ccw;
                    Point2D c = Point2D.Center(pl, pr);

                    p1 = new Point2D(c.X + norm.X * 1000, c.Y + norm.Y * 1000);
                    p2 = new Point2D(c.X - norm.X * 1000, c.Y - norm.Y * 1000);
                }
            }

            for (int i = 0; i < HalfEdges.Count; i++)
            {
                initEdgePointsVis(HalfEdges[i], out var p1, out var p2);
                sb.AppendLine($"plt.plot([{p1.X}, {p2.X}], [{p1.Y}, {p2.Y}], color='black', linewidth=1)");
            }

            return sb.ToString();
        }

#endif

        public static VoronoiDiagram Fortune(
            Point2D[] points,
            double pointEpsilon = 1e-6,
            double circleCenterEpsilon = 1e-7,
            double breakPointsEpsilon = 1e-5)
        {
            Event[] initial = new Event[points.Length];
            for (int i = 0; i < initial.Length; i++)
                initial[i] = Event.CreateSite(i, points[i]);

            var queue = new BinaryHeap1<Event>(points.Length);
            queue.AddRange(initial);

            var halfEdges = new List<HalfEdge>();
            var vertices = new List<Vertex>();

            BeachLineNode root = null;
            double sweepline;

            #region local funcs

            /*
                Calculate number of intersection points between two parabolas with foci `f1` and `f2` and with given `directrix`
            */
            int intersectionPointsNum(Point2D f1, Point2D f2, double directrix)
            {
                if (Math.Abs(f1.X - f2.X) < pointEpsilon && Math.Abs(f1.Y - f2.Y) < pointEpsilon)
                    return -1;

                if (Math.Abs(f1.Y - f2.Y) < pointEpsilon)
                    return 1;

                return 2;
            }

            /*
                Find intersection points of two parabolas with foci `f1` and `f2` and with directrix given `d`
                Returns  intersection points ordered by x-coordinate
            */
            List<Point2D> findIntersectionPoints(Point2D f1, Point2D f2, double d)
            {
                List<Point2D> result = new List<Point2D>();
                if (Math.Abs(f1.X - f2.X) < pointEpsilon)
                {
                    double y = 0.5 * (f1.Y + f2.Y);
                    double D = Math.Sqrt(d * d - d * (f1.Y + f2.Y) + f1.Y * f2.Y);
                    result.Add(new Point2D(f1.X - D, y));
                    result.Add(new Point2D(f1.X + D, y));
                }
                else if (Math.Abs(f1.Y - f2.Y) < pointEpsilon)
                {
                    double x = 0.5 * (f1.X + f2.X);
                    result.Add(new Point2D(x, 0.5 * ((x - f1.X) * (x - f1.X) + f1.Y * f1.Y - d * d) / (f1.Y - d)));
                }
                else
                {
                    double D = 2.0 * Math.Sqrt(Math.Pow(f1.X - f2.X, 2) * (d - f1.Y) * (d - f2.Y) * (Math.Pow(f1.X - f2.X, 2) + Math.Pow(f1.Y - f2.Y, 2)));
                    double T = -2.0 * d * Math.Pow(f1.X - f2.X, 2) + (f1.Y + f2.Y) * (Math.Pow(f2.X - f1.X, 2) + Math.Pow(f2.Y - f1.Y, 2));
                    double Q = 2.0 * Math.Pow(f1.Y - f2.Y, 2);

                    double y1 = (T - D) / Q, y2 = (T + D) / Q;
                    double x1 = 0.5 * (f1.X * f1.X - f2.X * f2.X + (2 * y1 - f2.Y - f1.Y) * (f2.Y - f1.Y)) / (f1.X - f2.X);
                    double x2 = 0.5 * (f1.X * f1.X - f2.X * f2.X + (2 * y2 - f2.Y - f1.Y) * (f2.Y - f1.Y)) / (f1.X - f2.X);

                    if (x1 > x2)
                    {
                        (x1, x2) = (x2, x1);
                        (y1, y2) = (y2, y1);
                    }

                    result.Add(new Point2D(x1, y1));
                    result.Add(new Point2D(x2, y2));
                }
                return result;
            }

            // Return x-coordinate of:
            //  - in case of leaf node - corresponding focus of parabola;
            //  - in case of internal node - breakpoint;
            double getNodeValue(BeachLineNode node)
            {
                //if (points == nullptr)
                //    return std::numeric_limits<double>::infinity();
                if (node.IsLeaf)
                {
                    return points[node.Indices.Item1].X;
                }
                else
                {
                    Point2D p1 = points[node.Indices.Item1];
                    Point2D p2 = points[node.Indices.Item2];

                    List<Point2D> ips = findIntersectionPoints(p1, p2, sweepline);
                    if (ips.Count == 2)
                    {
                        if (p1.Y < p2.Y)
                        {
                            return ips[0].X;
                        }
                        else
                        {
                            return ips[1].X;
                        }
                    }
                    else
                    {
                        return ips[0].X;
                    }
                }
            }

            BeachLineNode findNode(BeachLineNode node, double x)
            {
                while (!node.IsLeaf)
                {
                    if (getNodeValue(node) < x)
                    {
                        node = node.Right;
                    }
                    else
                    {
                        node = node.Left;
                    }
                }
                return node;
            }

            (HalfEdge, HalfEdge) make_twins(int left_index, int right_index)
            {

                HalfEdge h = new HalfEdge { LeftIndex = left_index, RightIndex = right_index };
                HalfEdge h_twin = new HalfEdge { LeftIndex = right_index, RightIndex = left_index };

                h.twin = h_twin;
                h_twin.twin = h;

                return (h, h_twin);
            }

            void connect(BeachLineNode prev, BeachLineNode next)
            {
                prev.Next = next;
                next.Prev = prev;
            }

            /*
                Get height of the node
            */
            int get_height(BeachLineNode node)
            {
                if (node == null) return 0;
                return node.Height;
            }

            /*
                Update height of the node
            */
            void update_height(BeachLineNode node)
            {
                if (node == null)
                    return;
                node.Height = Math.Max(get_height(node.Left), get_height(node.Right)) + 1;
            }

            BeachLineNode make_simple_subtree(
                int index,
                int index_behind,
                List<HalfEdge> edges)
            {

                BeachLineNode node, leaf_l, leaf_r;

                (HalfEdge, HalfEdge) twin_edges = make_twins(index_behind, index);

                edges.Add(twin_edges.Item1);
                edges.Add(twin_edges.Item2);

                if (points[index].X < points[index_behind].X)
                {
                    // Depends on the point order
                    node = new BeachLineNode { Indices = (index, index_behind), Height = 1 };
                    leaf_l = new BeachLineNode { Indices = (index, index), Height = 1 };
                    leaf_r = new BeachLineNode { Indices = (index_behind, index_behind), Height = 1 };
                    node.Edge = twin_edges.Item2;//twin_edges.first;
                }
                else
                {
                    node = new BeachLineNode { Indices = (index_behind, index), Height = 1 };
                    leaf_l = new BeachLineNode { Indices = (index_behind, index_behind), Height = 1 };
                    leaf_r = new BeachLineNode { Indices = (index, index), Height = 1 };
                    node.Edge = twin_edges.Item1;//twin_edges.second;
                }

                node.Left = leaf_l;
                node.Right = leaf_r;

                leaf_l.Parent = node;
                leaf_r.Parent = node;

                connect(leaf_l, leaf_r);
                update_height(node);

                return node;
            }

            BeachLineNode make_subtree(
                int index,
                int index_behind,
                List<HalfEdge> edges)
            {
                // create nodes corresponding to branching points
                BeachLineNode node1 = new BeachLineNode { Indices = (index_behind, index), Height = 1 };
                BeachLineNode node2 = new BeachLineNode { Indices = (index, index_behind), Height = 1 };

                // create leaf nodes
                BeachLineNode leaf1 = new BeachLineNode { Indices = (index_behind, index_behind), Height = 1 };
                BeachLineNode leaf2 = new BeachLineNode { Indices = (index, index), Height = 1 };
                BeachLineNode leaf3 = new BeachLineNode { Indices = (index_behind, index_behind), Height = 1 };

                // adjust tree connections
                node1.Right = node2;
                node2.Parent = node1;

                node1.Left = leaf1;
                leaf1.Parent = node1;

                node2.Left = leaf2;
                leaf2.Parent = node2;

                node2.Right = leaf3;
                leaf3.Parent = node2;

                // add halfedges
                (HalfEdge, HalfEdge) twin_edges = make_twins(index_behind, index);
                node1.Edge = twin_edges.Item1;//second;//first;
                node2.Edge = twin_edges.Item2;//first;//second;

                edges.Add(twin_edges.Item1);
                edges.Add(twin_edges.Item2);

                // connect leaf nodes
                connect(leaf1, leaf2);
                connect(leaf2, leaf3);

                // reset height of a node
                update_height(node2);
                update_height(node1);

                // return the result
                return node1;
            }

            int get_balance(BeachLineNode node)
            {
                return get_height(node.Left) - get_height(node.Right);
            }

            /*
                Check if the node is a root node
            */
            bool is_root(BeachLineNode node)
            {
                return node.Parent == null;
            }

            /*
                Performs rotation of a tree around `node` such that it goes to the left subtree
            */
            BeachLineNode rotate_left(BeachLineNode node)
            {

                if (node == null)
                    return null;

                if (node.Right == null)
                    return node;

                // get right node, which becomes a new root node
                BeachLineNode rnode = node.Right;

                // establish connections with a root node if threre is one
                if (!is_root(node))
                {
                    if (node.Parent.Left == node)
                    {
                        node.Parent.Left = rnode;
                    }
                    else
                    {
                        node.Parent.Right = rnode;
                    }
                }
                rnode.Parent = node.Parent;

                // connect right subtree of the left child as a left subtree of `node`
                node.Right = rnode.Left;
                if (rnode.Left != null)
                {
                    rnode.Left.Parent = node;
                }

                // connect `node` as a right child of it's child
                rnode.Left = node;
                node.Parent = rnode;

                // update height attribute
                update_height(node);
                update_height(rnode);
                update_height(rnode.Parent);

                return rnode;
            }

            /*
                Performs rotation of a tree around `node` such that it goes to the right subtree
            */
            BeachLineNode rotate_right(BeachLineNode node)
            {

                if (node == null)
                    return null;

                if (node.Left == null)
                    return node;

                // left node becomes root node of subtree
                BeachLineNode lnode = node.Left;

                // establish connections with a root node if threre is one
                if (!is_root(node))
                {
                    if (node.Parent.Left == node)
                    {
                        node.Parent.Left = lnode;
                    }
                    else
                    {
                        node.Parent.Right = lnode;
                    }
                }
                lnode.Parent = node.Parent;

                // connect right subtree of the left child as a left subtree of `node`
                node.Left = lnode.Right;
                if (lnode.Right != null)
                {
                    lnode.Right.Parent = node;
                }

                // connect `node` as a right child of it's child
                lnode.Right = node;
                node.Parent = lnode;

                // update height attribute
                update_height(node);
                update_height(lnode);
                update_height(lnode.Parent);

                return lnode;
            }

            /*
                Replace a leaf `node` with a new subtree, which has root `new_node`.
                The function rebalances the tree and returns the pointer to a new root node.
            */
            BeachLineNode replace(BeachLineNode node, BeachLineNode new_node)
            {

                if (node == null)
                {
                    return new_node;
                }

                // Find x-coordinate
                double x = getNodeValue(new_node);

                // Get a parent node
                BeachLineNode parent_node = node.Parent;

                // Remove leaf, because it's replaced by a new subtree
                //        delete node;

                // Insert the node
                new_node.Parent = parent_node;
                if (parent_node != null)
                {
                    if (getNodeValue(parent_node) < x)
                    {
                        parent_node.Right = new_node;
                    }
                    else
                    {
                        parent_node.Left = new_node;
                    }
                }

                // Rebalance the tree
                node = new_node;
                while (parent_node != null)
                {
                    update_height(parent_node);
                    int balance = get_balance(parent_node);
                    if (balance > 1)
                    { // left subtree is higher than right subtree by more than 1
                        if (parent_node.Left != null && !parent_node.Left.IsLeaf && get_balance(parent_node.Left) < 0)
                        { // @TODO ensure that
                            parent_node.Left = rotate_left(parent_node.Left);
                        }
                        parent_node = rotate_right(parent_node);
                    }
                    else if (balance < -1)
                    { // right subtree is lower than left subtree by more than 1
                        if (parent_node.Right != null && !parent_node.Right.IsLeaf && get_balance(parent_node.Right) > 0)
                        {
                            parent_node.Right = rotate_right(parent_node.Right);
                        }
                        parent_node = rotate_left(parent_node);
                    }

                    //_validate(parent_node);

                    node = parent_node;
                    parent_node = parent_node.Parent;
                }

                //_check_balance(node);

                return node;
            }

            bool findCircleCenter(Point2D p1, Point2D p2, Point2D p3, out Point2D center)
            {

                // get normalized vectors
                Vector2D u1 = (p1 - p2).Normalized;
                Vector2D u2 = (p3 - p2).Normalized;

                double cross = Vector2D.CrossProduct(u1, u2);

                // check if vectors are collinear
                if (Math.Abs(cross) < circleCenterEpsilon)
                {
                    center = new Point2D();
                    return false;
                }

                // get cental points
                Point2D pc1 = Point2D.Center(p1, p2);
                Point2D pc2 = Point2D.Center(p2, p3);

                // get free components
                double b1 = Vector2D.DotProduct(u1, pc1);
                double b2 = Vector2D.DotProduct(u2, pc2);

                // calculate the center of a circle
                center = new Point2D((b1 * u2.Y - b2 * u1.Y) / cross, (u1.X * b2 - u2.X * b1) / cross);

                return true;
            }


            Event checkCircleEvent(BeachLineNode n1, BeachLineNode n2, BeachLineNode n3)
            {
                if (n1 == null || n2 == null || n3 == null)
                    return null;

                Point2D p1 = points[n1.Id];
                Point2D p2 = points[n2.Id];
                Point2D p3 = points[n3.Id];
                Point2D center, bottom;

                if (p2.Y > p1.Y && p2.Y > p3.Y)
                    return null;

                if (!findCircleCenter(p1, p2, p3, out center))
                    return null;

                bottom = new Point2D(center.X, center.Y + (center - p2).Norm);

                // check circle event
                if (Math.Abs(bottom.Y - sweepline) < pointEpsilon || sweepline < bottom.Y)
                {
                    // create a circle event structure
                    Event e = Event.CreateCircle(bottom);
                    // initialize attributes
                    e.Center = center;
                    e.Arc = n2;
                    // add reference in the corresponding node
                    n2.CircleEvent = e;
                    return e;
                }

                return null;
            }

            /*
                Returns breakpoints for a given arc
            */
            (BeachLineNode, BeachLineNode) getbreakpoints(BeachLineNode leaf)
            {

                if (leaf == null || leaf.Next == null || leaf.Prev == null)
                    return (null, null);

                BeachLineNode parent = leaf.Parent, gparent = leaf.Parent;
                (int, int) bp1 = (leaf.Prev.Id, leaf.Id); // left breakpoint
                (int, int) bp2 = (leaf.Id, leaf.Next.Id); // right breakpoint
                (int, int) other_bp = default;

                bool left_is_missing = true;

                if (parent.has_indices(bp1))
                {
                    other_bp = bp2;
                    left_is_missing = false;
                }
                else if (parent.has_indices(bp2))
                {
                    other_bp = bp1;
                    left_is_missing = true;
                }

                // Go up and rebalance the whole tree
                while (gparent != null)
                {
                    if (gparent.has_indices(other_bp))
                    {
                        break;
                    }
                    gparent = gparent.Parent;
                }

                if (left_is_missing)
                {
                    return (gparent, parent);
                }
                else
                {
                    return (parent, gparent);
                }

                //        // BUG doesn't take into account gparent WRONG!!!
                //        if (parent->parent != nullptr) {
                //            if (parent->parent->left == parent) {
                //                return std::make_pair(parent, gparent);
                //            } else {
                //                return std::make_pair(gparent, parent);
                //            }
                //        }
                //
                //        return std::make_pair(parent, gparent);
            }

            /*
                Remove a disappearing arc related to a circle event.
                The function rebalances the tree and returns the pointer to a new root node.
            */
            BeachLineNode remove(BeachLineNode leaf)
            {
                // General idea behind this code:
                // This function removes the leaf and it's parent corresponding to one breakpoint.
                // It moves up in a tree and rebalaces it. If function encounters second breakpoint,
                // it replaces this breakpoint with a new one. This is possible because when the circle
                // event appears, two breakpoints coincide and thus they should be represented by one.

                if (leaf == null)
                    return null;

                BeachLineNode parent = leaf.Parent, grandparent = parent.Parent;
                (int, int) bp1 = (leaf.Prev.Id, leaf.Id);
                (int, int) bp2 = (leaf.Id, leaf.Next.Id);
                (int, int) other_bp = default;

                System.Diagnostics.Debug.Assert(leaf.Next != null);
                System.Diagnostics.Debug.Assert(leaf.Prev != null);
                System.Diagnostics.Debug.Assert(parent != null);
                System.Diagnostics.Debug.Assert(grandparent != null);

                System.Diagnostics.Debug.Assert(parent.has_indices(bp1) || parent.has_indices(bp2));

                if (parent.has_indices(bp1))
                {
                    other_bp = bp2;
                }
                else if (parent.has_indices(bp2))
                {
                    other_bp = bp1;
                }

                BeachLineNode other_subtree;
                if (parent.Left == leaf)
                    other_subtree = parent.Right;
                else
                    other_subtree = parent.Left;

                other_subtree.Parent = grandparent;
                if (grandparent.Left == parent)
                {
                    grandparent.Left = other_subtree;
                }
                else
                {
                    grandparent.Right = other_subtree;
                }

                BeachLineNode new_root = grandparent;
                // Go up and rebalance the whole tree
                while (grandparent != null)
                {
                    if (grandparent.has_indices(other_bp))
                        grandparent.Indices = (leaf.Prev.Id, leaf.Next.Id);
                    // update height of a node
                    update_height(grandparent);
                    // calculate balance factor of a node
                    int balance = get_balance(grandparent);
                    if (balance > 1)
                    { // left subtree is higher than right subtree by more than 1
                        if (grandparent.Left != null && !grandparent.Left.IsLeaf && get_balance(grandparent.Left) < 0)
                        {
                            grandparent.Left = rotate_left(grandparent.Left);
                        }
                        grandparent = rotate_right(grandparent);
                    }
                    else if (balance < -1)
                    { // right subtree is lower than left subtree by more than 1
                        if (grandparent.Right != null && !grandparent.Right.IsLeaf && get_balance(grandparent.Right) > 0)
                        {
                            grandparent.Right = rotate_right(grandparent.Right);
                        }
                        grandparent = rotate_left(grandparent);
                    }

                    //_validate(grandparent);

                    new_root = grandparent;
                    grandparent = grandparent.Parent;
                }

                // Connect previous with next leaf
                connect(leaf.Prev, leaf.Next);

                //_check_balance(new_root);

                return new_root;
            }

            void connect_halfedges(HalfEdge p1, HalfEdge p2)
            {
                p1.next = p2;
                p2.prev = p1;
            }

            #endregion

            while (queue.Count > 0)
            {
                var e = queue.Pop();
                sweepline = e.Point.Y;

                System.Diagnostics.Debug.WriteLine($"{e.Type} {e.Point.X:F3}, {e.Point.Y:F3}");

                if (e.Type == EventType.Site)
                {
                    int index = e.Index;
                    if (root == null)
                    {
                        root = new BeachLineNode
                        {
                            Indices = (index, index),
                            Height = 1
                        };
                    }
                    else
                    {
                        BeachLineNode arc = findNode(root, e.Point.X);
                        BeachLineNode subtree, leftLeaf, rightLeaf;

                        if (arc.CircleEvent != null)
                        {
                            Event circle_e = arc.CircleEvent;
                            circle_e.Type = EventType.Skip; // ignore corresponding event
                        }

                        // check number of intersection points
                        int isp_num = intersectionPointsNum(points[arc.Indices.Item1], e.Point, sweepline);

                        // different subtrees depending on the number of intersection points
                        if (isp_num == 1)
                        {
                            subtree = make_simple_subtree(index, arc.Id, halfEdges);
                            leftLeaf = subtree.Left;
                            rightLeaf = subtree.Right;
                        }
                        else if (isp_num == 2)
                        {
                            subtree = make_subtree(index, arc.Id, halfEdges);
                            leftLeaf = subtree.Left;
                            rightLeaf = subtree.Right.Right;
                        }
                        else
                        {
                            continue;
                        }

                        if (arc.Prev != null)
                            connect(arc.Prev, leftLeaf);

                        if (arc.Next != null)
                            connect(rightLeaf, arc.Next);

                        // Replace old leaf with a subtree and rebalance it
                        root = replace(arc, subtree);

                        // Check circle events
                        Event circle_event = checkCircleEvent(leftLeaf.Prev, leftLeaf, leftLeaf.Next);
                        if (circle_event != null)
                        {
                            queue.Push(circle_event);
                        }
                        circle_event = checkCircleEvent(rightLeaf.Prev, rightLeaf, rightLeaf.Next);
                        if (circle_event != null)
                        {
                            queue.Push(circle_event);
                        }
                    }
                }
                else if (e.Type == EventType.Circle)
                { // handle circle event

                    BeachLineNode arc = e.Arc, prev_leaf, next_leaf;

                    // get breakpoint nodes
                    (BeachLineNode, BeachLineNode) breakpoints = getbreakpoints(arc);

                    // recheck if it's a false alarm 1
                    if (breakpoints.Item1 == null || breakpoints.Item2 == null)
                    {
                        continue;
                    }

                    // recheck if it's a false alarm 2
                    double v1 = getNodeValue(breakpoints.Item1), v2 = getNodeValue(breakpoints.Item2);

                    if (Math.Abs(v1 - v2) > breakPointsEpsilon)
                    {
                        continue;
                    }

                    // create a new vertex and insert into doubly-connected edge list
                    Vertex vertex = new Vertex { Point = e.Center };
                    HalfEdge h_first = breakpoints.Item1.Edge;
                    HalfEdge h_second = breakpoints.Item2.Edge;

                    // store vertex of Voronoi diagram
                    vertices.Add(vertex);

                    // remove circle event corresponding to next leaf
                    if (arc.Prev != null && arc.Prev.CircleEvent != null)
                    {
                        Event circle_e = arc.Prev.CircleEvent;
                        circle_e.Type = EventType.Skip; // ignore corresponding event
                    }

                    // remove circle event corresponding to prev leaf
                    if (arc.Next != null && arc.Next.CircleEvent != null)
                    {
                        Event circle_e = arc.Next.CircleEvent;
                        circle_e.Type = EventType.Skip; // ignore corresponding event
                    }

                    // store pointers to the next and previous leaves
                    prev_leaf = arc.Prev;
                    next_leaf = arc.Next;

                    // They should not be null
                    System.Diagnostics.Debug.Assert(prev_leaf != null);
                    System.Diagnostics.Debug.Assert(next_leaf != null);

                    // get node associated with a new edge
                    BeachLineNode new_edge_node;
                    if (arc.Parent == breakpoints.Item1)
                        new_edge_node = breakpoints.Item2;
                    else
                        new_edge_node = breakpoints.Item1;

                    // remove arc from the beachline
                    root = remove(arc);

                    // make a new pair of halfedges
                    (HalfEdge, HalfEdge) twin_nodes = make_twins(prev_leaf.Id, next_leaf.Id);
                    new_edge_node.Edge = twin_nodes.Item1;
                    //1/ new_edge_node->edge = twin_nodes.first;

                    // connect halfedges
                    connect_halfedges(h_second, h_first.twin);
                    connect_halfedges(h_first, twin_nodes.Item1);
                    connect_halfedges(twin_nodes.Item2, h_second.twin);

                    // halfedges are pointing into a vertex  -----> O <-----
                    // not like this <---- O ----->
                    // counterclockwise
                    h_first.vertex = vertex;
                    h_second.vertex = vertex;
                    twin_nodes.Item2.vertex = vertex;
                    vertex.Edge = h_second;

                    halfEdges.Add(twin_nodes.Item1);
                    halfEdges.Add(twin_nodes.Item2);

                    // check new circle events
                    if (prev_leaf != null && next_leaf != null)
                    {
                        Event circle_event = checkCircleEvent(prev_leaf.Prev, prev_leaf, next_leaf);
                        if (circle_event != null)
                        {
                            queue.Push(circle_event);
                        }
                        circle_event = checkCircleEvent(prev_leaf, next_leaf, next_leaf.Next);
                        if (circle_event != null)
                        {
                            queue.Push(circle_event);
                        }
                    }
                }
            }

            var faces = new List<HalfEdge>(points.Length);
            for (int i = 0; i < points.Length; i++)
                faces.Add(null);

            // Fill edges corresponding to faces
            for (int i = 0; i < halfEdges.Count; ++i)
            {
                HalfEdge he = halfEdges[i];
                if (he.prev == null || faces[he.LeftIndex] == null)
                {
                    faces[he.LeftIndex] = he;
                }
            }

            return new VoronoiDiagram(points, halfEdges, vertices, faces);
        }

        public static VoronoiDiagram Fortune2(
            Point2D[] points,
            double epsilon = 1e-6)
        {
            var fortune = new FortuneAlgorithm();
            fortune.Init(points, epsilon);
            return fortune.Run();
        }

        #region Nested classes

        #region Public

        public class HalfEdge
        {
            public int LeftIndex;
            public int RightIndex;

            public Vertex vertex;
            public HalfEdge twin;
            public HalfEdge next;
            public HalfEdge prev;
        }

        public class Vertex
        {
            public Point2D Point;
            public HalfEdge Edge; // The edge points towards this vertex [-->o]
        }

        #endregion

        #region Private

        private struct FortuneAlgorithm
        {
            private Point2D[] Points;
            private double Epsilon;

            public void Init(Point2D[] points, double epsilon)
            {
                Points = points;
                Epsilon = epsilon;
            }

            public VoronoiDiagram Run()
            {
                BinaryHeap1<Event> queue = new BinaryHeap1<Event>(Points.Length * 2, new EventComparer());

                Event[] siteEvents = new Event[Points.Length];
                for (int i = 0; i < Points.Length; i++)
                    siteEvents[i] = Event.CreateSite(i, Points[i]);

                queue.AddRange(siteEvents);

                var beachline = new RedBlackTree<Arc>();

                while (queue.Count > 0)
                {
                    var evt = queue.Pop();
                    if (evt.Type == EventType.None)
                        continue;

                    if (evt.Type == EventType.Site)
                        ProcessSiteEvent(evt, beachline);
                }

                throw new NotImplementedException();
            }

            private void ProcessSiteEvent(Event evt, RedBlackTree<Arc> beachline)
            {
                if (beachline.Count == 0)
                {
                    beachline.Add(new Arc(evt.Index));
                }
                else
                {

                }
            }

            private class Event
            {
                public EventType Type;
                public int Index;
                public Point2D Point;

                public static Event CreateSite(int index, Point2D point)
                {
                    return new Event
                    {
                        Type = EventType.Site,
                        Index = index,
                        Point = point
                    };
                }
            }

            private enum EventType
            {
                None,
                Site,
                Circle
            }

            private class EventComparer : IComparer<Event>
            {
                public int Compare(Event e1, Event e2)
                {
                    return Math.Sign(e2.Point.Y - e1.Point.X);
                }
            }

            private class Arc
            {
                public int Index;
                public Event CircleEvent;

                public Arc(int index)
                {
                    Index = index;
                }
            }
        }

        private class Event : IComparable<Event>
        {
            public EventType Type;
            public Point2D Point;
            public int Index;
            public Point2D Center;
            public BeachLineNode Arc;

            public static Event CreateSite(int index, Point2D point)
            {
                return new Event
                {
                    Type = EventType.Site,
                    Index = index,
                    Point = point
                };
            }

            public static Event CreateCircle(Point2D point)
            {
                return new Event
                {
                    Type = EventType.Circle,
                    Index = -1,
                    Point = point
                };
            }

            public int CompareTo(Event other)
            {
                Point2D p1 = Point;
                Point2D p2 = other.Point;
                int compare = Math.Sign(p1.Y - p2.Y);
                if (compare != 0)
                    return compare;
                return Math.Sign(p1.X - p2.X);
            }
        }

        private enum EventType
        {
            Skip = 0,
            Site = 1,
            Circle = 2
        }

        private class BeachLineNode
        {
            // Height of the tree
            public int Height;

            // Indices of the points
            public (int, int) Indices;

            // Pointers to left, right children and parent node
            public BeachLineNode Left;
            public BeachLineNode Right;
            public BeachLineNode Parent;

            // Pointer to a circle event for a leaf node or halfedge for an internal node
            public Event CircleEvent;
            public HalfEdge Edge;

            // Pointers to a next and previous arc-nodes
            public BeachLineNode Next, Prev;

            public bool IsLeaf => Indices.Item1 == Indices.Item2;
            public int Id => Indices.Item1;

            public bool has_indices((int, int) p)
            {
                return Indices.Item1 == p.Item1 && Indices.Item2 == p.Item2;
            }
        }

        #endregion

        #endregion
    }
}