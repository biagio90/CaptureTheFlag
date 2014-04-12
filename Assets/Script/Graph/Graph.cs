using UnityEngine;
using System.Collections;

public class Graph {
	public Node[] nodes;
	public int[][] adjacency;
	
	public Graph(Area[] aree) {
		int size = aree.Length;
		nodes = new Node[size];
		adjacency = new int[size][];
		for (int i=0; i<size; i++) {
			nodes[i] = new Node(aree[i]);
			adjacency[i] = new int[size];
		}
		
		constructgraph ();
	}

	public void constructgraph(){ //also fill the djacency matrix
		int size = nodes.Length;
		int numconnections = 0;
		for (int i=0; i<size; i++) { //for every node
			Vector3 nodeipos = nodes[i].area.center3D;
			for (int j=0; j<size; j++) {
				Vector3 nodejpos = nodes[j].area.center3D;
				
				adjacency[i][j] = 0;
				
				//check all the other nodes in the graph
				if ( nodeipos != nodejpos && freePath(nodeipos,nodejpos) ){
					nodes[i].addConnection(nodes[j]);
					adjacency[i][j] = 1; //if it's visible there is a 1 and a connection
					numconnections++;
					//(new MyLine()).drawLine(nodeipos, nodejpos, Color.red);
				}
			}
			nodes[i].numconnection = numconnections; //store in each node how many other nodes can he see
			numconnections = 0;
		}
	}

	public Vector3[] getNeighbors(Vector3 position) {
		int nodePos = findInGraph (position);
		if(nodePos == -1) return null;

		ArrayList neighbors = new ArrayList();
		for (int i=0; i<nodes.Length; i++){
			if(adjacency[nodePos][i] == 1 && i!=nodePos){
				neighbors.Add(nodes[i].area.center3D);
			}
		}

		Vector3[] ret = new Vector3[neighbors.Count];
		for (int i=0; i<neighbors.Count; i++){
			ret[i] = (Vector3)neighbors[i];
		}
		return ret;
	}

	public void removenode(Node nod){
		int index = findindex(nod);
		int length = nodes.Length;
		if (length == 1) return;

		Node[] new_nodes_vector = new Node[length-1];
		for (int i=0; i<index; i++) {
			new_nodes_vector[i] = nodes[i];
		}
		
		for (int i= index +1; i<length; i++) {
			new_nodes_vector[i-1] = nodes[i];
		}
		nodes = new_nodes_vector; //biagio secondo te da problemi di allocamento o roba simile???
	}


	 private bool freePath (Vector3 p1, Vector3 p2) {
		Vector3 direction = (p2 - p1).normalized;
		float sight = (p2 - p1).magnitude;
		//RaycastHit hit;
		//return Rigidbody.SweepTest (direction, out hit, sight);
		
		Ray ray = new Ray(p1, direction);
		RaycastHit hit = new RaycastHit ();
		bool collision = true;
		
		if (Physics.Raycast (ray, out hit, sight)) {
			collision = false;
			//Debug.Log("Collision: "+hit.collider.name);
		}
		return collision;
	}
	//non so se funziona anche con i nodi, al massimo si può fare
	//il controllo non del nodo intero ma dell'area che contiene
	private int findindex(Node nod){
		int index = 0;
		for (int i=0; i<nodes.Length; i++) {
			if( nodes[i] == nod) index = i;		
		}
		return index;
	}

	public int getnumnodes(){
		int numnodes = nodes.Length;
		return numnodes;
	}

	public Node findmaxconnection(){
		Node nod = nodes [0];
		int maxconn = 0;
		for(int i=0; i<nodes.Length; i++){
			if (nodes[i].numconnection > maxconn){
				maxconn = nodes[i].numconnection;
				nod = nodes[i];
			}
		}
		return nod;
	}

	
	public int findInGraph (Vector3 pos) {
		int ret = -1;
		float minDist = 1000f;
		RaycastHit hit = new RaycastHit ();

		for (int i=0; i<nodes.Length; i++) {
			Vector3 pos2 = nodes[i].area.center3D;
			Vector3 dir = (pos2 - pos).normalized;
			float sight = (pos2 - pos).magnitude;

			Ray ray = new Ray(pos, dir);

			if(!Physics.Raycast (ray, out hit, sight) && sight < minDist) {
				minDist = sight;
				ret = i;
			}
		}
		return ret;
	}
	

	// find the minimal distance between two node
	private int Lbest;
	public int dist(int x, int d) {
		Lbest = nodes.Length;
		dfs (x, d, 0);
		return Lbest;
	}
	
	//DEBUGGED
	private void dfs(int x, int d, int l) {
		if (x == d) {
			if (l < Lbest) Lbest = l;
			return ;
		}
		
		for (int i=0; i<nodes.Length; i++) {
			if(i!=x && adjacency[x][i] == 1) {
				adjacency[x][i] = 0;
				adjacency[i][x] = 0;
				dfs (i, d, l+1);
				adjacency[x][i] = 1;
				adjacency[i][x] = 1;
			}
		}
	}

	
	public void printNodes() {
		for (int i=0; i<nodes.Length; i++) {
			Debug.Log(i+": "+nodes[i].area.center3D);
		}
	}

	static public Node[] reduceGraphToStaticGuard(Graph graph){
		ArrayList nodesforVRP = new ArrayList();
		int j = 0;
		for(int i = graph.getnumnodes(); i>1; i = graph.getnumnodes()){
			Node nodetopick = graph.findmaxconnection (); //return the node with maximum connections
			ArrayList list = nodetopick.connection;
			foreach (Node nod in list) {//è pseudo codice perchè non ho capito come prendere il nodo dall'arraylist
				//Debug.Log (nod.area.center3D);			
				graph.removenode (nod);	
			}
			graph.removenode (nodetopick);
			graph.constructgraph ();
			nodesforVRP.Add(nodetopick);
			j++;
		}
		
		nodesforVRP.Add (graph.nodes[0]);
		
		Node[] ret = new Node[nodesforVRP.Count];
		for (int i=0; i<nodesforVRP.Count; i++) {
			ret[i] = (Node) nodesforVRP[i];
		}
		
		return ret;
	}

	public void drawConnections() {
/*		MyLine draw = new MyLine ();
		
		for (int i=0; i<nodes.Length-1; i++) {
			for (int j=i; j<nodes.Length; j++) {
				if (adjacency[i][j] == 1) {
					draw.drawLine(nodes[i].area.center3D, nodes[j].area.center3D, Color.red);
				}
			}
		}*/
	}
}
