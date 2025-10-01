//using UnityEngine;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.HighDefinition;
//using UnityEngine.Rendering.RendererUtils;

//class EnemyOutlinePass : CustomPass
//{
//    // Assign in inspector
//    public LayerMask enemyLayer;
//    public Material edgeDetectionMaterial;

//    // Internal mask RT
//    RTHandle enemyMask;

//    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
//    {
//        // Allocate a single-channel mask RT
//        enemyMask = RTHandles.Alloc(
//            Vector2.one, TextureXR.slices, dimension: TextureDimension.Tex2D,
//            colorFormat: UnityEngine.Experimental.Rendering.GraphicsFormat.R8_UNorm,
//            useDynamicScale: true, name: "_EnemyMask"
//        );
//    }

//    protected override void Execute(CustomPassContext ctx)
//    {
//        // 1. Clear and set up the mask target
//        CoreUtils.SetRenderTarget(ctx.cmd, enemyMask, ClearFlag.Color, Color.black);

//        // 2. Build a RendererList description
//        var rendererListDesc = new RendererListDesc(
//            new ShaderTagId("Forward"),          // which pass to use
//            ctx.cullingResults,
//            ctx.hdCamera.camera
//        )
//        {
//            sortingCriteria = SortingCriteria.None,
//            rendererConfiguration = PerObjectData.None,
//            renderQueueRange = RenderQueueRange.all,
//            layerMask = enemyLayer
//        };

//        // 3. Create the RendererList
//        var rendererList = ctx.renderContext.CreateRendererList(rendererListDesc);

//        // 4. Draw it into the mask RT
//        ctx.cmd.DrawRendererList(rendererList);

//        // 5. Run fullscreen edge detection, sampling the mask
//        ctx.cmd.SetGlobalTexture("_EnemyMask", enemyMask);
//        CoreUtils.DrawFullScreen(ctx.cmd, edgeDetectionMaterial);
//    }


//    protected override void Cleanup()
//    {
//        enemyMask?.Release();
//    }
//}
