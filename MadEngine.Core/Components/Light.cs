using OpenTK.Mathematics;

namespace MadEngine.Core;

public class Light : Component
{
    public Vector4 AmbientColor = Vector4.One * 0.2f;
    public Vector4 DiffuseColor = Vector4.One;
    public Vector4 SpecularColor = Vector4.One * 0.5f;

    public virtual void Use(Shader shader, int index)
    {
        UseColors(shader, index, "lights");
    }

    public void UseColors(Shader shader, int index, string type)
    {
        shader.SetVector4($"{type}[{index}].ambient", AmbientColor);
        shader.SetVector4($"{type}[{index}].diffuse", DiffuseColor);
        shader.SetVector4($"{type}[{index}].specular", SpecularColor);
    }

    public static void UseLights(Shader shader, Light[] lights)
    {
        int dir = 0;
        int point = 0;
        int spot = 0;

        foreach (Light light in lights)
        {
            switch (light)
            {
                case DirectionalLight dl:
                    dl.Use(shader, dir++);
                    break;

                case SpotLight sl:
                    sl.Use(shader, spot++);
                    break;

                case PointLight pl:
                    pl.Use(shader, point++);
                    break;
            }
        }
        shader.SetInt("dirLightCount", dir);
        shader.SetInt("pointLightCount", point);
        shader.SetInt("spotLightCount", spot);
    }
}

public class DirectionalLight(Vector3 direction) : Light
{
    public Vector3 Direction = direction;

    public override void Use(Shader shader, int index)
    {
        UseColors(shader, index, "dirLights");
        shader.SetVector3($"dirLights[{index}].direction", Direction);
    }
}

public class PointLight : Light
{
    public float Constant = 1f;
    public float Linear = 0.07f;
    public float Quadratic = 0.017f;
    
    public override void Use(Shader shader, int index)
    {
        UseColors(shader, index, "pointLights");
        shader.SetFloat($"pointLights[{index}].constant", Constant);
        shader.SetFloat($"pointLights[{index}].linear", Linear);
        shader.SetFloat($"pointLights[{index}].quadratic", Quadratic);
        shader.SetVector3($"pointLights[{index}].position", GameObject.Transform.GetWorldPosition());
    }
}

public class SpotLight : PointLight
{
    public Vector3 Direction;
    public float CutOff = 12.5f;
    public float OuterCutOff = 17.5f;
    
    public override void Use(Shader shader, int index)
    {
        UseColors(shader, index, "spotLights");
        shader.SetVector3($"spotLights[{index}].direction", Direction.Normalized());
        shader.SetFloat($"spotLights[{index}].cutOff", (float)Math.Cos(MathHelper.DegreesToRadians(CutOff)));
        shader.SetFloat($"spotLights[{index}].outerCutOff", (float)Math.Cos(MathHelper.DegreesToRadians(OuterCutOff)));
        shader.SetFloat($"spotLights[{index}].constant", Constant);
        shader.SetFloat($"spotLights[{index}].linear", Linear);
        shader.SetFloat($"spotLights[{index}].quadratic", Quadratic);
        shader.SetVector3($"spotLights[{index}].position", GameObject.Transform.GetWorldPosition());
    }
}