using UnityEngine;

/// <summary>
/// Generador procedural inicial de MUNDO PUJLLAY para Unity 6.
/// Añade este componente a un GameObject vacío y usa el menú contextual
/// "Generate World" en el Inspector. No necesita paquetes externos.
/// </summary>
public class PujllayWorldGenerator : MonoBehaviour
{
    [Header("Generación")]
    public bool generateOnStart = false;
    public int randomSeed = 21;
    public bool includeForest = true;
    public bool includeBoard = true;
    public bool includeRobot = true;

    Transform root;
    Material grass, darkGrass, soil, wood, white, blue, navy, cyan, yellow;
    Material pink, purple, leaf, leafDark, stone, tile, tileAlt, water, coral, roof, glass, sand;

    void Start()
    {
        if (generateOnStart && transform.childCount == 0) GenerateWorld();
    }

    [ContextMenu("Generate World")]
    public void GenerateWorld()
    {
        ClearGenerated();
        Random.InitState(randomSeed);
        root = new GameObject("MUNDO_PUJLLAY_GENERADO").transform;
        root.SetParent(transform, false);
        CreateMaterials();
        BuildIsland();
        BuildEnvironment();
        if (includeBoard) BuildBoard();
        if (includeRobot) BuildRobot();
        Debug.Log("MUNDO PUJLLAY: escenario inicial generado. Revisa la jerarquía.");
    }

    [ContextMenu("Clear Generated World")]
    public void ClearGenerated()
    {
        Transform old = transform.Find("MUNDO_PUJLLAY_GENERADO");
        if (old != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(old.gameObject);
            else Destroy(old.gameObject);
#else
            Destroy(old.gameObject);
#endif
        }
    }

    void CreateMaterials()
    {
        grass = Mat("Hierba menta", new Color(.20f,.65f,.40f));
        darkGrass = Mat("Hierba oscura", new Color(.12f,.41f,.27f));
        soil = Mat("Tierra arena", new Color(.61f,.43f,.29f));
        wood = Mat("Madera miel", new Color(.48f,.26f,.16f));
        white = Mat("Porcelana robot", new Color(.88f,.97f,1f));
        blue = Mat("Azul Pujllay", new Color(.055f,.42f,.95f), .18f);
        navy = Mat("Azul medianoche", new Color(.025f,.09f,.20f));
        cyan = Mat("Cian luminoso", new Color(.08f,.84f,1f), 0f, .25f, 1.2f);
        yellow = Mat("Oro recompensa", new Color(1f,.68f,.10f), .12f, .3f, .25f);
        pink = Mat("Rosa flores", new Color(1f,.28f,.57f));
        purple = Mat("Morado", new Color(.46f,.28f,.88f));
        leaf = Mat("Follaje claro", new Color(.23f,.82f,.47f));
        leafDark = Mat("Follaje oscuro", new Color(.08f,.52f,.40f));
        stone = Mat("Piedra", new Color(.49f,.63f,.70f));
        tile = Mat("Baldosa cielo", new Color(.37f,.76f,.92f));
        tileAlt = Mat("Baldosa crema", new Color(.83f,.94f,.87f));
        water = Mat("Agua cristalina", new Color(.03f,.50f,.90f), .18f, .2f, .2f);
        coral = Mat("Ladrillo crema coral", new Color(.98f,.63f,.43f));
        roof = Mat("Tejado coral", new Color(.93f,.28f,.32f));
        glass = Mat("Vidrio azul", new Color(.24f,.84f,.96f), .15f, .18f, .35f);
        sand = Mat("Sendero amarillo", new Color(.97f,.77f,.47f));
    }

    Material Mat(string n, Color c, float metal=0f, float smooth=.42f, float emission=0f)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material m = new Material(shader) { name = n, color = c };
        if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metal);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 1f-smooth);
        if (emission > 0f)
        {
            m.EnableKeyword("_EMISSION");
            if (m.HasProperty("_EmissionColor")) m.SetColor("_EmissionColor", c * emission);
        }
        return m;
    }

    GameObject Primitive(string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat, Transform parent=null)
    {
        GameObject g = GameObject.CreatePrimitive(type);
        g.name = name;
        g.transform.SetParent(parent ? parent : root, false);
        g.transform.localPosition = pos;
        g.transform.localScale = scale;
        Renderer r = g.GetComponent<Renderer>();
        if (r && mat) r.sharedMaterial = mat;
        return g;
    }

    void Box(string n, Vector3 p, Vector3 s, Material m) => Primitive(n, PrimitiveType.Cube, p, s, m);
    void Ball(string n, Vector3 p, Vector3 s, Material m) => Primitive(n, PrimitiveType.Sphere, p, s, m);
    void Cylinder(string n, Vector3 p, Vector3 s, Material m) => Primitive(n, PrimitiveType.Cylinder, p, s, m);

    void BuildIsland()
    {
        Box("ISLA | roca flotante", new Vector3(0, -.77f, 0), new Vector3(43,1.5f,39), stone);
        Box("TIERRA | pradera gigante", new Vector3(0,-.03f,0), new Vector3(42.4f,.48f,38.4f), grass);
        Box("PLATAFORMA | zona de juego", new Vector3(0,.21f,-.2f), new Vector3(11.7f,.4f,11.7f), soil);
        Box("Cimiento tablero", new Vector3(0,.44f,-.2f), new Vector3(10.65f,.19f,10.65f), navy);

        for (int i=0;i<38;i++)
        {
            float a = Mathf.PI*2*i/38f + Random.Range(-.08f,.08f);
            float x = 10.9f*Mathf.Cos(a), z = 9.55f*Mathf.Sin(a);
            Ball("Borde isla | roca", new Vector3(x,-.95f,z),
                new Vector3(Random.Range(.75f,1.2f),.58f,.60f), stone);
        }
    }

    void BuildEnvironment()
    {
        // Sendero de bienvenida
        for (int i=0;i<5;i++)
            Box("Camino bienvenida "+i, new Vector3(-7.7f+i*.25f,.24f,-6.65f+i*.47f),
                new Vector3(1.1f,.13f,.7f), tileAlt);

        // Río lateral sinuoso
        for (int i=0;i<25;i++)
        {
            float z=-8.7f+i*.69f;
            float x=8.45f+.38f*Mathf.Sin(i*.39f);
            Ball("Río | agua "+i,new Vector3(x,.34f,z),new Vector3(2.3f,.16f,1.26f),water);
            if (i%2==0)
                foreach(int side in new[]{-1,1})
                    Ball("Río | roca orilla",new Vector3(x+side*1.21f,.37f,z),new Vector3(.5f,.3f,.48f),stone);
        }

        BuildAcademy();
        BuildWorkshop();
        BuildExplorerHouse();
        BuildBridges();
        BuildTrees(155);
        BuildGarden(145);
        BuildLamps();
        BuildClouds();
        BuildWelcomeArch();
        BuildLookout();
        BuildSign("INICIO",new Vector3(-5.8f,.95f,-6.6f));
        BuildSign("APRENDE",new Vector3(-5.95f,.95f,5.9f));
        BuildSign("EXPLORA",new Vector3(5.75f,.95f,5.75f));
    }

    void BuildAcademy()
    {
        Vector3 p = new Vector3(-7.5f,0,5f);
        Box("Academia | cimiento",p+new Vector3(0,.2f,0),new Vector3(4.8f,.32f,4.5f),navy);
        Box("Academia | edificio",p+new Vector3(0,.94f,0),new Vector3(4.4f,2f,4f),white);
        Box("Academia | techo",p+new Vector3(0,2.11f,0),new Vector3(4.9f,.45f,4.5f),roof);
        Box("Academia | puerta",p+new Vector3(0,.78f,-2.04f),new Vector3(1f,1.38f,.13f),navy);
        foreach(float x in new[]{-1.35f,1.35f})
            Box("Academia | ventana",p+new Vector3(x,1.36f,-2.05f),new Vector3(.84f,.87f,.1f),glass);
        foreach(float x in new[]{-1.95f,1.95f})
        {
            Cylinder("Academia | torre",p+new Vector3(x,1.93f,0),new Vector3(.52f,2.5f,.52f),blue);
            Ball("Academia | antena",p+new Vector3(x,3.38f,0),new Vector3(.56f,.56f,.56f),cyan);
        }
        Box("Academia | letrero",p+new Vector3(0,2.5f,-2.3f),new Vector3(3.1f,.32f,.12f),blue);
    }

    void BuildWorkshop()
    {
        Vector3 p=new Vector3(-7.8f,0,3.2f);
        Box("Taller | base",p+new Vector3(0,.6f,0),new Vector3(3.6f,1.3f,4.2f),white);
        Box("Taller | techo",p+new Vector3(0,1.52f,0),new Vector3(4f,.44f,4.6f),blue);
        Box("Taller | puerta",p+new Vector3(0,.8f,-2.12f),new Vector3(1.12f,1.2f,.12f),navy);
        Box("Taller | ventanal",p+new Vector3(1.72f,.89f,-.1f),new Vector3(.12f,.7f,1.8f),glass);
    }

    void BuildExplorerHouse()
    {
        Vector3 p=new Vector3(6.9f,0,5.7f);
        Box("Casa exploradores | casa",p+new Vector3(0,.94f,0),new Vector3(3.15f,1.95f,3.18f),coral);
        Box("Casa exploradores | techo",p+new Vector3(0,2.02f,0),new Vector3(3.65f,.32f,3.65f),roof);
        Box("Casa exploradores | puerta",p+new Vector3(0,.78f,-1.66f),new Vector3(.87f,1.32f,.11f),wood);
        foreach(float x in new[]{-1f,1f})
            Box("Casa exploradores | ventana",p+new Vector3(x,1.21f,-1.68f),new Vector3(.55f,.55f,.11f),glass);
    }

    void BuildBridges()
    {
        foreach(float z in new[]{-4.5f,1.9f})
        {
            for(int i=0;i<6;i++)
                Box("Puente | tabla",new Vector3(7f+i*.48f,.76f,z),new Vector3(.43f,.15f,2.2f),wood);
            foreach(int side in new[]{-1,1})
            {
                for(int i=0;i<2;i++)
                    Cylinder("Puente | poste",new Vector3(7.05f+i*2.48f,1.22f,z+side*1.08f),new Vector3(.18f,.95f,.18f),white);
                Box("Puente | pasamanos",new Vector3(8.25f,1.67f,z+side*1.08f),new Vector3(2.75f,.16f,.13f),blue);
            }
        }
    }

    void BuildTrees(int count)
    {
        for(int i=0;i<count;i++)
        {
            float x=Random.Range(-19.8f,19.8f), z=Random.Range(-17.6f,17.6f);
            if(Mathf.Abs(x)<11.3f && Mathf.Abs(z)<10f) continue;
            float h=Random.Range(2.1f,4.5f);
            Cylinder("Bosque | tronco",new Vector3(x,.32f+h*.24f,z),new Vector3(.38f,.51f*h,.38f),wood);
            Material lm=(i%4==0)?leafDark:(i%4==1)?leaf:(i%4==2)?grass:darkGrass;
            for(int k=0;k<3;k++)
            {
                float offset=(i%3==0)?(k==1?.38f:k==2?-.32f:0f):0f;
                Ball("Bosque | copa",new Vector3(x+offset,.7f+h*.30f+k*.48f,z),
                    new Vector3(1.1f-k*.18f,.95f-k*.12f,1.1f-k*.18f),lm);
            }
        }
    }

    void BuildGarden(int count)
    {
        for(int i=0;i<count;i++)
        {
            float x=Random.Range(-20.5f,20.5f), z=Random.Range(-18.5f,18.5f);
            if(Mathf.Abs(x)<10.8f && Mathf.Abs(z)<9.7f) continue;
            if(i%5==0)
                Ball("Jardín | arbusto",new Vector3(x,.54f,z),new Vector3(.9f,.8f,.9f),leafDark);
            else if(i%7==0)
            {
                Cylinder("Seta | tallo",new Vector3(x,.53f,z),new Vector3(.2f,.39f,.2f),white);
                Ball("Seta | sombrero",new Vector3(x,.78f,z),new Vector3(.6f,.34f,.58f),roof);
            }
            else
            {
                Cylinder("Flor | tallo",new Vector3(x,.41f,z),new Vector3(.05f,.3f,.05f),leaf);
                Ball("Flor | pétalo",new Vector3(x,.60f,z),new Vector3(.26f,.13f,.26f),(i%2==0)?pink:yellow);
            }
        }
    }

    void BuildLamps()
    {
        Vector3[] pts={new(-6.3f,0,-6f),new(6.3f,0,-6f),new(-6.3f,0,6.4f),new(6.3f,0,6.4f)};
        for(int i=0;i<pts.Length;i++)
        {
            Cylinder("Farol | poste",pts[i]+Vector3.up, new Vector3(.2f,1.9f,.2f),navy);
            Ball("Farol | luz",pts[i]+Vector3.up*2.05f,new Vector3(.48f,.56f,.48f),cyan);
        }
    }

    void BuildClouds()
    {
        Vector3[] centers={new(-12,8,5),new(4,9,12),new(11,8,4),new(-10,10,-9)};
        foreach(Vector3 c in centers)
            for(int i=0;i<3;i++)
                Ball("Nube | volumen",c+new Vector3((i-1)*.9f,0,0),
                    new Vector3(i==1?2.1f:1.5f,1.1f,1.5f),white);
    }

    void BuildWelcomeArch()
    {
        Box("Plaza bienvenida",new Vector3(-7.9f,.3f,-4.9f),new Vector3(3.8f,.24f,4f),sand);
        foreach(float x in new[]{-9.65f,-6.3f})
        {
            Cylinder("Portal | columna",new Vector3(x,1.1f,-7.05f),new Vector3(.26f,2f,.26f),white);
            Ball("Portal | esfera",new Vector3(x,2.2f,-7.05f),new Vector3(.48f,.46f,.46f),yellow);
        }
        Box("Portal | arco",new Vector3(-7.95f,2.35f,-7.05f),new Vector3(3.75f,.54f,.36f),blue);
    }

    void BuildLookout()
    {
        Box("Mirador | plataforma",new Vector3(-14,.44f,-12),new Vector3(5.7f,.37f,4.9f),wood);
        foreach(float x in new[]{-2.4f,2.4f})
        {
            Box("Mirador | banca",new Vector3(-14+x,.89f,-12),new Vector3(1.1f,.17f,.55f),blue);
            Box("Mirador | respaldo",new Vector3(-14+x,1.22f,-11.74f),new Vector3(1.1f,.63f,.12f),white);
        }
    }

    void BuildSign(string label, Vector3 p)
    {
        Cylinder("Señal | poste "+label,p,new Vector3(.22f,1.45f,.22f),wood);
        Box("Señal | panel "+label,p+Vector3.up*.7f,new Vector3(1.55f,.46f,.16f),blue);
    }

    void BuildBoard()
    {
        const int size=5;
        const float step=1.94f, origin=-3.88f, boardY=-.2f;
        for(int r=0;r<size;r++)
            for(int c=0;c<size;c++)
            {
                float x=origin+c*step, z=boardY+origin+r*step;
                Material m=((r+c)%2==0)?tileAlt:tile;
                Box($"CASILLA_{c}_{r}",new Vector3(x,.70f,z),new Vector3(1.82f,.28f,1.82f),m);
            }
        // Inicio y meta de la etapa 1
        Vector3 start=BoardPosition(0,2), goal=BoardPosition(3,2);
        Box("INICIO | casilla",new Vector3(start.x,.72f,start.z),new Vector3(1.82f,.3f,1.82f),Mat("Inicio turquesa",new Color(.06f,.81f,.62f)));
        Box("META | casilla",new Vector3(goal.x,.72f,goal.z),new Vector3(1.82f,.3f,1.82f),yellow);
        Ball("META | pedestal",new Vector3(goal.x,1.03f,goal.z),new Vector3(.9f,.28f,.9f),yellow);
        // Obstáculos visibles de etapas 2 y 3
        foreach(Vector2Int cell in new[]{new(2,0),new(2,1),new(4,1),new(1,1),new(3,1),new(1,3),new(2,3),new(3,3)})
        {
            Vector3 p=BoardPosition(cell.x,cell.y);
            Ball("OBSTACULO | roca",new Vector3(p.x,1.25f,p.z),new Vector3(1.05f,.9f,1.0f),stone);
        }
    }

    Vector3 BoardPosition(int c,int r) => new Vector3(-3.88f+c*1.94f,0,-.2f-3.88f+r*1.94f);

    void BuildRobot()
    {
        Transform robot=new GameObject("ROBOTIN_ROOT").transform;
        robot.SetParent(root,false);
        Vector3 start=BoardPosition(0,2);
        robot.localPosition=new Vector3(start.x,.91f,start.z);

        void Part(string n, Vector3 p, Vector3 s, Material m)
        {
            GameObject g=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.name=n;g.transform.SetParent(robot,false);g.transform.localPosition=p;g.transform.localScale=s;
            g.GetComponent<Renderer>().sharedMaterial=m;
        }
        Part("Robotin | torso",new Vector3(0,.73f,0),new Vector3(.96f,.94f,.76f),white);
        Part("Robotin | abdomen azul",new Vector3(0,.39f,.025f),new Vector3(.76f,.44f,.68f),blue);
        Part("Robotin | mochila",new Vector3(0,.70f,.36f),new Vector3(.68f,.70f,.38f),blue);
        Part("Robotin | núcleo pecho",new Vector3(0,.80f,-.39f),new Vector3(.28f,.25f,.08f),cyan);

        Transform head=new GameObject("RIG | CABEZA").transform;
        head.SetParent(robot,false);head.localPosition=new Vector3(0,1.72f,0);
        void HeadPart(string n,Vector3 p,Vector3 s,Material m)
        {
            GameObject g=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.name=n;g.transform.SetParent(head,false);g.transform.localPosition=p;g.transform.localScale=s;
            g.GetComponent<Renderer>().sharedMaterial=m;
        }
        HeadPart("Robotin | casco",Vector3.zero,new Vector3(1.68f,1.28f,1.18f),white);
        HeadPart("Robotin | visor marco",new Vector3(0,-.03f,-.49f),new Vector3(1.44f,.94f,.22f),blue);
        HeadPart("Robotin | pantalla",new Vector3(0,-.03f,-.57f),new Vector3(1.33f,.80f,.13f),navy);
        foreach(float x in new[]{-.25f,.25f})
            HeadPart("Robotin | ojo",new Vector3(x,.03f,-.65f),new Vector3(.24f,.37f,.09f),cyan);
        foreach(float x in new[]{-.79f,.79f})
            HeadPart("Robotin | auricular",new Vector3(x,-.03f,0),new Vector3(.34f,.5f,.5f),blue);
        HeadPart("Robotin | antena",new Vector3(0,.83f,0),new Vector3(.15f,.4f,.15f),blue);
        HeadPart("Robotin | luz antena",new Vector3(0,1.04f,0),new Vector3(.34f,.34f,.34f),cyan);

        foreach(float side in new[]{-1f,1f})
        {
            Part("Robotin | brazo",new Vector3(side*.65f,.82f,-.02f),new Vector3(.36f,.62f,.38f),white);
            Part("Robotin | guante",new Vector3(side*.72f,.48f,-.12f),new Vector3(.4f,.36f,.38f),blue);
            Part("Robotin | pierna",new Vector3(side*.28f,.18f,0),new Vector3(.42f,.5f,.46f),white);
            Part("Robotin | zapato",new Vector3(side*.28f,.02f,-.18f),new Vector3(.56f,.34f,.68f),blue);
        }
    }
}
