using UnityEngine;
using System.Collections;

public class GraphPrint : MonoBehaviour {

	public bool print = true;

	private Graph graph;

	void OnDrawGizmos() {
		if (print){
			Gizmos.color = Color.magenta;
			Area[] aree = ConvexOverlapping.divideSpaceIntoArea ();
			graph = new Graph (aree);
			Node[] nodes = graph.nodes;
			//Node[] nodes = Graph.reduceGraphToStaticGuard(graph);

			for (int i=0; i<nodes.Length-1; i++) {
				for (int j=i; j<nodes.Length; j++) {
					if (graph.adjacency[i][j] == 1) {
						Gizmos.DrawLine(nodes[i].area.center3D, nodes[j].area.center3D);
					}
				}
			}
		}
	}
}
