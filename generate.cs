using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    int w = 10, h = 10, x = 0, y = 0;
    //public GameObject cellPrefab;

    public int ew = 600, eh = 300, mean = 100, sd = 50;
    public Sprite a ;
    public float GenerateNormalDistribution(float mean, float standardDev)
    {
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        float randNormal = mean + standardDev * randStdNormal;

        return Mathf.Max(0.1f, randNormal);
    }
    public void Dimensions(int ew, int eh, ref int x, ref int y)
    {
        int t = (int)(2 * Mathf.PI * Random.value);
        int u = (int)(Random.value + Random.value);
        int r;

        if (u > 1) r = 2 - u;
        else r = u;
        x = (int)(ew * r * Mathf.Cos(t) / 2);
        x = (int)Mathf.Floor((((x + 1 - 1) / 1)) * 1);

        y = (int)(eh * r * Mathf.Sin(t) / 2);
        y = (int)Mathf.Floor((((y + 1 - 1) / 1)) * 1);
    }
        public GameObject q;
     public void Generate(GameObject cellPrefab)
    {
        w=(int)GenerateNormalDistribution(mean, sd);
        h=(int)GenerateNormalDistribution(mean, sd);
        Dimensions(ew, eh, ref x, ref y);
        Vector3 spawnPosition = new Vector3(x, y, 0);
        Quaternion spawnRotation = Quaternion.identity;
        cellPrefab.transform.localScale = new Vector3(w, h, 0);
        
        q=Instantiate(cellPrefab, spawnPosition, spawnRotation);
        if(w<=1*mean || h <= 1*mean)
        {
           q.GetComponent<SpriteRenderer>().color = Color.black;
        }
        

        //Debug.Log("YES");
        
    }

}

public class generate : Room
{
    public GameObject prefab;
    public int n=50;
    public Room[] rooms ;
    public List<Vector2> points;
    public List<GameObject> g;
    private List<Edge> minimumSpanningTreeEdges, minimumSpanningTree;
    public TriangleNet.Mesh m;
    public Sprite aaa;
    private void Start()
    {
        
        for (int i = 0; i < 100; i++)
        {
            rooms[i] = gameObject.AddComponent<Room>();
        }
        Triangulate t = gameObject.AddComponent<Triangulate>();
        Time.timeScale = 80;
        for (int i=0;i<n;i++)
        {
            
            //r[i].cellPrefab = prefab;
            rooms[i].Generate(prefab);
        }
        

    }
    private void Update()
    {
        int flag = 1;
        if (Time.frameCount == 200 && flag==1)
        {

            Time.timeScale = 1;

            for (int i = 0; i < n; i++)
            {
                rooms[i].q.GetComponent<BoxCollider2D>().enabled=false;
                if (rooms[i].q.GetComponent<SpriteRenderer>().color != Color.black)
                {
                    Vector2 tem=new Vector2(rooms[i].q.transform.position.x, rooms[i].q.transform.position.y);
                    points.Add(tem);
                    g.Add(rooms[i].q);
                }
                
            }
            Triangulate t = gameObject.AddComponent<Triangulate>();
            m = t.triangle(points);
            minimumSpanningTree = FindMinimumSpanningTree(g,points);
            minimumSpanningTreeEdges = minimumSpanningTree;
            flag = 0;
            Vector3 froma , toa ,toa2 ;
            float r;
            /*for (int i = 0; i < points.Count; i++)
            {
                for (int j = i+1; j < points.Count; j++)
                {
                    r = Random.value;
                    bool temp=true;
                    Edge e = new Edge(points[i], points[j], 0, g[i], g[j]);
                    for (int k = 0; k < minimumSpanningTreeEdges.Count; k++)
                    {
                        if (e.init == minimumSpanningTreeEdges[i].init && e.final == minimumSpanningTreeEdges[i].final)
                            temp = false;
                    }
                    if (r>0.9 && temp)
                    {
                        minimumSpanningTreeEdges.Add(e);
                    }
                }
               
            }*/

            for (int i = 0; i < minimumSpanningTreeEdges.Count; i++)
            {
                GameObject m = minimumSpanningTreeEdges[i].init;
                GameObject n =minimumSpanningTreeEdges[i].final;
                if (close(m,n)==1)
                {
                    froma = new Vector3((n.transform.position.x + m.transform.position.x) / 2, (n.transform.position.y + m.transform.position.y) / 2,0);
                    toa = new Vector3(froma.x, n.transform.position.y, 0);
                    toa2 = new Vector3(froma.x, m.transform.position.y, 0);
                    DrawLine(froma, toa,Color.blue);
                    DrawLine(froma, toa2, Color.blue);
                    Debug.Log(n.transform.position.x);
                    Debug.Log(toa);
                    Debug.Log(toa2);
                }
                else if (close(m, n) == 2)
                {
                    froma = new Vector3((n.transform.position.x + m.transform.position.x) / 2, (n.transform.position.y + m.transform.position.y) / 2, 0);
                    toa = new Vector3(n.transform.position.x,froma.y , 0);
                    toa2 = new Vector3(m.transform.position.x,froma.y , 0);
                    DrawLine(froma, toa, Color.blue);
                    DrawLine(froma, toa2, Color.blue);
                }
                else
                {       froma = new Vector3(n.transform.position.x ,m.transform.position.y , 0);
                        toa = new Vector3(n.transform.position.x, n.transform.position.y, 0);
                        toa2 = new Vector3(m.transform.position.x, m.transform.position.y, 0);
                        DrawLine(froma, toa, Color.blue);
                        DrawLine(froma, toa2, Color.blue);
                }
            }

            for (int i = 0; i < n; i++)
            {
                if (rooms[i].q.GetComponent<SpriteRenderer>().color == Color.black)
                {
                   // Destroy(rooms[i].q);
                    //rooms[i].q.GetComponent<SpriteRenderer>().sortingLayerName="1";
                }

            }
        }
    }
    void OnDrawGizmos()
    {
        if (Time.frameCount > 400)
        {
            Gizmos.color = Color.red;

            foreach (var triangle in m.triangles)
            {
                Vector2 p1 = new Vector2((float)triangle.GetVertex(0).X, (float)triangle.GetVertex(0).Y);
                Vector2 p2 = new Vector2((float)triangle.GetVertex(1).X, (float)triangle.GetVertex(1).Y);
                Vector2 p3 = new Vector2((float)triangle.GetVertex(2).X, (float)triangle.GetVertex(2).Y);

                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p1);
            }
        }
        Gizmos.color = Color.green;

        foreach (Edge edge in minimumSpanningTreeEdges)
        {
            Gizmos.DrawLine(edge.from, edge.to);
        }
    }

    

    public struct Edge
    {
        public Vector2 from;
        public Vector2 to;
        public float weight;
        public GameObject init;
        public GameObject final;
        public Edge(Vector2 from, Vector2 to, float weight ,GameObject init , GameObject final)
        {
            this.from = from;
            this.to = to;
            this.weight = weight;
            this.init = init;
            this.final = final;
        }
    }

    public int pointCount = 10;
    public float range = 10f;
    List<Edge> FindMinimumSpanningTree(List<GameObject> g,List<Vector2> vertices)
    {
        List<Edge> minimumSpanningTree = new List<Edge>();

        List<Vector2> visitedVertices = new List<Vector2>();
        List<GameObject> visitedObj = new List<GameObject>();
        visitedVertices.Add(vertices[0]);
        visitedObj.Add(g[0]);
        while (visitedVertices.Count < vertices.Count)
        {
            Edge minEdge = new Edge(Vector2.zero, Vector2.zero, float.MaxValue ,gameObject ,gameObject);
            
            for (int i= 0; i < visitedVertices.Count ; i++)
            {
                for (int j = 0; j < vertices.Count; j++)
                {
                    if (!visitedVertices.Contains(vertices[j]))
                    {
                        float distance = Vector2.Distance(visitedVertices[i], vertices[j]);

                        if (distance < minEdge.weight)
                        {
                            minEdge = new Edge(visitedVertices[i], vertices[j], distance, visitedObj[i], g[j] );
                        }
                    }
                }
            }

            visitedVertices.Add(minEdge.to);
            visitedObj.Add(minEdge.final);
            minimumSpanningTree.Add(minEdge);
        }

        return minimumSpanningTree;
    }

    public Material material;
    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        Vector3 mid = new Vector3(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y), 0);
        myLine.AddComponent<LineRenderer>();
        
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        // myLine.AddComponent<SpriteRenderer>();
        //myLine.GetComponent<SpriteRenderer>().sprite = aaa;
        
        lr.material = material;
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 20f;
        lr.endWidth = 20f;
        lr.SetPosition(0, start);
        //lr.SetPosition(1, mid);
        lr.SetPosition(1, end);
        //GameObject.Destroy(myLine, duration);
    }

    int close(GameObject n, GameObject m)
    {
        int a = (int)(n.transform.position.x + m.transform.position.x) / 2;
        int b = (int)(n.transform.position.y + m.transform.position.y) / 2;
        float p = n.transform.position.x - n.transform.localScale.x / 2;
        float q = n.transform.position.x + n.transform.localScale.x / 2;
        float r = m.transform.position.x - m.transform.localScale.x / 2;
        float s = m.transform.position.x + m.transform.localScale.x / 2;
        float p1 = n.transform.position.y - n.transform.localScale.y / 2;
        float q1 = n.transform.position.y + n.transform.localScale.y / 2;
        float r1 = m.transform.position.y - m.transform.localScale.y / 2;
        float s1 = m.transform.position.y + m.transform.localScale.y / 2;
        if (a > p && a < q && a > r && a < s)
        {
            return 1;
        }
        else if (b > p && b < q && b > r && b < s)
            return 2;
        else return -1;
    }
}


