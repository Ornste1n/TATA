using Latios;
using Unity.Entities;
using Latios.Authoring;
using Unity.Collections;

[UnityEngine.Scripting.Preserve]
public class BakingBootstrap : ICustomBakingBootstrap
{
    public void InitializeBakingForAllWorlds(ref CustomBakingBootstrapContext context)
    {
        Latios.Kinemation.Authoring.KinemationBakingBootstrap.InstallKinemation(ref context);
        Latios.Mecanim.Authoring.MecanimBakingBootstrap.InstallMecanimAddon(ref context);
        Latios.Unika.Authoring.UnikaBakingBootstrap.InstallUnikaEntitySerialization(ref context);
    }
}

[UnityEngine.Scripting.Preserve]
public class EditorBootstrap : ICustomEditorBootstrap
{
    public World Initialize(string defaultEditorWorldName)
    {
        LatiosWorld world = new LatiosWorld(defaultEditorWorldName, WorldFlags.Editor);

        NativeList<SystemTypeIndex> systems = DefaultWorldInitialization.GetAllSystemTypeIndices(WorldSystemFilterFlags.Default, true);
        BootstrapTools.InjectUnitySystems(systems, world, world.simulationSystemGroup);

        Latios.Kinemation.KinemationBootstrap.InstallKinemation(world);
        Latios.Calligraphics.CalligraphicsBootstrap.InstallCalligraphics(world);

        BootstrapTools.InjectUserSystems(systems, world, world.simulationSystemGroup);

        return world;
    }
}

[UnityEngine.Scripting.Preserve]
public class Bootstrap : ICustomBootstrap
{
    public static World MenuWorld;
    public static LatiosWorld GameWorld;

    public static LatiosWorld CreateGameWorld()
    {
        LatiosWorld gameWorld = new LatiosWorld("GameWorld");
        GameWorld = gameWorld;

        NativeList<SystemTypeIndex> systems = DefaultWorldInitialization.GetAllSystemTypeIndices(WorldSystemFilterFlags.Default);
        BootstrapTools.InjectUnitySystems(systems, gameWorld, gameWorld.simulationSystemGroup);

        Latios.Myri.MyriBootstrap.InstallMyri(gameWorld);
        Latios.Kinemation.KinemationBootstrap.InstallKinemation(gameWorld);
        Latios.Mecanim.MecanimBootstrap.InstallMecanimAddon(gameWorld);
        Latios.Calligraphics.CalligraphicsBootstrap.InstallCalligraphics(gameWorld);
        Latios.Calligraphics.CalligraphicsBootstrap.InstallCalligraphicsAnimations(gameWorld);
        Latios.Unika.UnikaBootstrap.InstallUnikaEntitySerialization(gameWorld);

        BootstrapTools.InjectUserSystems(systems, gameWorld, gameWorld.simulationSystemGroup);

        gameWorld.initializationSystemGroup.SortSystems();
        gameWorld.simulationSystemGroup.SortSystems();
        gameWorld.presentationSystemGroup.SortSystems();

        ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(gameWorld);
        World.DefaultGameObjectInjectionWorld = gameWorld;
        
        return gameWorld;
    }

    public static void CreateMenuWorld()
    {
        World menuWorld = new World("MenuWorld", WorldFlags.Game);
        ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(menuWorld);
        World.DefaultGameObjectInjectionWorld = menuWorld;
        MenuWorld = menuWorld;
    }
    
    public bool Initialize(string defaultWorldName)
    {
#if !UNITY_EDITOR
        CreateMenuWorld();
#elif UNITY_EDITOR
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Menu")
            CreateMenuWorld();
        else
            CreateGameWorld();
#endif
        return true;
    }
}
