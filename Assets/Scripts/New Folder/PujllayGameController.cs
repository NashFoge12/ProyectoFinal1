using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Programación 2D para mover a Robotín por el tablero 3D.
/// F=avanzar, B=retroceder, L=girar izquierda, R=girar derecha.
/// </summary>
[DisallowMultipleComponent]
public class PujllayGameController : MonoBehaviour
{
    [System.Serializable]
    public class Stage
    {
        public string title;
        public Vector2Int start;
        [Range(0,3)] public int direction;
        public Vector2Int goal;
        public Vector2Int[] walls;
        [Min(1)] public int maxCommands;
    }

    public Stage[] stages =
    {
        new Stage{title="1 · Primer movimiento",start=new Vector2Int(0,2),direction=0,goal=new Vector2Int(3,2),walls=new Vector2Int[0],maxCommands=6},
        new Stage{title="2 · Aprende a girar",start=new Vector2Int(0,0),direction=0,goal=new Vector2Int(3,2),
            walls=new[]{new Vector2Int(2,0),new Vector2Int(2,1),new Vector2Int(4,1)},maxCommands=11},
        new Stage{title="3 · Camino inteligente",start=new Vector2Int(0,0),direction=1,goal=new Vector2Int(4,4),
            walls=new[]{new Vector2Int(1,1),new Vector2Int(2,1),new Vector2Int(3,1),new Vector2Int(1,3),new Vector2Int(2,3),new Vector2Int(3,3)},maxCommands=18}
    };

    [Header("Referencias")]
    public PujllayWorldGenerator world;
    public Transform robot;
    [Header("Movimiento")]
    [Min(.05f)] public float stepDuration=.42f;
    public float robotHeight=.91f;
    public bool createInterfaceOnStart=true;

    [SerializeField] int currentStage;
    [SerializeField] string program="";
    [SerializeField] string status="¡Agrega comandos para comenzar!";
    public bool IsRunning{get;private set;}
    public string Program=>program;
    public string Status=>status;

    int col,row,dir;
    Canvas canvas;
    TMP_Text titleText,statusText,programText,countText;
    Button executeButton;
    Transform stageVisuals;
    Transform goalStar;
    Vector3 starBasePosition;
    bool starCollected;
    PujllayCameraController cameraController;
    readonly Vector2Int[] directions={Vector2Int.right,Vector2Int.up,Vector2Int.left,Vector2Int.down};

    void Awake()
    {
        if(!world) world=GetComponent<PujllayWorldGenerator>();
    }

    void Start()
    {
        // Garantiza que el mundo exista antes de buscar a Robotín,
        // independientemente del orden en que Unity ejecute los Start().
        if(world && !world.GeneratedRoot) world.GenerateWorld();
        FindRobot();
        SetupGameCamera();
        if(createInterfaceOnStart) BuildInterface();
        ResetStage();
    }

    void FindRobot()
    {
        if(robot) return;
        if(world && world.GeneratedRoot)
        {
            Transform found=world.GeneratedRoot.Find("ROBOTIN_ROOT");
            if(found) robot=found;
        }
        if(!robot)
        {
            Transform found=transform.Find("MUNDO_PUJLLAY_GENERADO/ROBOTIN_ROOT");
            if(found) robot=found;
        }
    }

    public void AddCommand(char cmd)
    {
        if(IsRunning || stages==null || stages.Length==0) return;
        cmd=char.ToUpperInvariant(cmd);
        if(cmd!='F' && cmd!='B' && cmd!='L' && cmd!='R') return;
        if(program.Length>=stages[currentStage].maxCommands){SetStatus("¡Límite de comandos alcanzado!");return;}
        program+=cmd;
        SetStatus("Comando añadido.");
        RefreshUI();
    }

    public void UndoCommand()
    {
        if(IsRunning || program.Length==0) return;
        program=program.Substring(0,program.Length-1);
        SetStatus("Último comando eliminado.");
        RefreshUI();
    }

    public void ClearProgram()
    {
        if(IsRunning) return;
        program="";
        SetStatus("Programa borrado.");
        RefreshUI();
    }

    public void SelectStage(int index)
    {
        if(IsRunning || index<0 || index>=stages.Length) return;
        currentStage=index;
        ResetStage();
    }

    public void ResetStage()
    {
        if(stages==null || stages.Length==0) return;
        StopAllCoroutines();
        IsRunning=false;
        Stage s=stages[Mathf.Clamp(currentStage,0,stages.Length-1)];
        col=s.start.x; row=s.start.y; dir=s.direction; program="";
        FindRobot();
        if(robot) robot.position=CellPosition(col,row)+Vector3.up*robotHeight;
        UpdateRobotRotation();
        BuildStageVisuals();
        SetStatus(s.title+" · ¡Listo!");
        RefreshUI();
    }

    public void RunProgram()
    {
        if(IsRunning) return;
        if(program.Length==0){SetStatus("Primero agrega comandos.");return;}
        StartCoroutine(ExecuteProgram());
    }

    IEnumerator ExecuteProgram()
    {
        IsRunning=true;
        Stage s=stages[currentStage];
        col=s.start.x;row=s.start.y;dir=s.direction;
        if(robot) robot.position=CellPosition(col,row)+Vector3.up*robotHeight;
        UpdateRobotRotation();
        SetStatus("¡Robotín está ejecutando!");
        RefreshUI();

        for(int i=0;i<program.Length;i++)
        {
            char cmd=program[i];
            if(cmd=='L') dir=(dir+1)%4;
            else if(cmd=='R') dir=(dir+3)%4;
            else if(cmd=='F' || cmd=='B')
            {
                Vector2Int step=cmd=='F'?directions[dir]:-directions[dir];
                Vector2Int next=new Vector2Int(col,row)+step;
                if(!Inside(next))
                {
                    IsRunning=false;SetStatus("¡Cuidado! Robotín saldría del tablero.");RefreshUI();yield break;
                }
                if(IsWall(s,next))
                {
                    IsRunning=false;SetStatus("¡Obstáculo! Corrige tu programa.");RefreshUI();yield break;
                }
                Vector3 from=robot?robot.position:CellPosition(col,row)+Vector3.up*robotHeight;
                col=next.x;row=next.y;
                Vector3 to=CellPosition(col,row)+Vector3.up*robotHeight;
                UpdateRobotRotation();
                float t=0;
                while(t<1)
                {
                    t+=Time.deltaTime/stepDuration;
                    float smooth=Mathf.SmoothStep(0,1,Mathf.Clamp01(t));
                    if(robot) robot.position=Vector3.Lerp(from,to,smooth);
                    yield return null;
                }
                if(robot) robot.position=to;
            }
            UpdateRobotRotation();
            yield return new WaitForSeconds(stepDuration*.15f);
        }

        IsRunning=false;
        bool won=new Vector2Int(col,row)==s.goal;
        if(won)
        {
            SetStatus("¡Excelente! ¡Recogiste la estrella! ⭐");
            yield return StartCoroutine(CollectGoalStar());
        }
        else SetStatus("Terminó el programa, pero aún no llegaste. ¡Intenta otra vez!");
        RefreshUI();
    }

    bool Inside(Vector2Int p)=>world && p.x>=0 && p.x<world.boardSize && p.y>=0 && p.y<world.boardSize;
    bool IsWall(Stage s,Vector2Int p)
    {
        if(s.walls==null)return false;
        foreach(Vector2Int w in s.walls)if(w==p)return true;
        return false;
    }

    Vector3 CellPosition(int c,int r)
    {
        if(world)return world.BoardWorldPosition(c,r);
        return transform.TransformPoint(new Vector3(-3.88f+c*1.94f,0,-.2f-3.88f+r*1.94f));
    }

    void UpdateRobotRotation()
    {
        // El rostro del modelo mira a -Z. Dirección: 0 este, 1 norte, 2 oeste, 3 sur.
        if(robot)robot.rotation=Quaternion.Euler(0,90f-dir*90f,0);
    }

    void BuildStageVisuals()
    {
        if(!world || !world.GeneratedRoot)return;
        if(stageVisuals)
        {
            if(Application.isPlaying)Destroy(stageVisuals.gameObject);
            else DestroyImmediate(stageVisuals.gameObject);
        }
        stageVisuals=new GameObject("VISUALES_ETAPA").transform;
        stageVisuals.SetParent(world.GeneratedRoot,false);
        Stage s=stages[currentStage];
        starCollected=false;
        MakeMarker("INICIO",s.start,new Color(.08f,.82f,.55f),.87f);
        MakeMarker("META",s.goal,new Color(1f,.68f,.08f),.91f);
        CreateGoalStar(s.goal);
        if(s.walls==null)return;
        foreach(Vector2Int cell in s.walls)
        {
            if(!Inside(cell))continue;
            Vector3 p=world.BoardCellLocalPosition(cell.x,cell.y);
            GameObject rock=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name="Obstáculo";
            rock.transform.SetParent(stageVisuals,false);
            rock.transform.localPosition=new Vector3(p.x,1.15f,p.z);
            rock.transform.localScale=new Vector3(.92f,.92f,.92f);
            rock.GetComponent<Renderer>().sharedMaterial=NewMaterial(new Color(.43f,.57f,.66f));
            Collider c=rock.GetComponent<Collider>();if(c)Destroy(c);
        }
    }

    void SetupGameCamera()
    {
        Camera cam=Camera.main;
        if(!cam)
        {
            GameObject cameraObject=new GameObject("PUJLLAY_MAIN_CAMERA");
            cameraObject.tag="MainCamera";
            cam=cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }
        cam.enabled=true;
        // Reserva la zona inferior para el panel de controles en formato vertical.
        cam.rect=new Rect(0f, .36f, 1f, .64f);
        cam.fieldOfView=48f;
        cam.nearClipPlane=.1f;
        cam.farClipPlane=250f;
        cameraController=cam.GetComponent<PujllayCameraController>();
        if(!cameraController) cameraController=cam.gameObject.AddComponent<PujllayCameraController>();
        Vector3 target=world?world.transform.TransformPoint(new Vector3(1.2f,.9f,0)):transform.position+Vector3.up;
        cameraController.distance=16f;
        cameraController.pitch=48f;
        cameraController.minDistance=8f;
        cameraController.maxDistance=25f;
        cameraController.SetTarget(target);

        bool hasDirectional=false;
        foreach(Light lightSource in FindObjectsOfType<Light>())
            if(lightSource.type==LightType.Directional){hasDirectional=true;break;}
        if(!hasDirectional)
        {
            GameObject sun=new GameObject("PUJLLAY_LUZ_SOLAR");
            Light lightSource=sun.AddComponent<Light>();
            lightSource.type=LightType.Directional;
            lightSource.intensity=1.25f;
            sun.transform.rotation=Quaternion.Euler(48f,-32f,0);
        }
    }

    void CreateGoalStar(Vector2Int cell)
    {
        Vector3 p=world.BoardCellLocalPosition(cell.x,cell.y);
        goalStar=new GameObject("ESTRELLA_COLECCIONABLE").transform;
        goalStar.SetParent(stageVisuals,false);
        starBasePosition=new Vector3(p.x,2.05f,p.z);
        goalStar.localPosition=starBasePosition;
        goalStar.localScale=Vector3.one*.9f;

        Mesh mesh=new Mesh();
        Vector3[] vertices=new Vector3[11];
        int[] triangles=new int[60];
        vertices[0]=Vector3.zero;
        for(int i=0;i<10;i++)
        {
            float angle=Mathf.PI*.5f+i*Mathf.PI/5f;
            float radius=(i%2==0)? .72f : .32f;
            vertices[i+1]=new Vector3(Mathf.Cos(angle)*radius,Mathf.Sin(angle)*radius,0);
        }
        for(int i=0;i<10;i++)
        {
            int t=i*3;
            triangles[t]=0;
            triangles[t+1]=(i+1)%10+1;
            triangles[t+2]=i+1;
            int back=t+30;
            triangles[back]=0;
            triangles[back+1]=i+1;
            triangles[back+2]=(i+1)%10+1;
        }
        mesh.vertices=vertices;mesh.triangles=triangles;mesh.RecalculateNormals();
        GameObject starFace=new GameObject("Estrella dorada");
        starFace.transform.SetParent(goalStar,false);
        starFace.AddComponent<MeshFilter>().sharedMesh=mesh;
        MeshRenderer renderer=starFace.AddComponent<MeshRenderer>();
        renderer.sharedMaterial=NewMaterial(new Color(1f,.72f,.08f));
        renderer.sharedMaterial.EnableKeyword("_EMISSION");
        if(renderer.sharedMaterial.HasProperty("_EmissionColor"))
            renderer.sharedMaterial.SetColor("_EmissionColor",new Color(1f,.55f,.03f)*1.5f);

        GameObject glow=GameObject.CreatePrimitive(PrimitiveType.Sphere);
        glow.name="Brillo de estrella";glow.transform.SetParent(goalStar,false);
        glow.transform.localPosition=new Vector3(0,0,.12f);
        glow.transform.localScale=new Vector3(.95f,.95f,.16f);
        glow.GetComponent<Renderer>().sharedMaterial=NewMaterial(new Color(1f,.83f,.25f,.35f));
        Collider col=glow.GetComponent<Collider>();if(col)Destroy(col);
    }

    IEnumerator CollectGoalStar()
    {
        if(!goalStar) yield break;
        starCollected=true;
        Vector3 initial=goalStar.localScale;
        float elapsed=0f;
        while(elapsed<.45f && goalStar)
        {
            elapsed+=Time.deltaTime;
            float t=Mathf.Clamp01(elapsed/.45f);
            goalStar.localScale=Vector3.Lerp(initial,Vector3.zero,t);
            goalStar.Rotate(Vector3.forward,900f*Time.deltaTime,Space.Self);
            yield return null;
        }
        if(goalStar)goalStar.gameObject.SetActive(false);
    }

    void MakeMarker(string n,Vector2Int cell,Color color,float y)
    {
        Vector3 p=world.BoardCellLocalPosition(cell.x,cell.y);
        GameObject tile=GameObject.CreatePrimitive(PrimitiveType.Cube);
        tile.name=n;
        tile.transform.SetParent(stageVisuals,false);
        tile.transform.localPosition=new Vector3(p.x,y,p.z);
        tile.transform.localScale=new Vector3(1.78f,.055f,1.78f);
        tile.GetComponent<Renderer>().sharedMaterial=NewMaterial(color);
        Collider c=tile.GetComponent<Collider>();if(c)Destroy(c);
    }

    Material NewMaterial(Color c)
    {
        Shader s=Shader.Find("Universal Render Pipeline/Lit");
        if(!s)s=Shader.Find("Standard");
        return new Material(s){color=c};
    }

    // UI 2D optimizada para pantalla vertical: mundo arriba, controles abajo.
    void BuildInterface()
    {
        GameObject cgo=new GameObject("PUJLLAY_UI_2D");
        canvas=cgo.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=30;
        CanvasScaler scaler=cgo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution=new Vector2(1080,1920);scaler.matchWidthOrHeight=.5f;
        cgo.AddComponent<GraphicRaycaster>();

        if(!FindObjectOfType<EventSystem>())
        {
            GameObject es=new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        GameObject panelGO=new GameObject("Panel inferior de programación",typeof(RectTransform),typeof(Image));
        panelGO.transform.SetParent(cgo.transform,false);
        RectTransform panel=panelGO.GetComponent<RectTransform>();
        panel.anchorMin=new Vector2(0,0);panel.anchorMax=new Vector2(1,0);panel.pivot=new Vector2(.5f,0);
        panel.anchoredPosition=new Vector2(0,18);panel.sizeDelta=new Vector2(-28,680);
        Image panelImage=panelGO.GetComponent<Image>();panelImage.color=new Color(.025f,.07f,.14f,.97f);

        titleText=Text(panel,"Título","MUNDO PUJLLAY",29,FontStyles.Bold,new Vector2(0,-16),new Vector2(-30,48),Color.white);
        statusText=Text(panel,"Estado",status,17,FontStyles.Normal,new Vector2(0,-67),new Vector2(-30,52),new Color(.72f,.9f,1));
        programText=Text(panel,"Programa","TUS COMANDOS: —",21,FontStyles.Bold,new Vector2(0,-122),new Vector2(-30,66),Color.white);
        countText=Text(panel,"Contador","0 comandos",15,FontStyles.Normal,new Vector2(0,-190),new Vector2(-30,28),new Color(.7f,.82f,.92f));

        // Cuatro movimientos, en una cuadrícula 2 x 2 con botones grandes para tacto.
        ButtonAt(panel,"⬆  AVANZAR",new Vector2(22,-228),new Vector2(490,76),new Color(.08f,.48f,.88f),()=>AddCommand('F'));
        ButtonAt(panel,"⬇  RETROCEDER",new Vector2(526,-228),new Vector2(490,76),new Color(.08f,.48f,.88f),()=>AddCommand('B'));
        ButtonAt(panel,"↶  IZQUIERDA",new Vector2(22,-316),new Vector2(490,76),new Color(.48f,.28f,.82f),()=>AddCommand('L'));
        ButtonAt(panel,"↷  DERECHA",new Vector2(526,-316),new Vector2(490,76),new Color(.48f,.28f,.82f),()=>AddCommand('R'));

        executeButton=ButtonAt(panel,"▶ EJECUTAR",new Vector2(22,-408),new Vector2(245,70),new Color(.08f,.62f,.38f),RunProgram);
        ButtonAt(panel,"↶ DESHACER",new Vector2(275,-408),new Vector2(235,70),new Color(.25f,.35f,.47f),UndoCommand);
        ButtonAt(panel,"BORRAR",new Vector2(518,-408),new Vector2(220,70),new Color(.72f,.24f,.3f),ClearProgram);
        ButtonAt(panel,"REINICIAR",new Vector2(746,-408),new Vector2(270,70),new Color(.82f,.48f,.12f),ResetStage);

        Text(panel,"Etapas","ELIGE UNA ETAPA",15,FontStyles.Bold,new Vector2(0,-495),new Vector2(-30,28),new Color(.7f,.84f,.95f));
        for(int i=0;i<stages.Length;i++)
        {
            int index=i;
            ButtonAt(panel,"ETAPA "+(i+1),new Vector2(22+i*336,-532),new Vector2(320,62),new Color(.13f,.25f,.39f),()=>SelectStage(index));
        }
        RefreshUI();
    }

    RectTransform Panel(Transform parent,string n,Vector2 pos,Vector2 size,Color color)
    {
        GameObject g=new GameObject(n,typeof(RectTransform),typeof(Image));
        g.transform.SetParent(parent,false);
        RectTransform r=g.GetComponent<RectTransform>();
        r.anchorMin=new Vector2(0,1);r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);
        r.anchoredPosition=pos;r.sizeDelta=size;g.GetComponent<Image>().color=color;
        return r;
    }

    TMP_Text Text(Transform parent,string n,string value,float size,FontStyles style,Vector2 pos,Vector2 dimensions,Color color)
    {
        GameObject g=new GameObject(n,typeof(RectTransform),typeof(TextMeshProUGUI));
        g.transform.SetParent(parent,false);
        RectTransform r=g.GetComponent<RectTransform>();
        r.anchorMin=new Vector2(0,1);r.anchorMax=new Vector2(1,1);r.pivot=new Vector2(.5f,1);
        r.anchoredPosition=pos;r.sizeDelta=dimensions;
        TMP_Text t=g.GetComponent<TMP_Text>();
        t.text=value;t.fontSize=size;t.fontStyle=style;t.color=color;
        t.alignment=TextAlignmentOptions.Center;t.enableWordWrapping=true;
        return t;
    }

    Button ButtonAt(Transform parent,string label,Vector2 pos,Vector2 dimensions,Color color,UnityEngine.Events.UnityAction action)
    {
        GameObject g=new GameObject(label,typeof(RectTransform),typeof(Image),typeof(Button));
        g.transform.SetParent(parent,false);
        RectTransform r=g.GetComponent<RectTransform>();
        r.anchorMin=new Vector2(0,1);r.anchorMax=new Vector2(0,1);r.pivot=new Vector2(0,1);
        r.anchoredPosition=pos;r.sizeDelta=dimensions;
        Image image=g.GetComponent<Image>();image.color=color;
        Button b=g.GetComponent<Button>();b.onClick.AddListener(action);
        ColorBlock cb=b.colors;cb.highlightedColor=Color.Lerp(color,Color.white,.2f);cb.pressedColor=Color.Lerp(color,Color.black,.2f);b.colors=cb;
        GameObject labelGO=new GameObject("Texto",typeof(RectTransform),typeof(TextMeshProUGUI));
        labelGO.transform.SetParent(g.transform,false);
        RectTransform lr=labelGO.GetComponent<RectTransform>();lr.anchorMin=Vector2.zero;lr.anchorMax=Vector2.one;lr.offsetMin=new Vector2(4,3);lr.offsetMax=new Vector2(-4,-3);
        TMP_Text text=labelGO.GetComponent<TMP_Text>();text.text=label;text.fontSize=19;text.fontStyle=FontStyles.Bold;text.color=Color.white;text.alignment=TextAlignmentOptions.Center;
        return b;
    }

    void SetStatus(string message){status=message;if(statusText)statusText.text=message;}

    void RefreshUI()
    {
        if(titleText && stages.Length>0)titleText.text="MUNDO PUJLLAY · "+stages[currentStage].title.ToUpperInvariant();
        if(statusText)statusText.text=status;
        if(programText)programText.text="TUS COMANDOS: "+(program.Length==0?"—":string.Join("   ",System.Array.ConvertAll(program.ToCharArray(), c => c=='F'?"⬆":c=='B'?"⬇":c=='L'?"↶":"↷")));
        if(countText && stages.Length>0)countText.text=program.Length+" / "+stages[currentStage].maxCommands+" comandos";
        if(executeButton)executeButton.interactable=!IsRunning && program.Length>0;
    }

    void Update()
    {
        if(goalStar && !starCollected && goalStar.gameObject.activeInHierarchy)
        {
            goalStar.localPosition=starBasePosition+Vector3.up*(Mathf.Sin(Time.time*2.8f)*.16f);
            goalStar.Rotate(Vector3.up,75f*Time.deltaTime,Space.World);
        }
        if(Input.GetKeyDown(KeyCode.F))AddCommand('F');
        if(Input.GetKeyDown(KeyCode.B))AddCommand('B');
        if(Input.GetKeyDown(KeyCode.L))AddCommand('L');
        if(Input.GetKeyDown(KeyCode.R))AddCommand('R');
        if(Input.GetKeyDown(KeyCode.Backspace))UndoCommand();
        if(Input.GetKeyDown(KeyCode.Return))RunProgram();
    }

    void OnDestroy()
    {
        if(canvas)Destroy(canvas.gameObject);
    }
}
