using UnityEngine;
using System.Collections;

/// <summary>
/// Lógica de las tres etapas de Primeros Pasos.
/// Se puede añadir al mismo objeto que PujllayWorldGenerator.
/// Controles de prueba: F = avanzar, L = girar izquierda, R = girar derecha.
/// La interfaz completa con botones se añadirá en el siguiente bloque.
/// </summary>
public class PujllayGameController : MonoBehaviour
{
    [System.Serializable]
    public class Stage
    {
        public string title;
        public Vector2Int start;
        public int direction; // 0 este, 1 norte, 2 oeste, 3 sur
        public Vector2Int goal;
        public Vector2Int[] walls;
        public int maxCommands;
    }

    public Stage[] stages = new Stage[]
    {
        new Stage { title="1 / Primer movimiento", start=new Vector2Int(0,2), direction=0, goal=new Vector2Int(3,2), walls=new Vector2Int[0], maxCommands=6 },
        new Stage { title="2 / Aprende a girar", start=new Vector2Int(0,0), direction=0, goal=new Vector2Int(3,2), walls=new[]{new Vector2Int(2,0),new Vector2Int(2,1),new Vector2Int(4,1)}, maxCommands=11 },
        new Stage { title="3 / Camino inteligente", start=new Vector2Int(0,0), direction=1, goal=new Vector2Int(4,4), walls=new[]{new Vector2Int(1,1),new Vector2Int(2,1),new Vector2Int(3,1),new Vector2Int(1,3),new Vector2Int(2,3),new Vector2Int(3,3)}, maxCommands=18 }
    };

    public int currentStage = 0;
    public Transform robot;
    public float stepDuration = .45f;
    public string program = "";
    public bool isRunning { get; private set; }
    public string status = "Agrega comandos F, L o R y ejecuta.";
    int col,row,dir;
    static readonly Vector2Int[] Directions={Vector2Int.right,Vector2Int.up,Vector2Int.left,Vector2Int.down};

    void Start()
    {
        if (!robot)
        {
            Transform found=transform.Find("MUNDO_PUJLLAY_GENERADO/ROBOTIN_ROOT");
            if(found) robot=found;
        }
        ResetStage();
    }

    public void AddCommand(char command)
    {
        if(isRunning) return;
        if(program.Length>=stages[currentStage].maxCommands) { status="Llegaste al límite de comandos."; return; }
        if(command!='F' && command!='L' && command!='R') return;
        program+=command;
        status="Programa: "+program;
    }

    public void UndoCommand()
    {
        if(isRunning || program.Length==0) return;
        program=program.Substring(0,program.Length-1);
        status="Último comando eliminado.";
    }

    public void ClearProgram()
    {
        if(isRunning) return;
        program="";
        status="Programa borrado.";
    }

    public void SelectStage(int index)
    {
        if(isRunning || index<0 || index>=stages.Length) return;
        currentStage=index;
        ResetStage();
    }

    public void ResetStage()
    {
        Stage s=stages[currentStage];
        col=s.start.x;row=s.start.y;dir=s.direction;
        program="";
        if(robot) robot.position=BoardWorldPosition(col,row)+Vector3.up*.91f;
        UpdateRobotRotation();
        status=s.title+" — listo.";
    }

    public void RunProgram()
    {
        if(!isRunning && program.Length>0) StartCoroutine(Execute());
    }

    IEnumerator Execute()
    {
        isRunning=true;
        Stage s=stages[currentStage];
        col=s.start.x;row=s.start.y;dir=s.direction;
        if(robot) robot.position=BoardWorldPosition(col,row)+Vector3.up*.91f;
        UpdateRobotRotation();
        status="Robotín está ejecutando...";
        for(int i=0;i<program.Length;i++)
        {
            char cmd=program[i];
            if(cmd=='L') dir=(dir+1)%4;
            else if(cmd=='R') dir=(dir+3)%4;
            else if(cmd=='F')
            {
                Vector2Int next=new Vector2Int(col,row)+Directions[dir];
                if(next.x<0 || next.x>=5 || next.y<0 || next.y>=5 || IsWall(s,next))
                {
                    status="¡Ups! Robotín chocaría. Corrige el programa.";
                    isRunning=false;
                    yield break;
                }
                col=next.x;row=next.y;
                if(robot)
                {
                    Vector3 from=robot.position;
                    Vector3 to=BoardWorldPosition(col,row)+Vector3.up*.91f;
                    float t=0;
                    while(t<1)
                    {
                        t+=Time.deltaTime/stepDuration;
                        robot.position=Vector3.Lerp(from,to,Mathf.Clamp01(t));
                        yield return null;
                    }
                }
            }
            UpdateRobotRotation();
            yield return new WaitForSeconds(stepDuration*.25f);
        }
        isRunning=false;
        status=(new Vector2Int(col,row)==s.goal) ? "¡Excelente! Etapa completada." : "Aún no llegaste a la meta. Intenta otra vez.";
    }

    bool IsWall(Stage s,Vector2Int p)
    {
        foreach(Vector2Int w in s.walls) if(w==p) return true;
        return false;
    }

    Vector3 BoardWorldPosition(int c,int r)
    {
        return transform.position+new Vector3(-3.88f+c*1.94f,0,-.2f-3.88f+r*1.94f);
    }

    void UpdateRobotRotation()
    {
        if(robot) robot.rotation=Quaternion.Euler(0,-dir*90f,0);
    }

    void OnGUI()
    {
        GUI.Box(new Rect(16,16,285,220),"MUNDO PUJLLAY | PRIMEROS PASOS");
        GUI.Label(new Rect(30,48,250,22),"Etapa: "+stages[currentStage].title);
        GUI.Label(new Rect(30,72,250,22),"Programa: "+(program.Length==0?"(vacío)":program));
        GUI.Label(new Rect(30,96,250,40),status);
        if(GUI.Button(new Rect(30,142,75,30),"AVANZAR (F)")) AddCommand('F');
        if(GUI.Button(new Rect(112,142,75,30),"IZQ (L)")) AddCommand('L');
        if(GUI.Button(new Rect(194,142,75,30),"DER (R)")) AddCommand('R');
        if(GUI.Button(new Rect(30,180,75,25),"Deshacer")) UndoCommand();
        if(GUI.Button(new Rect(112,180,75,25),"Borrar")) ClearProgram();
        if(GUI.Button(new Rect(194,180,75,25),"EJECUTAR")) RunProgram();
        if(GUI.Button(new Rect(30,210,75,22),"Etapa 1")) SelectStage(0);
        if(GUI.Button(new Rect(112,210,75,22),"Etapa 2")) SelectStage(1);
        if(GUI.Button(new Rect(194,210,75,22),"Etapa 3")) SelectStage(2);
    }
}
