using MadEngine.Core.SceneManagement;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MadEngine.Core;

public class Engine
{
    public void Initialize()
    {
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);
    }
    
    public void Render(Scene scene, Camera camera)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.Enable(EnableCap.DepthTest);
        
        Light.BindToShaders(SceneManager.ActiveScene.Lights.ToArray());
        
        Matrix4 view = camera.GetViewMatrix();
        Matrix4 projection = camera.GetPerspectiveMatrix();

        foreach (MeshRenderer meshRenderer in scene.MeshRenderers)
        {
            meshRenderer.Draw(view, projection);
        }
    }

    public void Awake(Scene scene)
    {
        foreach (GameObject gameObject in scene.GameObjects)
        {
            gameObject.Awake();
        }
    }
    
    public void Start(Scene scene)
    {
        foreach (GameObject gameObject in scene.GameObjects)
        {
            gameObject.Start();
        }
    }
    
    public void Update(float deltaTime, Scene scene)
    {
        foreach (GameObject gameObject in scene.GameObjects)
        {
            gameObject.Update(deltaTime);
        }
    }
}