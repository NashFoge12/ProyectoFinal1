using UnityEngine;

/// <summary>Mundo 3D de MUNDO PUJLLAY. Los controles se crean aparte en 2D.</summary>
[DisallowMultipleComponent]
public class PujllayWorldGenerator : MonoBehaviour
{
    public bool generateOnStart;
    public int randomSeed = 21;
    [Range(0,150)] public int treeCount = 85;
    [Range(0,150)] public int flowerCount = 90;
    public bool includeForest = true, includeBoard = true, includeRobot = true;
    public int boardSize = 5;
    public float cellSpacing = 1.94f;
    public Vector2 boardOrigin = new Vector2(-3.88f,-3.88f);
    public float boardCenterZ = -.2f;

    public Transform GeneratedRoot { get; private set; }

    Material grass, soil, wood, white, blue, navy, cyan, gold, leaf, darkLeaf, stone, tileA, tileB, water, coral, roof, glass, sand;

    void Start()
    {
        if (generateOnStart && !transform.Find("MUNDO_PUJLLAY_GENERADO")) GenerateWorld();
    }

    [ContextMenu("Generate World")]
    public void GenerateWorld()
    {
        ClearGenerated();
        Random.InitState(randomSeed);
        GeneratedRoot = new GameObject("MUNDO_PUJLLAY_GENERADO").transform;
        GeneratedRoot.SetParent(transform,false);
        MakeMaterials();
        BuildIsland();
        BuildEnvironment();
        if(includeBoard) BuildBoard();
        if(includeRobot) BuildRobot();
        Debug.Log("MUNDO PUJLLAY generado.",this);
    }

    [ContextMenu("Clear Generated World")]
    public void ClearGenerated()
    {
        Transform old=transform.Find("MUNDO_PUJLLAY_GENERADO");
        if(old)
        {
            if(Application.isPlaying) Destroy(old.gameObject);
            else DestroyImmediate(old.gameObject);
        }
        GeneratedRoot=null;
    }

    void MakeMaterials()
    {
        grass=Mat("Hierba menta",new Color(.2f,.65f,.4f));
        soil=Mat("Tierra",new Color(.61f,.43f,.29f));
        wood=Mat("Madera",new Color(.48f,.26f,.16f));
        white=Mat("Porcelana",new Color(.88f,.97f,1));
        blue=Mat("Azul Pujllay",new Color(.055f,.42f,.95f));
        navy=Mat("Azul noche",new Color(.025f,.09f,.2f));
        cyan=Mat("Cian",new Color(.08f,.84f,1),0,1.1f);
        gold=Mat("Oro",new Color(1,.68f,.1f));
        leaf=Mat("Follaje",new Color(.23f,.82f,.47f));
        darkLeaf=Mat("Follaje oscuro",new Color(.08f,.52f,.4f));
        stone=Mat("Piedra",new Color(.49f,.63f,.7f));
        tileA=Mat("Baldosa crema",new Color(.83f,.94f,.87f));
        tileB=Mat("Baldosa cielo",new Color(.37f,.76f,.92f));
        water=Mat("Agua",new Color(.03f,.5f,.9f),.15f);
        coral=Mat("Coral",new Color(.98f,.63f,.43f));
        roof=Mat("Tejado",new Color(.93f,.28f,.32f));
        glass=Mat("Vidrio",new Color(.24f,.84f,.96f));
        sand=Mat("Sendero",new Color(.97f,.77f,.47f));
    }

    Material Mat(string n,Color c,float metallic=0,float emission=0)
    {
        Shader s=Shader.Find("Universal Render Pipeline/Lit");
        if(!s) s=Shader.Find("Standard");
        Material m=new Material(s){name=n,color=c};
        if(m.HasProperty("_Metallic")) m.SetFloat("_Metallic",metallic);
        if(emission>0 && m.HasProperty("_EmissionColor"))
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor",c*emission);
        }
        return m;
    }

    GameObject Part(string n,PrimitiveType type,Vector3 p,Vector3 scale,Material mat,Transform parent=null)
    {
        GameObject g=GameObject.CreatePrimitive(type);
        g.name=n;
        g.transform.SetParent(parent?parent:GeneratedRoot,false);
        g.transform.localPosition=p;
        g.transform.localScale=scale;
        Renderer r=g.GetComponent<Renderer>();
        if(r && mat) r.sharedMaterial=mat;
        Collider c=g.GetComponent<Collider>();
        if(c)
        {
            if(Application.isPlaying) Destroy(c);
            else DestroyImmediate(c);
        }
        return g;
    }
    void Box(string n,Vector3 p,Vector3 s,Material m)=>Part(n,PrimitiveType.Cube,p,s,m);
    void Ball(string n,Vector3 p,Vector3 s,Material m)=>Part(n,PrimitiveType.Sphere,p,s,m);
    void Cylinder(string n,Vector3 p,Vector3 s,Material m)=>Part(n,PrimitiveType.Cylinder,p,s,m);

    public Vector3 BoardCellLocalPosition(int c,int r)
        => new Vector3(boardOrigin.x+c*cellSpacing,0,boardCenterZ+boardOrigin.y+r*cellSpacing);

    public Vector3 BoardWorldPosition(int c,int r)
        => GeneratedRoot ? GeneratedRoot.TransformPoint(BoardCellLocalPosition(c,r)) : transform.TransformPoint(BoardCellLocalPosition(c,r));

    void BuildIsland()
    {
        Box("Isla rocosa",new Vector3(0,-.77f,0),new Vector3(43,1.5f,39),stone);
        Box("Pradera",new Vector3(0,-.03f,0),new Vector3(42.4f,.48f,38.4f),grass);
        Box("Plataforma de juego",new Vector3(0,.21f,-.2f),new Vector3(11.7f,.4f,11.7f),soil);
        Box("Base del tablero",new Vector3(0,.44f,-.2f),new Vector3(10.65f,.19f,10.65f),navy);
        for(int i=0;i<38;i++)
        {
            float a=Mathf.PI*2*i/38f;
            Ball("Roca del borde",new Vector3(10.9f*Mathf.Cos(a),-.95f,9.55f*Mathf.Sin(a)),
                new Vector3(Random.Range(.75f,1.2f),.58f,.6f),stone);
        }
    }

    void BuildEnvironment()
    {
        for(int i=0;i<25;i++)
        {
            float z=-8.7f+i*.69f, x=8.45f+.38f*Mathf.Sin(i*.39f);
            Ball("Río cristalino",new Vector3(x,.34f,z),new Vector3(2.3f,.16f,1.26f),water);
        }
        Building("Academia",new Vector3(-7.5f,0,5),white,roof,4.4f,4);
        Building("Taller",new Vector3(-7.8f,0,-.3f),white,blue,3.6f,4.2f);
        Building("Casa exploradores",new Vector3(6.9f,0,5.7f),coral,roof,3.15f,3.18f);
        Bridges();
        if(includeForest){Trees(); Garden();}
        Lamps(); Clouds(); WelcomeArch(); Lookout();
        Sign("INICIO",new Vector3(-5.8f,.95f,-6.6f));
        Sign("APRENDE",new Vector3(-5.95f,.95f,5.9f));
        Sign("EXPLORA",new Vector3(5.75f,.95f,5.75f));
    }

    void Building(string n,Vector3 p,Material wall,Material top,float w,float d)
    {
        Box(n+" cimiento",p+new Vector3(0,.2f,0),new Vector3(w+.4f,.32f,d+.4f),navy);
        Box(n+" edificio",p+new Vector3(0,.94f,0),new Vector3(w,2,d),wall);
        Box(n+" techo",p+new Vector3(0,2.11f,0),new Vector3(w+.5f,.45f,d+.5f),top);
        Box(n+" puerta",p+new Vector3(0,.78f,-d*.51f),new Vector3(1,1.38f,.13f),navy);
        foreach(float x in new float[]{-w*.3f,w*.3f})
            Box(n+" ventana",p+new Vector3(x,1.36f,-d*.51f-.02f),new Vector3(.75f,.8f,.1f),glass);
    }

    void Bridges()
    {
        foreach(float z in new float[]{-4.5f,1.9f})
        {
            for(int i=0;i<6;i++) Box("Puente tabla",new Vector3(7+i*.48f,.76f,z),new Vector3(.43f,.15f,2.2f),wood);
            foreach(int side in new int[]{-1,1})
                Box("Puente baranda",new Vector3(8.25f,1.45f,z+side*1.08f),new Vector3(2.75f,.16f,.13f),blue);
        }
    }

    void Trees()
    {
        for(int i=0;i<treeCount;i++)
        {
            float x=Random.Range(-19.8f,19.8f),z=Random.Range(-17.6f,17.6f);
            if(Mathf.Abs(x)<11.3f && Mathf.Abs(z)<10) continue;
            float h=Random.Range(2.1f,4.5f);
            Cylinder("Tronco",new Vector3(x,.32f+h*.24f,z),new Vector3(.38f,.51f*h,.38f),wood);
            Material crown=i%2==0?leaf:darkLeaf;
            for(int k=0;k<3;k++)
                Ball("Copa",new Vector3(x,.7f+h*.3f+k*.48f,z),new Vector3(1.1f-k*.18f,.95f-k*.12f,1.1f-k*.18f),crown);
        }
    }

    void Garden()
    {
        for(int i=0;i<flowerCount;i++)
        {
            float x=Random.Range(-20.5f,20.5f),z=Random.Range(-18.5f,18.5f);
            if(Mathf.Abs(x)<10.8f && Mathf.Abs(z)<9.7f) continue;
            if(i%5==0) Ball("Arbusto",new Vector3(x,.54f,z),new Vector3(.9f,.8f,.9f),darkLeaf);
            else
            {
                Cylinder("Tallo",new Vector3(x,.41f,z),new Vector3(.05f,.3f,.05f),leaf);
                Ball("Flor",new Vector3(x,.6f,z),new Vector3(.26f,.13f,.26f),i%2==0?gold:coral);
            }
        }
    }

    void Lamps()
    {
        Vector3[] pts={new Vector3(-6.3f,0,-6),new Vector3(6.3f,0,-6),new Vector3(-6.3f,0,6.4f),new Vector3(6.3f,0,6.4f)};
        foreach(Vector3 p in pts)
        {
            Cylinder("Farol poste",p+Vector3.up,new Vector3(.2f,1.9f,.2f),navy);
            Ball("Farol luz",p+Vector3.up*2.05f,new Vector3(.48f,.56f,.48f),cyan);
        }
    }

    void Clouds()
    {
        Vector3[] centers={new Vector3(-12,8,5),new Vector3(4,9,12),new Vector3(11,8,4),new Vector3(-10,10,-9)};
        foreach(Vector3 p in centers)
            for(int i=0;i<3;i++) Ball("Nube",p+new Vector3((i-1)*.9f,0,0),new Vector3(i==1?2.1f:1.5f,1.1f,1.5f),white);
    }

    void WelcomeArch()
    {
        Box("Plaza bienvenida",new Vector3(-7.9f,.3f,-4.9f),new Vector3(3.8f,.24f,4),sand);
        foreach(float x in new float[]{-9.65f,-6.3f})
        {
            Cylinder("Portal columna",new Vector3(x,1.1f,-7.05f),new Vector3(.26f,2,.26f),white);
            Ball("Portal esfera",new Vector3(x,2.2f,-7.05f),new Vector3(.48f,.46f,.46f),gold);
        }
        Box("Portal arco",new Vector3(-7.95f,2.35f,-7.05f),new Vector3(3.75f,.54f,.36f),blue);
    }

    void Lookout()
    {
        Box("Mirador plataforma",new Vector3(-14,.44f,-12),new Vector3(5.7f,.37f,4.9f),wood);
        foreach(float x in new float[]{-2.4f,2.4f})
        {
            Box("Mirador banca",new Vector3(-14+x,.89f,-12),new Vector3(1.1f,.17f,.55f),blue);
            Box("Mirador respaldo",new Vector3(-14+x,1.22f,-11.74f),new Vector3(1.1f,.63f,.12f),white);
        }
    }

    void Sign(string label,Vector3 p)
    {
        Cylinder("Señal poste "+label,p,new Vector3(.22f,1.45f,.22f),wood);
        Box("Señal panel "+label,p+Vector3.up*.7f,new Vector3(1.55f,.46f,.16f),blue);
    }

    void BuildBoard()
    {
        for(int r=0;r<boardSize;r++)
        for(int c=0;c<boardSize;c++)
        {
            Vector3 p=BoardCellLocalPosition(c,r);
            Box("CASILLA_"+c+"_"+r,new Vector3(p.x,.70f,p.z),new Vector3(1.82f,.28f,1.82f),
                (r+c)%2==0?tileA:tileB);
        }
    }

    void BuildRobot()
    {
        Transform robot=new GameObject("ROBOTIN_ROOT").transform;
        robot.SetParent(GeneratedRoot,false);
        Vector3 start=BoardCellLocalPosition(0,2);
        robot.localPosition=new Vector3(start.x,.91f,start.z);
        void Piece(string n,Vector3 p,Vector3 s,Material m)=>Part(n,PrimitiveType.Sphere,p,s,m,robot);
        Piece("Torso",new Vector3(0,.73f,0),new Vector3(.96f,.94f,.76f),white);
        Piece("Abdomen azul",new Vector3(0,.39f,.025f),new Vector3(.76f,.44f,.68f),blue);
        Piece("Mochila",new Vector3(0,.70f,.36f),new Vector3(.68f,.70f,.38f),blue);
        Piece("Núcleo pecho",new Vector3(0,.80f,-.39f),new Vector3(.28f,.25f,.08f),cyan);
        Transform head=new GameObject("CABEZA").transform;
        head.SetParent(robot,false); head.localPosition=new Vector3(0,1.72f,0);
        void Face(string n,Vector3 p,Vector3 s,Material m)=>Part(n,PrimitiveType.Sphere,p,s,m,head);
        Face("Casco",Vector3.zero,new Vector3(1.68f,1.28f,1.18f),white);
        Face("Marco visor",new Vector3(0,-.03f,-.49f),new Vector3(1.44f,.94f,.22f),blue);
        Face("Pantalla",new Vector3(0,-.03f,-.57f),new Vector3(1.33f,.8f,.13f),navy);
        foreach(float x in new float[]{-.25f,.25f}) Face("Ojo",new Vector3(x,.03f,-.65f),new Vector3(.24f,.37f,.09f),cyan);
        foreach(float x in new float[]{-.79f,.79f}) Face("Auricular",new Vector3(x,-.03f,0),new Vector3(.34f,.5f,.5f),blue);
        Face("Antena",new Vector3(0,.83f,0),new Vector3(.15f,.4f,.15f),blue);
        Face("Luz antena",new Vector3(0,1.04f,0),new Vector3(.34f,.34f,.34f),cyan);
        foreach(float side in new float[]{-1,1})
        {
            Piece("Brazo",new Vector3(side*.65f,.82f,-.02f),new Vector3(.36f,.62f,.38f),white);
            Piece("Guante",new Vector3(side*.72f,.48f,-.12f),new Vector3(.4f,.36f,.38f),blue);
            Piece("Pierna",new Vector3(side*.28f,.18f,0),new Vector3(.42f,.5f,.46f),white);
            Piece("Zapato",new Vector3(side*.28f,.02f,-.18f),new Vector3(.56f,.34f,.68f),blue);
        }
    }
}
