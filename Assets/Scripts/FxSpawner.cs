using UnityEngine;

// Cheap, code-only "juice" effects - no particle asset needed. Builds one
// tiny flat-color texture/material once and reuses it for every burst, so
// there is zero per-effect asset cost beyond the ParticleSystem itself.
// Each burst is a short-lived GameObject that plays once and destroys
// itself - fine for how infrequently hits/deaths happen relative to a
// frame, no pooling needed.
public static class FxSpawner
{
    private static Material cachedMaterial;

    private static Material GetMaterial()
    {
        if (cachedMaterial != null) return cachedMaterial;

        Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[16];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = new Color32(255, 255, 255, 255);
        tex.SetPixels32(pixels);
        tex.Apply();

        // Sprites/Default is a built-in shader that renders correctly
        // under URP's 2D Renderer without needing a custom URP particle
        // shader asset - the same trick used for simple flat-color UI
        // sprites elsewhere in this project.
        Shader shader = Shader.Find("Sprites/Default");
        cachedMaterial = new Material(shader);
        cachedMaterial.mainTexture = tex;
        return cachedMaterial;
    }

    // Spawns a short radial particle burst at 'position' - a handful of
    // tiny squares flying outward and fading out. Used for knife hits,
    // enemy deaths, and the player taking damage; color/count/size tune
    // how big the moment feels.
    public static void Burst(Vector3 position, Color color, int count = 8, float speed = 2.5f, float lifetime = 0.3f, float size = 0.12f)
    {
        GameObject go = new GameObject("FxBurst");
        go.transform.position = position;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.duration = lifetime;
        main.loop = false;
        main.startLifetime = lifetime;
        main.startSpeed = speed;
        main.startSize = size;
        main.startColor = color;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0f;
        main.stopAction = ParticleSystemStopAction.None;

        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, (short)count) });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.05f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = grad;

        ParticleSystemRenderer psRenderer = go.GetComponent<ParticleSystemRenderer>();
        psRenderer.material = GetMaterial();
        psRenderer.sortingOrder = 10;

        ps.Play();
        Object.Destroy(go, lifetime + 0.2f);
    }
}
